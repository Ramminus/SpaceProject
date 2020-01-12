using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;

public class SpaceObjectLoaderEditor : EditorWindow
{
    FileStream selectedFile;
    string path ="";

    ObjectType objectsType;
    int numberToMake;

    ObjectDatabase database;
    

    static double G = 6.67259e-20;
    static string databasePath = "ObjectDatabase";
    static string objectPath = "Assets/Resources/Objects";
    [MenuItem("Window/Space Object Loader")]
   public static void ShowWindow()
    {
        GetWindow<SpaceObjectLoaderEditor>("Loader");
        
        
    }

    public void OpenFileSelect()
    {
       path = EditorUtility.OpenFilePanel("Select Object Json file","", "json");
       // GetPathFromReources(path);
    }
    public void GetPathFromReources(string fullPath)
    {
        bool adding = false;
        string[] fullPathSplit = fullPath.Split(char.Parse("/"));
        path ="";
        int index = 0;
        foreach(string split in fullPathSplit)
        {
            if (split == "Resources")
            {
                adding = true;
            }
            else if (adding)
            {
                if(index == fullPathSplit.Length - 1)
                {
                    string fileNoExtension = split.Split(char.Parse("."))[0];
                    path += "/" +fileNoExtension;
                }
                else
                    path += path.Length == 0 ? split : "/" + split;
            }
            index++;
        }
        
    }
    private void OnGUI()
    {
        if (path.Length != 0)
            selectedFile = File.OpenRead(path);
        GUILayout.BeginHorizontal();
        
        GUILayout.Label(selectedFile == null ? "No File Selected" : selectedFile.Name);
        if(GUILayout.Button("Select File"))
        {
            OpenFileSelect();
        }
        GUILayout.EndHorizontal();
        if(selectedFile != null)
        {
            objectsType = (ObjectType)EditorGUILayout.EnumPopup("Objects Type", objectsType);
            numberToMake = EditorGUILayout.IntField("Number of assets to be made", numberToMake);
            if (GUILayout.Button("Generate"))
            {
                Generate();
                //MoonDataJsonArray data = JsonConvert.DeserializeObject<MoonDataJsonArray>(json);
               


              
            }
        }
        

    }
    public void Generate()
    {
        if (database == null) database = (ObjectDatabase)Resources.Load(databasePath);
        Debug.Log(database.moons.Length);
        string json = File.ReadAllText(path);
        int index = 0;
        if (objectsType == ObjectType.Moon)
        {
            MoonDataJsonArray data = JsonUtility.FromJson<MoonDataJsonArray>(json);
            foreach (MoonDataJson moonData in data.moons)
            {
                if (numberToMake != 0 && index == numberToMake) return;

                if (!database.ArrayContainsName<MoonData>(database.moons, moonData.name))
                {
                    MoonData moonAsset = ScriptableObject.CreateInstance<MoonData>();
                    moonAsset.SetId(moonData.id);
                    moonAsset.objectName = moonData.name;
                    moonAsset.planetId = moonData.planetId;
                    moonAsset.diameter = moonData.radius * 2;
                    moonAsset.mass = moonData.gm / G;
                    moonAsset.density = moonData.density;
                    EditorUtility.SetDirty(moonAsset);
                    string assetName = moonData.name;
                    assetName = assetName.Replace("/", "_");
                    string assetPath = objectPath + "/" + assetName + ".asset";
                    AssetDatabase.CreateAsset(moonAsset, assetPath);
                    index++;
                }

            }
        }
    }
    [System.Serializable]
    public class MoonDataJson
    {
        public int id;
        public int planetId;
        public string name;
        public float gm;
        public float radius;
        public float density;
        public float magnitude;
        public float albedo;
    }
    [System.Serializable]
    public class MoonDataJsonArray
    {
        public  List<MoonDataJson> moons;
    }
}
