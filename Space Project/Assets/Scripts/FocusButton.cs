using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FocusButton : MonoBehaviour
{
    
    public CustomPhysicsBody attachedBody;
    [SerializeField]
    TextMeshProUGUI tmp;
    private void Awake()
    {
        SolarSystemManager.DestroyBody += DestroyObject;
        SolarSystemManager.OnClearSolarSystem += DestroyObject;
    }
    private void OnDestroy()
    {
        SolarSystemManager.DestroyBody -= DestroyObject;
        SolarSystemManager.OnClearSolarSystem -= DestroyObject;
    }
    public void DestroyObject(CustomPhysicsBody body)
    {
        if (body == attachedBody) Destroy(gameObject);
    }
    public void DestroyObject()
    {
         Destroy(gameObject);
    }
    private void Start()
    {
        if (attachedBody != null) tmp.text = attachedBody.data.name;
        else DestroyObject();
    }
    public void SetCameraFocusAndStats()
    {
        PlanetCamera.instance.SetFocusAndStats(attachedBody);
    }
    public void SetCameraFocus()
    {
        PlanetCamera.instance.SetFocus(attachedBody);
    }
}
