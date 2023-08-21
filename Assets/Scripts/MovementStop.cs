using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//for ellipsoids to set Sleep mode (is kinematic) when not moving "a lot" -- tiny movements for defined period of time

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

    void Update()
    {
        //using is kinematic
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

        //variables for controlling values in inspector
        velocityMagnitude = rb.velocity.magnitude;
        DurationOfLowVelocity = Time.time - LowVelocityTimeStart;
        DurationFromAwake = Time.time - InvokeTime;
        isKinematic = rb.isKinematic;
        
    }

    private void FixedUpdate()
    {

        //using Sleep
        //every fifth frame
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
