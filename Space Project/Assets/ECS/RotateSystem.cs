using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

namespace Space.ECS
{
    
    public class RotateSystem : JobComponentSystem
    {
        public struct RotateJob : IJobForEach<RotationEulerXYZ>
        {
            public float dt;
            public float rotSpeed;
            public void Execute(ref RotationEulerXYZ c0)
            {




                c0.Value.y += dt * rotSpeed;

            }


        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new RotateJob
            {
                dt = Time.deltaTime,
                rotSpeed = 30f
            };

            return job.Schedule(this, inputDeps);
        }

      
    }
}