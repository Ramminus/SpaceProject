using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitPathController : MonoBehaviour
{

    [SerializeField]
    LineRenderer[] lineRenenderes;
    [SerializeField]
    GameObject lineRendererPrefab;
    float timer;
    [SerializeField]
    float updateTime = 0.5f;
    [SerializeField]
    int maxPositions = 50;
    CustomPhysicsBody targetObject;
    bool clearPoints;
    public static OrbitPathController instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        SolarSystemManager.OnAddObejctToSystem += AddNewRenderer; 
        SolarSystemManager.DestroyBodyWithIndex += RemoveLineRenderer;
        PlanetCamera.OnSetNewFocus += OnChangeFocus;
    }
    private void OnDestroy()
    {
        if (instance == this) instance = null;
        SolarSystemManager.OnAddObejctToSystem -= AddNewRenderer;
        SolarSystemManager.DestroyBodyWithIndex -= RemoveLineRenderer;
        PlanetCamera.OnSetNewFocus -= OnChangeFocus;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init()
    {
        lineRenenderes = new LineRenderer[SolarSystemManager.instance.ObjectsInSolarSystem.Count];
        for (int i = 0; i < lineRenenderes.Length; i++)
        {
            lineRenenderes[i] = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
            lineRenenderes[i].transform.parent = transform;
            lineRenenderes[i].positionCount = 0;
            lineRenenderes[i].endColor = SolarSystemManager.instance.ObjectsInSolarSystem[i].data.orbitPathColour;
            
        }
        timer = updateTime;
        targetObject = SolarSystemManager.instance.MainObject;
    }
    public void AddNewRenderer(CustomPhysicsBody body)
    {
        List<LineRenderer> tempList = new List<LineRenderer>(lineRenenderes);
        LineRenderer lr = Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
        lr.transform.parent = transform;
        lr.positionCount = 0;
        lr.transform.localScale = Vector3.one;
        tempList.Add(lr);
        lineRenenderes = tempList.ToArray();


    }
    void RemoveLineRenderer(CustomPhysicsBody body, int index)
    {
        List<LineRenderer> tempList = new List<LineRenderer>(lineRenenderes);
        Destroy(tempList[index]);
        tempList.RemoveAt(index);

        lineRenenderes = tempList.ToArray();
    }
    public void ClearAllPoints()
    {
        for (int i = 0; i < lineRenenderes.Length; i++)
        {
            lineRenenderes[i].positionCount = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (timer > 0) timer -= Time.deltaTime;
        else
        {
            timer = updateTime;
            for (int i = 0; i < lineRenenderes.Length; i++)
            {
                Vector3[] pos = new Vector3[lineRenenderes[i].positionCount + 1];
               
                lineRenenderes[i].GetPositions(pos);
                if (pos.Length > maxPositions)
                {
                    List<Vector3> tempList = new List<Vector3>(pos);
                    tempList.RemoveAt(0);
                    pos = tempList.ToArray();
                    tempList.Clear();

                }
                Vector3 objWorldPos =  SolarSystemManager.instance.ObjectsInSolarSystem[i].transform.localPosition;
                pos[pos.Length - 1] =  objWorldPos;
                lineRenenderes[i].positionCount = pos.Length;
                lineRenenderes[i].SetPositions(pos);
            }
        }
        if (clearPoints)
        {
            clearPoints = false;
            ClearAllPoints();
        }



    }

    void OnChangeFocus(CustomPhysicsBody body)
    {
        clearPoints = true;
    }


}
