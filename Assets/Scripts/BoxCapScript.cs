using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCapScript : MonoBehaviour
{
    StoneSpawning SpawningScript;
    EllipsoidsSpawning EllipsoidSpawningScript;

    Collider BoxCollider;

    int NumberOfAttemptsToFillTopOfTheBox = 0;
    float DestroyTime = 0f;
    

    // Start is called before the first frame update
    void Start()
    {
        SpawningScript = transform.parent.gameObject.GetComponent<StoneSpawning>();
        EllipsoidSpawningScript = GameObject.Find("Box").gameObject.GetComponent<EllipsoidsSpawning>();
        BoxCollider = GetComponent<Collider>();
        BoxCollider.enabled = false;
    }

    private void Update()
    {
        if (SpawningScript.isActiveAndEnabled)
        {
            if (SpawningScript.ProcessPaused)
            {
                if(BoxCollider.enabled == false)
                    BoxCollider.enabled = true;
                if (Time.time - DestroyTime > 10f && Time.time - SpawningScript.ProcessPausedTime > 10f)
                {
                    if (NumberOfAttemptsToFillTopOfTheBox < SpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                    {
                        NumberOfAttemptsToFillTopOfTheBox++;
                        Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);

                        SpawningScript.NoDestroyedStones = 0;
                        BoxCollider.enabled = false;
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
                if (BoxCollider.enabled == false) 
                    BoxCollider.enabled = true;
                if (Time.time - DestroyTime > 10f && Time.time - EllipsoidSpawningScript.ProcessPausedTime > 10f)
                {
                    if (NumberOfAttemptsToFillTopOfTheBox < EllipsoidSpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                    {
                        NumberOfAttemptsToFillTopOfTheBox++;
                        Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);

                        EllipsoidSpawningScript.NoDestroyedStones = 0;
                        BoxCollider.enabled = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "BoxMesh")
        {
            //Debug.Log(collision.gameObject.name + " BoxCap");
            Destroy(collision.gameObject);
            DestroyTime = Time.time;
        }
    }


}
