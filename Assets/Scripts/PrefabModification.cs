using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//use when ellipsoids mesh references are missing in saved prefab of model
//this function adds reference from collider mesh


public class PrefabModification : MonoBehaviour
{
    [SerializeField] string path;
    [SerializeField] string modelIter;

   
    void Start()
    {
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(path + "/Model_" + modelIter + "/Model_" + modelIter + ".prefab"))
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
