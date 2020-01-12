using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PeriapsisCalc : EditorWindow

{
    float eccentricity;
    double semiMajorAxis;
    double periapsis;
    string objectName;
    [MenuItem("Window/PeriapsisCalc")]
    public static void ShowWindow()
    {
        GetWindow<PeriapsisCalc>("Calculator");


    }

    private void OnGUI()
    {
        eccentricity =  EditorGUILayout.FloatField("Eccentricity", eccentricity);
        semiMajorAxis =  EditorGUILayout.DoubleField("Semi_Major Axis", semiMajorAxis);
       
        
        GUILayout.Label(periapsis.ToString());
        
        objectName = EditorGUILayout.TextField("Object Name", objectName);
        if (GUILayout.Button("Set"))
        {
           
            periapsis = (1 - eccentricity) * semiMajorAxis * 1000;
            MoonData data = Resources.Load<MoonData>("Objects/" + objectName);
            if (data != null)
            {
                data.avrgDistanceFromPlanet = periapsis;
                EditorUtility.SetDirty(data);
                Debug.Log("Set the periapsis of " + objectName + " to " + periapsis.ToString());
            }
            else
            {
                Debug.LogError("Object with name " + objectName + " could not be found");
            }
            objectName = "";

        }
        
    }
}
