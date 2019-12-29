using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SolarSystemManager : MonoBehaviour
{

    [SerializeField]
    SolarSystemData solarSystemToLoad;
    [HideInInspector]
    public CustomPhysicsBody sun;
    List<CustomPhysicsBody> planets;
    List<CustomPhysicsBody> moons;
    [ReadOnly, SerializeField]
    List<CustomPhysicsBody> objectsInSolarSystem;
   
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
    [SerializeField]
    float smoothing = 0.3f;
    float scale = 1;
    [SerializeField, ReadOnly]
    float targetScale = 1;
    [SerializeField]
    private AnimationCurve scrollSpeedCurve;
    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private float minScale, maxScale;
    
    public float timeSpeed =1f;


    public float modelScale =1;
    public Ellipse ellipse;
    [SerializeField]
    float addedCameraRayLength;
    [SerializeField]
    Transform light;
    private void Awake()
    {
        if (instance == null) instance = this;

     
    }
    [Button]
    public void ChangeTimeScale(float newTimescale)
    {
        Time.timeScale = newTimescale;
    }
    private void Start()
    {

        SetUpSolarSystem();
    }
    private void Update()
    {

        bool canScaleForward = true;
        bool canScaleBack = targetScale > minScale;

        RaycastHit hit;
        Ray ray =Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        if(Physics.Raycast(ray,Camera.main.nearClipPlane + addedCameraRayLength))
        {
            canScaleForward = false;
            targetScale = scale;
        }
        
        Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && canScaleForward || Input.GetAxis("Mouse ScrollWheel") < 0 && canScaleBack)
        {
            float scrollSpeedMultiplier = scrollSpeed;
            if ((Input.GetAxis("Mouse ScrollWheel") > 0)) scrollSpeedMultiplier *= targetScale;
            else
            {
                if (targetScale > .5f)
                {
                    scrollSpeedMultiplier *= targetScale * 2;
                }
            }
            if (scrollSpeedMultiplier < 0.1f) scrollSpeedMultiplier = 0.1f;

            targetScale += Input.GetAxis("Mouse ScrollWheel")   * scrollSpeedMultiplier;
        }
        if (targetScale < minScale) targetScale = minScale;
        scale = Mathf.Lerp(scale, targetScale, smoothing);
        SetScale(scale);
       
       
        light.LookAt((Vector3)PlanetCamera.instance.CameraRenderdPos,Vector3.up);
    }
    public void SetUpSolarSystem(){
        sun = Instantiate(sunModel, Vector3.zero, Quaternion.identity, transform).GetComponent<CustomPhysicsBody>();
        sun.transform.localScale *= modelScale;
        sun.data = solarSystemToLoad.sun;
        sun.CreateModel();
        UiHandler.instance.OnAddBodyToSpace(sun);
        planets = new List<CustomPhysicsBody>();
        moons = new List<CustomPhysicsBody>();
        for (int i = 0; i < solarSystemToLoad.planets.Length; i++)
        {
            CustomPhysicsBody planet = Instantiate(planetModel, Vector3.right * (float)(solarSystemToLoad.planets[i].avrgDistanceFromSun / proportion), Quaternion.identity, transform).GetComponent<CustomPhysicsBody>();
            planet.data = solarSystemToLoad.planets[i];
            planet.SetParent(sun);
            planet.transform.localScale *= modelScale;
            planets.Add(planet);
            planet.CreateModel();
            UiHandler.instance.OnAddBodyToSpace(planet);
            PlanetData data = planet.data as PlanetData;
            for (int x = 0; x < data.moons.Length; x++)
            {
                CustomPhysicsBody moon = Instantiate(moonModel, planet.transform.position + Vector3.right * (float)(solarSystemToLoad.planets[i].moons[x].avrgDistanceFromPlanet / proportion), Quaternion.identity, transform).GetComponent<CustomPhysicsBody>();
                moon.data = data.moons[x];
                moon.SetParent(planet);
                moon.transform.localScale *= modelScale;
                moons.Add(moon);
                moon.CreateModel();
                UiHandler.instance.OnAddBodyToSpace(moon);
            }
        }
        objectsInSolarSystem = new List<CustomPhysicsBody>();
        objectsInSolarSystem.Add(sun);
        objectsInSolarSystem.AddRange(planets);
        objectsInSolarSystem.AddRange(moons);
        PlanetCamera.instance.SetFocus(sun);

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
    public Vector3d CalculateForce(CustomPhysicsBody o1, CustomPhysicsBody o2, bool debug = false)
    {


        Vector3d direction = o2.WorldPos  - o1.WorldPos;
        double r =Vector3d.Distance(o2.WorldPos, o1.WorldPos)  ;

        double m1 = o1.data.mass;

        double m2 = o2.data.mass;

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
