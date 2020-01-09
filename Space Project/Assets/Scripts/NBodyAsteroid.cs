using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBodyAsteroid : MonoBehaviour
{

    
    [SerializeField]
    Transform model;
    float rotationSpeed;
    public float scale;



    private void Start()
    {
        model.transform.rotation = Random.rotation;
        rotationSpeed = Random.Range(1f, 30f);
        transform.localScale =Vector3.one *  (scale * SolarSystemManager.instance.transform.localScale.x);
       
        //transform.LookAt(transform.parent);
    }
    private void Update()
    {
        //model.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
