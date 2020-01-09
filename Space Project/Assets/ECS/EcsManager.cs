using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Space.ECS
{
    //public class EcsManager : MonoBehaviour
    //{
    //    [SerializeField]
    //    GameObject cubePrefab;

    //    EntityManager manager;
    //    Entity cube;
    //    [SerializeField]
    //    bool usingEcs;

    //    [SerializeField]
    //    Vector3 spawnPoint;

    //    [Header("Spawn Settings")]
    //    public RingData[] rings;

    //    // Start is called before the first frame update
    //    void Start()
    //    {
    //        if (usingEcs)
    //        {
    //            manager = World.Active.EntityManager;
    //            cube = GameObjectConversionUtility.ConvertGameObjectHierarchy(cubePrefab.gameObject, World.Active);


    //            foreach (RingData ring in rings)
    //            {
    //                Vector3 dir = Vector3.forward;
    //                for (int i = 0; i < ring.number; i++)
    //                {

    //                    dir = Quaternion.Euler(0, ring.spacing, 0) * dir;
    //                    Entity instantiatedObj = manager.Instantiate(cube);
    //                    manager.SetComponentData(instantiatedObj, new Translation { Value = dir * Random.Range(ring.innerRadius, ring.outerRadius) });
    //                    manager.AddComponentData(instantiatedObj, new Scale { Value = Random.Range(ring.minScale, ring.maxScale) });
    //                    manager.AddComponentData(instantiatedObj, new Rotation { Value = Random.rotation });
    //                }
    //            }
    //        }


    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }
    //}
   
}
