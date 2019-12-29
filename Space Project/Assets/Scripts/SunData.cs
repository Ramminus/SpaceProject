using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSun", menuName = "Create/Sun")]
public class SunData : SpaceObjectData
{



    public SunData()
    {
        objectType = ObjectType.Sun;
    }
}
