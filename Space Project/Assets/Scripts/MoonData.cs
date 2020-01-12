using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoon", menuName = "Create/Moon")]
public class MoonData : SpaceObjectData
{
    public int planetId;
    public double avrgDistanceFromPlanet;

    public MoonData()
    {
        objectType = ObjectType.Moon;
    }
}
