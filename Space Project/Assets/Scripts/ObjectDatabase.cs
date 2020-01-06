﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

[CreateAssetMenu(fileName = "ObjectDatabase", menuName ="Create/Object Database")]
public class ObjectDatabase : SerializedScriptableObject
{
    [SerializeField]
    string objectResourcePath;
    [SerializeField]
    string iconPath;
    public SunData[] suns;
    public PlanetData[] planets;
    public MoonData[] moons;





#if UNITY_EDITOR
    [Button]
    public void FillDatabase()
    {
        suns =  Resources.LoadAll<SunData>(objectResourcePath);
        planets =  Resources.LoadAll<PlanetData>(objectResourcePath);
        moons =  Resources.LoadAll<MoonData>(objectResourcePath);
    }

    [Button]
    public void UpdateObjectIcons()
    {
       List<SpaceObjectData> allObjs = new List<SpaceObjectData>();
        allObjs.AddRange(suns);
        allObjs.AddRange(planets);
        allObjs.AddRange(moons);

        foreach(SpaceObjectData obj in allObjs)
        {
            obj.icon = Resources.Load<Sprite>(iconPath + "/" + obj.objectName);
            EditorUtility.SetDirty(obj);
        }

    }
#endif


}