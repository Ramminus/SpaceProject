using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;


public class PlaceholderObject : MonoBehaviour
{

    bool zeroVelocity;
    [SerializeField]
    LayerMask mask;
    Renderer model;
    [SerializeField]
    Material backupSunMat, backupPlanetMat, backupMoonMat;
    [SerializeField]
    CustomPhysicsBody sunPrefab, planetPrefab, moonPrefab;
    public static PlaceholderObject instance;
    [SerializeField, ReadOnly]
    Vector3d worldPos;
    [SerializeField]
    LineRenderer selfLineRenderer;
    [SerializeField]
    TextMeshProUGUI distanceText;
    [SerializeField]
    Ellipse ellipse;
    SpaceObjectData currentData;
    CustomPhysicsBody currentParent;

    bool create;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private void Start()
    {
        model = GetComponentInChildren<Renderer>();
        gameObject.SetActive(false);
        ellipse.gameObject.SetActive(false);
    }
    public Material GetBackUpMat(ObjectType objectType)
    {
        if (objectType == ObjectType.Sun) return backupSunMat;
        if (objectType == ObjectType.Planet) return backupPlanetMat;
        if (objectType == ObjectType.Sun) return backupMoonMat;
        return null;

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CreateObject();
           
           
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gameObject.SetActive(false);
            distanceText.gameObject.SetActive(false);
            ellipse.gameObject.SetActive(false);
     
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            zeroVelocity = !zeroVelocity;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 40f, mask))
        {
            Vector3 pos = hit.point;
        
            pos.y = 0;
            transform.position = pos;
            GetWorldPos();
            //CustomPhysicsBody parent = GetParent();
            currentParent = GetParent();
            if (currentParent != null)
            {
                Vector3 dir = (transform.position - currentParent.transform.position);
                Vector3 surfacePosParent = currentParent.transform.position + (dir.normalized * currentParent.RenderRadiusScaled);
                dir = (transform.position - currentParent.transform.position);
                selfLineRenderer.SetPosition(0, transform.position);
                selfLineRenderer.SetPosition(1, surfacePosParent);

                //Vector3 mp = surfacePosParent + (dir * 0.5f);
                //Vector3 offset = Quaternion.Euler(0, 90, 0) * dir.normalized;
                //mp += offset * (float)(SolarSystemManager.startProportion/SolarSystemManager.instance.proportion  );
                distanceText.transform.position = Camera.main.WorldToScreenPoint(transform.position);
                distanceText.text = string.Format("{0:n0}", Mathd.Round(Vector3d.Distance(worldPos, currentParent.worldPos) * 0.001)) + " Km";
                CreateOrbitEllipse(currentParent);
            }
        }
        
        transform.localScale = SolarSystemManager.instance.transform.localScale;
    }
    public void SetPlaceholder(SpaceObjectData data)
    {
        model.transform.localScale = Vector3.one * CustomPhysicsBody.CalculateRadius(data.mass, data.density) / SolarSystemManager.startProportion;
        SolarSystemManager.instance.SetTargetScale(model.transform.localScale.x, 4f);
        model.material = data.customMaterial != null ? data.customMaterial : GetBackUpMat(data.ObjectType);
        gameObject.SetActive(true);
        distanceText.gameObject.SetActive(true);
        ellipse.gameObject.SetActive(true);
        currentData = data;
        zeroVelocity = false;


    }
    public void CreateObject()
    {
        CustomPhysicsBody body = null;
        if (currentData.ObjectType == ObjectType.Sun) body = sunPrefab;
        if (currentData.ObjectType == ObjectType.Planet) body = planetPrefab;
        if (currentData.ObjectType == ObjectType.Moon) body = moonPrefab;

        if (body == null) return;

        body = Instantiate(body, transform.position, Quaternion.identity);
        body.zeroVelocity = zeroVelocity;
        ObjectToAddData data = new ObjectToAddData(body, transform.position - currentParent.transform.position, currentParent, currentData);
        SolarSystemManager.instance.AddBodyToQueue(data);
        //body.PlaceInSolarsystem(worldPos, currentParent, currentData);
        //ellipse.transform.parent = currentParent.transform;

    }
    public CustomPhysicsBody GetParent()
    {
        for (int i = 0; i < SolarSystemManager.instance.objectsInSolarSystemByMass.Count; i++)
        {
            CustomPhysicsBody body = SolarSystemManager.instance.objectsInSolarSystemByMass[i];
            if (i != SolarSystemManager.instance.objectsInSolarSystemByMass.Count - 1)
            {
                if (Vector3d.Distance(worldPos, body.WorldPos) < body.HillSphere) return body;
            }
            else return body;
        }
        return null;
    }
    public void GetWorldPos()
    {
        if(SolarSystemManager.instance != null)
        {
            CustomPhysicsBody obj = SolarSystemManager.instance.MainObject;
            if (obj != null)
            {
                Vector3 direction = transform.position - obj.transform.position;
                //worldPos = new Vector3d(direction.normalized );
                worldPos  = obj.worldPos + new Vector3d(direction) * SolarSystemManager.instance.proportion;
            }
        
           
        }
    }
    public void CreateOrbitEllipse(CustomPhysicsBody parent)
    {
        float dist = Vector3.Distance(parent.transform.position, transform.position);
        float a = dist / (1 - currentData.e);
        float b = Mathf.Sqrt(1 - Mathf.Pow(currentData.e, 2)) * a;

        Vector3 ellipsePos = (parent.transform.position - transform.position);
        Vector3 center = transform.position + ellipsePos.normalized * a;

        ellipse.transform.position = center;
        ellipse.radius = new Vector2(a / 2, b / 2);
        ellipse.UpdateEllipse();

    }
}
