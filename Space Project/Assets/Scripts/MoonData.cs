using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoon", menuName = "Create/Moon")]
public class MoonData : SpaceObjectData
{
    public double avrgDistanceFromPlanet;

    public MoonData()
    {
        objectType = ObjectType.Moon;
    }
}
