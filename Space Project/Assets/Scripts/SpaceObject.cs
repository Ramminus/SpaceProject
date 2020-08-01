using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpaceObject : MonoBehaviour
{
    //// Newtonian Gravity Formula:
    //// F1 = F2 = G x m1 x m2 / r^2
    //// m1 = mass of less massive object, m2 = mass of more massive object

   
  
 
    //[SerializeField]
    //ObjectType objectType;
    //public ObjectType ObjectType { get { return objectType; } }
    //Rigidbody thisRb;

    //[SerializeField]
    //public float eValue = 1f;

    //public double customMass;
    //public SpaceObject parentObject;
    //public SpaceObject parentOverride;
    //public float startingVeloicty;
    //public Rigidbody RigidBody
    //{
    //    get
    //    {
    //        if (thisRb == null)
    //        {
    //            thisRb = GetComponent<Rigidbody>();
    //        }
    //        return thisRb;
    //    }
    //}
   
    
   
    //[Button]
    //public void Follow()
    //{
    //    Camera.main.transform.position = transform.position;

    //    Camera.main.transform.Translate  (new Vector3(1, 1, 0) * (transform.localScale.x * SolarSystemManager.instance.cameraDistance + SolarSystemManager.instance.cameraDeadZone));
    //    Camera.main.transform.parent = transform;

        
    //    Camera.main.transform.LookAt(transform);

    //}
    //[Button]
    //public void GetGravForce(SpaceObject other)
    //{

        
       
    //}
}
public enum ObjectType
{
    Sun,
    Planet,
    Moon
}