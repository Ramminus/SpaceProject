using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpaceObjectData : ScriptableObject
{
    public string objectName;
    public double mass;
    public double diameter;
    [SerializeField, ReadOnly]
    protected ObjectType objectType;
    public ObjectType ObjectType { get => objectType; }
    public float e;
    public Color32 orbitPathColour =  new Color32(1,1,1,1);
    public GameObject customModel;




}

[CreateAssetMenu(fileName ="NewPlanet", menuName = "Create/Planet")]
public class PlanetData : SpaceObjectData
{
    public double avrgDistanceFromSun;
    public MoonData[] moons;

    public PlanetData()
    {
        objectType = ObjectType.Planet;
    }
}

[CreateAssetMenu(fileName ="NewSolarSystem", menuName = "Create/Solar System")]
public class SolarSystemData : ScriptableObject
{
    public SunData sun;
    public PlanetData[] planets;
}