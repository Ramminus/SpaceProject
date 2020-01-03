using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCamera : MonoBehaviour
{

    [SerializeField]
    CustomPhysicsBody lockedTo;
    [SerializeField]
    float rotationSpeed;
    public static PlanetCamera instance;
    [SerializeField]
    float heightOffset;


    public static System.Action OnUpdateCameraPos;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {


            transform.RotateAround(Vector3.zero, transform.TransformDirection(Vector3.up), Input.GetAxis("Mouse X") * rotationSpeed);
            transform.RotateAround(Vector3.zero, transform.TransformDirection(Vector3.left), Input.GetAxis("Mouse Y") * rotationSpeed);
        }
    }

    public Vector3d CameraWorldPos { get { return lockedTo != null ? lockedTo.WorldPos : Vector3d.zero; } }
    public Vector3d CameraRenderdPos { get { return lockedTo != null ?  lockedTo.WorldPos / SolarSystemManager.instance.proportion : Vector3d.zero; } }

    public void SetFocus(CustomPhysicsBody newFocus)
    {
        if(lockedTo != null)lockedTo.isFocus = false;
        lockedTo = newFocus;
        Stats.instance.SetStatPage(newFocus);
        lockedTo.isFocus = true;
    }

}
