using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RendererArrayExtension
{
    public static Bounds ComputeBounds(this Collider[] colliders)
    {
        Bounds bounds = new Bounds();
        for (int ir = 0; ir < colliders.Length; ir++)
        {
            Collider collider = colliders[ir];
            if (ir == 0)
                bounds = collider.bounds;
            else
                bounds.Encapsulate(collider.bounds);
        }
        return bounds;
    }
}
