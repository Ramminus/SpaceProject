using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSolarSystem", menuName = "Create/Solar System")]
public class SolarSystemData : ScriptableObject
{
    public SunData sun;
    public PlanetData[] planets;
    public bool includeMoons;
    public bool includeRings;
}
