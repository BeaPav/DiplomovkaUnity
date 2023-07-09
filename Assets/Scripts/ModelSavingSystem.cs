using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public static class ModelSavingSystem
{
    public static void SaveModel(Transform parent, int folderIter, string path, bool generatingEllipsoids, bool controlPrefabReference)
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

        /*
        //iba kontrola ci je nieco bez referencie pred ulozenim
        MeshFilter[] meshfiltTemp = parent.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter childMesh in meshfiltTemp)
        {
            if (childMesh.mesh == null)
                Debug.Log("nulovy mesh pre elipsoid " + childMesh.transform.parent.name);
        }
        */

        //ulozenie modelu ako prefab
        //PrefabUtility.SaveAsPrefabAssetAndConnect(parent.parent.gameObject, path + "/Model" + folderIter + ".prefab", InteractionMode.AutomatedAction);
        PrefabUtility.SaveAsPrefabAsset(parent.parent.gameObject, path + "/Model" + folderIter + ".prefab");
        
        if (controlPrefabReference)
        {
            using (var editingScope = new PrefabUtility.EditPrefabContentsScope(path + "/Model" + folderIter + ".prefab"))
            {
                var prefabRoot = editingScope.prefabContentsRoot;
                //Debug.Log("meno root of prefab " + prefabRoot.name);


                // Adding and removing components are supported
                var ellParent = prefabRoot.transform.GetChild(2);
                //Debug.Log("meno ellParent " + ellParent.name);

                foreach (MeshFilter mf in ellParent.GetComponentsInChildren<MeshFilter>())
                {
                    if (mf == null) Debug.Log("no Meshfilter in this object during control in saving function");
                    if (true)//mf.mesh == null)
                    {
                        //Debug.Log("meno elipsoidu ktoremu pridavam referenciu " + mf.transform.parent.name);
                        mf.mesh = mf.transform.GetComponent<MeshCollider>().sharedMesh;
                    }
                }
            }
        }
        


    }

    public static void SaveTestingModel(Transform TestingObject, string path, string name, int modelIter = 0,  string directoryName = "!noName")
    {
        //ak chceme vytvorit specialny priecinok
        if (directoryName != "!noName")
        {
            if(!Directory.Exists(path + "/" + directoryName))
            {
                Directory.CreateDirectory(path + "/" + directoryName);
                AssetDatabase.Refresh();
            }

            path += "/" + directoryName;
        }


        //najdenie kolky to je model
        while (Directory.Exists(path + "/Model_" + modelIter))
        {
            modelIter++;
            //Debug.Log("zvysujeme folderIter");
        }

        //vytvorenie priecinku
        Directory.CreateDirectory(path + "/Model_" + modelIter);
        AssetDatabase.Refresh();

        path += "/Model_" + modelIter;

        PrefabUtility.SaveAsPrefabAsset(TestingObject.gameObject, path + "/" + name + "_" + modelIter + ".prefab");


    }
}
