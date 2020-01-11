using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Entities;
using System;

public class RingSimulator : MonoBehaviour
{

    [SerializeField]
    bool usingGPU;

    CustomPhysicsBody planet;

    int totalNumber;
    public Transform asteroidParent;
    public Transform AsteroidParent { get => asteroidParent; }
    [SerializeField] NBodyAsteroid objToSpawn;

    [SerializeField]
    RingData[] rings;
    [SerializeField]
    ComputeShader shader;

    [SerializeField]
    NBodyAsteroid[] asteroids;
    [SerializeField]
    AstroidDataCPU[] asteroidDataCPU;
    
    [SerializeField]

    ComputeBuffer asteroidBuffer;
   
    int kernalIndex;

    [Header("GPU Instancing Variables")]
    //GPU exlcusive parameters
    [SerializeField]
    Vector4[] asteroidPositions;
    [SerializeField]
    AstroidDataGPU[] asteroidDataGPU;
    ComputeBuffer positionBufferRender;
  
    [SerializeField]
    ComputeShader shaderGpu;


    public Mesh instanceMesh;
    public Material instanceMaterial;
    public int subMeshIndex = 0;

    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    public CustomPhysicsBody Planet { get => planet;  }

    private void Awake()
    {
        SolarSystemManager.UpdatedPlanetedBuffer += UpdatePlanetBuffer;
        SolarSystemManager.RunComputeShader += RunShaderCalculations;
    }
    private void OnDestroy()
    {
        SolarSystemManager.UpdatedPlanetedBuffer -= UpdatePlanetBuffer;
        SolarSystemManager.RunComputeShader -= RunShaderCalculations;
        asteroidBuffer.Dispose();
        positionBufferRender.Dispose();
    }

    private void Start()
    {
        if (usingGPU)
            CreateSpawnPointsCircleGPU();
        
        else
            CreateSpawnPointsCircleCPU();
    }
    [Button]
    public void Spawn()
    {

        //for (int x = 0; x < volume; x++)
        //{
        //    for (int y = 0; y < volume; y++)
        //    {
        //        for (int z = 0; z < volume; z++)
        //        {
        //            Vector3 pos = transform.position + new Vector3(x * spacing, y * spacing, z * spacing);

        //            instances.Add(Matrix4x4.TRS(pos, transform.rotation, transform.localScale));
        //        }
        //    }
        //}
        CreateSpawnPointsCircleCPU();
       

    }
    private void UpdatePlanetBuffer()
    {
        if (usingGPU)
        {
            shaderGpu.SetBuffer(kernalIndex, "planetBuffer", SolarSystemManager.instance.planetBuffer);
            shaderGpu.SetInt("number", SolarSystemManager.instance.planetComputeData.Length);
        }
        else
        {
            shader.SetBuffer(kernalIndex, "planetBuffer", SolarSystemManager.instance.planetBuffer);
            shader.SetInt("number", SolarSystemManager.instance.planetComputeData.Length);
        }
    }

