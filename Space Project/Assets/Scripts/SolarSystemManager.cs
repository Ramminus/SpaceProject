using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
/// <summary>
/// Main Class for Dealing with the initialization of the chosen solar system.
/// </summary>
public class SolarSystemManager : MonoBehaviour
{


    //Compute Shader Variables
    public bool usingCompute; //bool check to run simulation on GPU or CPU
    [SerializeField]
    ComputeShader shader; //Reference to the N-Body Shader
    int kernal;//int value of the kernal for the shader
    int updateKernal;
    int bufferSize = 64;//buffer size for the shader.
    ComputeBuffer planetBuffer;//Buffer for all the N-Bodies that effect each other.
    public ComputeBuffer PlanetBuffer => planetBuffer;
    [SerializeField]
    public PlanetDataCompute[] planetComputeData; //struct array to get the data from the GPU.


    //General Settings
    bool gridMode;//check to go into scale mode in the solar system
    public bool GridMode { get => gridMode; }

    public Transform sunLight; //Reference to the sun object if it isn't null/
    [SerializeField]
    bool useOneBody;//bool check to use only one body physics where other far away bodies don't effect each other.
    public bool UseOneBody { get => useOneBody; }

    public int BodyCount { get => objectsInSolarSystem.Count; } // property for the body count in the current solar system.
    [SerializeField]
    SolarSystemData solarSystemToLoad; // reference to the system that is being loaded
    [HideInInspector]
    public CustomPhysicsBody sun;// reference to the sun object.


    List<CustomPhysicsBody> planets;//list of all planets in the system
    List<CustomPhysicsBody> moons; // reference to all moons in the system
    [ReadOnly, SerializeField]
    List<CustomPhysicsBody> objectsInSolarSystem; // reference to all the bodies in the system

    public List<CustomPhysicsBody> ObjectsInSolarSystem { get => objectsInSolarSystem; }
    public List<CustomPhysicsBody> objectsInSolarSystemByMass;//sorted list by mass of all bodies
    bool cleanBuffer;//check to clean out buffer if a body has been destroyed
    public Vector3 rotation = Vector3.zero;// current rotation of the whole system.
    [SerializeField]
    float rotSpeed;//rotation speed
    [SerializeField]
    GameObject sunModel, planetModel, moonModel;//fall back models of each type of body
    

    [ReadOnly]
    public static double GConstant = 6.674E-11; //G Constant

    [SerializeField]
    public double proportion = 1000000; // solar system proportion scale
    public static float startProportion = 100000000f;

    float scale = 1; // Current Solar System Scale
    [SerializeField, ReadOnly]
    float targetScaleT = 0.5f;// target scale

    [SerializeField]
    private float scrollSpeed; //mouse wheel scale scroll speed
    [SerializeField]
    private float minScale; //Min scale of the solar system.
    [ReadOnly]
    public float timeSpeed = 1f; // current time speed in seconds
    float secondsPerSecond;
    public float SecondsPerSecond { get => SimulatorLoader.instance.paused ? 0 : secondsPerSecond; }

    CustomPhysicsBody mainObject; // reference to the main center object - Normally the sun.
    public CustomPhysicsBody MainObject { get => mainObject; }

    public float modelScale = 1; // Model Scale
    public Ellipse ellipse;// reference to orbit ellipse class
    [SerializeField]
    float addedCameraRayLength;//added distance for raycasting
    [SerializeField]
    Transform light;

    [SerializeField]
    Slider slider; // time slider.

    [SerializeField]
    float targetT;
    public Integrationtypes integrationtype;
    [SerializeField]
    float scalePow;

    Transform solarSystemParent;
    List<CustomPhysicsBody> removeList = new List<CustomPhysicsBody>();


    //Unaffecting Moon Variables

    ComputeShader moonCompute;
    bool unaffectedMoons;
    ComputeBuffer moonMasses;
    ComputeBuffer moonData;

    List<CustomPhysicsBody> unaffectedMoonsList;



    PosVelComputeData[] unaffectedMoonsData;

