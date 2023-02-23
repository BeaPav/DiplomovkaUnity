using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBounds : MonoBehaviour
{
    public Color color_sphere = new Color(0.0f, 0.0f, 0.0f, 0.5f);
    public Color color_bounds = new Color(1.0f, 1.0f, 0.0f, 0.5f);
    public Color color_bounds_renderer = new Color(1.0f, 0f, 0.0f, 0.5f);
    public bool Hierarchical = false;
    public bool Disable = false;

    /// <summary>
    /// tu som urobila upravy vramci testovania, pre spravne pocitanie bounds je treba ich ratat priamo z meshu
    /// co je tu implementovane pre NIE hierarchical
    /// </summary>

    public void OnDrawGizmos()
    {
        if (!Disable)
        {
            Bounds b = new Bounds();
            Bounds bRenderer = new Bounds();
            if (Hierarchical)
            {
                MeshCollider[] r = gameObject.GetComponentsInChildren<MeshCollider>();
                b = r.ComputeBounds();
                

                Renderer[] ren = gameObject.GetComponentsInChildren<MeshRenderer>();
                if (ren.Length > 0)
                {
                    bRenderer = ren[0].bounds;
                    foreach( Renderer l in ren)
                    {
                        bRenderer.Encapsulate(l.bounds);
                    }
                }
                
            }
            else
            {
                MeshFilter mf = gameObject.GetComponent<MeshFilter>();
                MeshCollider r = gameObject.GetComponent<MeshCollider>();
                if (r != null)
                {
                    //b = r.bounds;
                    b = GeometryUtility.CalculateBounds(mf.sharedMesh.vertices, transform.localToWorldMatrix);
                }

                Renderer ren = gameObject.GetComponent<Renderer>();
                if (ren != null)
                {
                    bRenderer = ren.bounds;
                }
            }
            Gizmos.color = color_bounds;
            Gizmos.DrawCube(b.center, b.size);

            Gizmos.color = color_bounds_renderer;
            Gizmos.DrawCube(bRenderer.center, bRenderer.size);
            //Gizmos.color = color_sphere;
            //Gizmos.DrawSphere(b.center, b.size.magnitude* 0.1f);
        }
    }


}
