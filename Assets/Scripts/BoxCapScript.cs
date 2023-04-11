using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCapScript : MonoBehaviour
{
    StoneSpawning SpawningScript;
    EllipsoidsSpawning EllipsoidSpawningScript;
    int NumberOfAttemptsToFillTopOfTheBox = 0;
    float DestroyTime;

    // Start is called before the first frame update
    void Start()
    {
        SpawningScript = transform.parent.gameObject.GetComponent<StoneSpawning>();
        EllipsoidSpawningScript = GameObject.Find("Box").gameObject.GetComponent<EllipsoidsSpawning>();
    }

    private void Update()
    {
        if (SpawningScript.isActiveAndEnabled)
        {
            if (SpawningScript.ProcessPaused)
            {
                if (Time.time - DestroyTime > 20f && Time.time - SpawningScript.ProcessPausedTime > 10f)
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

        else if (EllipsoidSpawningScript.isActiveAndEnabled)
        {
            if (EllipsoidSpawningScript.ProcessPaused)
            {
                if (Time.time - DestroyTime > 10f && Time.time - EllipsoidSpawningScript.ProcessPausedTime > 10f)
                {
                    if (NumberOfAttemptsToFillTopOfTheBox < EllipsoidSpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                    {
                        Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);
                        NumberOfAttemptsToFillTopOfTheBox++;
                        EllipsoidSpawningScript.NoDestroyedStones = 0;
                        EllipsoidSpawningScript.ProcessPaused = false;

                    }
                    else
                    {
                        EllipsoidSpawningScript.ProcessEnded = true;
                    }
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
        if (SpawningScript.isActiveAndEnabled && SpawningScript.ProcessPaused)
        {
            Destroy(other.gameObject);
            DestroyTime = Time.time;
        }
        else if (EllipsoidSpawningScript.isActiveAndEnabled && EllipsoidSpawningScript.ProcessPaused)
        {
            Destroy(other.gameObject);
            DestroyTime = Time.time;
        }
    }


}
