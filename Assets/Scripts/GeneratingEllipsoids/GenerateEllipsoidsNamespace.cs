using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenerateEllipsoidsNamespace
{
    public static class genE
    {
        #region GENERATE SPHERE
        public static Mesh GenerateSphere(float Radius, int meridians, int parallels)
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            //deviation from the y axis, size of angular step for points on one meridian
            float alpha = Mathf.PI / (1f + parallels);
            //deviation from the x axis, size of angular step for points on one parallel
            float beta = 2 * Mathf.PI / meridians;

            //  ------------
            // |  vertices  |
            //  ------------
            
            //top vertex
            vertices.Add(new Vector3(0, Radius, 0));
            //vertices which lies on meridians ane parallels intersections
            for (int alphaIter = 1; alphaIter <= parallels; alphaIter++)
            {
                float locRadius = Radius * Mathf.Sin(alpha * alphaIter);
                float y = Radius * Mathf.Cos(alpha * alphaIter);

                for (int betaIter = 0; betaIter<meridians; betaIter++)
                {
                    float x = locRadius * Mathf.Cos(beta * betaIter);
                    float z = locRadius * Mathf.Sin(beta * betaIter);

                    vertices.Add(new Vector3(x, y, z));
                }
            }
            //bottom vertex
            vertices.Add(new Vector3(0, -Radius, 0));

            //  -------------
            // |  triangles  |
            //  -------------
            
            //triangles with top vertex
            for (int i = 1; i < meridians; i++)
            {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i);
            }
                //connection of first and last triangle to create cycle (i = meridians)
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(meridians);
            
            //triangles formed from quads in the middle
            for(int parlevel = 0; parlevel < parallels - 1; parlevel++)
            {
                for(int merlevel = 1; merlevel < meridians; merlevel++)
                {
                    //quad
                    int v1 = merlevel + parlevel * meridians;
                    int v2 = (merlevel + 1) + parlevel * meridians;
                    int v3 = merlevel + (parlevel + 1) * meridians;
                    int v4 = (merlevel + 1) + (parlevel + 1) * meridians;

                    triangles.Add(v1);
                    triangles.Add(v2);
                    triangles.Add(v4);

                    triangles.Add(v1);
                    triangles.Add(v4);
                    triangles.Add(v3);
                }

                    //connection of first and last triangle to create cycle (merlevel = meridians)
                //quad
                int w1 = meridians + parlevel * meridians;
                int w2 = 1 + parlevel * meridians;
                int w3 = meridians + (parlevel + 1) * meridians;
                int w4 = 1 + (parlevel + 1) * meridians;

                triangles.Add(w1);
                triangles.Add(w2);
                triangles.Add(w4);

                triangles.Add(w1);
                triangles.Add(w4);
                triangles.Add(w3);
            }

            //triangles with bottom vertex
            int last = vertices.Count - 1;
            for (int i = 1 + (parallels - 1) * meridians; i < meridians + (parallels - 1) * meridians; i++)
            {
                triangles.Add(last);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
                //connection of first and last triangle to create cycle (i = meridians + (parallels - 1) * meridians)
            triangles.Add(last);
            triangles.Add(meridians + (parallels - 1) * meridians);
            triangles.Add(1 + (parallels - 1) * meridians);



            //  --------
            // |  mesh  |
            //  --------
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }
        #endregion

        
        public static void GenerateEllipsoid(GameObject parent, int noMeridiansOnSphere, int noParallelsOnSphere, float ellAxeX, float ellAxeY, float ellAxeZ)
        {
            //number of parallels and meridians according to locale scale of sphere in x,y,z directions
            int noParallels = Mathf.RoundToInt(ellAxeY) * noParallelsOnSphere;
            float ratioEllAxeXZ = ellAxeX > ellAxeZ? ellAxeX / ellAxeZ : ellAxeZ / ellAxeX;
            int noMeridians = Mathf.FloorToInt(ratioEllAxeXZ) * noMeridiansOnSphere;

            GameObject meshObject = parent.GetComponentInChildren<MeshFilter>().gameObject;

            //generate unit sphere with certain number of meridians ane parallels
            meshObject.GetComponent<MeshFilter>().mesh = GenerateSphere(1f, noMeridians, noParallels);

            //sphere scaling in order to create ellipsoid
            meshObject.transform.localScale = new Vector3(ellAxeX, ellAxeY, ellAxeZ);

            //deformation of vertices


        }
    }
}
