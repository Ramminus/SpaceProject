using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Space.ECS
{
    public class EcsManager : MonoBehaviour
    {
        [SerializeField]
        GameObject cubePrefab;

        EntityManager manager;
        Entity cube;
        [SerializeField]
        bool usingEcs;

        [SerializeField]
        Vector3 spawnPoint;
        // Start is called before the first frame update
        void Start()
        {
            if (usingEcs)
            {
                manager = World.Active.EntityManager;
                cube = GameObjectConversionUtility.ConvertGameObjectHierarchy(cubePrefab.gameObject, World.Active);

                Entity instantiatedObj = manager.Instantiate(cube);
                manager.SetComponentData(instantiatedObj, new Translation { Value = spawnPoint });
                manager.AddComponent(instantiatedObj, typeof(RotateSystem));
            }


        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
