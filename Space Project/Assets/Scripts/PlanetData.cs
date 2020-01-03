using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


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

