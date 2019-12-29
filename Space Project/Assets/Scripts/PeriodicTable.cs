using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;



[CreateAssetMenu(fileName = "Periodic Table", menuName ="Create/Periodic Table")]
public class PeriodicTable : ScriptableObject
{
    [SerializeField]
    List<Element> elements;
    [SerializeField]
    string xmlPath;

    [Button]
    public void CreatePeriodicTable()
    {
        elements = new List<Element>();
        TextAsset csv = Resources.Load<TextAsset>(xmlPath);
        string[] data = csv.text.Split(new char[] { '\n' });
        
        for (int i = 0; i < data.Length; i++)
        {
            if(i > 0)
            {
                
                string[] line = data[i].Split(new char[] {',' });
                Debug.Log(data[i]);
                Debug.Log(line.Length);
                Element newElement = new Element(IntFromString(line[0]), line[1], line[2], float.Parse(line[3]), IntFromString(line[4]), IntFromString(line[5]),
                    IntFromString(line[6]), IntFromString(line[7]), IntFromString(line[8]), line[9], BoolFromString(line[10]), BoolFromString(line[11]), BoolFromString(line[12]), BoolFromString(line[13]), BoolFromString(line[14]), line[15]
                    , FloatFromString(line[16]), FloatFromString(line[17]), FloatFromString(line[18]), DoubleFromString(line[19], System.Globalization.NumberStyles.Float), FloatFromString(line[20]), FloatFromString(line[21]),IntFromString(line[22]),line[23],IntFromString(line[24])
                    ,FloatFromString(line[25]),IntFromString(line[26]),IntFromString(line[27]));

                elements.Add(newElement);
            }
        }

    }

    bool BoolFromString(string text, string trueText = "yes", string falseText = "no")
    {
        if (text == "") return false;
        else if (text == trueText) return true;
        else if (text == falseText) return false;
        return false;

        
    }
    int IntFromString(string text)
    {
        if (text == "") return -1;
        return int.Parse(text);
    }
    public float FloatFromString(string text)
    {
        if (text == "") return -1;
        return float.Parse(text);
    }
    public double DoubleFromString(string text, System.Globalization.NumberStyles styles )
    {
        if (text == "") return -1;
        return (double.Parse(text, styles));
    }
}

