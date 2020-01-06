using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
public class SolarSystemManager : MonoBehaviour
{
    public Transform sunLight;
    [SerializeField]
    bool useOneBody;
    public bool UseOneBody { get => useOneBody; }

    [SerializeField]
    SolarSystemData solarSystemToLoad;
    [HideInInspector]
    public CustomPhysicsBody sun;
    List<CustomPhysicsBody> planets;
    List<CustomPhysicsBody> moons;
    [ReadOnly, SerializeField]
    List<CustomPhysicsBody> objectsInSolarSystem;
    public List<CustomPhysicsBody> objectsInSolarSystemByMass;
   
    public Vector3 rotation = Vector3.zero;
    [SerializeField]
    float rotSpeed;
    [SerializeField]
    GameObject sunModel, planetModel, moonModel;
    public static SolarSystemManager instance;

    public float cameraDistance;
    public float cameraDeadZone;
    double GConstant = 6.674E-11;
    [SerializeField]
    private float ellipticalStrength;

    [SerializeField]
    public double proportion = 1000000;
    public static float startProportion = 100000000f;
    [SerializeField]
    float smoothing = 0.3f;
    float scale = 1;
    [SerializeField, ReadOnly]
    float targetScaleT = 0.5f;
    [SerializeField]
    private AnimationCurve scrollSpeedCurve;
    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float minScale, maxScale;
    [ReadOnly]
    public float timeSpeed =1f;
    [SerializeField]
    float maxTimeSpeed =200f;


    public float modelScale =1;
    public Ellipse ellipse;
    [SerializeField]
    float addedCameraRayLength;
    [SerializeField]
    Transform light;

    [SerializeField]
    Slider slider;

    [SerializeField]
    float targetT, currentT,tSmoothing;
    bool disabledScaling;
   
    public Integrationtypes integrationtype;
    [SerializeField]
    float scalePow;

    Transform solarSystemParent;

    public static System.Action OnSetOrbitPath;
    public static System.Action OnSetOrbitTrail;

    public static System.Action<float> UpdateVelocityAndForcesBig;
    public static System.Action<float> UpdateVelocityAndForces;
    public static System.Action UpdatePositionBig;
    public static System.Action UpdatePosition;
    public static System.Action OnClearSolarSystem;

    public static System.Action<CustomPhysicsBody> DestroyBody;
    [SerializeField]
    private int intervals= 20;

    public  void SetOrbitPath()
    {
        OnSetOrbitPath?.Invoke();
    }
    public  void SetOrbitTrail()
    {
        OnSetOrbitTrail?.Invoke();
    }
    private void Awake()
    {
        if (instance == null) instance = this;

     
    }
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
    [Button]
    public void ChangeTimeScale(float newTimescale)
    {
        Time.timeScale = newTimescale;

        Time.fixedDeltaTime = 0.02f * newTimescale;
    }
    private void Start()
    {
        
        UiHandler.instance.timeSlider.onValueChanged.AddListener( ChangeSpeed);
        targetScaleT = 1;
        SetUpSolarSystem();
        
    }

