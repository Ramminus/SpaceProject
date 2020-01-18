using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewSolarSystem", menuName = "Create/Solar System")]
public class SolarSystemData : ScriptableObject
{
    public SunData sun;
    public PlanetData[] planets;
    public bool includeMoons;
    [ShowIf("includeMoons")]
    public int maxNumberOfMoons = 5;
    public bool includeRings;
}
