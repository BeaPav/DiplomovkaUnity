using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxVibrations : MonoBehaviour
{
    Vector3 UpVector = new Vector3(0, 1f, 0);
    Vector3 StartPosition;
    float startTime;
    [SerializeField] float Amplitude = 1f;
    [SerializeField] float Frequency = 1f;
    Rigidbody Rb;
    const float c = Mathf.PI * 2;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Time.fixedDeltaTime + "fixed Delta Time ");
        Rb = GetComponent<Rigidbody>();
        startTime = Time.time;
        UpVector *= transform.localScale.y;
        StartPosition = transform.position;
    }

    private void FixedUpdate()
    {
        float t = Time.time - startTime;
        //Rb.velocity = UpVector * Amplitude * Mathf.Sin(c * t * Frequency);
        //Rb.MovePosition(transform.position + UpVector * 5f * Time.deltaTime);

        Vector3 newPos = StartPosition + UpVector * Amplitude * Mathf.Sin(c * t * Frequency);
        Rb.MovePosition(newPos);
    }
}
