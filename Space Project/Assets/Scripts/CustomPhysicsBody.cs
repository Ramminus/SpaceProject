using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CustomPhysicsBody : MonoBehaviour
{
    
    public SpaceObjectData data;
    public CustomPhysicsBody[] children;
    [SerializeField]
    float eValue = 1;
    [SerializeField]
    Vector3d worldPos;

    Vector3d velocity, acceleration = new Vector3d(0,0,0);
    [SerializeField]
    double velocityMag;
    Vector3d sumForces = Vector3d.zero;
    double GConstant = 6.674E-11;
    [SerializeField]
    CustomPhysicsBody parent;
    public CustomPhysicsBody Parent { get => parent; }
    public bool isFocus;
 
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

        if (data.ObjectType != ObjectType.Sun)
        {
            SetInitialVelocityToParent();
            CreateOrbitEllipse();

        }

        transform.localScale = Vector3.one *(float)((data.diameter / SolarSystemManager.instance.proportion) );
       
    }
  
    
    private void Update()
    {
        updateTimer -= Time.deltaTime;
        
        if(updateTimer <= 0)
        {
           if(parent != null) orbitalPeriod = CalculatePeriod();
            updateTimer = 1;
        }
        Vector3 pos = new Vector3((float)((worldPos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x), (float)(worldPos.y / SolarSystemManager.instance.proportion), (float)(worldPos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);
        transform.position = isFocus? Vector3.zero : pos;
        if(data.ObjectType != ObjectType.Sun)
        {
            distanceFromParent = Vector3d.Distance(WorldPos, parent.WorldPos);
        }
    }


    private void FixedUpdate()
    {
        if (data.ObjectType != ObjectType.Sun)
        {


            AddForce(SolarSystemManager.instance.GetForces(this));
            acceleration = sumForces / data.mass;
            velocity += acceleration * SolarSystemManager.instance.timeSpeed;
            worldPos += velocity * SolarSystemManager.instance.timeSpeed;
            velocityMag = velocity.magnitude/1000;
            acceleration = Vector3d.zero;
            sumForces = Vector3d.zero;
        }
       
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
        if (data.customModel != null)
        {
            Destroy(transform.GetChild(0).gameObject);
            Instantiate(data.customModel, transform.position, Quaternion.identity, transform);
        }
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
        ellipse.transform.parent = parent.transform;
        ellipse.self_lineRenderer.startColor = data.orbitPathColour;
    }

    public void SetInitialVelocityToParent()
    {
        worldPos = new Vector3d(transform.position.x * SolarSystemManager.instance.proportion, transform.position.y * SolarSystemManager.instance.proportion , transform.position.y * SolarSystemManager.instance.proportion );
        if (parent != null) velocity = parent.velocity;
        double top = (GConstant * parent.data.mass) * (1 + data.e);
        double bottom = (Vector3d.Distance(parent.worldPos, worldPos));
        bottom /= (1 - data.e);
        bottom *= 1 - data.e;
        Vector3d initialVel = new Vector3d(0,0,1) * Mathd.Sqrt( top/bottom);
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



        return val/ 86400;
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

}
