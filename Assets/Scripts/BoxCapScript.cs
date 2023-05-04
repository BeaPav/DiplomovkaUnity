using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCapScript : MonoBehaviour
{
    StoneSpawning SpawningScript;
    EllipsoidsSpawning EllipsoidSpawningScript;

    Collider BoxCapCollider;

    int NumberOfAttemptsToFillTopOfTheBox = 0;
    float DestroyTime = 0f;

    Vector3 StartPos;
    Vector3 TargetPos;
    Rigidbody Rb;
    bool MoveToTargetPos = false;
    float StartOfMovementTime;
    [SerializeField] float SpeedOfCapCollider;
    [SerializeField] bool DestroyStones = false;


    // Start is called before the first frame update
    void Start()
    {
        SpawningScript = transform.parent.gameObject.GetComponent<StoneSpawning>();
        EllipsoidSpawningScript = transform.parent.gameObject.GetComponent<EllipsoidsSpawning>();
        BoxCapCollider = GetComponent<Collider>();
        BoxCapCollider.enabled = false;
        StartPos = transform.position;
        TargetPos = StartPos;
        TargetPos.x = transform.parent.position.x - transform.localPosition.x * transform.parent.localScale.x;
        Rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (SpawningScript.isActiveAndEnabled)
        {
            if (SpawningScript.ProcessPaused)
            {
                if (BoxCapCollider.enabled == false)
                { 
                    BoxCapCollider.enabled = true;
                    MoveToTargetPos = true;
                    StartOfMovementTime = Time.time;
                }
                if ((transform.position - TargetPos).magnitude < 0.1f)
                {
                    MoveToTargetPos = false;
                    
                    if (Time.time - DestroyTime > 10f && Time.time - SpawningScript.ProcessPausedTime > 10f)
                    {
                        if (NumberOfAttemptsToFillTopOfTheBox < SpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                        {
                            NumberOfAttemptsToFillTopOfTheBox++;
                            Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);

                            SpawningScript.NoDestroyedStones = 0;
                            BoxCapCollider.enabled = false;
                            transform.position = StartPos;
                            SpawningScript.ProcessPaused = false;

                        }
                        else
                        {
                            SpawningScript.ProcessEnded = true;
                        }
                    }
                }

            }
        }

        else if (EllipsoidSpawningScript.isActiveAndEnabled)
        {
            if (EllipsoidSpawningScript.ProcessPaused)
            {
                if (BoxCapCollider.enabled == false)
                {
                    BoxCapCollider.enabled = true;
                    MoveToTargetPos = true;
                    StartOfMovementTime = Time.time;
                }
                if ((transform.position - TargetPos).magnitude < 0.1f)
                {
                    MoveToTargetPos = false;

                    if (Time.time - DestroyTime > 10f && Time.time - EllipsoidSpawningScript.ProcessPausedTime > 10f)
                    {
                        if (NumberOfAttemptsToFillTopOfTheBox < EllipsoidSpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                        {
                            NumberOfAttemptsToFillTopOfTheBox++;
                            Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);

                            EllipsoidSpawningScript.NoDestroyedStones = 0;
                            BoxCapCollider.enabled = false;
                            transform.position = StartPos;
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
    }

    private void FixedUpdate()
    {
        if(MoveToTargetPos)
        {
            float t = Time.time - StartOfMovementTime;
            Vector3 newPos = StartPos + (TargetPos - StartPos).normalized * SpeedOfCapCollider * t; ;
            Rb.MovePosition(newPos);
            if((newPos - TargetPos).magnitude < 0.1f)
            {
                MoveToTargetPos = false;
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "BoxMesh")
        {
            //Debug.Log(collision.gameObject.name + " BoxCap");
            if(DestroyStones)
                Destroy(collision.gameObject);
            DestroyTime = Time.time;
        }
    }


}
