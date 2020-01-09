using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;


    
    public class RotateSystem : ComponentSystem
    {
        

    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation) =>
        {
           
            
        });
       
    }
}
