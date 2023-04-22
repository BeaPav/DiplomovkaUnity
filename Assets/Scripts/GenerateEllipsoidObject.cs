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
using FractionDefinition;

public class GenerateEllipsoidObject : MonoBehaviour
{
    [SerializeField] int noParallelsOnSphere; //6
    [SerializeField] int noMeridiansOnSphere; //12
    //[SerializeField] float DensityOfStoneMaterial = 2600;


    public void GenerateEllipsoid(Fraction fraction)
    {
        //urcenie rozmerov podla frakcie
        (Vector3 axes, float frNum) = genE.AxesOfEllipsoid(fraction);

        /*
        //gula s priemerom jedna
        axes = new Vector3(0.5f, 0.5f, 0.5f);
        frNum = 1f;
        */

        //vygenerovanie meshu
        genE.GenerateEllipsoidMesh(this.gameObject, noMeridiansOnSphere, noParallelsOnSphere, axes);

        //collider
        //GenerateVHACDColliders();
        GetComponentInChildren<MeshFilter>().gameObject.AddComponent<MeshCollider>();
        GetComponentInChildren<MeshCollider>().convex = true;

        //nastavenie a vypocet vlastnosti zrna
        StoneMeshProperties s = GetComponent<StoneMeshProperties>();
        s.SetVolume(Prop.VolumeOfMesh(GetComponentInChildren<MeshFilter>()));
        s.SetFractionNumber(frNum);
        s.SetLength(2f * axes[1]);
        s.SetWidth(2f * axes[0]);
        //GetComponent<Rigidbody>().mass = s.GetVolume() * DensityOfStoneMaterial;

    }


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
}
