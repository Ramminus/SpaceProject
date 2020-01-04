using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    StatTypes barType;

    float updatePeriod, updateTimer;
    private void Start()
    {
        
        
        
    }
    private void Update()
    {
        if (updatePeriod == 0) return;
        updateTimer -= Time.unscaledDeltaTime;
        if(updateTimer <= 0)
        {
            UpdateInfo(Stats.instance.CurrentBody);
            updateTimer = updatePeriod;
        }
    }
    public string GetStatText(CustomPhysicsBody body)
    {
        if (barType == StatTypes.Mass) return "Mass";
        if (barType == StatTypes.OrbitalVelocity) return "Orbital Velocity";
        if (body.Parent != null)
        {
            if (barType == StatTypes.DistanceToParent) return "Distance To " + body.Parent.data.objectName;
        }
        return "";
    }
    public void SetBar(CustomPhysicsBody selectedBody)
    {
        UpdateInfo(selectedBody);
      
    }
    public void UpdateInfo(CustomPhysicsBody selectedBody)
    {
        Debug.Log("UPDATED");
        statText.text = GetStatText(selectedBody);
        if(barType == StatTypes.Mass)
        {
            ConvertedUnit unit = Stats.GetMassUnit(selectedBody.Mass);
          
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
            ConvertedUnit unit = Stats.GetDistanceUnit(selectedBody.DistanceFromParent);
            
            valueText.text = unit.value.ToString();
            dropDown.value = unit.unitIndex;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Stats.instance.statBarParent);
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
    public void OnDropDownChanged(int value)
    {
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
    }
    public void FillUnits(string[] names)
    {
        dropDown.ClearOptions();
        
        dropDown.AddOptions(new List<string>(names));
    }
}
