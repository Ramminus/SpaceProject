using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class LoaderButton : MonoBehaviour
{
    int index;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(LoadSolarSystem);
        string str = SimulatorLoader.instance.GetSolarSystemByIndex(index).name;
        str = Regex.Replace(str, "(\\B[A-Z])", " $1");
        GetComponentInChildren<TextMeshProUGUI>().text = str;
    }

    public void LoadSolarSystem()
    {
        SimulatorLoader.instance.LoadSolarSystem(index);
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }
}
