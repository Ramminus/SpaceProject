using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCamera : MonoBehaviour
{

    [SerializeField]
    CustomPhysicsBody lockedTo;
    CustomPhysicsBody showingStatsFor;
    [SerializeField]
    float rotationSpeed;
    public static PlanetCamera instance;
    [SerializeField]
    float heightOffset;
    Vector3d worldPos;
    bool freeCam;
    [SerializeField]
    float freeCamSpeed;
    float currentCamSpeed;
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(lockedTo != null)
            {
                SolarSystemManager.instance.SetTargetScale(lockedTo.Model.transform.localScale.x, 4f, false);
            }
        }
        if (SolarSystemManager.instance != null)
        {
            currentCamSpeed = (float)SolarSystemManager.instance.proportion * freeCamSpeed;
            if (lockedTo != null)
            {
                worldPos = lockedTo.worldPos;
            }
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                if (lockedTo != null) lockedTo.isFocus = false;
                lockedTo = null;
                freeCam = true;

                worldPos += new Vector3d(Input.GetAxis("Horizontal") * currentCamSpeed, 0, Input.GetAxis("Vertical") * currentCamSpeed);
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {


                transform.RotateAround(Vector3.zero, transform.TransformDirection(Vector3.up), Input.GetAxis("Mouse X") * rotationSpeed);
                transform.RotateAround(Vector3.zero, transform.TransformDirection(Vector3.left), Input.GetAxis("Mouse Y") * rotationSpeed);
            }
        }


    }

    public Vector3d CameraWorldPos { 
        get {
            if (freeCam) return worldPos;
            return lockedTo != null ? lockedTo.WorldPos : Vector3d.zero; 
        } 
    }
    public Vector3d CameraRenderdPos { 
        get {
            if (freeCam) return worldPos / SolarSystemManager.instance.proportion;
            return lockedTo != null ?  lockedTo.WorldPos / SolarSystemManager.instance.proportion : Vector3d.zero; 
        } 
    }

    public void SetFocusAndStats(CustomPhysicsBody newFocus, bool onStart =false)
    {
        Stats.instance.SetStatPage(newFocus);
        

        if (showingStatsFor == newFocus || onStart)
        {
            if (lockedTo != null) lockedTo.isFocus = false;
            lockedTo = newFocus;
            worldPos = lockedTo.WorldPos;
            worldPos.y = 0;
            freeCam = false;
            lockedTo.isFocus = true;
            SolarSystemManager.instance.SetTargetScale(newFocus.Model.transform.localScale.x, 4f);
        }
        showingStatsFor = newFocus;
    }
    public void SetFocus(CustomPhysicsBody newFocus)
    {


        if (lockedTo != null) lockedTo.isFocus = false;
        lockedTo = newFocus;
        worldPos = lockedTo.WorldPos;
        worldPos.y = 0;
        freeCam = false;
        lockedTo.isFocus = true;
        SolarSystemManager.instance.SetTargetScale(newFocus.Model.transform.localScale.x, 4f);

    }

}
