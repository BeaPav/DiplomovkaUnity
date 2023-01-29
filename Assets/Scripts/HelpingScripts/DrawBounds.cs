using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBounds : MonoBehaviour
{
    public Color color_sphere = new Color(0.0f, 0.0f, 0.0f, 0.5f);
    public Color color_bounds = new Color(1.0f, 1.0f, 0.0f, 0.5f);
    public bool Hierarchical = false;
    public bool Disable = false;

    public void OnDrawGizmos()
    {
        if (!Disable)
        {
            Bounds b = new Bounds();
            if (Hierarchical)
            {
                Collider[] r = gameObject.GetComponentsInChildren<Collider>();
                b = r.ComputeBounds();
            }
            else
            {
                Collider r = gameObject.GetComponent<Collider>();
                if (r != null)
                {
                    b = r.bounds;
                }
            }
            Gizmos.color = color_bounds;
            Gizmos.DrawCube(b.center, b.size);
            //Gizmos.color = color_sphere;
            //Gizmos.DrawSphere(b.center, b.size.magnitude* 0.1f);
        }
    }


}
