using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCompute : MonoBehaviour
{
    [SerializeField]
    ComputeShader shader;
    [SerializeField]
    PlanetDataCompute[] dataInput;
    [SerializeField]
    PlanetDataCompute[] dataOutput;
    ComputeBuffer buffer;
    int kernal;
    // Start is called before the first frame update
    void Start()
    {
        //dataInput = new PlanetDataCompute[2];
        //dataOutput = new PlanetDataCompute[dataInput.Length];


        buffer = new ComputeBuffer(dataInput.Length, 48);
        
        kernal = shader.FindKernel("CSMain");
        shader.SetInt("number", dataInput.Length);
       
     

    }

    private void FixedUpdate()
    {
        
        buffer.SetData(dataInput);
        shader.SetBuffer(kernal, "dataBuffer", buffer);
        shader.Dispatch(kernal, dataInput.Length, 1, 1);
        buffer.GetData(dataInput);
        //dataInput = (PlanetDataCompute[])dataOutput.Clone();
    }
}
[System.Serializable]
public struct PlanetDataCompute
{

    public Vector3d pos;
    public double mass;
    public Vector3d velocity;
    public int index;
    public int destroyed;
    


    public bool IsDestroyed()
    {
        return destroyed > 0;
    }

};
public struct TBool
{
    private readonly byte _value;
    public TBool(bool value) { _value = (byte)(value ? 1 : 0); }
    public static implicit operator TBool(bool value) { return new TBool(value); }
    public static implicit operator bool(TBool value) { return value._value != 0; }
}