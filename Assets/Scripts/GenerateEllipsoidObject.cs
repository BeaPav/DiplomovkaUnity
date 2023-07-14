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
    [SerializeField] PhysicMaterial mat;

    public void GenerateEllipsoid(Fraction fraction, float meshScaleFactor = 1f)
    {
        //urcenie rozmerov podla frakcie
        (Vector3 axes, float frNum, (int, int, int) indGrFlSh, (bool, bool) shapeFlatLong) = genE.AxesOfEllipsoid(fraction, meshScaleFactor);

        /*
        //gula s priemerom jedna
        axes = new Vector3(0.5f, 0.5f, 0.5f);
        frNum = 1f;
        */

        /*
        if(axes == new Vector3(1f,1f,1f))
        {
            Debug.Log("frNum " + frNum);
            Debug.Log("indexes Gr,Fl,Sh " + indGrFlSh);
            Debug.Log("FlatLong " + shapeFlatLong);
        }
        */
        GetComponent<StoneMeshProperties>().axes = new Vector3(axes.x, axes.y, axes.z);
        GetComponent<StoneMeshProperties>().indGrFlSh = indGrFlSh;

        //vygenerovanie meshu
        genE.GenerateEllipsoidMesh(this.gameObject, noMeridiansOnSphere, noParallelsOnSphere, axes);

        //vydeletovanie colliderov
        MeshFilter mf = GetComponentInChildren<MeshFilter>();
        var existingColliders = mf.gameObject.GetComponents<MeshCollider>();
        if (existingColliders.Length > 0)
        {
            foreach (var coll in existingColliders)
            {
                DestroyImmediate(coll);
            }
        }

        //collider
        //GenerateVHACDColliders();
        mf.gameObject.AddComponent<MeshCollider>();
        GetComponentInChildren<MeshCollider>().convex = true;
        GetComponentInChildren<MeshCollider>().material = mat;

        //nastavenie a vypocet vlastnosti zrna
        StoneMeshProperties s = GetComponent<StoneMeshProperties>();
        float volume = Prop.VolumeOfEllipsoidMesh(GetComponentInChildren<MeshFilter>());
        s.SetVolume(volume);
        s.SetFractionNumber(frNum);
        s.SetLength(2f * axes[1]);
        s.SetWidth(2f * axes[0]);
        s.SetIsFlat(shapeFlatLong.Item1);
        s.SetIsLong(shapeFlatLong.Item2);
        //GetComponent<Rigidbody>().mass = s.GetVolume() * DensityOfStoneMaterial; //toto neviem ci nie je v hlavnom scripte

        fraction.ActualizeVolume(volume, indGrFlSh, shapeFlatLong);
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
