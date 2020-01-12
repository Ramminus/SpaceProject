using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SpriteToggler : MonoBehaviour
{
    [SerializeField]
    bool on;

    [SerializeField]
    Sprite offSprite, onSprite;

    Button button;


    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
        if (on) button.image.sprite = onSprite;
        else button.image.sprite = offSprite;
    }

    public void Toggle()
    {
        on = !on;
        if (on) button.image.sprite = onSprite;
        else button.image.sprite = offSprite;
    }
}
