using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FocusButton : MonoBehaviour
{
    [HideInInspector]
    public CustomPhysicsBody attachedBody;
    [SerializeField]
    TextMeshProUGUI tmp;

    private void Start()
    {
        if (attachedBody != null) tmp.text = attachedBody.data.name;
    }
    public void SetCameraFocus()
    {
        PlanetCamera.instance.SetFocus(attachedBody);
    }
}
