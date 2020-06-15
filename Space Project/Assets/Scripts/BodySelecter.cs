using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BodySelecter : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI hoverText;
    [SerializeField]
    Image circleIcon;
    [SerializeField]
    LayerMask planetLayer;
    Ray ray;
    [SerializeField]
    float iconRotSpeed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, 200, planetLayer))
        {
            if (!circleIcon.gameObject.activeSelf) circleIcon.gameObject.SetActive(true);
            CustomPhysicsBody body = hit.collider.GetComponentInParent<CustomPhysicsBody>();
            circleIcon.rectTransform.Rotate(Vector3.forward, iconRotSpeed * Time.deltaTime);
            circleIcon.rectTransform.position = Camera.main.WorldToScreenPoint(hit.collider.transform.position);
            circleIcon.rectTransform.localScale = (Vector3.one * body.RenderRadiusScaled) * 1.9f;
            if(circleIcon.rectTransform.localScale.x < 0.2f)
            {
                circleIcon.rectTransform.localScale = Vector3.one * 0.2f;
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlanetCamera.instance.SetFocusAndStats(body);
            }
            hoverText.text = body.data.objectName;
        }
        else
        {
           if(circleIcon.gameObject.activeSelf) circleIcon.gameObject.SetActive(false);
            hoverText.text = "";
        }
    }
}
