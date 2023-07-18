using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStop : MonoBehaviour
{
    [SerializeField] float StopTime = 50f;
    [SerializeField] float LowVelocityBeforeStopDuration = 50f;
    [SerializeField] float LowVelocityMagnitudeLevel = 0.1f;

    [ReadOnly] public float velocityMagnitude;

    float InvokeTime;
    float LowVelocityTimeStart;
    [ReadOnly] public float DurationOfLowVelocity;
    [ReadOnly] public float DurationFromAwake;
    [ReadOnly] public bool LowVelocityInterval = false;
    [ReadOnly] public bool isKinematic = false;
    Rigidbody rb;

    int fixedUpdateCounter = 0;


    private void Awake()
    {
        InvokeTime = Time.time;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //cez is kinematic
        /*
        if (!isKinematic)
        {
            if (rb.velocity.magnitude < LowVelocityMagnitudeLevel)
            {
                if (!LowVelocityInterval)
                {
                    LowVelocityInterval = true;
                    LowVelocityTimeStart = Time.time;
                }
                else
                {
                    if (Time.time - LowVelocityTimeStart > LowVelocityBeforeStopDuration && Time.time - InvokeTime > StopTime)
                    {
                        rb.isKinematic = true;
                    }
                }
            }
            else
            {
                if (LowVelocityInterval) LowVelocityInterval = false;
            }

        }
        */
        velocityMagnitude = rb.velocity.magnitude;
        DurationOfLowVelocity = Time.time - LowVelocityTimeStart;
        DurationFromAwake = Time.time - InvokeTime;
        isKinematic = rb.isKinematic;
        
    }

    private void FixedUpdate()
    {

        //cez Sleep
        if (fixedUpdateCounter % 5 == 0)
        {
            if (!rb.IsSleeping())
            {
                if (rb.velocity.magnitude < LowVelocityMagnitudeLevel)
                {
                    if (!LowVelocityInterval)
                    {
                        LowVelocityInterval = true;
                        LowVelocityTimeStart = Time.time;
                    }
                    else
                    {
                        if (Time.time - LowVelocityTimeStart > LowVelocityBeforeStopDuration && Time.time - InvokeTime > StopTime)
                        {
                            rb.Sleep();
                        }
                    }
                }
                else
                {
                    if (LowVelocityInterval) LowVelocityInterval = false;
                }
            }
            
        }
        fixedUpdateCounter = (fixedUpdateCounter + 1) % 5;
    }
}
