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

    [Button]
    public void GetMoons()
    {
        MoonData[] allMoons = Resources.LoadAll<MoonData>("Objects");
        List<MoonData> foundMoons = new List<MoonData>();
        foreach(MoonData moon in allMoons)
        {
            if(moon.planetId == ID)
            {
                foundMoons.Add(moon);
            }
        }
        moons = foundMoons.ToArray(); 
    }
}

