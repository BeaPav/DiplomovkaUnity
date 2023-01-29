using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCapScript : MonoBehaviour
{
    StoneSpawning SpawningScript;
    int NumberOfAttemptsToFillTopOfTheBox = 0;
    float DestroyTime;

    // Start is called before the first frame update
    void Start()
    {
        SpawningScript = transform.parent.gameObject.GetComponent<StoneSpawning>();
    }

    private void Update()
    {
        if(SpawningScript.ProcessPaused)
        {
            if (Time.time - DestroyTime > 20f)
            {
                if (NumberOfAttemptsToFillTopOfTheBox < SpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                {
                    Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);
                    NumberOfAttemptsToFillTopOfTheBox++;
                    SpawningScript.NoDestroyedStones = 0;
                    SpawningScript.ProcessPaused = false;

                }
                else
                {
                    SpawningScript.ProcessEnded = true;
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DestroyStonesAboveBoxCap(other);
    }

    private void OnTriggerStay(Collider other)
    {
        DestroyStonesAboveBoxCap(other);
    }


    //Destroy stones above BoxCap
    private void DestroyStonesAboveBoxCap(Collider other)
    {
        if (SpawningScript.ProcessPaused)
        {
            Destroy(other.gameObject);
            DestroyTime = Time.time;
        }
    }


}
