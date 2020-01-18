using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Entities;
using System;
using UnityEngine.Experimental.VFX;
public class CustomPhysicsBody : MonoBehaviour, IComparable<CustomPhysicsBody>
{
    public int index;
    public float disableScale;
    public bool destroyed;
    public Vector3 gridPos;
    Vector3 beforeGridPos;
    public Vector3 ScaledGridPos { get => gridPos * SolarSystemManager.instance.transform.localScale.x; }
    float gridLerpTimer;
    bool updatedMass;
    [SerializeField]
    float customTimeStep = 1;
    [SerializeField]
    float massMultiplier = 1.0f;
    [SerializeField]
    SphereCollider objectCollider;
    public SpaceObjectData data;
    public CustomPhysicsBody[] children;
    double startingMajorAxis;
    [SerializeField, ReadOnly]
    double hillSphere;
    [SerializeField]
    public Vector3d worldPos;
    Vector3 renderPos;
    Vector3d newWorldPos;
    RingSimulator ringSystem;
    public Vector3d velocity = new Vector3d(0, 0, 0);
    Vector3d acceleration = new Vector3d(0,0,0);
    [SerializeField]
    double velocityMag;
    
    Vector3d sumForces = Vector3d.zero;
    double GConstant = 6.674E-11;
    [SerializeField]
    CustomPhysicsBody parent;
    
    public bool isFocus;
    [SerializeField]
    LineRenderer trail;
    [SerializeField]
    float minLineRenderDistance;
    [SerializeField]
    int maxLineRendererPoints;
    float radius;
    public System.Action onUpdatedPos;
    
    int lightDirProp;
    Material mat;
    bool isPlaced;
    public bool zeroVelocity;
    //Sun Properties
    bool isBlue;
    double densityMultipler = -352.5;
    float minDensity = 0.000012f;
    public bool isMainObject;
    //PEFRL Constants;
    double pefrlX = 0.1786178958448091;
    double pefrlY = -0.2123418310626054;
    double pefrlz = -0.6626458266981849E-1;
    


