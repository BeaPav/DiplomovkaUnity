using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using MeshProcess;
using GenerateEllipsoidsNamespace;
using PropertiesCounter;

public class GenerateEllipsoid : MonoBehaviour
{
    [SerializeField] int noParallelsOnSphere = 6;
    [SerializeField] int noMeridiansOnSphere = 12;
    [SerializeField] float DensityOfStoneMaterial = 2600;


    
    void Awake()
    {
        //generate ellipsoid of required shape according to SI and TI
        genE.GenerateEllipsoid(this.gameObject, noMeridiansOnSphere, noParallelsOnSphere, 1f, 1.5f, 0.3f);
        
        //create collider
        GenerateVHACDColliders();

        //counting ofessential properties of stone (volume, frction number, length, width, mass)
        StoneMeshProperties s = GetComponent<StoneMeshProperties>();
        s.SetVolume(f.VolumeOfMesh(GetComponentInChildren<MeshFilter>().mesh));
        s.SetFractionNumber(f.FrNumber(this.gameObject, 20));
        Vector2 lengthWidth = f.GetLengthAndWidthOfStone(this.gameObject);
        s.SetLength(lengthWidth.x);
        s.SetWidth(lengthWidth.y);
        GetComponent<Rigidbody>().mass = s.GetVolume() * DensityOfStoneMaterial;

              Debug.Log("properties: " + "length:" + s.GetLength() + " width: " + s.GetWidth() + " frNum: " + s.GetFractionNumber() +  " volume: " + s.GetVolume() + " mass: " + GetComponent<Rigidbody>().mass);
              

        //scale ellipsoid according to grading curve
        
    }


    #region GENERATE VHACD COLLIDERS
    public void GenerateVHACDColliders()
    {
        //inicialization of variables
        VHACD.Parameters m_Parameters = VhacdSettings.DefaultParameters();
        m_Parameters.m_resolution = 200000;

        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        var child = meshFilter.gameObject;

        //deleting existing colliders
        var existingColliders = child.GetComponents<MeshCollider>();
        if (existingColliders.Length > 0)
        {
            Debug.Log($"{child.name} had existing colliders; overwriting!");
            foreach (var coll in existingColliders)
            {
                DestroyImmediate(coll);
            }
        }
        
        //creating set of convex colliders
        var decomposer = child.AddComponent<VHACD>();
        decomposer.m_parameters = m_Parameters;

        var colliderMeshes = decomposer.GenerateConvexMeshes(meshFilter.sharedMesh);

        foreach (var collider in colliderMeshes)
        {
            var current = child.AddComponent<MeshCollider>();
            current.sharedMesh = collider;
            current.convex = true;
        }

        DestroyImmediate(child.GetComponent<VHACD>());
    }
    #endregion
}
