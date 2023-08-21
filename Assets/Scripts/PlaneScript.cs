using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//destroy ellipsoids which are out of the box

public class PlaneScript : MonoBehaviour
{
    StoneSpawning SpawningScript;
    EllipsoidsSpawning EllipsoidSpawningScript;

    
    void Start()
    {
        SpawningScript = GameObject.Find("Box").gameObject.GetComponent<StoneSpawning>();
        EllipsoidSpawningScript = GameObject.Find("Box").gameObject.GetComponent<EllipsoidsSpawning>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        Destroy(collision.gameObject);
        if (SpawningScript.isActiveAndEnabled)
            SpawningScript.NoDestroyedStones++;
        else if (EllipsoidSpawningScript.isActiveAndEnabled)
            EllipsoidSpawningScript.NoDestroyedStones++;
    }
}
