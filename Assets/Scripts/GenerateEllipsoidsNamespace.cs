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

        }
        #endregion

        #region DETERMINE AXES AND FRNUM OF ELLIPSOID ACCORDING TO FRACTION PROPERTIES (GRADING CURVE, FLAT INDEX, SHAPE INDEX)
        public static (Vector3 axes, float frNum, (int,int,int) indGradingFlatShape, (bool, bool) shapeFlatLong) AxesOfEllipsoid(Fraction fraction, float meshScaleFactor = 1f)
        {
            //grading category (fraction) d/D (index i, meaning d = GradingCurveIndexes[i] , D = GradingCurveIndexes[i + 1] )
            //generating ellipsoid from "fraction"
            
            //choice of category from grading informations
            int frGrIndex = fraction.GradingChoice();

            //random choice of frNum according to selected grading category
            float frNum = Random.Range(fraction.GradingSubfractions[frGrIndex].FractionBoundaries.Item1, 
                                       fraction.GradingSubfractions[frGrIndex].FractionBoundaries.Item2);

            //is flat determination
            (int frFlIndex, bool isFlat, float flatSieveSize) = fraction.IsFlat(frNum);

            //is long determination
            (int frShIndex, bool isLong) = fraction.IsLong(frNum);

            //dimension ellX considering flatness
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

                //Debug.Log("Flat");
            }
            else
            {
                ellX = Random.Range(flatSieveSize / 2f, frNum / 2f);

            }

            
            //dimension ellZ considering frNum and ellX
            float ellZ = Mathf.Sqrt(frNum * frNum / 2f - ellX * ellX);

            //dimension ellY considering if the stone is long
            float ellY = 0f;
            if (isLong)
            {
                if (3f * ellX >= ellZ)
                {
                    ellY = Random.Range(3f * ellX, 4f * ellX);
                }
                else
                {
                    float upperBoundCoeff = Mathf.Max(4f, Mathf.Ceil(ellZ / ellX));
                    ellY = Random.Range(ellZ, upperBoundCoeff * ellX);
                    //Debug.Log("horna hranica: " + upperBoundCoeff);
                }
                
                //Debug.Log("Long");
            }
            else
            {
                ellY = Random.Range(ellZ, 3f * ellX);
            }

           

            //Debug.Log("dimensions => 2*ellX: " + 2*ellX + " 2*ellY: " + 2*ellY + " 3*(2*ellX): " + 3*2*ellX + " 2*ellZ: " + 2*ellZ);
            (int, int, int) indices = (frGrIndex, frFlIndex, frShIndex);
            (bool, bool) shape = (isFlat, isLong);
            return (new Vector3(ellX, ellY, ellZ) * meshScaleFactor, frNum, indices, shape);

        }

        #endregion
    }
}
