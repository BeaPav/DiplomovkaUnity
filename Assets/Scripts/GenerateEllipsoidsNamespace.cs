using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FractionDefinition;
using UnityEditor;

namespace GenerateEllipsoidsNamespace
{
    public static class genE
    {
        #region GENERATE SPHERE MESH
        public static Mesh GenerateSphereMesh(float Radius, int meridians, int parallels)
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

        #region GENERATE ELLIPSOID MESH
        public static void GenerateEllipsoidMesh(GameObject parent, int noMeridiansOnSphere, int noParallelsOnSphere, Vector3 axes)
        {

            float ellAxeX = axes[0];
            float ellAxeY = axes[1];
            float ellAxeZ = axes[2];


            //number of parallels and meridians according to locale scale of sphere in x,y,z directions
            int noParallels = Mathf.Max(Mathf.FloorToInt(ellAxeY) * noParallelsOnSphere, noParallelsOnSphere);
            float ratioEllAxeXZ = ellAxeX > ellAxeZ? ellAxeX / ellAxeZ : ellAxeZ / ellAxeX;
            int noMeridians = Mathf.FloorToInt(ratioEllAxeXZ) * noMeridiansOnSphere;

            GameObject meshObject = parent.GetComponentInChildren<MeshFilter>().gameObject;

            //generate/load unit sphere with certain number of meridians ane parallels
            //meshObject.GetComponent<MeshFilter>().mesh = GenerateSphereMesh(1f, noMeridians, noParallels);
            Mesh mesh = Resources.Load<Mesh>("Ellipsoids/EllipsoidMesh_" + noParallels + "Parallels_" + noMeridians + "Meridians");
            if (mesh == null)
            {
                mesh = GenerateSphereMesh(1f, noMeridians, noParallels);
                AssetDatabase.CreateAsset(mesh, "Assets/SavedModels/Meshes/Resources/Ellipsoids/" + 
                                        "EllipsoidMesh_" + noParallels + "Parallels_" + noMeridians + "Meridians" + ".asset");
                //Debug.Log("vytvaram mesh " + "EllipsoidMesh_" + noParallels + "Parallels_" + noMeridians + "Meridians" + ".asset");
            }
            
            meshObject.GetComponent<MeshFilter>().sharedMesh = mesh;
            

            //sphere scaling in order to create ellipsoid
            meshObject.transform.localScale = new Vector3(ellAxeX, ellAxeY, ellAxeZ);

            //meshObject.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            //deformation of vertices

            //add mesh collider
            //meshObject.AddComponent<MeshCollider>(); //asi este convex

        }
        #endregion

