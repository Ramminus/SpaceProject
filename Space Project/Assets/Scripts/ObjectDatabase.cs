using System.Collections;
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
    public List<MoonData> fixedMoons;




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
    [Button]
    public void FixMoons()
    {
        foreach(MoonData moon in moons)
        {
            if (fixedMoons.Contains(moon)) continue;
            if (moon.mass == 0) moon.mass = 2.599E13;
            if (moon.density == 0)
            {
                moon.density = 2.5f;

                moon.density *= 1000;
            }
           
            moon.orbitPathColour = new Color(1, 1, 1, 1);
            EditorUtility.SetDirty(moon);
            
        }
    }

    [Button]
    public void FixMoonOrbitPaths()
    {
        foreach (MoonData moon in moons)
        {
            
            moon.orbitPathColour = new Color(1, 1, 1, 1);
            EditorUtility.SetDirty(moon);

        }
    }
#endif

    public bool  ArrayContainsName<T>(T[] array, string name)
    {
       if(!(array.GetType() != typeof(SpaceObjectData[]) ))
        {
            throw new System.Exception("Parameter array is not of type System.Array");
        }
        SpaceObjectData[] data = (SpaceObjectData[])(object)array;
        foreach(SpaceObjectData obj in data)
        {
            if (obj.objectName == name) return true;
        }
        return false;
    }

}
