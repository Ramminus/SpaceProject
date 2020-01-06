using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "SolarSystemDatabase", menuName ="Create/Solar System Database")]
public class SolarSystemsDatabase : ScriptableObject
{
    public string path;
    public SolarSystemData[] solarSystemDatabase;
    
#if UNITY_EDITOR
   
    [Button]
    public void FillDatabase()
    {


        object[] obj = Resources.LoadAll(path, typeof(SolarSystemData));
        solarSystemDatabase = new SolarSystemData[obj.Length];
        for (int i = 0; i < obj.Length; i++)
        {
            solarSystemDatabase[i] = (SolarSystemData)obj[i];
        }

    }
    public static T[] GetAtPath<T>(string path)
    {

        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(path);
        foreach (string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("/");
            string localPath = "Assets/" + path;

            if (index > 0)
                localPath += fileName.Substring(index);

            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));

            if (t != null)
                al.Add(t);
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }
#endif


}


