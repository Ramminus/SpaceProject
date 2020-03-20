using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GpuCpuSwitch : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI buttonText;
    public void Toggle()
    {
        if(SolarSystemManager.instance != null)
        {
            bool usingGpu = SolarSystemManager.instance.ToggleGpuCpu();
            buttonText.text = usingGpu ? "GPU" : "CPU";
        }
    }
}
