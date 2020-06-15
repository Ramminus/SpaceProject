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


    public static System.Action<CustomPhysicsBody> OnSetNewFocus;
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
        if (Input.GetKeyDown(KeyCode.Delete))
        {

            if (showingStatsFor != null) UiHandler.instance.InitDeletePanel(showingStatsFor.data.objectName);
        }
        if (SolarSystemManager.instance != null)
        {
            currentCamSpeed = (float)SolarSystemManager.instance.proportion * freeCamSpeed;
            if (lockedTo != null )
            {
               
                    worldPos = lockedTo.worldPos;
            }
           
            if (Input.GetKey(KeyCode.Mouse1) && !SolarSystemManager.instance.GridMode)
            {

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (lockedTo != null) lockedTo.isFocus = false;
                    lockedTo = null;
                    freeCam = true;
                    float yMovment = SolarSystemManager.instance.GridMode ? 0 : -Input.GetAxis("Mouse Y") * currentCamSpeed;
                    worldPos += new Vector3d(transform.TransformDirection(new Vector3(-Input.GetAxis("Mouse X") * currentCamSpeed, yMovment , 0 )));
                    if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                    {
                        OnSetNewFocus.Invoke(null);
                    }
                }
                else if(!SolarSystemManager.instance.GridMode)
                {
                    transform.RotateAround(Vector3.zero, transform.TransformDirection(Vector3.up), Input.GetAxis("Mouse X") * rotationSpeed);
                    transform.RotateAround(Vector3.zero, transform.TransformDirection(Vector3.left), Input.GetAxis("Mouse Y") * rotationSpeed);
                }
            }
        }


    }
    public void DeleteBodies()
    {
        if (showingStatsFor != null) SolarSystemManager.instance.RemoveBody(showingStatsFor);
    }
    public Vector3d CameraWorldPos { 
        get {
            if (SolarSystemManager.instance.GridMode) return worldPos;
            if (freeCam) return worldPos;
            return lockedTo != null ? lockedTo.WorldPos : Vector3d.zero; 
        } 
    }
    public Vector3d CameraRenderdPos { 
        get {
            if (lockedTo != null && SolarSystemManager.instance.GridMode)return new Vector3d(lockedTo.gridPos);
            else if (freeCam) return worldPos / SolarSystemManager.instance.proportion;
            return lockedTo != null ?  lockedTo.WorldPos / SolarSystemManager.instance.proportion : Vector3d.zero; 
        } 
    }
    public void OnActivateGridMode(CustomPhysicsBody largestObject)
    {
        transform.position = new Vector3(8, 0, 0);
        transform.LookAt(Vector3.zero);
        SetFocus(largestObject);
    }
    public void SetFocusAndStats(CustomPhysicsBody newFocus, bool onStart =false)
    {
        Stats.instance.SetStatPage(newFocus);
        

        if (showingStatsFor == newFocus || onStart)
        {
            SetFocus(newFocus);
            OnSetNewFocus?.Invoke(newFocus);
        }
        showingStatsFor = newFocus;
    }
    public void SetFocus(CustomPhysicsBody newFocus)
    {


        if (lockedTo != null) lockedTo.isFocus = false;
        lockedTo = newFocus;
        OnSetNewFocus?.Invoke(newFocus);
        
        worldPos = lockedTo.WorldPos;
        worldPos.y = 0;
        freeCam = false;
        lockedTo.isFocus = true;
        SolarSystemManager.instance.SetTargetScale(newFocus.Model.transform.localScale.x, 4f);

    }

}
