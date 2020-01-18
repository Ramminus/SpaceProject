using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class Ellipse : MonoBehaviour
{
    public CustomPhysicsBody owner;
    public Vector2 radius = new Vector2(1f, 1f);
    public float width = 1f;
    public float rotationAngle = 45;
    public int resolution = 500;

    float scaleAlphaZero;
    float scaleAlphaOne;

    private Vector3[] positions;
    public  LineRenderer self_lineRenderer;

    Color start, end;

    private void Update()
    {
        //float a= Mathf.Clamp(  Mathf.InverseLerp(scaleAlphaZero, scaleAlphaOne, SolarSystemManager.instance.transform.localScale.z),0,1);
        //start.a = a;
        
        //self_lineRenderer.startColor = start;
        

        
    }

    private void Awake()
    {
        //SolarSystemManager.OnSetOrbitPath += () => gameObject.SetActive(true);
        //SolarSystemManager.OnSetOrbitTrail += () => gameObject.SetActive(false);

        SolarSystemManager.DestroyBody += DestroyObject;
        //SolarSystemManager.OnClearSolarSystem += DestroyObject;

    }
    private void OnDestroy()
    {
        //SolarSystemManager.OnSetOrbitPath -= () => gameObject.SetActive(true);
       // SolarSystemManager.OnSetOrbitTrail -= () => gameObject.SetActive(false);

        SolarSystemManager.DestroyBody -= DestroyObject;
       // SolarSystemManager.OnClearSolarSystem -= DestroyObject;
    }
    private void Start()
    {
        start = self_lineRenderer.startColor;
        end = self_lineRenderer.endColor;
        
        
    }
    
    public void DestroyObject(CustomPhysicsBody body)
    {
        if (body == owner && body != null) Destroy(gameObject);
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    [Button]
    public void UpdateEllipse()
    {
        if (owner != null)
        {
            if (owner.data.ObjectType == ObjectType.Moon)
            {
                scaleAlphaZero = 2f; scaleAlphaOne = 4f;
            }
            else
            {
                scaleAlphaZero = .0001f;
                scaleAlphaOne = .0004f;
            }

        }
        if (self_lineRenderer == null)
            self_lineRenderer = GetComponent<LineRenderer>();

        self_lineRenderer.SetVertexCount(resolution + 3);

        //self_lineRenderer.SetWidth(width, width);


        AddPointToLineRenderer(0f, 0);
        for (int i = 1; i <= resolution + 1; i++)
        {
            AddPointToLineRenderer((float)i / (float)(resolution) * 2.0f * Mathf.PI, i);
        }
        AddPointToLineRenderer(0f, resolution + 2);
    }

    void AddPointToLineRenderer(float angle, int index)
    {
        Quaternion pointQuaternion = Quaternion.AngleAxis(rotationAngle, Vector3.up);
        Vector3 pointPosition;

        pointPosition = new Vector3(radius.x * Mathf.Cos(angle), 0.0f, radius.y * Mathf.Sin(angle) );
        pointPosition = pointQuaternion * pointPosition;

        self_lineRenderer.SetPosition(index, pointPosition);
    }
}