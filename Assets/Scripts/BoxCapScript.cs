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

    [SerializeField] bool SlideCollider;
    [SerializeField] float SpeedOfCapCollider;
    [SerializeField] bool DestroyStonesWhileSliding = false;


    // Start is called before the first frame update
    void Start()
    {
        SpawningScript = transform.parent.gameObject.GetComponent<StoneSpawning>();
        EllipsoidSpawningScript = transform.parent.gameObject.GetComponent<EllipsoidsSpawning>();
        BoxCapCollider = GetComponent<Collider>();
        BoxCapCollider.enabled = false;

        if (SlideCollider)
        {
            StartPos = transform.parent.position + new Vector3(3f, 0f, 0f) * transform.parent.localScale.x + new Vector3(0f, 2f, 0f) * transform.parent.localScale.y;
            TargetPos = StartPos;
            TargetPos.x = transform.parent.position.x - 3f * transform.parent.localScale.x;
        }
        else
        {
            transform.position = transform.parent.position + new Vector3(0f, 1.9f, 0f) * transform.parent.localScale.y;
            BoxCapCollider.isTrigger = true;
        }
        
        Rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (SpawningScript.isActiveAndEnabled)
        {
            if (SpawningScript.ProcessPaused && Time.time - SpawningScript.ProcessPausedTime > 3f)
            {
                if (SlideCollider)
                {
                    if (BoxCapCollider.enabled == false)
                    {
                        transform.position = StartPos;
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
                                SpawningScript.ProcessPaused = false;

                            }
                            else
                            {
                                SpawningScript.ProcessEnded = true;
                            }
                        }
                    }
                }
                else
                {
                    if (BoxCapCollider.enabled == false)
                    {
                        BoxCapCollider.enabled = true;
                    }
                    if (Time.time - DestroyTime > 10f && Time.time - SpawningScript.ProcessPausedTime > 10f)
                    {
                        if (NumberOfAttemptsToFillTopOfTheBox < SpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                        {
                            NumberOfAttemptsToFillTopOfTheBox++;
                            Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);

                            SpawningScript.NoDestroyedStones = 0;
                            BoxCapCollider.enabled = false;
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
            if (EllipsoidSpawningScript.ProcessPaused && Time.time - EllipsoidSpawningScript.ProcessPausedTime > 3f)
            {
                if (SlideCollider)
                {
                    if (BoxCapCollider.enabled == false)
                    {
                        transform.position = StartPos;
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
                                EllipsoidSpawningScript.ProcessPaused = false;

                            }
                            else
                            {
                                EllipsoidSpawningScript.ProcessEnded = true;
                            }
                        }
                    }
                }
                else
                {
                    if (BoxCapCollider.enabled == false)
                    {
                        BoxCapCollider.enabled = true;
                    }
                    if (Time.time - DestroyTime > 10f && Time.time - EllipsoidSpawningScript.ProcessPausedTime > 10f)
                    {
                        if (NumberOfAttemptsToFillTopOfTheBox < EllipsoidSpawningScript.MaxNumberOfAttemptsToFillTopOfTheBox)
                        {
                            NumberOfAttemptsToFillTopOfTheBox++;
                            Debug.Log("Attepmts to fill top: " + NumberOfAttemptsToFillTopOfTheBox);

                            EllipsoidSpawningScript.NoDestroyedStones = 0;
                            BoxCapCollider.enabled = false;
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
            //Debug.Log(collision.gameObject.name + " BoxCap Collision");
            if (DestroyStonesWhileSliding)
            {
                Destroy(collision.gameObject);
                DestroyTime = Time.time;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject stone = other.transform.parent.gameObject;
        if (stone.tag == "Stone")
        {
            //Debug.Log(stone.name + " BoxCap Trigger");
            Mesh m = other.GetComponent<MeshFilter>().mesh;
            Matrix4x4 localToWorld = other.transform.localToWorldMatrix;
            Vector3 AverageGravityCentre = Vector3.zero;
            for(int i = 0; i < m.vertexCount; i++)
            {
                AverageGravityCentre += m.vertices[i];
            }
            AverageGravityCentre /= m.vertexCount;
            AverageGravityCentre = localToWorld.MultiplyPoint3x4(AverageGravityCentre);

            float controlHightLevel = transform.parent.position.y + 2f * transform.parent.localScale.y;

            if (AverageGravityCentre.y > controlHightLevel)
            {
                Destroy(stone.gameObject);
                DestroyTime = Time.time;
            }

        }
    }

}
