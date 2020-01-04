using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHandler : MonoBehaviour
{
    [SerializeField]
    Transform focusParent;
    [SerializeField]
    GameObject focusPrefab;
    [SerializeField]
    NameTag nametagPrefab;
    [SerializeField]
    RectTransform nameTagParent;

    public static UiHandler instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public void OnAddBodyToSpace(CustomPhysicsBody body)
    {
        GameObject button =  Instantiate(focusPrefab, focusParent);
        button.GetComponent<FocusButton>().attachedBody = body;
        NameTag tag = Instantiate(nametagPrefab, nameTagParent);
        tag.SetNametag(body);
    }



}
