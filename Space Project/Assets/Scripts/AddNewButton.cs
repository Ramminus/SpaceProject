﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class AddNewButton : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    Image icon;
    [SerializeField, ReadOnly]
    int index;
    ObjectType objectType;

    public void SetButton(int index, SpaceObjectData data)
    {
        icon.sprite = data.icon;
        nameText.text = data.objectName;
        this.index = index;
        this.objectType = data.ObjectType;
    }
    public void OnClick()
    {
        SetPlaceholder();
    }
    void SetPlaceholder()
    {
        PlaceholderObject.instance.SetPlaceholder(SimulatorLoader.instance.GetSpaceObjectData(index, objectType));
    }

}