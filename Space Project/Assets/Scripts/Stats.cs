using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI title;
    [SerializeField]
    TextMeshProUGUI body;
    CustomPhysicsBody currentBody;
    float updateTimer = 1;


    public static Stats instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Update()
    {
        if(currentBody != null)
        {
            updateTimer -= Time.deltaTime;
            if (updateTimer <= 0) UpdateStats();
        }
    }
    public void SetStatPage(CustomPhysicsBody physBody)
    {
        currentBody = physBody;
        title.text = physBody.data.objectName;

        string bodyText = "Mass: " + physBody.data.mass + " Kg \n\n";
        bodyText += "Diameter: " + physBody.data.diameter + " Km\n\n";
        if (currentBody.data.ObjectType != ObjectType.Sun)
        {
            bodyText += "Distance from " + physBody.Parent.data.name + ": " + Mathd.Round(physBody.DistanceFromParent * 10) / 10 + " Km \n\n";
            bodyText += "Oribtal Period: " + FormatOrbitPeriod(physBody.OrbitPeriod) + " \n\n";
            bodyText += "Orbital Velocity: " + Mathd.Round(physBody.OrbitalVelocity / 100) / 100 + "Km/s \n\n";
        }

        body.text = bodyText;

    }

    public void UpdateStats()
    {
        string bodyText = "Mass: " + currentBody.data.mass + "Kg \n\n";
        bodyText += "Diameter: " + currentBody.data.diameter + " Km\n\n";
        if (currentBody.data.ObjectType != ObjectType.Sun)
        {
            bodyText += "Distance from " + currentBody.Parent.data.name + ": " + Mathd.Round(currentBody.DistanceFromParent / 100) / 10 + " Km \n\n";
            bodyText += "Oribtal Period: " + FormatOrbitPeriod(currentBody.OrbitPeriod) + " \n\n";
            bodyText += "Orbital Velocity: " + Mathd.Round(currentBody.OrbitalVelocity * 10) / 10 + "Km/s \n\n";
        }
        body.text = bodyText;
        updateTimer = 1;
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
}