    public double Mass { get => data.mass * massMultiplier; }
    public double DistanceFromParent { get => distanceFromParent; }
    public double OrbitalVelocity { get => velocity.magnitude/1000; }
    public CustomPhysicsBody Parent { get => parent; }
    public float RenderRadiusScaled { get => radius/ (float)SolarSystemManager.instance.proportion; }
    public double StartingMajorAxis { get => startingMajorAxis; }
    public double HillSphere { get => hillSphere;  }
    public Vector3 WorldToRender {  get => new Vector3((float)(worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(worldPos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z); }

    private void Awake()
    {
        //SolarSystemManager.OnSetOrbitPath += () => SetTrailRenderer(false);
        //SolarSystemManager.OnSetOrbitTrail += () => SetTrailRenderer(true);
       
        

    }
    public void SetParent(CustomPhysicsBody parent)
    {
        this.parent = parent;
        
    }
    public void SetTrailRenderer(bool enabled)
    {
        //if(trail != null)
        //{
        //    trail.enabled = enabled;
        //}
    }

    public Vector3d WorldPos { get => worldPos;}
   

    [SerializeField]
    double orbitalPeriod, distanceFromParent;
    
    public double OrbitPeriod { get => orbitalPeriod; }
    float updateTimer = 1;

    Renderer model;
    public Renderer Model {
        get {
            if (model == null) model = transform.GetChild(0).GetComponent<Renderer>();
            return model;
        } }

    public PlanetDataCompute GetComputeData()
    {
        return new PlanetDataCompute { mass = Mass, pos = worldPos, velocity = velocity, destroyed = false };
    }
    private void OnDestroy()
    {
        SolarSystemManager.UpdateVelocityAndForces -= UpdateAccelerationAndVelocity;
        SolarSystemManager.UpdatePosition -= UpdatePosition;
    }
    // Start is called before the first frame update
    //void Start()
    //{
    //    lightDirProp =  Shader.PropertyToID("_LightDir");

    //    transform.localScale = Vector3.one;
    //    transform.position = new Vector3((float)(worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(worldPos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);
    //    SolarSystemManager.UpdateVelocityAndForces += UpdateAccelerationAndVelocity;
    //    SolarSystemManager.UpdatePosition += UpdatePosition ;

    //    name = data.objectName;

    //    if (data.ObjectType == ObjectType.Moon)
    //    {
    //        customTimeStep = 50;

    //    }
    //    if (data.ObjectType != ObjectType.Sun)
    //    {

    //        //transform.position = new Vector3((float)(worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(worldPos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);

    //        SetInitialVelocityToParent();
    //        CreateOrbitEllipse();

    //    }

    //    mat  = Material.Instantiate(GetComponentInChildren<Renderer>().material);
    //    GetComponentInChildren<Renderer>().material = mat;
    //    radius = CalculateRadius();
    //    Model.transform.localScale = Vector3.one * radius/ 100000000;
    //    if (SolarSystemManager.instance.usingCompute)
    //    {
    //        SolarSystemManager.instance.SetComputeData(new PlanetDataCompute { mass = Mass, pos = worldPos, velocity = velocity , index = index}, index);
    //    }
    //    if (isStartingSun) PlanetCamera.instance.SetFocusAndStats(this, true);
    //}
    private void Start()
    {
        lightDirProp = Shader.PropertyToID("_LightDir");
        if (isMainObject) PlanetCamera.instance.SetFocusAndStats(this, true);
        mat = Material.Instantiate(GetComponentInChildren<Renderer>().material);
        GetComponentInChildren<Renderer>().material = mat;
        transform.localScale = Vector3.one;
        SpawnRings();
        if(parent != null)CreateOrbitEllipse();
        //Invoke("SpawnRings", 0.2f);
    }

    //Initialize the Planet - Use before Start()
    public void Init()
    {
        if (SolarSystemManager.instance.usingCompute)
        {
            SolarSystemManager.ComputeRequestMassUpdate += UpdateMassCompute;
        }


       // transform.localScale = Vector3.one;
        transform.position = new Vector3((float)(worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(worldPos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);
        SolarSystemManager.UpdateVelocityAndForces += UpdateAccelerationAndVelocity;
        SolarSystemManager.UpdatePosition += UpdatePosition;

        name = data.objectName;

        if (data.ObjectType == ObjectType.Moon)
        {
            customTimeStep = 50;

        }
        if (data.ObjectType != ObjectType.Sun)
        {

            //transform.position = new Vector3((float)(worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(worldPos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);
            if (parent != null)
            {
                SetInitialVelocityToParent();

               
            }

        }

        
        radius = CalculateRadius();
        Model.transform.localScale = Vector3.one * radius / 100000000;
        
        if (SolarSystemManager.instance.usingCompute)
        {
            //SolarSystemManager.instance.SetComputeData(new PlanetDataCompute { mass = Mass, pos = worldPos, velocity = velocity, index = index }, index);
        }
     
    }

    private void UpdateMassCompute()
    {
        if (updatedMass)
        {
            SolarSystemManager.instance.UpdateComputeMass(this);
            updatedMass = false;
        }
    }
    public void SpawnRings()
    {
        if ( SimulatorLoader.instance.solarSystemToLoad.includeRings)
        {
            SpaceObjectData pData = data;
            if (pData.hasRings)
            {
                RingSimulator ringSim = Instantiate(pData.rings, transform);
                ringSim.transform.localPosition = Vector3.zero;
                ringSim.SetPlanet(this);
                ringSystem = ringSim;
            }
        }
    }
    public void SetAsDestroyed()
    {
        destroyed = true;
        gameObject.SetActive(false);
    }
    private void Update()
    {
       
        if (model != null)
        {
            if(model.transform.localScale.x * SolarSystemManager.instance.transform.localScale.x < disableScale )
            {

               // if(model.gameObject.activeSelf) model.gameObject.SetActive(false);
               // if (ringSystem != null && ringSystem.AsteroidParent.gameObject.activeSelf) ringSystem.AsteroidParent.gameObject.SetActive(false);
            }
            else
            {
               // if(!model.gameObject.activeSelf) model.gameObject.SetActive(true);
                //if (ringSystem != null && !ringSystem.AsteroidParent.gameObject.activeSelf) ringSystem.AsteroidParent.gameObject.SetActive(true);
            }
            Model.transform.Rotate(new Vector3(0, data.rotationalVelocity * SolarSystemManager.instance.SecondsPerSecond, 0), UnityEngine.Space.Self);
        }
        mat.SetVector(lightDirProp, (SolarSystemManager.instance.sunLight.position));
        renderPos = new Vector3((float)(worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(worldPos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);

        if (trail != null)
        {
            if (trail.positionCount == 0 || Vector3.Distance(trail.GetPosition(trail.positionCount - 1), transform.position) > minLineRenderDistance)
            {
           
                Vector3[] posArray = new Vector3[trail.positionCount];



                trail.GetPositions(posArray);

                List<Vector3> posList = new List<Vector3>(posArray);
                if (trail.positionCount == maxLineRendererPoints)
                {
                    posList.RemoveAt(0);
                }
                posList.Add(transform.position);
                posArray = posList.ToArray();
                trail.positionCount = posArray.Length;
                trail.SetPositions(posArray);


            }
        }
        updateTimer -= Time.deltaTime;
        
        if(updateTimer <= 0)
        {
           if(parent != null) orbitalPeriod = CalculatePeriod();
            updateTimer = 1;
        }
        if (SolarSystemManager.instance.GridMode)
        {
            Vector3 offsetGridPos = (gridPos -(Vector3)PlanetCamera.instance.CameraRenderdPos)*SolarSystemManager.instance.transform.localScale.x;
            if (gridLerpTimer < 1)
            {
                gridLerpTimer += 1f * Time.unscaledDeltaTime;
                if (gridLerpTimer > 1) gridLerpTimer = 1f;
            }
            transform.position = Vector3.Lerp(beforeGridPos, offsetGridPos, gridLerpTimer);
        }
        else
            transform.position = isFocus? Vector3.zero : renderPos;
        if(data.ObjectType != ObjectType.Sun)
        {
            if (parent != null)
                distanceFromParent = Vector3d.Distance(WorldPos, parent.WorldPos) / 1000;
            else
                distanceFromParent = 0;
        }
    }
    private void FixedUpdate()
    {
       
    }

    void UpdatePosition()
    {
        worldPos = newWorldPos;
        
    }

    public void ChangeMass(float deltaMultiplier)
    {
        updatedMass = true;
        massMultiplier += deltaMultiplier;
        radius = CalculateRadius();
        Model.transform.localScale =  Vector3.one * radius / 100000000;
        //objectCollider.radius = massMultiplier;
        if(data.ObjectType == ObjectType.Sun)
        {
            if(massMultiplier >= 8 && !isBlue)
            {
                mat.SetInt("_IsBlue", 1);
                model.GetComponentInChildren<VisualEffect>().SetBool("BlueStar", true);
                isBlue = true;
            }
            else if(massMultiplier <8 && isBlue)
            {
                mat.SetInt("_IsBlue", 0);
                model.GetComponentInChildren<VisualEffect>().SetBool("BlueStar", false);
                isBlue = false;
            }
        }
    }
    public float CalculateRadius()
    {
        double density = data.density;
        if(data.ObjectType == ObjectType.Sun )
        {
            density += densityMultipler * (massMultiplier -1);
            if (density < minDensity) density = minDensity;
            Debug.Log("Density is " + density);
        }
        double volume = Mass / density;
        double r = 3 * volume / (4 * Mathd.PI);
        r = Mathd.Pow(r, 0.333f);
        return (float)(r * 0.5f);
    }
    public static float CalculateRadius(double mass, double density)
    {
        //double density = data.density;
        //if (data.ObjectType == ObjectType.Sun)
        //{
        //    density += densityMultipler * (massMultiplier - 1);
        //    if (density < minDensity) density = minDensity;
        //    Debug.Log("Density is " + density);
        //}
        double volume = mass / density;
        double r = 3 * volume / (4 * Mathd.PI);
        r = Mathd.Pow(r, 0.333f);
        return (float)(r * 0.5f);
    }
    public void UpdateAccelerationAndVelocity(float h) {

        
            if (SolarSystemManager.instance.integrationtype == Integrationtypes.SIEUler)
            {
                // Implicit Euler Implementation
                
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
                acceleration = force / Mass;
                velocityMag = velocity.magnitude / 1000;
                velocity = halfVelocity + acceleration * (SolarSystemManager.instance.timeSpeed / 2) * Time.deltaTime;

            }
            //RK4 implemntation

            else if (SolarSystemManager.instance.integrationtype == Integrationtypes.RK4)
            {
                
                h /= customTimeStep;
                for (int i = 0; i < customTimeStep; i++)
                {
                    Vector3d force = SolarSystemManager.instance.GetForces(this);
                    acceleration = force / Mass;
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

               
                RKDerivatives k1 = new RKDerivatives(velocity, SolarSystemManager.instance.GetForces(this) / Mass);
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
                

                double hStep = h ;
                newWorldPos = worldPos;

                newWorldPos += pefrlX * hStep * velocity;
                    velocity += (1 - 2 * pefrlY) * (hStep * 0.5) * GetAccelerationAtPos(newWorldPos);
                newWorldPos += pefrlz * hStep * velocity;
                    velocity += pefrlY * hStep * GetAccelerationAtPos(newWorldPos); 
                newWorldPos += (1 - 2 * (pefrlz + pefrlX)) * hStep * velocity;
                    velocity += pefrlY * hStep * GetAccelerationAtPos(newWorldPos); 
                newWorldPos += pefrlz * hStep * velocity;
                    velocity += (1 - 2 * pefrlY) * (0.5 * hStep) * GetAccelerationAtPos(newWorldPos);
                newWorldPos += pefrlX * hStep * velocity;
                
                velocityMag = velocity.magnitude / 1000;
            }
        
    }
    public Vector3d GetAcceleration()
    {
        if (SolarSystemManager.instance.UseOneBody) return SolarSystemManager.instance.GetForcesBetweenTwoObjects(this, parent)/ Mass;
       return SolarSystemManager.instance.GetForces(this) / Mass;
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



        RKDerivatives output = new RKDerivatives(vel , SolarSystemManager.instance.GetForcesAtPos(this, pos)/ Mass);

        return output;

    }
    public RKDerivatives RKFEvaluate(float h, float timescaleMulitpler, RKDerivatives derivatives)
    {
        Vector3d pos = WorldPos + derivatives.vel * ( h);

        Vector3d vel = velocity + derivatives.accel * ( h);



        RKDerivatives output = new RKDerivatives(vel, SolarSystemManager.instance.GetForcesAtPos(this, pos)/ Mass);

        return output;

    }
    public Vector3d GetAccelerationAtPos(Vector3d pos)
    {
        if (SolarSystemManager.instance.UseOneBody) return SolarSystemManager.instance.GetForcesBetweenTwoObjects(this, parent) / Mass;
        return SolarSystemManager.instance.GetForcesAtPos(this,pos) / Mass;
    }
    [Button]
    public void CreateOrbitEllipse()
    {
        Ellipse ellipse = Instantiate(SolarSystemManager.instance.ellipse, Vector3.zero, SolarSystemManager.instance.ellipse.transform.rotation, SolarSystemManager.instance.transform);
        startingMajorAxis = Vector3d.Distance(worldPos, parent.WorldPos)/(1-data.e);
        float dist = Vector3.Distance(parent.transform.position, transform.position);
        float a = dist / (1 - data.e);
        
        hillSphere = startingMajorAxis * (1 - data.e) * Mathd.Pow(Mass / (3 * parent.Mass), 0.33f);
        float b = Mathf.Sqrt(1 - Mathf.Pow(data.e, 2)) * a;
       
        Vector3 ellipsePos = (parent.transform.position - transform.position);
        Vector3 center = transform.position + ellipsePos.normalized * a;
       
        ellipse.transform.position = center;
        ellipse.radius =  new Vector2(a/2,b/2);
        ellipse.owner = this;
        ellipse.UpdateEllipse();
        
        ellipse.transform.parent = parent.transform;
        ellipse.transform.localScale =Vector3.one * 2 / SolarSystemManager.instance.transform.localScale.x;
        ellipse.self_lineRenderer.startColor = data.orbitPathColour;
        ellipse.transform.LookAt(transform, Vector3.up);
        if (isPlaced) return;
        //ellipse.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //if (data.ObjectType == ObjectType.Planet)
        //    ellipse.transform.Rotate(Vector3.forward * (data.angleOfOrbit));
        //else if (data.ObjectType == ObjectType.Moon) 
        //    ellipse.transform.Rotate(Vector3.right * (data.angleOfOrbit));


    }

    internal void InitiateGridMode()
    {
        beforeGridPos = transform.localPosition;
        gridLerpTimer = 0;
    }

    public void SetInitialVelocityToParent()
    {

        if(zeroVelocity)
        {
            velocity = Vector3d.zero;
            return;
        }
        //worldPos = new Vector3d(transform.position.x * SolarSystemManager.instance.proportion, transform.position.y * SolarSystemManager.instance.proportion , transform.position.z * SolarSystemManager.instance.proportion );
        if (parent != null) velocity = parent.velocity;
        double dist = Vector3d.Distance(parent.worldPos, worldPos);
        //double a = dist / (1 - data.e);
        //double k = (GConstant * parent.Mass);
        double a =  dist / (1 - data.e);


        //double top = k* (2/dist - 1/a);
        double top = (1 + data.e) * GConstant * parent.Mass;
        //double bottom = (Vector3d.Distance(parent.worldPos, worldPos));
        double bottom = (1 - data.e) * a;
        //bottom /= (1 - data.e);
        //bottom *= 1 - data.e;
        
        Vector3d initialVel = new Vector3d(transform.TransformDirection(Vector3.right)) * Mathd.Sqrt(top/bottom);
        velocity += (initialVel ) ;
        
    }
    public static Vector3d GetAstroidInitialVelocity( NBodyAsteroid asteroidObject, AstroidDataCPU astroidData, CustomPhysicsBody parent , double gConstant)
    {
        Vector3d velocity = parent.velocity;
        double dist = Vector3d.Distance(parent.worldPos, astroidData.pos);
        //double a = dist / (1 - data.e);
        //double k = (GConstant * parent.Mass);
        double a = dist ;


        //double top = k* (2/dist - 1/a);
        double top = gConstant * parent.Mass;
        //double bottom = (Vector3d.Distance(parent.worldPos, worldPos));
        double bottom =  a;
        //bottom /= (1 - data.e);
        //bottom *= 1 - data.e;

        Vector3d initialVel = velocity + new Vector3d(asteroidObject.transform.TransformDirection(Vector3.right)) * Mathd.Sqrt(top / bottom);
        return initialVel;
    }
    public static Vector3d GetAstroidInitialVelocityGPU(NBodyAsteroid asteroidObject, AstroidDataGPU astroidData ,CustomPhysicsBody parent, double gConstant)
    {
        Vector3d velocity = parent.velocity;
        double dist = Vector3d.Distance(parent.worldPos, astroidData.pos);
        //double a = dist / (1 - data.e);
        //double k = (GConstant * parent.Mass);
        double a = dist;


        //double top = k* (2/dist - 1/a);
        double top = gConstant * parent.Mass;
        //double bottom = (Vector3d.Distance(parent.worldPos, worldPos));
        double bottom = a;
        //bottom /= (1 - data.e);
        //bottom *= 1 - data.e;

        Vector3d initialVel = velocity + new Vector3d(asteroidObject.transform.TransformDirection(Vector3.right)) * Mathd.Sqrt(top / bottom);
        return initialVel;
    }
    public static Vector3d GetAstroidInitialVelocity(GameObject asteroidObject, AstroidDataCPU astroidData, Vector3d parentPos, Vector3d parentVel,double parentMass, double gConstant)
    {
        Vector3d velocity = parentVel;
        double dist = Vector3d.Distance(parentPos, astroidData.pos);
        //double a = dist / (1 - data.e);
        //double k = (GConstant * parent.Mass);
        double a = a = dist;


        //double top = k* (2/dist - 1/a);
        double top = gConstant * parentMass;
        //double bottom = (Vector3d.Distance(parent.worldPos, worldPos));
        double bottom = a;
        //bottom /= (1 - data.e);
        //bottom *= 1 - data.e;

        Vector3d initialVel = velocity + new Vector3d(asteroidObject.transform.TransformDirection(Vector3.right)) * Mathd.Sqrt(top / bottom);
        return initialVel;
    }
    [Button]
    public void DebugInitialVelocity()
    {
        SolarSystemManager manager = FindObjectOfType<SolarSystemManager>();
        Vector3d thisWorldPos = new Vector3d(transform.position.x * manager.proportion, transform.position.y * manager.proportion, transform.position.y * manager.proportion);
        Vector3d parentWorldPos = new Vector3d(parent.transform.position.x * manager.proportion, parent.transform.position.y * manager.proportion, parent.transform.position.y * manager.proportion);


        Vector3d initialVel = Vector3d.right * Mathd.Sqrt((GConstant * parent.Mass) / (Vector3d.Distance(thisWorldPos, parentWorldPos) * 1000)) * data.e;
        Debug.Log((initialVel / 1000).magnitude);
    }
    
    double CalculatePeriod()
    {
        double val = 4 * Mathd.Pow(Mathd.PI, 2) * Mathd.Pow(Vector3d.Distance(this.WorldPos, parent.WorldPos), 3);
        val /= GConstant * parent.Mass;
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
    public void PlaceInSolarsystem(Vector3d worldPos, CustomPhysicsBody parent, SpaceObjectData data)
    {
        this.worldPos = worldPos;
        this.parent = parent;
        transform.LookAt(parent.transform);
        this.data = data;
        isPlaced = true;
        CreateModel();
        Init();
        SolarSystemManager.instance.AddObjectToSolarsystem(this);

        
        
    }
    public void Consume(CustomPhysicsBody other, float massGainPercentage)
    {
        double otherMass = other.Mass * massGainPercentage;
        otherMass /= data.mass;
        ChangeMass((float)otherMass);
    }

    public int CompareTo(CustomPhysicsBody obj)
    {
        if (obj == null) return 1;
        return Mass.CompareTo(obj.Mass);
    }

    public static Comparison<CustomPhysicsBody> RadiusComparison = delegate (CustomPhysicsBody object1, CustomPhysicsBody object2)
    {
        return object2.radius.CompareTo(object1.radius);
    };
    //public static Comparison<CustomPhysicsBody> OtherComparison = delegate (CustomPhysicsBody object1, CustomPhysicsBody object2)
    //{
    //    return object1.Mass.CompareTo(object2.Mass);
    //};


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
