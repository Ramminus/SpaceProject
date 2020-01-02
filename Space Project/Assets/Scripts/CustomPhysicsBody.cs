using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Entities;
using System;
public class CustomPhysicsBody : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    float customTimeStep = 1;
    public SpaceObjectData data;
    public CustomPhysicsBody[] children;
    [SerializeField]
    float eValue = 1;
    [SerializeField]
    Vector3d worldPos;

    Vector3d velocity, acceleration = new Vector3d(0,0,0);
    [SerializeField]
    double velocityMag;
    public double OrbitalVelocity { get => velocityMag; }
    Vector3d sumForces = Vector3d.zero;
    double GConstant = 6.674E-11;
    [SerializeField]
    CustomPhysicsBody parent;
    public CustomPhysicsBody Parent { get => parent; }
    public bool isFocus;


    public System.Action onUpdatedPos;


    //PEFRL Constants;
    double pefrlX = 0.1786178958448091;
    double pefrlY = -0.2123418310626054;
    double pefrlz = -0.6626458266981849E-1;
 
    public void SetParent(CustomPhysicsBody parent)
    {
        this.parent = parent;
    }

    public Vector3d WorldPos { get => worldPos;}
   

    [SerializeField]
    double orbitalPeriod, distanceFromParent;
    public double DistanceFromParent { get => distanceFromParent; }
    public double OrbitPeriod { get => orbitalPeriod; }
    float updateTimer = 1;

    Renderer model;
    public Renderer Model {
        get {
            if (model == null) model = transform.GetChild(0).GetComponent<Renderer>();
            return model;
        } }
    // Start is called before the first frame update

    void Start()
    {
        name = data.objectName;
        if(data.ObjectType == ObjectType.Moon)
        {
            customTimeStep = 50;
            
        }
        if (data.ObjectType != ObjectType.Sun)
        {
            SetInitialVelocityToParent();
            CreateOrbitEllipse();

        }
        transform.Rotate(Vector3.forward, data.axisTiltInDeg);
        transform.localScale = Vector3.one *(float)(((data.diameter*0.5f) / SolarSystemManager.instance.proportion) );
       
    }
  
    
    private void Update()
    {
       
        updateTimer -= Time.deltaTime;
        
        if(updateTimer <= 0)
        {
           if(parent != null) orbitalPeriod = CalculatePeriod();
            updateTimer = 1;
        }
        Vector3 pos = new Vector3((float)((worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x), (float)(worldPos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);
        transform.position = isFocus? Vector3.zero : pos;
        if(data.ObjectType != ObjectType.Sun)
        {
            distanceFromParent = Vector3d.Distance(WorldPos, parent.WorldPos);
        }
    }


    private void FixedUpdate()
    {
            UpdatePositionAndVelocity();
          

        

    }

    public void UpdatePositionAndVelocity() {

        if (data.ObjectType != ObjectType.Sun)
        {
            if (SolarSystemManager.instance.integrationtype == Integrationtypes.SIEUler)
            {
                // Implicit Euler Implementation
                float h = SolarSystemManager.instance.timeSpeed * Time.fixedDeltaTime;
                float hStep = h / customTimeStep;

                for (int i = 0; i < customTimeStep; i++)
                {
                    acceleration = GetAcceleration();
                    velocity += acceleration * hStep;
                    worldPos += velocity * hStep;
                    velocityMag = velocity.magnitude / 1000;
                    acceleration = Vector3d.zero;
                    sumForces = Vector3d.zero;
                }

            }

            else if (SolarSystemManager.instance.integrationtype == Integrationtypes.VelVerlet)
            {
                // Verlet Velocity Implementation
                Vector3d halfVelocity = velocity + acceleration * (SolarSystemManager.instance.timeSpeed / 2) * Time.fixedDeltaTime;
                worldPos += halfVelocity * SolarSystemManager.instance.timeSpeed * Time.deltaTime;
                Vector3d force = SolarSystemManager.instance.GetForces(this);
                acceleration = force / data.mass;
                velocityMag = velocity.magnitude / 1000;
                velocity = halfVelocity + acceleration * (SolarSystemManager.instance.timeSpeed / 2) * Time.deltaTime;

            }
            //RK4 implemntation

            else if (SolarSystemManager.instance.integrationtype == Integrationtypes.RK4)
            {
                float h = SolarSystemManager.instance.timeSpeed;
                h /= customTimeStep;
                for (int i = 0; i < customTimeStep; i++)
                {
                    Vector3d force = SolarSystemManager.instance.GetForces(this);
                    acceleration = force / data.mass;
                    RKDerivatives a = new RKDerivatives(velocity, acceleration);
                    RKDerivatives b = RKEvaluate(h, 0.5f, a);
                    RKDerivatives c = RKEvaluate(h, 0.5f, b);
                    RKDerivatives d = RKEvaluate(h, 1, c);

                    Vector3d pos = (1f / 6.0f) * (a.vel + 2 * (b.vel + c.vel) + d.vel);
                    Vector3d vel = (1f / 6.0f) * (a.accel + 2 * (b.accel + c.accel) + d.accel);

                    worldPos += pos * h;
                    velocity += vel * h;
                }

                velocityMag = velocity.magnitude / 1000;
            }
            else if (SolarSystemManager.instance.integrationtype == Integrationtypes.RKF45)
            {

                float h = SolarSystemManager.instance.timeSpeed * Time.fixedDeltaTime;
                RKDerivatives k1 = new RKDerivatives(velocity, SolarSystemManager.instance.GetForces(this) / data.mass);
                RKDerivatives k2 = RKFEvaluate(h, 0.25f, (0.25f) * k1);
                RKDerivatives k3 = RKFEvaluate(h, 3 / 8f, (3f / 32f) * k1 + (9f / 32f) * k2);
                RKDerivatives k4 = RKFEvaluate(h, 12f / 13f, (1932f / 2197f) * k1 - (7200f / 2197f) * k2 + (7296f / 2197f) * k3);
                RKDerivatives k5 = RKFEvaluate(h, 1f, (439f / 216f) * k1 - (8f) * k2 + (3680f / 513f) * k3 - (845f / 4104f) * k4);
                RKDerivatives k6 = RKFEvaluate(h, 0.5f, -(8f / 27f) * k1 + (2f) * k2 - (3544f / 2565f) * k3 + (1859f / 4104f) * k4 - (11f / 40f) * k5);


                Vector3d pos = ((16f / 135f) * k1.vel + (6656f / 12825f) * k3.vel + (28561f / 56430f) * k4.vel - (9f / 50f) * k5.vel + (2f / 55f) * k6.vel);
                //Vector3d pos =  ((25f / 216f) * k1.vel + (1408f / 2565f) * k3.vel + (2197f / 4101f) * k4.vel - (1f / 5f) * k5.vel);
                Vector3d vel = ((16f / 135f) * k1.accel) + ((6656f / 12825f) * k3.accel) + ((28561f / 56430f) * k4.accel) - ((9f / 50f) * k5.accel) + ((2f / 55f) * k6.accel);
                //Vector3d vel = ((25f / 216f) * k1.accel + (1408f / 2565f) * k3.accel + (2197f / 4101f) * k4.accel - (1f / 5f) * k5.accel);
                //Debug.Log(( * new RKDerivatives { vel = Vector3d.one * 2, accel = Vector3d.one * 2 }).vel);
                worldPos += pos * (h);
                velocity += vel * (h);
                velocityMag = velocity.magnitude / 1000;
            }
            else if (SolarSystemManager.instance.integrationtype == Integrationtypes.PEFRL)
            {
                double h = SolarSystemManager.instance.timeSpeed * Time.fixedDeltaTime;

                double hStep = h / customTimeStep;
                for (int i = 0; i < customTimeStep; i++)
                {

                    worldPos += pefrlX * hStep * velocity;
                    velocity += (1 - 2 * pefrlY) * (hStep * 0.5) * GetAcceleration();
                    worldPos += pefrlz * hStep * velocity;
                    velocity += pefrlY * hStep * GetAcceleration();
                    worldPos += (1 - 2 * (pefrlz + pefrlX)) * hStep * velocity;
                    velocity += pefrlY * hStep * GetAcceleration();
                    worldPos += pefrlz * hStep * velocity;
                    velocity += (1 - 2 * pefrlY) * (0.5 * hStep) * GetAcceleration();
                    worldPos += pefrlX * hStep * velocity;
                }
                velocityMag = velocity.magnitude / 1000;
            }
        }
    }
    public Vector3d GetAcceleration()
    {
        if (SolarSystemManager.instance.UseOneBody) return SolarSystemManager.instance.GetForcesBetweenTwoObjects(this, parent)/data.mass;
       return SolarSystemManager.instance.GetForces(this) / data.mass;
    }
    private void LateUpdate()
    {
       
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + ((Vector3)velocity.normalized * 20));
    }
    public void CreateModel()
    {
        if (data.customMaterial != null)
        {
            transform.GetChild(0).GetComponent<Renderer>().material = data.customMaterial;
            
        }
    }


    
    public RKDerivatives RKEvaluate(float h,float timescaleMulitpler, RKDerivatives derivatives)
    {
        Vector3d pos = WorldPos + derivatives.vel * (h * timescaleMulitpler);

        Vector3d vel = velocity + derivatives.accel * (h * timescaleMulitpler);



        RKDerivatives output = new RKDerivatives(vel , SolarSystemManager.instance.GetForcesAtPos(this, pos)/data.mass);

        return output;

    }
    public RKDerivatives RKFEvaluate(float h, float timescaleMulitpler, RKDerivatives derivatives)
    {
        Vector3d pos = WorldPos + derivatives.vel * ( h);

        Vector3d vel = velocity + derivatives.accel * ( h);



        RKDerivatives output = new RKDerivatives(vel, SolarSystemManager.instance.GetForcesAtPos(this, pos)/data.mass);

        return output;

    }
    public Vector3d GetAccelerationAtPos(Vector3d pos)
    {
       return SolarSystemManager.instance.GetForcesAtPos(this, pos)/data.mass;
    }
    [Button]
    public void CreateOrbitEllipse()
    {
        Ellipse ellipse = Instantiate(SolarSystemManager.instance.ellipse, Vector3.zero, SolarSystemManager.instance.ellipse.transform.rotation, SolarSystemManager.instance.transform);
        float dist = Vector3.Distance(parent.transform.position, transform.position);
        float a = dist / (1 - data.e);
        float b = Mathf.Sqrt(1 - Mathf.Pow(data.e, 2)) * a;
       
        Vector3 ellipsePos = (parent.transform.position - transform.position);
        Vector3 center = transform.position + ellipsePos.normalized * a;
       
        ellipse.transform.position = center;
        ellipse.radius =  new Vector2(a/2,b/2);
        ellipse.UpdateEllipse();
        ellipse.transform.Rotate(Vector3.forward * data.angleOfOrbit);
        ellipse.transform.parent = parent.transform;
        ellipse.self_lineRenderer.startColor = data.orbitPathColour;
    }

    public void SetInitialVelocityToParent()
    {

        worldPos = new Vector3d(transform.position.x * SolarSystemManager.instance.proportion, transform.position.y * SolarSystemManager.instance.proportion , transform.position.z * SolarSystemManager.instance.proportion );
        if (parent != null) velocity = parent.velocity;
        double dist = Vector3d.Distance(parent.worldPos, worldPos);
        double a = dist / (1 - data.e);
        double k = (GConstant * parent.data.mass);



        double top = k* (2/dist - 1/a);
        //double bottom = (Vector3d.Distance(parent.worldPos, worldPos));
        //bottom /= (1 - data.e);
        //bottom *= 1 - data.e;
        
        Vector3d initialVel = new Vector3d(transform.TransformDirection(Vector3.forward)) * Mathd.Sqrt( top);
        velocity += (initialVel ) ;
        
    }
    [Button]
    public void DebugInitialVelocity()
    {
        SolarSystemManager manager = FindObjectOfType<SolarSystemManager>();
        Vector3d thisWorldPos = new Vector3d(transform.position.x * manager.proportion, transform.position.y * manager.proportion, transform.position.y * manager.proportion);
        Vector3d parentWorldPos = new Vector3d(parent.transform.position.x * manager.proportion, parent.transform.position.y * manager.proportion, parent.transform.position.y * manager.proportion);


        Vector3d initialVel = new Vector3d(0, 0, 1) * Mathd.Sqrt((GConstant * parent.data.mass) / (Vector3d.Distance(thisWorldPos, parentWorldPos) * 1000)) * eValue;
        Debug.Log((initialVel / 1000).magnitude);
    }
    
    double CalculatePeriod()
    {
        double val = 4 * Mathd.Pow(Mathd.PI, 2) * Mathd.Pow(Vector3d.Distance(this.WorldPos, parent.WorldPos), 3);
        val /= GConstant * parent.data.mass;
        val = Mathd.Sqrt(val);



        return val;
    }
    

    [Button]
    public void AddForce(Vector3d force)
    {
        sumForces += force;
        
    }
    [Button]
    public void GetForceBetweenOtherObject(CustomPhysicsBody other)
    {
        Debug.Log(FindObjectOfType<SolarSystemManager>().CalculateForce(this, other).magnitude);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
       
    }
}
public struct RK4State
{
    public Vector3d pos;
    public Vector3d vel;

    public RK4State(Vector3d pos, Vector3d vel)
    {
        this.pos = pos;
        this.vel = vel;
    }
}

public struct RKDerivatives
{
    public Vector3d  vel;
    public Vector3d  accel;

    public RKDerivatives(Vector3d vel, Vector3d accel)
    {
        this.vel = vel;
        this.accel = accel;
    }


    public static RKDerivatives operator *(float a, RKDerivatives b) => new RKDerivatives ( b.vel * a, b.accel * a );
    public static RKDerivatives operator +(RKDerivatives a, RKDerivatives b) => new RKDerivatives ( a.vel + b.vel, b.accel + a.accel );
    public static RKDerivatives operator -(RKDerivatives a, RKDerivatives b) => new RKDerivatives ( a.vel - b.vel, a.accel - b.accel );
   
}
