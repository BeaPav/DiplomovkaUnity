using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public static class ModelSavingSystem
{
    public static void SaveModel(Transform parent, int folderIter, string path, bool generatingEllipsoids)
    {
        //najdenie kolky to je model
        while (Directory.Exists(path + "/Model_" + folderIter))
        {
            folderIter++;
            //Debug.Log("zvysujeme folderIter");
        }

        //vytvorenie priecinku
        Directory.CreateDirectory(path + "/Model_" + folderIter);
        if (generatingEllipsoids)
            Directory.CreateDirectory(path + "/Model_" + folderIter + "/Meshes");
        AssetDatabase.Refresh();


        path += "/Model_" + folderIter;

        
        //ulozenie vsetkych vygenerovanych meshov (pre elipsoidy)
        if (generatingEllipsoids)
        {
            MeshFilter[] mf = parent.GetComponentsInChildren<MeshFilter>();
            int mfCounter = 0;
            foreach (MeshFilter childMesh in mf)
            {
                AssetDatabase.CreateAsset(childMesh.mesh, path + "/Meshes/Ellipsoid" + mfCounter + ".asset");
                mfCounter++;
            }
        }
        

        //ulozenie modelu ako prefab
        PrefabUtility.SaveAsPrefabAssetAndConnect(parent.parent.gameObject, path + "/Model" + folderIter + ".prefab", InteractionMode.UserAction);
        

    }
}