    public void SetPlanet(CustomPhysicsBody planet)
    {
        this.planet = planet;
    }
    public void CreateSpawnPointsCircleCPU()
    {
        totalNumber = 0;
        foreach (RingData ring in rings)
        {
            totalNumber += ring.number;
        }
        asteroidParent = new GameObject().transform;
        asteroidParent.transform.parent = transform;
        asteroidParent.transform.localPosition = Vector3.zero;
        asteroids = new NBodyAsteroid[totalNumber];
        asteroidDataCPU = new AstroidDataCPU[totalNumber];
        Vector3 dir = Vector3.forward;
        int start = 0;
        Color[] colors = new Color[rings.Length];
        float[] numbers = new float[rings.Length];
        int index = 0;
        foreach (RingData ring in rings)
        {
            for (int i = start; i < ring.number + start; i++)
            {


                Vector3 pos =  dir * UnityEngine.Random.Range(ring.innerRadius + planet.Model.transform.localScale.x, ring.outerRadius + planet.Model.transform.localScale.x);
                pos *= SolarSystemManager.instance.transform.localScale.x;
                asteroids[i] = Instantiate(objToSpawn, planet.transform.position + pos, Quaternion.identity, asteroidParent.transform);
                asteroids[i].scale= UnityEngine.Random.Range(ring.minScale, ring.maxScale);
           
                asteroids[i].transform.LookAt(planet.transform);
                asteroidDataCPU[i] = new AstroidDataCPU { mass = 6.1E18, pos = planet.WorldPos + new Vector3d(pos * (float)SolarSystemManager.instance.proportion), velocity = Vector3d.zero };

                asteroidDataCPU[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocity(asteroids[i], asteroidDataCPU[i], planet, SolarSystemManager.instance.GConstant);
                // asteroidData[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocity(asteroids[i], asteroidData[i], worldPos, Vector3d.zero, thisData.mass, 6.674E-11);

                dir = Quaternion.Euler(0, ring.spacing, 0) * dir;
            }
            start += ring.number;
            colors[index] = ring.ringColour;
            numbers[index] = start;
            index++;
        }
        kernalIndex = shader.FindKernel("CSMain");
        asteroidBuffer = new ComputeBuffer(asteroidDataCPU.Length, 56);
        asteroidBuffer.SetData(asteroidDataCPU);
        //planetBuffer = new ComputeBuffer(, 64);
        //planetBuffer.SetData(planetData);
        shader.SetBuffer(kernalIndex, "astroidBuffer", asteroidBuffer);
        shader.SetBuffer(kernalIndex, "planetBuffer", SolarSystemManager.instance.planetBuffer);
        shader.SetInt("number", SolarSystemManager.instance.planetComputeData.Length);
    }
    public void CreateSpawnPointsCircleGPU()
    {
        instanceMaterial = Material.Instantiate(instanceMaterial);
        
        shaderGpu = (ComputeShader)Instantiate(Resources.Load(shaderGpu.name));
        totalNumber = 0;
        foreach (RingData ring in rings)
        {
            totalNumber += ring.number;
        }
        
        asteroidParent = new GameObject().transform;
        asteroidParent.transform.parent = transform;
        asteroidParent.transform.localPosition = Vector3.zero;
        //asteroids = new NBodyAsteroid[totalNumber];
        asteroidDataGPU = new AstroidDataGPU[totalNumber];
        Color[] colors = new Color[rings.Length];
        float[] numbers = new float[rings.Length];
        asteroidPositions = new Vector4[totalNumber];
        Vector3 dir = Vector3.forward;
        int start = 0;
        int index = 0;
        NBodyAsteroid asteroidGO = Instantiate(objToSpawn, Vector3.zero, Quaternion.identity, asteroidParent.transform);
        foreach (RingData ring in rings)
        {
            for (int i = start; i < ring.number + start; i++)
            {

                
                Vector3 pos = dir * UnityEngine.Random.Range(ring.innerRadius + planet.Model.transform.localScale.x, ring.outerRadius + planet.Model.transform.localScale.x);
                pos *= SolarSystemManager.instance.transform.localScale.x;
                asteroidGO.transform.position = planet.transform.position + pos;
                // asteroids[i].scale = UnityEngine.Random.Range(ring.minScale, ring.maxScale);
                float scale = UnityEngine.Random.Range(ring.minScale, ring.maxScale); 
                asteroidGO.transform.LookAt(planet.transform);
                asteroidDataGPU[i] = new AstroidDataGPU { mass = 6.1E18, pos = planet.WorldPos + new Vector3d(pos * (float)SolarSystemManager.instance.proportion), velocity = Vector3d.zero, scale = scale };

                asteroidDataGPU[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocityGPU(asteroidGO, asteroidDataGPU[i], planet, SolarSystemManager.instance.GConstant);
                asteroidPositions[i] = HelperFunctions.WorldToRenderPosition(asteroidDataGPU[i].pos);
                asteroidPositions[i].w = scale;
                // asteroidData[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocity(asteroids[i], asteroidData[i], worldPos, Vector3d.zero, thisData.mass, 6.674E-11);

                dir = Quaternion.Euler(0, ring.spacing, 0) * dir;
            }
            start += ring.number;
            colors[index] = ring.ringColour;
            numbers[index] = start;
            index++;
        }
        kernalIndex = shaderGpu.FindKernel("CSMain");
        asteroidBuffer = new ComputeBuffer(asteroidDataGPU.Length, 64);
       
        positionBufferRender = new ComputeBuffer(asteroidPositions.Length, 16);
        asteroidBuffer.SetData(asteroidDataGPU);
        positionBufferRender.SetData(asteroidPositions);
        //planetBuffer = new ComputeBuffer(, 64);
        //planetBuffer.SetData(planetData);
        shaderGpu.SetBuffer(kernalIndex, "astroidBuffer", asteroidBuffer);
        shaderGpu.SetBuffer(kernalIndex, "planetBuffer", SolarSystemManager.instance.planetBuffer);

        shaderGpu.SetBuffer(kernalIndex, "asteroidRenderPos", positionBufferRender);
        shaderGpu.SetInt("number", SolarSystemManager.instance.planetComputeData.Length);
        shaderGpu.SetFloat("proportion", (float)SolarSystemManager.instance.proportion);
        shaderGpu.SetVector("camerarenderPos", (Vector3)PlanetCamera.instance.CameraRenderdPos);
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        //int positionID = Shader.PropertyToID("positionBuffer");
        //instanceMaterial.SetColorArray("colours", colors);
        //instanceMaterial.SetFloatArray("numbers", numbers);
        //instanceMaterial.SetInt("length", numbers.Length);
        //Shader.SetGlobalBuffer(positionID, positionBufferRender);
        args[0] = (uint)instanceMesh.GetIndexCount(subMeshIndex);
        args[1] = (uint)totalNumber;
        args[2] = (uint)instanceMesh.GetIndexStart(subMeshIndex);
        args[3] = (uint)instanceMesh.GetBaseVertex(subMeshIndex);
        argsBuffer.SetData(args);
       

    }

    //public void SetAstroidVelocity(int i)
    //{
    //    asteroidData[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocity(asteroids[i], asteroidData[i], planet, SolarSystemManager.instance.GConstant);
     

    //}
    [Button]
    public void RunShaderCalculations()
    {
        if (usingGPU)
        {
            shaderGpu.SetFloat("proportion", (float)SolarSystemManager.instance.proportion);
            shaderGpu.SetVector("camerarenderPos", (Vector3)PlanetCamera.instance.CameraRenderdPos);
            shaderGpu.SetFloat("timestep", SolarSystemManager.instance.timeSpeed);
            shaderGpu.Dispatch(kernalIndex, totalNumber / 128, 1, 1);
            //positionBufferRender.GetData(asteroidPositions);
           // positionBufferWorld.GetData(asteroidPositionsWorld);
        }
        else
        {
            shader.SetFloat("timestep", SolarSystemManager.instance.timeSpeed);
            shader.Dispatch(kernalIndex, totalNumber / 128, 1, 1);
            asteroidBuffer.GetData(asteroidDataCPU);
        }
        

    }
    private void Update()
    {
        if (usingGPU)
        {
            instanceMaterial.SetBuffer("positionBuffer", positionBufferRender);
            Graphics.DrawMeshInstancedIndirect(instanceMesh, subMeshIndex, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer, 0,null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
        }
        for (int i = 0; i < asteroidDataCPU.Length; i++)
        {
            asteroids[i].transform.position = new Vector3((float)(asteroidDataCPU[i].pos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(asteroidDataCPU[i].pos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(asteroidDataCPU[i].pos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);
        }
    }

}
[System.Serializable]
public struct AstroidDataCPU
{
    public Vector3d pos;
    public double mass;
    public Vector3d velocity;




};
[System.Serializable]
public struct AstroidDataGPU
{
    public Vector3d pos;
    public double mass;
    public Vector3d velocity;
    public float scale;



};

[System.Serializable]
public struct RingData
{

    public int number;
    public float innerRadius, outerRadius;
    public float minScale, maxScale;
    public float spacing;
    public Color ringColour;

}