using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class SpaceObjectData : ScriptableObject
{
    public string objectName;
    public double mass;
    public double diameter;
    public float axisTiltInDeg;
    protected ObjectType objectType;
    public ObjectType ObjectType { get => objectType; }
    public float e;
    public Color32 orbitPathColour = new Color32(1, 1, 1, 1);
    public Material customMaterial;
    [HideIf("objectType", ObjectType.Sun)]
    public float angleOfOrbit;
    public float rotationalVelocity;
    [LabelText("Density(Kg/m3)")]
    public float density;
    public Sprite icon;

#if UNITY_EDITOR
    [Button]
    public void GetMaterial()
    {
      
        string path = AssetDatabase.GetAssetPath(this);
        List<string> split =new List<string>( path.Split('/'));
        split.RemoveAt(split.Count - 1);
        path = "";
        for (int i = 0; i < split.Count; i++)
        {
            path += split[i] + "/";
        }
        Debug.Log(path);
        Material mat = (Material)AssetDatabase.LoadAssetAtPath("Assets/OurSolarSytem/Materials/" + objectName + ".mat", typeof(Material));
        if(mat != null)
        {
            customMaterial = mat;
        }
        else
        {
            Debug.Log("No material found");
        }
    }
#endif

}