    double[] unaffectedMoonsMasses;
    int kernalMoon;

    //Events
    public static System.Action OnSetOrbitPath;
    public static System.Action OnSetOrbitTrail;

    public static System.Action<float> UpdateVelocityAndForcesBig;
    public static System.Action<float> UpdateVelocityAndForces;
    public static System.Action UpdatePositionBig;
    public static System.Action UpdatePosition;
    public static System.Action OnClearSolarSystem;
    public static System.Action ComputeRequestMassUpdate;
    public static System.Action UpdatedPlanetBuffer;
    public static System.Action RunComputeShader;
    public static System.Action<CustomPhysicsBody> DestroyBody;
    public static System.Action<CustomPhysicsBody, int> DestroyBodyWithIndex;
    public static System.Action<bool> OnToggleGrid;
    public static System.Action<bool> OnToggleOrbitPaths;
    public static System.Action<CustomPhysicsBody> OnAddObejctToSystem;
    [SerializeField]
    private int intervals = 20;


 

    public static SolarSystemManager instance;//Singleton reference.

    public void SetOrbitPath()
    {
        OnSetOrbitPath?.Invoke();
    }
    public void SetOrbitTrail()
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
        planetBuffer?.Dispose();
    }
    [Button]
    public void ChangeTimeScale(float newTimescale)
    {
        Time.timeScale = newTimescale;

        Time.fixedDeltaTime = 0.02f * newTimescale;
    }
    private void Start()
    {

        UiHandler.instance.timeSlider.onValueChanged.AddListener(ChangeSpeed);
        secondsPerSecond = 1f;
        targetScaleT = 1;
        SetUpSolarSystem();
        if (objectsInSolarSystem.Count == 0) return;
        if (usingCompute)
        {
            planetBuffer.SetData(planetComputeData);

            shader.SetBuffer(kernal, "dataBuffer", planetBuffer);

            if (unaffectedMoons)
            {
                shader.SetBuffer(kernalMoon, "unaffectedBuffer", moonData);
                shader.SetBuffer(kernalMoon, "unaffectedMasses", moonMasses);
            }
        }
    }
 
    public bool ToggleGpuCpu()
    {
        usingCompute = !usingCompute;
        return usingCompute;
    }
    public void ChangeSpeed(float t)
    {
        targetT = t;
    }
    public void ResetTimeSlider()
    {
        slider.value = 0;
    }
    private void Update()
    {

        //Update used for raycasts and user input.
        bool canScaleForward = true;
        bool canScaleBack = targetScaleT > minScale;


        secondsPerSecond *= 1 + targetT;
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, Camera.main.nearClipPlane + addedCameraRayLength))
        {
            canScaleForward = false;
            targetScaleT = scale;
        }
        scrollSpeed = Mathf.Pow(targetScaleT, scalePow);

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && canScaleForward || Input.GetAxis("Mouse ScrollWheel") < 0 && canScaleBack)
        {
            if (UiHandler.instance.scrollBlocker < 1)
            {
                targetScaleT += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            }


        }
        if (targetScaleT < minScale) targetScaleT = minScale;
        scale = Mathf.Lerp(scale, targetScaleT, 0.1f);
        SetScale(scale);
        light.LookAt((Vector3)PlanetCamera.instance.CameraRenderdPos, Vector3.up);
    }

    public void SetTargetScale(float modelScale, float targetScale, bool onlyIfGreaterThan = true)
    {
        float newScale = targetScale / (modelScale * 2);
        if (targetScaleT > newScale || !onlyIfGreaterThan)
        {
            targetScaleT = newScale;

            scale = targetScaleT;

        }
    }
    private void FixedUpdate()
    {
        if (!SimulatorLoader.instance.paused) 
        {
            timeSpeed = SecondsPerSecond * Time.fixedDeltaTime;
            if (objectsInSolarSystem.Count == 0) return;
            if (!usingCompute)
            {
                for (int i = 0; i < intervals; i++)
                {
                    UpdateVelocityAndForces?.Invoke((timeSpeed) / (float)intervals);
                    UpdatePosition?.Invoke();
                    CheckForCollisions();


                }
            }
            else
            {
                ComputeRequestMassUpdate?.Invoke();
                int length = planetComputeData.Length;
                if (unaffectedMoons) length += unaffectedMoonsData.Length;
                shader.SetFloat("timestep", timeSpeed);
             
                shader.Dispatch(kernal, length, 1, 1);
                RunComputeShader?.Invoke();
                planetBuffer.GetData(planetComputeData);
                int i = 0;
                foreach (PlanetDataCompute data in planetComputeData)
                {
                    if (!data.IsDestroyed() )
                    {
                        objectsInSolarSystem[i].worldPos = data.pos;
                        objectsInSolarSystem[i].velocity = data.velocity;
                        i++;
                    }
                }

                if (unaffectedMoons)
                {


                    int x = 0;
                    foreach (PosVelComputeData data in unaffectedMoonsData)
                    {

                        unaffectedMoonsList[x].worldPos = data.worldPos;
                        unaffectedMoonsList[x].velocity = data.velocity;
                        x++;

                    }
                }
              


            }
        }
        if (usingCompute)
        {
            RemoveDestroyedBodies();
            CheckForCollisions();
            if (cleanBuffer)
            {
                CleanUpComputeBuffer();
                cleanBuffer = false;
            }
        }

    }
    public void CleanUpComputeBuffer()
    {
        List<PlanetDataCompute> list = new List<PlanetDataCompute>(planetComputeData);
        int i = 0;

        while (i < list.Count)
        {
            if (list[i].IsDestroyed())
            {
                list.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }

        planetComputeData = list.ToArray();
        planetBuffer = new ComputeBuffer(planetComputeData.Length, bufferSize);
        shader.SetInt("number", planetComputeData.Length);
        planetBuffer.SetData(planetComputeData);
        shader.SetBuffer(kernal, "dataBuffer", planetBuffer);
        UpdatedPlanetBuffer?.Invoke();

    }
    public void UpdateComputeMass(CustomPhysicsBody body)
    {
        planetComputeData[objectsInSolarSystem.IndexOf(body)].mass = body.Mass;
        planetBuffer.SetData(planetComputeData);
        shader.SetBuffer(kernal, "dataBuffer", planetBuffer);
        UpdatedPlanetBuffer?.Invoke();



    }
    public void AddUnaffectedMoon(PlanetData planetData, CustomPhysicsBody planet, int moonIndex, int moonNumber)
    {
        CustomPhysicsBody moon = Instantiate(moonModel, Vector3.zero, Quaternion.identity, solarSystemParent).GetComponent<CustomPhysicsBody>();
        moon.data = planetData.moons[moonIndex];
        moon.SetParent(planet);
        Vector3 moonRot = Quaternion.Euler(0, 0, planet.data.axisTiltInDeg + moon.data.angleOfOrbit) * (Vector3.right);
        moon.worldPos = planet.WorldPos + new Vector3d((moonRot * (float)planetData.moons[moonIndex].avrgDistanceFromPlanet));
        moon.transform.LookAt(planet.transform);
        unaffectedMoonsList.Add(moon);
        moon.transform.position = new Vector3((float)(moon.worldPos.x / SolarSystemManager.instance.proportion), (float)(moon.worldPos.y / SolarSystemManager.instance.proportion), (float)(moon.worldPos.z / SolarSystemManager.instance.proportion));
        moon.Init();
        moon.CreateModel();
        unaffectedMoonsData[moonNumber] = new PosVelComputeData { worldPos = moon.WorldPos, velocity = moon.velocity };
        unaffectedMoonsMasses[moonNumber] = moon.data.mass;
        UiHandler.instance.OnAddBodyToSpace(moon);

    }
    public void AddObjectToSolarsystem(CustomPhysicsBody body)
    {
        objectsInSolarSystem.Add(body);
        UiHandler.instance.OnAddBodyToSpace(body);
        body.transform.parent = solarSystemParent;
        objectsInSolarSystemByMass = new List<CustomPhysicsBody>(objectsInSolarSystem);
        objectsInSolarSystemByMass.Sort();
        System.Array.Resize(ref planetComputeData, planetComputeData.Length + 1);
        planetComputeData[planetComputeData.Length - 1] = body.GetComputeData();
        planetBuffer = new ComputeBuffer(planetComputeData.Length, bufferSize);
        shader.SetInt("number", planetComputeData.Length);
        planetBuffer.SetData(planetComputeData);
        shader.SetBuffer(kernal, "dataBuffer", planetBuffer);
        UpdatedPlanetBuffer?.Invoke();
        OnAddObejctToSystem.Invoke(body);
    }
    public void CheckForCollisions()
    {
        for (int i = 0; i < objectsInSolarSystem.Count; i++)
        {
            for (int x = i + 1; x < objectsInSolarSystem.Count; x++)
            {
                bool hasCollided = HasCollided(objectsInSolarSystem[i], objectsInSolarSystem[x]);
                if (hasCollided)
                {
                    Debug.Log(objectsInSolarSystem[i].data.objectName + " and " + objectsInSolarSystem[x].data.objectName + " Has Collided");
                    HandleCollision(objectsInSolarSystem[i], objectsInSolarSystem[x]);
                    cleanBuffer = true;
                }
            }
        }
    }
    public bool HasCollided(CustomPhysicsBody b1, CustomPhysicsBody b2)
    {

        return (Vector3.Distance(b1.WorldToRender, b2.WorldToRender) <= b1.RenderRadiusScaled + b2.RenderRadiusScaled);
    }
    public void RemoveBody(CustomPhysicsBody body)
    {
        if (!removeList.Contains(body)) removeList.Add(body);
    }

    void RemoveDestroyedBodies()
    {
        for (int i = 0; i < removeList.Count; i++)
        {
            if (removeList[i] != null)
            {
                planetComputeData[objectsInSolarSystem.IndexOf(removeList[i])].destroyed = 1;
                DestroyBody?.Invoke(removeList[i]);
                int index = ObjectsInSolarSystem.IndexOf(removeList[i]);
                DestroyBodyWithIndex?.Invoke(removeList[i], index);
                objectsInSolarSystem.Remove(removeList[i]);
                removeList[i].SetAsDestroyed();
                cleanBuffer = true;
                
            }
        }
        removeList = new List<CustomPhysicsBody>();
    }
    public void HandleCollision(CustomPhysicsBody b1, CustomPhysicsBody b2)
    {
        if (b1.data.ObjectType == ObjectType.Sun)
        {
            b1.Consume(b2, 1f);
            planetComputeData[objectsInSolarSystem.IndexOf(b2)].destroyed = 1;
            DestroyBody?.Invoke(b2);
            int index = ObjectsInSolarSystem.IndexOf(b2);
            DestroyBodyWithIndex?.Invoke(b2, index);
            objectsInSolarSystem.Remove(b2);
            b2.SetAsDestroyed();

        }
        if (b2.data.ObjectType == ObjectType.Sun)
        {
            planetComputeData[objectsInSolarSystem.IndexOf(b1)].destroyed = 1;
            b2.Consume(b1, 1f);
            DestroyBody?.Invoke(b1);
            int index = ObjectsInSolarSystem.IndexOf(b1);
            DestroyBodyWithIndex?.Invoke(b1, index);
            objectsInSolarSystem.Remove(b1);
            b1.SetAsDestroyed();


        }

    }
    public void SetUpSolarSystem()
    {
        mainObject = null;
        objectsInSolarSystem = new List<CustomPhysicsBody>();
        int index = 0;
        int moonNumber = 0;

        solarSystemToLoad = SimulatorLoader.instance.solarSystemToLoad;

        unaffectedMoonsList = new List<CustomPhysicsBody>();
        solarSystemParent = new GameObject().GetComponent<Transform>();
        solarSystemParent.parent = transform;

        int length = solarSystemToLoad.maxNumberOfMoons;
        if (solarSystemToLoad.maxNumberOfMoons == 0 || solarSystemToLoad.maxNumberOfMoons > SimulatorLoader.instance.objectDatabase.moons.Length) length = SimulatorLoader.instance.objectDatabase.moons.Length;

        if (unaffectedMoons)
        {
            unaffectedMoonsData = new PosVelComputeData[length];
            unaffectedMoonsMasses = new double[length];
        }
        if (solarSystemToLoad.sun != null)
        {

            sun = Instantiate(sunModel, Vector3.zero, Quaternion.identity, solarSystemParent).GetComponent<CustomPhysicsBody>();

            sun.data = solarSystemToLoad.sun;
            sun.index = index;

            sun.Init();
            objectsInSolarSystem.Add(sun);
            index++;
            sun.CreateModel();
            mainObject = sun;
            sunLight.transform.parent = sun.transform;
            sunLight.localPosition = Vector3.zero;
            UiHandler.instance.OnAddBodyToSpace(sun);
        }
        else
        {
            sunLight.position = new Vector3(0, 0, -1000000);
        }
        planets = new List<CustomPhysicsBody>();
        moons = new List<CustomPhysicsBody>();
        for (int i = 0; i < solarSystemToLoad.planets.Length; i++)
        {
            CustomPhysicsBody planet = Instantiate(planetModel, Vector3.zero, Quaternion.identity, solarSystemParent).GetComponent<CustomPhysicsBody>();

            planet.data = solarSystemToLoad.planets[i];
            Vector3 rot = Quaternion.Euler(0, Random.Range(0f, 360f), planet.data.angleOfOrbit) * Vector3.right;
            planet.worldPos = new Vector3d(rot * (float)solarSystemToLoad.planets[i].avrgDistanceFromSun);

            Debug.Log(planet.transform.position);
            if (solarSystemToLoad.sun != null)
                planet.SetParent(sun);
            planet.transform.position = new Vector3((float)(planet.worldPos.x / SolarSystemManager.instance.proportion), (float)(planet.worldPos.y / SolarSystemManager.instance.proportion), (float)(planet.worldPos.z / SolarSystemManager.instance.proportion));
            //planet.transform.localScale *= modelScale;
            if (solarSystemToLoad.sun != null)
                planet.transform.LookAt(sun.transform);
            planet.transform.Rotate(Vector3.right, planet.data.axisTiltInDeg);
            planet.index = index;
            index++;
            objectsInSolarSystem.Add(planet);
            planet.Init();
            //planet.CreateOrbitEllipse();
            planets.Add(planet);
            planet.CreateModel();
            UiHandler.instance.OnAddBodyToSpace(planet);
            if (mainObject == null)
            {
                mainObject = planet;
            }
            PlanetData data = planet.data as PlanetData;

            if (solarSystemToLoad.includeMoons && (moonNumber < solarSystemToLoad.maxNumberOfMoons || solarSystemToLoad.maxNumberOfMoons == 0))
            {
                if (!unaffectedMoons)
                {
                    for (int x = 0; x < data.moons.Length; x++)
                    {

                        if (solarSystemToLoad.maxNumberOfMoons == moonNumber && solarSystemToLoad.maxNumberOfMoons != 0)
                        {
                            break;
                        }
                        CustomPhysicsBody moon = Instantiate(moonModel, Vector3.zero, Quaternion.identity, solarSystemParent).GetComponent<CustomPhysicsBody>();
                        moon.data = data.moons[x];
                        moon.SetParent(planet);
                       
                        Vector3 moonRot = Quaternion.Euler(0, Random.Range(0f, 360f), planet.data.axisTiltInDeg + moon.data.angleOfOrbit) * Vector3.right;
                        moon.worldPos = planet.WorldPos + new Vector3d((moonRot * (float)data.moons[x].avrgDistanceFromPlanet));
                        moon.index = index;
                        index++;
                        objectsInSolarSystem.Add(moon);
                        moon.transform.position = new Vector3((float)(moon.worldPos.x / SolarSystemManager.instance.proportion), (float)(moon.worldPos.y / SolarSystemManager.instance.proportion), (float)(moon.worldPos.z / SolarSystemManager.instance.proportion));
                        moon.transform.LookAt(planet.transform);
                        moon.Init();
                        moons.Add(moon);
                        moon.CreateModel();
                        UiHandler.instance.OnAddBodyToSpace(moon);
                        moonNumber++;
                    }
                }
                else
                {

                    for (int x = 0; x < data.moons.Length; x++)
                    {
                        if (solarSystemToLoad.maxNumberOfMoons == moonNumber && solarSystemToLoad.maxNumberOfMoons != 0)
                        {
                            break;
                        }
                        AddUnaffectedMoon(data, planet, x, moonNumber);
                        moonNumber++;
                    }
                }
            }
        }

   
        if (mainObject != null) mainObject.isMainObject = true;

        objectsInSolarSystemByMass = new List<CustomPhysicsBody>(objectsInSolarSystem);
        objectsInSolarSystemByMass.Sort();
        if (objectsInSolarSystem.Count == 0) return;
        if (usingCompute)
        {
            planetComputeData = new PlanetDataCompute[objectsInSolarSystem.Count];
            planetBuffer = new ComputeBuffer(planetComputeData.Length, bufferSize);

            kernal = shader.FindKernel("CSMain");

            shader.SetInt("number", planetComputeData.Length);
            for (int i = 0; i < objectsInSolarSystem.Count; i++)
            {
                CustomPhysicsBody obj = objectsInSolarSystem[i];
                planetComputeData[i] = new PlanetDataCompute { mass = obj.Mass, pos = obj.WorldPos, velocity = obj.velocity, index = obj.index, destroyed = 0 };
            }
            if (unaffectedMoons)
            {


                moonData = new ComputeBuffer(unaffectedMoonsData.Length, 56);
                moonMasses = new ComputeBuffer(unaffectedMoonsMasses.Length, 8);
                moonData.SetData(unaffectedMoonsData);
                moonMasses.SetData(unaffectedMoonsMasses);
    
            }
        }

        SimulatorLoader.instance.loaded = true;
        OrbitPathController.instance.Init();

    }
    public void SetComputeData(PlanetDataCompute data, int index)
    {
        planetComputeData[index] = data;
    }
    public void ClearSolarSystem()
    {
       
        light.parent = null;
        if(solarSystemParent != null) Destroy(solarSystemParent.gameObject);
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
        Vector3d direction = o2.WorldPos - o1.WorldPos;
        double r = Vector3d.Distance(o2.WorldPos, o1.WorldPos);
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
    public Vector3d CalculateForceAtPos(CustomPhysicsBody o1, CustomPhysicsBody o2, Vector3d pos, bool debug = false)
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
    [Button]
    public void ToggleGridMode()
    {
        gridMode = !gridMode;
        OnToggleGrid?.Invoke(gridMode);
        if (gridMode)
        {

            targetScaleT = scale = 1;
            List<CustomPhysicsBody> radiusSort = new List<CustomPhysicsBody>(objectsInSolarSystem);
            radiusSort.Sort(CustomPhysicsBody.RadiusComparison);

            Vector3 prevPos = Vector3.zero;
            for (int i = 0; i < radiusSort.Count; i++)
            {

                if (i == 0)
                {
                    radiusSort[i].gridPos = Vector3.zero;
                }
                else
                {
                    Vector3 pos = prevPos + (Vector3.forward * (radiusSort[i - 1].Model.transform.localScale.x)) + (Vector3.forward * (radiusSort[i].Model.transform.localScale.x));
                    radiusSort[i].gridPos = pos;
                    prevPos = pos;

                }
                radiusSort[i].InitiateGridMode();
            }
            PlanetCamera.instance.OnActivateGridMode(radiusSort[0]);
        }
        else
        {

        }

    }


}

public struct PosVelComputeData
{
    public Vector3d worldPos;
    public Vector3d velocity;
    public TBool destroyed;
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