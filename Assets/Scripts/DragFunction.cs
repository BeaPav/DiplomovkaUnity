using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragFunction : MonoBehaviour
{
    [SerializeField] float Drag = 0.5f;
    Rigidbody Rb;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float v = Rb.velocity.magnitude;
        Vector3 dir = -1f * Rb.velocity.normalized;
        Vector3 dragForce = dir * v * v * Drag;
        Rb.AddForce(dragForce);

    }
}
