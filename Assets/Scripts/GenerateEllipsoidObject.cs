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


    
    void Awake()
    {
        //GenerateEllipsoid();


        #region VYGENEROVANIE ELIPSOIDU HNED PO INSTANTIATE ALE TU NEOVPLYVNUJEME AXES -- ZAKOMENTOVANE
        /*
        //generate ellipsoid of required shape according to SI and TI
        genE.GenerateEllipsoidMesh(this.gameObject, noMeridiansOnSphere, noParallelsOnSphere, new Vector3(0.2f, 0.5f, 0.3f));

        //create collider
        //GenerateVHACDColliders();
        GetComponentInChildren<MeshFilter>().gameObject.AddComponent<MeshCollider>();
        GetComponentInChildren<MeshCollider>().convex = true;

        //counting ofessential properties of stone (volume, frction number, length, width, mass)
        StoneMeshProperties s = GetComponent<StoneMeshProperties>();
        s.SetVolume(Prop.VolumeOfMesh(GetComponentInChildren<MeshFilter>().mesh));
        s.SetFractionNumber(Prop.FrNumber(this.gameObject, 20));
        Vector2 lengthWidth = Prop.LengthAndWidthOfStone(this.gameObject);
        s.SetLength(lengthWidth.x);
        s.SetWidth(lengthWidth.y);
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        //GetComponent<Rigidbody>().mass = s.GetVolume() * DensityOfStoneMaterial;

        //Debug.Log("properties: " + "length:" + s.GetLength() + " width: " + s.GetWidth() + " frNum: " + s.GetFractionNumber() +  " volume: " + s.GetVolume() + " mass: " + GetComponent<Rigidbody>().mass);


        //scale ellipsoid according to grading curve
        */
        #endregion

    }


    public void GenerateEllipsoid(Fraction fraction)
    {
        //urcenie rozmerov podla frakcie
        (Vector3 axes, float frNum) = genE.AxesOfEllipsoid(fraction);

        /*
        axes = new Vector3(1f, 1f, 1f);
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
