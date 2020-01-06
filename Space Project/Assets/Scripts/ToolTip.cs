using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    string toolTipText;
    public void OnPointerEnter(PointerEventData eventData)
    {
        UiHandler.instance.SetTooltip(toolTipText, eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UiHandler.instance.Toggle(UiHandler.instance.ToolTipParent.gameObject);   
    }

    
}
