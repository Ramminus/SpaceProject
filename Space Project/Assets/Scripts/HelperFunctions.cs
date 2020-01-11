using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
    public static Vector3 WorldToRenderPosition(Vector3d worldPos)
    {
        return (Vector3)(worldPos - PlanetCamera.instance.CameraWorldPos) / (float)SolarSystemManager.instance.proportion;
    }
    public static Vector3d RenderToWorldPosition(Vector3 renderPos)
    {
        return new Vector3d((renderPos)  ) * SolarSystemManager.instance.proportion - PlanetCamera.instance.CameraWorldPos;
    }
}
