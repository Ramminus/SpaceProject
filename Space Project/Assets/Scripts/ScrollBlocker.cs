using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ScrollBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool added;
    public void OnPointerEnter(PointerEventData eventData)
    {
        UiHandler.instance.scrollBlocker++;
        added = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UiHandler.instance.scrollBlocker--;
        added = false;
    }
    private void OnDisable()
    {
        if (!added) return;
        UiHandler.instance.scrollBlocker--;
        added = false;
    }
    private void OnDestroy()
    {
        if (!added) return;
        UiHandler.instance.scrollBlocker--;
        added = false;
    }
}
