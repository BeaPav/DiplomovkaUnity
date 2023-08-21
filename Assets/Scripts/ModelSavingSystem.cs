using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public static class ModelSavingSystem
{
    public static void SaveModel(Transform parent, int folderIter, string path, (float, float, float) fractionsRatios, string textResults, bool generatingEllipsoids, bool controlPrefabReference)
    {
        //name of model directory according to fractions ratios
        string modelTypeName = "Model_" + (fractionsRatios.Item1 * 100f) + "%_[4,8]_" + (fractionsRatios.Item2 * 100f) + "%_[8,16]_" + (fractionsRatios.Item3 * 100f) + "%_[16,22]";
        modelTypeName += "_VypnuteImprovedPatchFriction";

        //create directory according to fractions ratios if it is needed
        if (!Directory.Exists(path + "/" + modelTypeName))
        {
            Directory.CreateDirectory(path + "/" + modelTypeName);
            AssetDatabase.Refresh();
        }

        path += "/" + modelTypeName;



        //finding the number of existing models in this directory to set number in models name
        while (Directory.Exists(path + "/Model_" + folderIter))
        {
            folderIter++;
        }

        //directory for model
        Directory.CreateDirectory(path + "/Model_" + folderIter);
        if (generatingEllipsoids)
            Directory.CreateDirectory(path + "/Model_" + folderIter + "/Meshes");
        AssetDatabase.Refresh();


        path += "/Model_" + folderIter;

        
        //save all generated meshes of ellipsoids if needed (by default NOT -- meshes can repeat and has reference)
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


        //save model as prefab
        //PrefabUtility.SaveAsPrefabAssetAndConnect(parent.parent.gameObject, path + "/Model" + folderIter + ".prefab", InteractionMode.AutomatedAction);
        PrefabUtility.SaveAsPrefabAsset(parent.parent.gameObject, path + "/Model_" + folderIter + ".prefab");
        SaveTextFile(path, "/Model_" + folderIter + "_TextResults", textResults);

        //control of ellipsoids mesh references, adding reference from collider mesh if needed
        if (controlPrefabReference)
        {
            using (var editingScope = new PrefabUtility.EditPrefabContentsScope(path + "/Model_" + folderIter + ".prefab"))
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

    public static void SaveTestingModel(Transform TestingObject, string path, string name, string textResults, bool createDirForModel, int modelIter = 0,  string directoryName = "!noName")
    {
        //create special directory  for model if needed
        if (directoryName != "!noName")
        {
            if(!Directory.Exists(path + "/" + directoryName))
            {
                Directory.CreateDirectory(path + "/" + directoryName);
                AssetDatabase.Refresh();
            }

            path += "/" + directoryName;
        }

        //directory for model
        if (createDirForModel)
        {
            //finding the number of existing models in this directory to set number in models name
            while (Directory.Exists(path + "/Model_" + modelIter))
            {
                modelIter++;
            }
            Directory.CreateDirectory(path + "/Model_" + modelIter);
            AssetDatabase.Refresh();

            path += "/Model_" + modelIter;
        }
        else
        {
            //finding the number of existing models in this directory to set number in models name
            while (File.Exists(path + "/" + name + "_" + modelIter + ".prefab"))
            {
                modelIter++;
            }
        }


        //save
        PrefabUtility.SaveAsPrefabAsset(TestingObject.gameObject, path + "/" + name + "_" + modelIter + ".prefab");
        SaveTextFile(path, name + "_" + modelIter + "_TextResults", textResults);

    }

    public static void SaveTextFile(string path, string name, string text)
    {
        File.WriteAllText(path + "/" + name + ".txt", text);
        AssetDatabase.Refresh();
    }
}
