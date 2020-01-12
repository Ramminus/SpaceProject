using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UiHandler : MonoBehaviour
{
    [SerializeField]
    Transform focusParent;
    [SerializeField]
    GameObject focusPrefab;
    [SerializeField]
    NameTag nametagPrefab;
    [SerializeField]
    RectTransform nameTagParent;
    public Slider timeSlider;
    [SerializeField]
    RectTransform loaderButtonParent;

    [SerializeField]
    LoaderButton loadButtonPrefab;

    [SerializeField]
    RectTransform toolTipParent;
    [SerializeField]
    TextMeshProUGUI tooltipText;


    [SerializeField]
    RectTransform addPanelParent;
    [SerializeField]
    AddNewButton newButtonPrefab;


    [SerializeField]
    TextMeshProUGUI secondsPerSecondText;
    public int scrollBlocker = 0;
    public static UiHandler instance;
    
    public RectTransform LoaderButtonParent { get => loaderButtonParent; }
    public RectTransform ToolTipParent { get => toolTipParent;  }

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        for (int i = 0; i < SimulatorLoader.instance.GetNumberOfSolarSytems(); i++)
        {
           LoaderButton button =  (LoaderButton)Instantiate(loadButtonPrefab, loaderButtonParent);
            button.SetIndex(i);
        }
        LoaderButtonParent.gameObject.SetActive(false);
        LoadAddObject(ObjectType.Sun);
    }
    private void Update()
    {
        if(SolarSystemManager.instance != null)
        {
            ConvertedUnit unit = Stats.GetTimeUnit(SolarSystemManager.instance.SecondsPerSecond);
            secondsPerSecondText.text = unit.value.ToString() + " " + (TimeUnits)unit.unitIndex+"/s";
        }
    }
    public void OnAddBodyToSpace(CustomPhysicsBody body)
    {
        GameObject button =  Instantiate(focusPrefab, focusParent);
        button.GetComponent<FocusButton>().attachedBody = body;
        NameTag tag = Instantiate(nametagPrefab, nameTagParent);
        tag.SetNametag(body);
    }
    public void SetTooltip(string text, Vector2 mousePos)
    {
        Vector2 pivot = toolTipParent.pivot;
        if (mousePos.x > Screen.width / 2) pivot.x = 1;
        else pivot.x = 0;
        toolTipParent.pivot = pivot;
        Toggle(toolTipParent.gameObject);
        toolTipParent.transform.position = mousePos;
        tooltipText.text = text;
    }

    public void Toggle(GameObject go)
    {
        
        go.SetActive(!go.activeSelf);
    }
    public void Enable(GameObject go)
    {
        
        go.SetActive(true);
    }
    public void Disable(GameObject go)
    {

        go.SetActive(false);
    }
    public void ResetTimeSlider()
    {
        timeSlider.value = 0;
    }
    public void LoadAddObject(ObjectType objectType)
    {
        SpaceObjectData[] data =  new SpaceObjectData[0];
        if (objectType == ObjectType.Sun) data = SimulatorLoader.instance.objectDatabase.suns;
        if (objectType == ObjectType.Planet) data = SimulatorLoader.instance.objectDatabase.planets;
        if (objectType == ObjectType.Moon) data = SimulatorLoader.instance.objectDatabase.moons;
        DestroyAllChildren(addPanelParent);
        int i = 0;
        foreach(SpaceObjectData obj in data)
        {
            AddNewButton button = Instantiate(newButtonPrefab, addPanelParent);
            button.SetButton(i, obj);
            i++;
        }

    }
    public void LoadAddObjectInt(int objectTypeInt)
    {
        ObjectType objectType = (ObjectType)objectTypeInt;

        LoadAddObject(objectType);

    }
    public void DestroyAllChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    public void ClickDebug()
    {
        Debug.Log("Clicked");
    }
    public void ToggleGridView()
    {
        SolarSystemManager.instance.ToggleGridMode();
    }
}
