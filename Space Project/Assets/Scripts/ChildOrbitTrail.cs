using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class ChildOrbitTrail : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    int maxSegments, vertCounter;
    float vertTimer;
    LineRenderer lr;
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        Vector3 startPos = target.position - transform.position;
        lr.SetPositions(new Vector3[] { target.transform.position - transform.position, startPos });
    }
    private void Update()
    {
        vertTimer -= Time.deltaTime;
        lr.SetPosition(0, target.transform.position - transform.position);
        if (vertTimer <= 0) {
            Vector3[] posArray = new Vector3[lr.positionCount];

            List<Vector3> posList = new List<Vector3>(posArray);
            posList.Insert(1,target.transform.position - transform.position);
            if (posList.Count > maxSegments) posList.RemoveAt(posList.Count - 1);
            lr.positionCount = posList.Count;
            lr.SetPositions(posList.ToArray());
            vertTimer = vertCounter;
        }
    }
}
