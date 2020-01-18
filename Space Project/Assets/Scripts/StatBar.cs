using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Sirenix.OdinInspector;
public class StatBar : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI statText;
    [SerializeField]
    TMP_InputField valueText;
    [SerializeField]
    TMP_Dropdown dropDown;
    [SerializeField]
    GameObject editPanel;
    [SerializeField]
    Button editButton;
    [SerializeField]
    Slider editSlider;
    [SerializeField]
    TMP_InputField addInput, removeInput;
    [SerializeField, ReadOnly]
    StatTypes barType;
    bool editing;
    int unit = -1;

    float updatePeriod, updateTimer;


    private void Start()
    {
        addInput.text = "0.1";
        addInput.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        removeInput.text = "0.1";
        removeInput.characterValidation = TMP_InputField.CharacterValidation.Decimal;

        if (!SimulatorLoader.instance.loaded) gameObject.SetActive(false);
       
    }
    private void Update()
    {
        if (!SimulatorLoader.instance.loaded) return;
        if (editing)
        {
            OnEdit(GetSliderValue() * Time.unscaledDeltaTime);
        }
        if (updatePeriod == 0) return;
        updateTimer -= Time.unscaledDeltaTime;
        if(updateTimer <= 0)
        {
            UpdateInfo(Stats.instance.CurrentBody);
            updateTimer = updatePeriod;
        }
        
    }

    private void OnEdit(float v)
    {
        if(barType == StatTypes.Mass)
        {
            Stats.instance.CurrentBody.ChangeMass(v);
            UpdateInfo(Stats.instance.CurrentBody);
        }
    }
    public float GetSliderValue()
    {
        if (editSlider.value > 0) return editSlider.value * float.Parse(addInput.text);
        if (editSlider.value < 0) return editSlider.value * float.Parse(removeInput.text);
        return 0;
    }
    public string GetStatText(CustomPhysicsBody body)
    {
        if (!SimulatorLoader.instance.loaded)
        {
            gameObject.SetActive(false);
            return "";
        };
        gameObject.SetActive(true);
        if (barType == StatTypes.Mass) return "Mass";
        if (barType == StatTypes.OrbitalVelocity) return "Orbital Velocity";
        if (body != null &&  body.Parent != null)
        {
            if (barType == StatTypes.DistanceToParent) return "Distance To " + body.Parent.data.objectName;
        }
        if (barType == StatTypes.OrbitPeriod) return "Orbit Period";
        return "";
    }
    public void SetBar(CustomPhysicsBody selectedBody)
    {
        unit = -1;
        UpdateInfo(selectedBody);
      
    }
    public void UpdateInfo(CustomPhysicsBody selectedBody)
    {
       
        statText.text = GetStatText(selectedBody);
        if(barType == StatTypes.Mass)
        {
            ConvertedUnit unit = GetValueAndUnit(selectedBody, StatTypes.Mass); 
          
            valueText.text = unit.value.ToString();
            dropDown.value = unit.unitIndex;
        }
        else if (barType == StatTypes.OrbitalVelocity)
        {
            
            gameObject.SetActive(selectedBody.data.ObjectType != ObjectType.Sun);
            if (selectedBody.data.ObjectType == ObjectType.Sun) return;
            double value = selectedBody.OrbitalVelocity;
            value *= 100;
            value = Mathd.Round(value);
            value *= 0.01d;
            valueText.text = value.ToString();
        }
        else if(barType == StatTypes.DistanceToParent)
        {
            gameObject.SetActive(selectedBody.data.ObjectType != ObjectType.Sun);
            if (selectedBody.data.ObjectType == ObjectType.Sun) return;
            ConvertedUnit unit = GetValueAndUnit(selectedBody, StatTypes.DistanceToParent);

            valueText.text = unit.value.ToString();
            dropDown.value = unit.unitIndex;
        }
        else if (barType == StatTypes.OrbitPeriod)
        {
            gameObject.SetActive(selectedBody.data.ObjectType != ObjectType.Sun);
            if (selectedBody.data.ObjectType == ObjectType.Sun) return;
            ConvertedUnit unit = GetValueAndUnit(selectedBody, StatTypes.OrbitPeriod);

            valueText.text = unit.value.ToString();
            dropDown.value = unit.unitIndex;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Stats.instance.statBarParent);
    }
    public ConvertedUnit GetValueAndUnit(CustomPhysicsBody selectedBody ,StatTypes type)
    {
        if(unit == -1)
        {
            if (barType == StatTypes.Mass) return Stats.GetMassUnit(selectedBody.Mass);
            if (barType == StatTypes.DistanceToParent) return Stats.GetDistanceUnit(selectedBody.DistanceFromParent);
            if (barType == StatTypes.OrbitPeriod) return Stats.GetTimeUnit(selectedBody.OrbitPeriod);

        }
        else
        {
            if (barType == StatTypes.Mass) return Stats.GetMassUnit(selectedBody.Mass, (MassUnits)unit);
            if (barType == StatTypes.DistanceToParent) return Stats.GetDistanceUnit(selectedBody.DistanceFromParent, (DistanceUnits) unit);
            if (barType == StatTypes.OrbitPeriod) return Stats.GetTimeUnit(selectedBody.OrbitPeriod, (TimeUnits)unit);
        }
        return new ConvertedUnit();
    }
    public void SetType(StatTypes statType, int updatePeriod)
    {
        barType = statType;
        this.updatePeriod = updatePeriod;
        this.updateTimer = updatePeriod;
        editButton.gameObject.SetActive(Stats.instance.editable[statType]);
        if(barType == StatTypes.OrbitalVelocity ||
            barType == StatTypes.DistanceToParent)
        {
            gameObject.SetActive(false);
        }
    }
    public void ToggleEditPanel()
    {
        editPanel.SetActive(!editPanel.activeSelf);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Stats.instance.statBarParent);
    }

    internal void SetUpEvents()
    {
        Stats.OnSelectNewBody += SetBar;
        dropDown.onValueChanged.AddListener(OnDropDownChanged);
        
    }
    public void ResetSlider()
    {
        editing = false;
        editSlider.value = 0;
    }
    public void OnBeginDragSlider()
    {
        editing = true;
    }
    public void OnDropDownChanged(int value)
    {
        unit = value;
        if(barType == StatTypes.Mass)
        {
            ConvertedUnit unit = Stats.GetMassUnit(Stats.instance.CurrentBody.Mass, (MassUnits)value );
            
            valueText.text =  unit.value.ToString();
            
        }
        else if(barType == StatTypes.DistanceToParent)
        {
            ConvertedUnit unit = Stats.GetDistanceUnit(Stats.instance.CurrentBody.DistanceFromParent, (DistanceUnits)value);
            
            valueText.text = unit.value.ToString();
        }
        else if (barType == StatTypes.OrbitPeriod)
        {
            ConvertedUnit unit = Stats.GetTimeUnit(Stats.instance.CurrentBody.OrbitPeriod, (TimeUnits)value);

            valueText.text = unit.value.ToString();
        }
    }
    public void FillUnits(string[] names)
    {
        dropDown.ClearOptions();
        
        dropDown.AddOptions(new List<string>(names));
    }
}
