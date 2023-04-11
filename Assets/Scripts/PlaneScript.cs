using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    StoneSpawning SpawningScript;
    EllipsoidsSpawning EllipsoidSpawningScript;

    // Start is called before the first frame update
    void Start()
    {
        SpawningScript = GameObject.Find("Box").gameObject.GetComponent<StoneSpawning>();
        EllipsoidSpawningScript = GameObject.Find("Box").gameObject.GetComponent<EllipsoidsSpawning>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
        if (SpawningScript.isActiveAndEnabled)
            SpawningScript.NoDestroyedStones++;
        else if (EllipsoidSpawningScript.isActiveAndEnabled)
            EllipsoidSpawningScript.NoDestroyedStones++;
    }
}
