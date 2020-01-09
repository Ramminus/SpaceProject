using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;

public class ScreenShot : MonoBehaviour
{
    [SerializeField]
    bool overwrite;
    [SerializeField]
    ObjectDatabase db;
    [SerializeField]
    string resourcePath;
    [SerializeField]
    int size = 1024;
    [SerializeField]
    Renderer sphere;
    [SerializeField]
    Material backupSun, backupPlanet, backupMoon;

    List<SpaceObjectData> allObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    [Button]
    public void TakeAllScreenShots()
    {
        allObjects = new List<SpaceObjectData>();
        allObjects.AddRange(db.suns);
        allObjects.AddRange(db.planets);
        allObjects.AddRange(db.moons);

        StartCoroutine(TakeScreenShots());
    }
    IEnumerator  TakeScreenShots()
    {
        for (int i = 0; i < allObjects.Count; i++)
        {
            if (allObjects[i].icon == null || overwrite)
            {
                Material backup = null;
                SpaceObjectData data = allObjects[i];
                if (data.ObjectType == ObjectType.Sun) backup = backupSun;
                if (data.ObjectType == ObjectType.Planet) backup = backupPlanet;
                if (data.ObjectType == ObjectType.Moon) backup = backupMoon;
                sphere.material = data.customMaterial != null ? data.customMaterial : backup;

                yield return new WaitForEndOfFrame();

                TakeScreenshot(data.objectName);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public void TakeScreenshot(string name)
    {
        ScreenCapture.CaptureScreenshot(resourcePath + "/" + name + ".png", size);
       

    }
}
