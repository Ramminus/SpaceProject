using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NameTag : MonoBehaviour
{
    [SerializeField]
    CustomPhysicsBody nameTagOf;
    [SerializeField]
    TextMeshProUGUI text;

    private void Awake()
    {
        SolarSystemManager.DestroyBody += DestroyObject;
        SolarSystemManager.OnClearSolarSystem += DestroyObject;
    }
    private void Start()
    {
         
        text.text = nameTagOf.data.name;
        
    }
    private void OnDestroy()
    {
        SolarSystemManager.DestroyBody -= DestroyObject;
        SolarSystemManager.OnClearSolarSystem -= DestroyObject;
    }
    private void Update()
    {
        UpdateTag();
    }
    public void DestroyObject(CustomPhysicsBody body)
    {
        if (body == nameTagOf) Destroy(gameObject);
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void UpdateTag()
    {
        float scale = SolarSystemManager.instance.transform.localScale.x;

        

        if(nameTagOf.data.ObjectType == ObjectType.Moon)
        {
            text.alpha = 0;
            if (scale >= 3 && scale <= 3.5f)
            {
                text.alpha = Mathf.Lerp(0, 1, 0.5f / (scale - 0.5f));

            }
            else if (scale > 3.5f) text.alpha = 1;
        }




        if (nameTagOf.Model == null || !nameTagOf.Model.isVisible) text.enabled = false;
        else
        {
            text.enabled = true;
            transform.position = Camera.main.WorldToScreenPoint(nameTagOf.transform.position + Vector3.right * nameTagOf.Model.transform.lossyScale.x/2);
        }
    }

    public void SetNametag(CustomPhysicsBody body)
    {
        nameTagOf = body;
    }
}
