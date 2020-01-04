using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class Stats : SerializedMonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI title;
    [SerializeField]
    TextMeshProUGUI body;
    CustomPhysicsBody currentBody;
    public CustomPhysicsBody CurrentBody { get => currentBody; }
    float updateTimer = 1;
    [SerializeField]
    StatBar statBarPrefab;
    
    public Transform statBarParent;

    public static Stats instance;
    [SerializeField]
    Dictionary<StatTypes, int> UpdatePeriods;
    
    public Dictionary<StatTypes, bool> editable;

    //Static Units
    //Mass
    static double EarthMass = 5.972e+24;
    static double SunMass = 1.989e+30;


    //Distance
    static double AU = 1.496e+8;

    public static System.Action<CustomPhysicsBody> OnSelectNewBody;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        StatTypes[] statTypes = (StatTypes[])System.Enum.GetValues(typeof(StatTypes));
        for (int i = 0; i < statTypes.Length; i++)
        {
            InitStatBar(statTypes[i]);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)statBarParent);
    }
    public void InitStatBar(StatTypes statType)
    {
        StatBar bar = (StatBar)Instantiate(statBarPrefab, statBarParent);
        bar.SetType(statType, UpdatePeriods[statType]);
        bar.SetUpEvents();
        if (statType == StatTypes.Mass) bar.FillUnits(System.Enum.GetNames(typeof(MassUnits)));
        else if (statType == StatTypes.OrbitalVelocity) bar.FillUnits(System.Enum.GetNames(typeof(SpeedUnits)));
        else if (statType == StatTypes.DistanceToParent) bar.FillUnits(System.Enum.GetNames(typeof(DistanceUnits)));


    }
    private void Update()
    {
        //if(currentBody != null)
        //{
        //    updateTimer -= Time.deltaTime;
        //    if (updateTimer <= 0) UpdateStats();
        //}
    }
    public void SetStatPage(CustomPhysicsBody physBody)
    {
        currentBody = physBody;
        title.text = physBody.data.objectName;

        //string bodyText = "Mass: " + physBody.data.mass + " Kg \n\n";
        //bodyText += "Diameter: " + physBody.data.diameter + " Km\n\n";
        //if (currentBody.data.ObjectType != ObjectType.Sun)
        //{
        //    bodyText += "Distance from " + physBody.Parent.data.name + ": " + Mathd.Round(physBody.DistanceFromParent * 10) / 10 + " Km \n\n";
        //    bodyText += "Oribtal Period: " + FormatOrbitPeriod(physBody.OrbitPeriod) + " \n\n";
        //    bodyText += "Orbital Velocity: " + Mathd.Round(physBody.OrbitalVelocity / 100) / 100 + "Km/s \n\n";
        //}

        //body.text = bodyText;
        OnSelectNewBody?.Invoke(physBody);
    }

    public void UpdateStats()
    {
        //string bodyText = "Mass: " + currentBody.data.mass + "Kg \n\n";
        //bodyText += "Diameter: " + currentBody.data.diameter + " Km\n\n";
        //if (currentBody.data.ObjectType != ObjectType.Sun)
        //{
        //    bodyText += "Distance from " + currentBody.Parent.data.name + ": " + Mathd.Round(currentBody.DistanceFromParent / 100) / 10 + " Km \n\n";
        //    bodyText += "Oribtal Period: " + FormatOrbitPeriod(currentBody.OrbitPeriod) + " \n\n";
        //    bodyText += "Orbital Velocity: " + Mathd.Round(currentBody.OrbitalVelocity * 10) / 10 + "Km/s \n\n";
        //}
        //body.text = bodyText;
        //updateTimer = 1;
    }
    public string FormatOrbitPeriod(double orbitPeriod)
    {
        double tempPeriod;
        string returnString = orbitPeriod.ToString() + " Secs";
        tempPeriod = orbitPeriod/3600;
        if (tempPeriod >= 1) returnString =Mathd.Round(tempPeriod*10) / 10 + " Hours";
        else return returnString;
        tempPeriod /= 24;
        if (tempPeriod >= 1) returnString = Mathd.Round(tempPeriod *10) / 10 + " Days";
        else return returnString;
        tempPeriod /= 365;
        if(tempPeriod >=1) returnString = Mathd.Round(tempPeriod * 10) / 10 + " Years";
        
        return returnString;



    }

    
    public static ConvertedUnit GetMassUnit(double value, bool rounded = true)
    {
        if(value/SunMass >= 1)
        {
            return new ConvertedUnit { unitIndex = 2, value = rounded ?  RoundMassValue( value / SunMass, MassUnits.Sun) : value / SunMass };
        }
        if (value / EarthMass >= 1)
        {
            return new ConvertedUnit { unitIndex = 1, value = rounded ? RoundMassValue(value / EarthMass, MassUnits.Earth) : value / EarthMass };
        }
        return   new ConvertedUnit { unitIndex = 0, value = rounded ? RoundMassValue(value , MassUnits.Kg) : value  };
    }
    public static ConvertedUnit GetMassUnit(double value, MassUnits unit, bool rounded = true)
    {
        if (unit == MassUnits.Sun)
        {
            return new ConvertedUnit { unitIndex = 2, value = rounded ? RoundMassValue(value / SunMass, MassUnits.Sun) : value / SunMass };
        }
        if (unit == MassUnits.Earth)
        {
            return new ConvertedUnit { unitIndex = 1, value = rounded ? RoundMassValue(value / EarthMass, MassUnits.Earth) : value / EarthMass };
        }
        return new ConvertedUnit { unitIndex = 0, value = rounded ? RoundMassValue(value, MassUnits.Kg) : value };
    }
    public static ConvertedUnit GetDistanceUnit(double value, bool round = true)
    {
       
        if (value / AU >= 1)
        {
            
            return new ConvertedUnit { unitIndex = (int)DistanceUnits.Au, value = round ? RoundDistanceValue(value / AU, DistanceUnits.Au) :  value / AU};
        }

        return new ConvertedUnit { unitIndex = 0, value = round ? RoundDistanceValue(value, DistanceUnits.Km) : value };
    }
    public static ConvertedUnit GetDistanceUnit(double value, DistanceUnits unit, bool round = true)
    {
        if (unit == DistanceUnits.Au)
        {
            return new ConvertedUnit { unitIndex = (int)DistanceUnits.Au, value = round ? RoundDistanceValue(value / AU, DistanceUnits.Au) : value / AU };
        }
        
        return new ConvertedUnit { unitIndex = 0, value = round ? RoundDistanceValue(value , DistanceUnits.Km) : value  };
    }


    static double RoundMassValue(double value, MassUnits units)
    {
        double roundFactor = 0;
        
        if (units == MassUnits.Sun) roundFactor = 10000;
        if (units == MassUnits.Earth) roundFactor = 1000;

        double roundedValue = value * roundFactor;
        roundedValue = Mathd.Round(roundedValue);
        roundedValue = roundedValue / roundFactor;
        if(units == MassUnits.Kg)
        {
            double nums = Mathd.Floor(Mathd.Log10(value) + 1);
            Debug.Log(nums);
            if(nums > 8)
            {
                nums -= 4;
                nums = Mathd.Pow(10, nums);
                roundedValue = value / nums;
                roundedValue = Mathd.Round(roundedValue);
                roundedValue *= nums;
            }
        }
        return roundedValue;
    }
    static double RoundDistanceValue(double value, DistanceUnits units)
    {
        double roundFactor = 0;
        if (units == DistanceUnits.Au) roundFactor = 1000;
        if (units == DistanceUnits.Km) roundFactor = 1;
      

        double roundedValue = value * roundFactor;
        roundedValue = Mathd.Round(roundedValue);
        roundedValue = roundedValue / roundFactor;
        return roundedValue;
    }
}
public struct ConvertedUnit
{
    public int unitIndex;
    public double value;
}
public enum MassUnits
{
    
    Kg,
    Earth,
    Sun
}
public enum DistanceUnits
{
    Km,
    Au

}
public enum SpeedUnits
{
    Kmps,
}
public enum StatTypes
{
    Mass,
    DistanceToParent,
    OrbitalVelocity
}