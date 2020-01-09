using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Entities;
using System;

public class RingSimulator : MonoBehaviour
{


    CustomPhysicsBody planet;

    int totalNumber;

    [SerializeField] NBodyAsteroid objToSpawn;

    [SerializeField]
    RingData[] rings;
    [SerializeField]
    ComputeShader shader;

    [SerializeField]
    NBodyAsteroid[] asteroids;
    [SerializeField]
    AstroidData[] asteroidData;
    [SerializeField]

    ComputeBuffer asteroidBuffer;
   
    int kernalIndex;

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
    }

    private void Start()
    {
        CreateSpawnPointsCircle();
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
        CreateSpawnPointsCircle();


    }
    private void UpdatePlanetBuffer()
    {
        shader.SetBuffer(kernalIndex, "planetBuffer", SolarSystemManager.instance.planetBuffer);
        shader.SetInt("number", SolarSystemManager.instance.planetComputeData.Length);
    }

    public void SetPlanet(CustomPhysicsBody planet)
    {
        this.planet = planet;
    }
    public void CreateSpawnPointsCircle()
    {
        totalNumber = 0;
        foreach (RingData ring in rings)
        {
            totalNumber += ring.number;
        }
        GameObject parent = new GameObject();
        parent.transform.parent = transform;
        parent.transform.localPosition = Vector3.zero;
        asteroids = new NBodyAsteroid[totalNumber];
        asteroidData = new AstroidData[totalNumber];
        Vector3 dir = Vector3.forward;
        int start = 0;
        foreach (RingData ring in rings)
        {
            for (int i = start; i < ring.number + start; i++)
            {


                Vector3 pos =  dir * UnityEngine.Random.Range(ring.innerRadius + planet.Model.transform.localScale.x, ring.outerRadius + planet.Model.transform.localScale.x);
                pos *= SolarSystemManager.instance.transform.localScale.x;
                asteroids[i] = Instantiate(objToSpawn, planet.transform.position + pos, Quaternion.identity, parent.transform);
                asteroids[i].scale= UnityEngine.Random.Range(ring.minScale, ring.maxScale);
           
                asteroids[i].transform.LookAt(planet.transform);
                asteroidData[i] = new AstroidData { mass = 6.1E18, pos = planet.WorldPos + new Vector3d(pos * (float)SolarSystemManager.instance.proportion), velocity = Vector3d.zero };
                
                asteroidData[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocity(asteroids[i], asteroidData[i], planet, SolarSystemManager.instance.GConstant);
                // asteroidData[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocity(asteroids[i], asteroidData[i], worldPos, Vector3d.zero, thisData.mass, 6.674E-11);

                dir = Quaternion.Euler(0, ring.spacing, 0) * dir;
            }
            start = ring.number;
        }
        kernalIndex = shader.FindKernel("CSMain");
        asteroidBuffer = new ComputeBuffer(asteroidData.Length, 56);
        asteroidBuffer.SetData(asteroidData);
        //planetBuffer = new ComputeBuffer(, 64);
        //planetBuffer.SetData(planetData);
        shader.SetBuffer(kernalIndex, "astroidBuffer", asteroidBuffer);
        shader.SetBuffer(kernalIndex, "planetBuffer", SolarSystemManager.instance.planetBuffer);
        shader.SetInt("number", SolarSystemManager.instance.planetComputeData.Length);
    }
    public void SetAstroidVelocity(int i)
    {
        asteroidData[i].velocity = CustomPhysicsBody.GetAstroidInitialVelocity(asteroids[i], asteroidData[i], planet, SolarSystemManager.instance.GConstant);
     

    }
    [Button]
    public void RunShaderCalculations()
    {
        shader.SetFloat("timestep", SolarSystemManager.instance.timeSpeed);
        shader.Dispatch(kernalIndex, totalNumber / 128, 1, 1);
        asteroidBuffer.GetData(asteroidData);
        for (int i = 0; i < asteroidData.Length; i++)
        {
            asteroids[i].transform.position = new Vector3((float)(asteroidData[i].pos.x / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.x, (float)(asteroidData[i].pos.y / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.y, (float)(asteroidData[i].pos.z / SolarSystemManager.instance.proportion) - (float)PlanetCamera.instance.CameraRenderdPos.z);
        }

    }
    
}
[System.Serializable]
public struct AstroidData
{
    public Vector3d pos;
    public double mass;
    public Vector3d velocity;




};

[System.Serializable]
public struct RingData
{

    public int number;
    public float innerRadius, outerRadius;
    public float minScale, maxScale;
    public float spacing;

}