        #region DETERMINE AXES AND FRNUM OF ELLIPSOID ACCORDING TO FRACTION PROPERTIES (GRADING CURVE, FLAT INDEX, SHAPE INDEX)
        public static (Vector3 axes, float frNum, (int,int,int) indGradingFlatShape, (bool, bool) shapeFlatLong) AxesOfEllipsoid(Fraction fraction)
        {
            //??????      //cista frakcia (oznacujeme i, chapeme d = GradingCurveIndexes[i] , D = GradingCurveIndexes[i + 1] )

            //generujeme elipsoid z frakcie fraction
            
            //volba podfrakcie podla ciary zrnitosti
            int frGrIndex = fraction.GradingChoice();

            //nahodna volba frNum podla podfrakcie zrnitosti
            float frNum = Random.Range(fraction.GradingSubfractions[frGrIndex].FractionBoundaries.Item1, 
                                       fraction.GradingSubfractions[frGrIndex].FractionBoundaries.Item2);

            //teraz sa urci ci bude plochy podla pravdepodobnosti v danej frakcii
            (int frFlIndex, bool isFlat, float flatSieveSize) = fraction.IsFlat(frNum);

            //teraz sa urci ci bude dlhy podla pravdepodobnosti v danej frakcii
            (int frShIndex, bool isLong) = fraction.IsLong(frNum);

            //vygenerujeme ellX podla plochosti
            //deli sa dvomi pretoze ell je vzdy polomer a flatSieveSize predstavuje priemer kade sa kamen prepcha
            float ellX = 0f;
            if (isFlat)
            {
                
                if (isLong)
                {
                    ellX = Random.Range(flatSieveSize / 2f, flatSieveSize) / 2f;
                }
                else
                {
                    ellX = Random.Range(frNum / Mathf.Sqrt(20), flatSieveSize / 2f);
                }

                /*
                Debug.Log("isFlat , m/4 " + flatSieveSize / 4f + " frNum/sqrt20 " + frNum / Mathf.Sqrt(20) + " rozdiel " + (flatSieveSize / 4f- frNum / Mathf.Sqrt(20)));
                Debug.Log("dlzka intervalu pre m/4 " + (flatSieveSize / 2f - flatSieveSize / 4f));
                Debug.Log("dlzka intervalu pre sqrt20 " + (flatSieveSize / 2f - frNum / Mathf.Sqrt(20)));
                */
                //konstanta pre dolne ohranicenie intervalu - nechceme uzsie kamene ako polovica medzery na harfovom site (zvolili sme)
                
                //generovanie pre zvysenu hornu hranicu aby nebol problem s plochym aj dlhym zrnom naraz
                //ellX = Random.Range(frNum / (2f*Mathf.Sqrt(5f)), flatSieveSize / 2f);
                
                //Debug.Log("Flat");
            }
            else
            {
                ellX = Random.Range(flatSieveSize / 2f, frNum / 2f);

                //Debug.Log("Not Flat and ox is in interval " + flatSieveSize / 2f + "," + frNum / 2f + " a rovna sa      " + ellX);
            }

            //vygenerujeme ellZ v zavislosti od zvoleneho ellX tak, aby ostala zachovana frakcia zodpovedajuca frNum
            //(lebo podla ellZ je myslienkovo urcene frNum)
            //float ellZ = Random.Range(frNum / 2f, Mathf.Sqrt(frNum * frNum / 2f - ellX * ellX));
            
            //float average = (frNum / 2f) / 4f + (Mathf.Sqrt(frNum * frNum / 2f - ellX * ellX)) * 3f / 4f;
            //float ellZ = Random.Range(average, Mathf.Sqrt(frNum * frNum / 2f - ellX * ellX));
            
            
            //generovanie ze oz je len cislo nie interval
            float ellZ = Mathf.Sqrt(frNum * frNum / 2f - ellX * ellX);

            //vygenerovanie najdlhsieho rozmeru ellY podla shape indexu
            float ellY = 0f;
            if (isLong)
            {
                
                //koeficient pre horne ohr intervalu - elipsoidy maju najdlhsi rozmer max 4,5 krat vacsi ako najkratsi (zvolili sme)
                if (3f * ellX >= ellZ)
                {
                    ellY = Random.Range(3f * ellX, 4.5f * ellX);
                }
                else
                {
                    float upperBoundCoeff = Mathf.Max(4.5f, Mathf.Ceil(ellZ / ellX));
                    ellY = Random.Range(ellZ, upperBoundCoeff * ellX);
                    //Debug.Log("horna hranica: " + upperBoundCoeff);
                }
                
                
                
                //generovanie ked nie je problem s plochym a dlhym zrnom naraz
                //ellY = Random.Range(3f * ellX, 4.5f * ellX);
                
                //Debug.Log("Long");
            }
            else
            {
                //if (ellZ > 3 * ellX) Debug.Log("zle hranice pre Oy, nie je Long");
                ellY = Random.Range(ellZ, 3f * ellX);
                //Debug.Log("Not Long and oy is in interval " + ellZ + "," + 3f*ellX + " a rovna sa      " + ellY);
            }

            //ellZ = Random.Range(ellZ, 3/4f * ellZ + 1/4f * ellY);
            /*
            string s1 = "Oz < Oy     ";
            string s2 = "Oz < 3*Ox    ";
            string s3 = "Ox > frNum/sqrt(20)    ";

            if (ellZ > ellY)
            {
                 s1 = "Oz > Oy     ";
                Debug.Log(s1);
            }
            if(ellZ > 3f*ellX)
            {
                s2 = "Oz > 3*Ox    ";
                Debug.Log(s2);
                Debug.Log("Oz > Oy " + (ellZ > ellY));
            }
            if(ellX < frNum/Mathf.Sqrt(20))
            {
                s3 = "Ox < frNum/sqrt(20)    ";
                Debug.Log(s3);
                Debug.Log("isLong " + isLong + " isFlat " + isFlat);
            }
            */
          //Debug.Log(s3 + s2 + s1);

            //Debug.Log("2*ellX: " + 2*ellX + " 2*ellY: " + 2*ellY + " 3*(2*ellX): " + 3*2*ellX + " 2*ellZ: " + 2*ellZ);
            (int, int, int) indices = (frGrIndex, frFlIndex, frShIndex);
            (bool, bool) shape = (isFlat, isLong);
            //return (new Vector3(ellX * 0.95f, ellY * 0.95f, ellZ * 0.95f), frNum, indices, shape);
            return (new Vector3(ellX, ellY, ellZ), frNum, indices, shape);
            //return (new Vector3(ellX * 1.05f, ellY * 1.05f, ellZ * 1.05f), frNum, indices, shape);

        }

        #endregion
    }
}
