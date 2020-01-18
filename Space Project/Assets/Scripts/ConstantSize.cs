using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSize : MonoBehaviour
{
    [SerializeField]
    float scale = 1.0f;
    

    // Update is called once per frame
    void Update()
    {
        transform.localScale = (Vector3.one * scale) / SolarSystemManager.instance.transform.localScale.x;
    }
}
