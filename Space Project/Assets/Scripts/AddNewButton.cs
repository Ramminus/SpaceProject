using System.Collections;
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
    [SerializeField]
    Sprite backupSunIcon, backupPlanetIcon, backupMoonIcon;


    public void SetButton(int index, SpaceObjectData data)
    {
        
        nameText.text = data.objectName;
        this.index = index;
        this.objectType = data.ObjectType;
        icon.sprite = data.icon != null ? data.icon : GetBackupIcon();
    }
    Sprite GetBackupIcon()
    {
        if (objectType == ObjectType.Sun) return backupSunIcon;
        else if (objectType == ObjectType.Planet) return backupPlanetIcon;
        else if (objectType == ObjectType.Moon) return backupMoonIcon;
        return null;

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
