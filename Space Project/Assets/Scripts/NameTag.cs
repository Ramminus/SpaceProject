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
   


    private void Start()
    {
         
        text.text = nameTagOf.data.name;
    }
    // Update is called once per frame
    void Update()
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
            transform.position = Camera.main.WorldToScreenPoint(nameTagOf.transform.position + Vector3.right * nameTagOf.transform.lossyScale.x/2);
        }
    }

    public void SetNametag(CustomPhysicsBody body)
    {
        nameTagOf = body;
    }
}
