using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class SimulatorLoader : MonoBehaviour
{
    public bool paused;
    public bool loaded;
    [SerializeField]
    SolarSystemData solarSystemToLoadOnStart;
    [SerializeField]
    SolarSystemsDatabase solarSystemsDatabase;
    public SolarSystemData solarSystemToLoad;
    public ObjectDatabase objectDatabase;
    public static SimulatorLoader instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        LoadSolarSystem(solarSystemToLoadOnStart);
    }
    public void TogglePlayPause()
    {
        paused = !paused;
    }
    [Button]
    public void LoadSolarSystem(int index)
    {
        loaded = false;
        if (SolarSystemManager.instance != null)
        {
            SolarSystemManager.instance.ClearSolarSystem();
            SceneManager.UnloadSceneAsync(1);
        }
        solarSystemToLoad = solarSystemsDatabase.solarSystemDatabase[index];
        UiHandler.instance.Toggle(UiHandler.instance.LoaderButtonParent.gameObject);
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        

    }
    public void LoadSolarSystem(SolarSystemData data)
    {
        loaded = false;
        if (SolarSystemManager.instance != null)
        {
            SolarSystemManager.instance.ClearSolarSystem();
            SceneManager.UnloadSceneAsync(1);
        }
        solarSystemToLoad = data;
        UiHandler.instance.Toggle(UiHandler.instance.LoaderButtonParent.gameObject);
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        

    }
    public SpaceObjectData GetSpaceObjectData(int index, ObjectType objectType)
    {
        if (objectType == ObjectType.Sun) return objectDatabase.suns[index];
        if (objectType == ObjectType.Planet) return objectDatabase.planets[index];
        if (objectType == ObjectType.Moon) return objectDatabase.moons[index];
        return null;
    }
    public SolarSystemData GetSolarSystemByIndex(int index)
    {
        return solarSystemsDatabase.solarSystemDatabase[index];
    }
    public int GetNumberOfSolarSytems()
    {
        return solarSystemsDatabase.solarSystemDatabase.Length;
    }
}
