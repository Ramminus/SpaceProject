using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    TextMeshProUGUI title;
    TextMeshProUGUI body;
    CustomPhysicsBody currentBody;


    public void SetStatPage(CustomPhysicsBody physBody)
    {
        currentBody = physBody;
        title.text = physBody.data.objectName;

        string bodyText = "Mass: " + physBody.data.mass +"\n";
        bodyText += "Diameter: " + physBody.data.diameter +" Kg\n";
        bodyText += "Distance from " + physBody.Parent.data.name+": " + Mathd.Round(physBody.DistanceFromParent/100)/10 +" Km\n";
        bodyText += "Oribtal Period: " + FormatOrbitPeriod(physBody.OrbitPeriod)+"\n";

    }


    public string FormatOrbitPeriod(double orbitPeriod)
    {
        double tempPeriod;
        string returnString = orbitPeriod.ToString() + " Secs";
        tempPeriod = orbitPeriod/3600;
        if (tempPeriod >= 1) returnString =Mathd.Round(tempPeriod/10) * 10 + " Hours";
        else return returnString;
        tempPeriod /= 24;
        if (tempPeriod >= 1) returnString = Mathd.Round(tempPeriod / 10) * 10 + " Days";
        else return returnString;
        tempPeriod /= 365;
        if(tempPeriod >=1) returnString = Mathd.Round(tempPeriod / 10) * 10 + " Years";
        
        return returnString;



    }
}
