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

            //odklon od y  -  zvisla, iba velkost uhla, akym to rastie
            float alpha = Mathf.PI / (1f + parallels);
            //otacanie okolo y  -  vytvaranie bodov na kruznici v danej vyske s danym polomerom
            float beta = 2 * Mathf.PI / meridians;

            vertices.Add(new Vector3(0, Radius, 0));
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
            vertices.Add(new Vector3(0, -Radius, 0));

            //triangles
            //spojenie vrchnej capicky
            for(int i = 1; i < meridians; i++)
            {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i);
            }
                //spojenie prveho a posledneho trojuholnika dookola
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(meridians);
            
            //stredne trojuholniky
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
                //spojenie dokola, tj merlevel = meridians
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

            //spodna capicka
            int last = vertices.Count - 1;
            for (int i = 1 + (parallels - 1) * meridians; i < meridians + (parallels - 1) * meridians; i++)
            {
                triangles.Add(last);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
            //spojenie prveho a posledneho trojuholnika dookola
            triangles.Add(last);
            triangles.Add(meridians + (parallels - 1) * meridians);
            triangles.Add(1 + (parallels - 1) * meridians);
            





            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }
        #endregion

        //!!!!!!!!!!!!! treba doprogramovat ratanie meridians a parallels podla skalovania a daneho optimalu pre jednotkovu gulu
        public static void GenerateEllipsoid(GameObject parent, int noMeridians, int  noParallels)
        {
            GameObject meshObject = parent.GetComponentInChildren<MeshFilter>().gameObject;

            //zistenie kolko treba par a med podla toho ako skalujeme

            //vygenerovanie gule
            meshObject.GetComponent<MeshFilter>().mesh = GenerateSphere(1f, noMeridians, noParallels);

            //skalovanie
            meshObject.transform.localScale = new Vector3(0.5f, 2f, 1f);

            //deformacia

            //collider - bude treba urobit nekonvexny pre posunuty mesh
            MeshCollider col = meshObject.AddComponent<MeshCollider>();
            col.convex = true;

        }
    }
}
