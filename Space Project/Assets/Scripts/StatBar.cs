using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBar : MonoBehaviour
{
    [SerializeField]
    TMP_Dropdown dropDown;


    public void FillUnits(string[] names)
    {
        dropDown.ClearOptions();
        
        dropDown.AddOptions(new List<string>(names));
    }
}
