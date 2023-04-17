using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    StoneSpawning SpawningScript;
    EllipsoidsSpawning EllipsoidSpawningScript;

    List<GameObject> forDestruction;

    // Start is called before the first frame update
    void Start()
    {
        forDestruction = new List<GameObject>();
        SpawningScript = GameObject.Find("Box").gameObject.GetComponent<StoneSpawning>();
        EllipsoidSpawningScript = GameObject.Find("Box").gameObject.GetComponent<EllipsoidsSpawning>();
    }

    private void Update()
    {
        if(forDestruction.Count > 0)
        {
            Destroy(forDestruction[0]);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        forDestruction.Add(collision.gameObject);
        if (SpawningScript.isActiveAndEnabled)
            SpawningScript.NoDestroyedStones++;
        else if (EllipsoidSpawningScript.isActiveAndEnabled)
            EllipsoidSpawningScript.NoDestroyedStones++;
    }
}