    public void ChangeSpeed(float t)
    {
        targetT = t;
    }
    private void Update()
    {

        bool canScaleForward = true;
        bool canScaleBack = targetScaleT > minScale;

        if(currentT != targetT)
        {
            if(currentT > targetT)
            {
                currentT -= tSmoothing;
                if (currentT < targetT) currentT = targetT;
            }
            else if(currentT < targetT)
            {
                currentT += tSmoothing;
                if (currentT > targetT) currentT = targetT;
            }
            timeSpeed = Mathf.Lerp(1, maxTimeSpeed, currentT);

           // ChangeTimeScale(Mathf.Lerp(0,60, currentT));


        }

        RaycastHit hit;
        Ray ray =Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        if(Physics.Raycast(ray,Camera.main.nearClipPlane + addedCameraRayLength))
        {
            canScaleForward = false;
            targetScaleT = scale;
        }
        scrollSpeed = Mathf.Pow(targetScaleT, scalePow);

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && canScaleForward || Input.GetAxis("Mouse ScrollWheel") < 0 && canScaleBack)
        {
            if (UiHandler.instance.scrollBlocker < 1 )
            {
                targetScaleT += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            }

            
        }
        if (targetScaleT < minScale) targetScaleT = minScale;
        scale = Mathf.Lerp(scale, targetScaleT, 0.1f);
        //scale = targetScaleT;
        //if (scale == targetScaleT) disabledScaling = false;
        SetScale(scale);
       
       
        light.LookAt((Vector3)PlanetCamera.instance.CameraRenderdPos,Vector3.up);
    }
    public void SetTargetScale(float modelScale, float targetScale, bool onlyIfGreaterThan = true)
    {
        float newScale = targetScale / (modelScale*2);
        if (targetScaleT > newScale || !onlyIfGreaterThan)
        {
            targetScaleT = newScale;
           
            scale = targetScaleT;

        }
    }
    private void FixedUpdate()
    {

        for (int i = 0; i < intervals; i++)
        {
            UpdateVelocityAndForces?.Invoke((timeSpeed * Time.fixedDeltaTime) / (float)intervals);
            UpdatePosition?.Invoke();
            CheckForCollisions();
            
        }
        
    }
    public void AddObjectToSolarsystem(CustomPhysicsBody body)
    {
        objectsInSolarSystem.Add(body);
        UiHandler.instance.OnAddBodyToSpace(body);
        body.transform.parent = solarSystemParent;
        objectsInSolarSystemByMass = objectsInSolarSystem;
        objectsInSolarSystemByMass.Sort();
    }
    public void CheckForCollisions()
    {
        for (int i = 0; i < objectsInSolarSystem.Count; i++)
        {
            for (int x = i+1; x < objectsInSolarSystem.Count; x++)
            {
                bool hasCollided = HasCollided(objectsInSolarSystem[i], objectsInSolarSystem[x]);
                if (hasCollided)
                {
                   Debug.Log(objectsInSolarSystem[i].data.objectName + " and " + objectsInSolarSystem[x].data.objectName + " Has Collided");
                    HandleCollision(objectsInSolarSystem[i], objectsInSolarSystem[x]);
                }
            }
        }
    }
    public bool HasCollided(CustomPhysicsBody b1, CustomPhysicsBody b2)
    {

        return (Vector3.Distance(b1.WorldToRender, b2.WorldToRender) <= b1.RenderRadiusScaled + b2.RenderRadiusScaled);
    }
    public void HandleCollision(CustomPhysicsBody b1, CustomPhysicsBody b2)
    {
        if(b1.data.ObjectType == ObjectType.Sun)
        {
            b1.Consume(b2, 1f);
            DestroyBody?.Invoke(b2);
            objectsInSolarSystem.Remove(b2);
            Destroy(b2.gameObject);
        }
        if (b2.data.ObjectType == ObjectType.Sun)
        {
            b2.Consume(b1, 1f);
            DestroyBody?.Invoke(b1);
            objectsInSolarSystem.Remove(b1);
            Destroy(b1.gameObject);
        }
    }
    public void SetUpSolarSystem(){
        solarSystemParent = new GameObject().GetComponent<Transform>();
        solarSystemParent.parent = transform;
        sun = Instantiate(sunModel, Vector3.zero, Quaternion.identity, solarSystemParent).GetComponent<CustomPhysicsBody>();
        //sun.transform.localScale *= modelScale;
        solarSystemToLoad = SimulatorLoader.instance.solarSystemToLoad;
        sun.data = solarSystemToLoad.sun;
        sun.CreateModel();
        sun.isStartingSun = true;
        sunLight.transform.parent = sun.transform;
        sunLight.localPosition = Vector3.zero;
        UiHandler.instance.OnAddBodyToSpace(sun);
        planets = new List<CustomPhysicsBody>();
        moons = new List<CustomPhysicsBody>();
        for (int i = 0; i < solarSystemToLoad.planets.Length; i++)
        {
            CustomPhysicsBody planet = Instantiate(planetModel,Vector3.zero, Quaternion.identity, solarSystemParent).GetComponent<CustomPhysicsBody>();
           
            planet.data = solarSystemToLoad.planets[i];
            Vector3 rot = Quaternion.Euler(0, 0, planet.data.angleOfOrbit) * Vector3.right;
            //Vector3 pos3 = Quaternion.Euler(0, 0, planet.data.angleOfOrbit) * Vector3.right * (float)solarSystemToLoad.planets[i].avrgDistanceFromSun;
            planet.worldPos = new Vector3d(rot * (float)solarSystemToLoad.planets[i].avrgDistanceFromSun);
            //planet.transform.LookAt(sun.transform);

            
            //planet.transform.RotateAround(Vector3.zero, Vector3.forward, planet.data.angleOfOrbit);
           
            Debug.Log(planet.transform.position);
            planet.SetParent(sun);
            planet.transform.position = new Vector3((float)(planet.worldPos.x / SolarSystemManager.instance.proportion), (float)(planet.worldPos.y / SolarSystemManager.instance.proportion) , (float)(planet.worldPos.z / SolarSystemManager.instance.proportion) );
            //planet.transform.localScale *= modelScale;
            planet.transform.LookAt(sun.transform);
            planet.transform.Rotate(Vector3.right, planet.data.axisTiltInDeg);
            //planet.CreateOrbitEllipse();
            planets.Add(planet);
            planet.CreateModel();
            UiHandler.instance.OnAddBodyToSpace(planet);
            PlanetData data = planet.data as PlanetData;
            if (solarSystemToLoad.includeMoons)
            {
                for (int x = 0; x < data.moons.Length; x++)
                {
                    CustomPhysicsBody moon = Instantiate(moonModel, Vector3.zero, Quaternion.identity, solarSystemParent).GetComponent<CustomPhysicsBody>();
                    moon.data = data.moons[x];
                    moon.SetParent(planet);
                    
                    Vector3 moonRot = Quaternion.Euler(0, 0, planet.data.axisTiltInDeg + moon.data.angleOfOrbit) * (Vector3.left);
                    //Vector3 pos3 = Quaternion.Euler(0, 0, planet.data.angleOfOrbit) * Vector3.right * (float)solarSystemToLoad.planets[i].avrgDistanceFromSun;
                    moon.worldPos = planet.WorldPos + new Vector3d((moonRot * (float)data.moons[x].avrgDistanceFromPlanet));
                    //moon.worldPos = planet.WorldPos + moonRot * ;
                    moon.transform.LookAt(planet.transform);


                    // moon.transform.RotateAround(planet.transform.position, Vector3.forward, planet.transform.rotation.eulerAngles.z + moon.data.angleOfOrbit);

                    //moon.transform.localScale *= modelScale;
                    moon.transform.position = new Vector3((float)(moon.worldPos.x / SolarSystemManager.instance.proportion), (float)(moon.worldPos.y / SolarSystemManager.instance.proportion), (float)(moon.worldPos.z / SolarSystemManager.instance.proportion));
                    //moon.transform.LookAt(moon.Parent.transform);
                   // moon.transform.Rotate(Vector3.up * (90), UnityEngine.Space.Self);
                    //moon.transform.Rotate(Vector3.right * (planet.data.axisTiltInDeg + moon.data.angleOfOrbit), UnityEngine.Space.Self);
                    moons.Add(moon);
                    moon.CreateModel();
                    //moon.CreateOrbitEllipse();
                    UiHandler.instance.OnAddBodyToSpace(moon);
                }
            }
        }
        objectsInSolarSystem = new List<CustomPhysicsBody>();
        objectsInSolarSystem.Add(sun);
        objectsInSolarSystem.AddRange(planets);
        objectsInSolarSystem.AddRange(moons);
        objectsInSolarSystemByMass = objectsInSolarSystem;
        objectsInSolarSystemByMass.Sort();
        SimulatorLoader.instance.loaded = true;
        
    }
    
    public void ClearSolarSystem()
    {
        light.parent = null;
        Destroy(solarSystemParent.gameObject);
        SolarSystemManager.OnClearSolarSystem?.Invoke();
        
    }
    [Button]
    public void LoadSolarSystem()
    {
        SetUpSolarSystem();
    }
    public Vector3d GetForces(CustomPhysicsBody physicsObject)
    {
        Vector3d overallForce = Vector3d.zero;
        for (int i = 0; i < objectsInSolarSystem.Count; i++)
        {
            if (objectsInSolarSystem[i] != physicsObject)
            {
                overallForce += CalculateForce(physicsObject, objectsInSolarSystem[i]);
            }
        }
        return overallForce;
    }
    public Vector3d GetForcesBetweenTwoObjects(CustomPhysicsBody o1, CustomPhysicsBody o2)
    {
        return CalculateForce(o1, o2);
    }
    public Vector3d GetForcesAtPos(CustomPhysicsBody physicsObject, Vector3d pos)
    {
        Vector3d overallForce = Vector3d.zero;
        for (int i = 0; i < objectsInSolarSystem.Count; i++)
        {
            if (objectsInSolarSystem[i] != physicsObject)
            {
                overallForce += CalculateForceAtPos(physicsObject, objectsInSolarSystem[i], pos);
            }
        }
        return overallForce;
    }
    public Vector3d CalculateForce(CustomPhysicsBody o1, CustomPhysicsBody o2, bool debug = false)
    {


        Vector3d direction = o2.WorldPos  - o1.WorldPos;
        double r =Vector3d.Distance(o2.WorldPos, o1.WorldPos)  ;

        double m1 = o1.Mass;

        double m2 = o2.Mass;

        double force = (GConstant * m1 * m2 / Mathd.Pow(r, 2));
        

        if (debug)
        {
            Debug.Log("DISTANCE: " + r);
            Debug.Log("Mass of " + o2.name + ":" + m1);
            Debug.Log("Mass of " + o1.name + ":" + m2);
            Debug.Log("FORCE: " + force);
           // Debug.Log("Initial velocity: " + Mathf.Sqrt((float)(GConstant * (other.RigidBody.mass) / (Vector3.Distance(other.transform.position, spaceObject.transform.position))) * spaceObject.eValue));

        }
        
       

        return force * direction.normalized;


    }
    public Vector3d CalculateForceAtPos(CustomPhysicsBody o1, CustomPhysicsBody o2, Vector3d pos,bool debug = false)
    {


        Vector3d direction = o2.WorldPos - pos;
        double r = Vector3d.Distance(o2.WorldPos, pos);

        double m1 = o1.Mass;

        double m2 = o2.Mass;

        double force = (GConstant * m1 * m2 / Mathd.Pow(r, 2));


        if (debug)
        {
            Debug.Log("DISTANCE: " + r);
            Debug.Log("Mass of " + o2.name + ":" + m1);
            Debug.Log("Mass of " + o1.name + ":" + m2);
            Debug.Log("FORCE: " + force);
            // Debug.Log("Initial velocity: " + Mathf.Sqrt((float)(GConstant * (other.RigidBody.mass) / (Vector3.Distance(other.transform.position, spaceObject.transform.position))) * spaceObject.eValue));

        }



        return force * direction.normalized;


    }
    [Button]
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
        proportion = 100000000 / scale;
    }
    //public Vector3 CalculateForce(SpaceObject spaceObject, SpaceObject other, bool debug = false)
    //{



    //    float r = Vector3.Distance(spaceObject.transform.position, other.transform.position) ;
      
    //    double m1 = other.customMass;
        
    //    double m2 = spaceObject.customMass;
        
    //    double force = (GConstant * m1 * m2 / Mathf.Pow(r, 2));

        
    //    if (debug)
    //    {
    //        Debug.Log("DISTANCE: " + r);
    //        Debug.Log("Mass of " + other.name + ":" + m1);
    //        Debug.Log("Mass of " + spaceObject.name + ":" + m2);
    //        Debug.Log("FORCE: " + force);
    //        Debug.Log("Initial velocity: " + Mathf.Sqrt((float)(GConstant * (other.RigidBody.mass) / (Vector3.Distance(other.transform.position, spaceObject.transform.position))) * spaceObject.eValue));
           
    //    }
    //    double a = force / ((spaceObject.customMass));
    //    Vector3 direction = other.transform.position - spaceObject.transform.position;

    //    return (float)a * direction.normalized;


    //}

    //public void GetGravitationalForces(SpaceObject spaceObject, bool start)
    //{
    //    Vector3 overallForce = Vector3.zero;
    //    double greatestForce = 0;
    //    SpaceObject objectWithGreatestForce = null;
    //    for (int i = 0; i < objectsInSolarSystem.Count; i++)
    //    {
    //        if(objectsInSolarSystem[i] != spaceObject)
    //        {

    //            //float r = Vector3.Distance(spaceObject.transform.position, objectsInSolarSystem[i].transform.position) * proportion;
    //            //float m1 = objectsInSolarSystem[i].RigidBody.mass * proportion;
    //            //float m2 = spaceObject.RigidBody.mass * proportion;
    //            //double force = (GConstant * m1 * m2 / Mathf.Pow(r, 2));
    //            Vector3 tempForce = CalculateForce(spaceObject, objectsInSolarSystem[i]);
    //            overallForce += tempForce;
    //            if(tempForce.magnitude > greatestForce)
    //            {
    //                greatestForce = tempForce.magnitude;
    //                objectWithGreatestForce = objectsInSolarSystem[i];
    //            }
    //            //double a = force / spaceObject.RigidBody.mass;
    //            //Vector3 direction = objectsInSolarSystem[i].transform.position - spaceObject.transform.position;

                
               
    //        }
            
    //    }
    //    if (start)
    //    {
    //        spaceObject.parentObject = objectWithGreatestForce;
    //        if (spaceObject.ObjectType != ObjectType.Sun)
    //        {
    //            Debug.Log(spaceObject.parentObject.startingVeloicty);
    //            float initialVelocity = Mathf.Sqrt((float)(GConstant * (spaceObject.parentOverride!=null ? (spaceObject.parentOverride.customMass) : (objectWithGreatestForce.customMass )) / (Vector3.Distance(objectWithGreatestForce.transform.position, spaceObject.transform.position))) * spaceObject.eValue) + (spaceObject.parentOverride!=null ? spaceObject.parentOverride.startingVeloicty : spaceObject.parentObject.startingVeloicty);
                
    //            spaceObject.startingVeloicty = initialVelocity;
    //            spaceObject.RigidBody.velocity = spaceObject.transform.forward * spaceObject.startingVeloicty;
    //            //spaceObject.RigidBody.AddRelativeForce(spaceObject.parentObject.RigidBody.velocity, ForceMode.VelocityChange);
    //        }
    //    }
        
           
    //    spaceObject.RigidBody.AddForce(overallForce, ForceMode.Acceleration);
        
    //    if(spaceObject.ObjectType != ObjectType.Sun)
    //    {
          
    //    }
        
    //}

}
public enum Integrationtypes
{
    SIEUler,
    VelVerlet,
    RK4,
    RKF45,
    FR,
    PEFRL
}