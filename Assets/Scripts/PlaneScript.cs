using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
    StoneSpawning SpawningScript;


    // Start is called before the first frame update
    void Start()
    {
        SpawningScript = GameObject.Find("Box").gameObject.GetComponent<StoneSpawning>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
        SpawningScript.NoDestroyedStones++;
    }
}
