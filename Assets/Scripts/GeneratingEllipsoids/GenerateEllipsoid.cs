using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenerateEllipsoidsNamespace;

public class GenerateEllipsoid : MonoBehaviour
{
    MeshCollider col;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponentInChildren<MeshFilter>().mesh = genE.GenerateSphere(1f, 5, 3);
        col = GetComponentInChildren<MeshFilter>().gameObject.AddComponent<MeshCollider>();
        col.convex = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
