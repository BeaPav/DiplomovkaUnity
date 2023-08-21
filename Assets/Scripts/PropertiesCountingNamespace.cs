using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FractionDefinition;

namespace PropertiesCounter
{
    public static class Prop
    {
        #region COUNTING OF MESH VOLUME
        
        private static float SignedVolumeOfTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {            
            float v321 = v3.x * v2.y * v1.z;
            float v231 = v2.x * v3.y * v1.z;
            float v312 = v3.x * v1.y * v2.z;
            float v132 = v1.x * v3.y * v2.z;
            float v213 = v2.x * v1.y * v3.z;
            float v123 = v1.x * v2.y * v3.z;

            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }

        //volume of general mesh
        public static float VolumeOfMesh(Mesh mesh, float xScale = 1f, float yScale = 1f, float zScale = 1f)
        {
            float volume = 0;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v1 = vertices[triangles[i + 0]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];

                v1.x *= xScale;
                v2.x *= xScale;
                v3.x *= xScale;

                v1.y *= yScale;
                v2.y *= yScale;
                v3.y *= yScale;

                v1.z *= zScale;
                v2.z *= zScale;
                v3.z *= zScale;


                //T += v1 + v2 + v3;

                volume += SignedVolumeOfTriangle(v1, v2, v3);
            }

            return Mathf.Abs(volume);
        }

        //counting volume of ellipsoid`s mesh, taking parents transformations into consideration 
        public static float VolumeOfEllipsoidMesh(MeshFilter mf)
        {
            Transform transformMeshObject = mf.transform;
            Mesh mesh = mf.sharedMesh;
            
            float xScale = transformMeshObject.localScale.x;
            float yScale = transformMeshObject.localScale.y;
            float zScale = transformMeshObject.localScale.z;

            Transform parent = transformMeshObject.parent;

            while (parent != null)
            {
                xScale *= parent.localScale.x;
                yScale *= parent.localScale.y;
                zScale *= parent.localScale.z;
                parent = parent.parent;
            }

            return VolumeOfMesh(mesh, xScale, yScale, zScale);
            
        }

        #endregion

        //Not used for ellipsoids, NOT ACTUAL for stones
        #region LENGTH AND WIDTH    
        //COUNTING OF STONE`S    WIDTH = minimal distance of two paralell planes
        //                       LENGTH = maximal distance of two paralell planes

        //used with general stone mesh, is NOT ACTUAL (counting of bounding box is not precise, it is needed to count it from mesh like in frNum)
        public static Vector2 LengthAndWidthOfStone(GameObject stone)
        {
            MeshFilter meshFilter = stone.GetComponentInChildren<MeshFilter>();
            Bounds b;

            //Saving of original transformations for later
            Vector3 originalPos = stone.transform.position;
            Quaternion originalRot = stone.transform.rotation;
            Vector3 originalScale = stone.transform.localScale;

            //Inicialization of length and width
            float width = float.MaxValue;
            float length = float.MinValue;


            //Rotations step to create AABB of actual rotation
            Vector3 zrot = new Vector3(0,0,10);
            Vector3 xrot = new Vector3(10,0,0);

            //Rotating and comparing AABBs in order to find minimal and maximal dimensions posiible
            for (int i = 1; i <= 36; i++)
            {
                for (int j = 1; j <= 36; j++)
                {
                    //New AABB bounding box of mesh
                    b = GeometryUtility.CalculateBounds(meshFilter.sharedMesh.vertices, meshFilter.transform.localToWorldMatrix);

                    float min = Mathf.Min(b.size.x, b.size.y, b.size.z);
                    float max = Mathf.Max(b.size.x, b.size.y, b.size.z);

                    if (min < width)
                        width = min;
                    if (max > length)
                        length = max;

                    stone.transform.Rotate(zrot);
                }

                stone.transform.Rotate(xrot);
            }

            //Return stones original transformations
            stone.transform.position = originalPos;
            stone.transform.rotation = originalRot;
            stone.transform.localScale = originalScale;

            //Debug.Log("width: " + width + " length: " + length);

            return new Vector2(length, width);
        }
        #endregion


        //frNum obtained through rotations -- minimum from maximal dimensions of xz AABB of rotated object
        #region FRACTION NUMBER

        public static float FrNumber(GameObject stone, int numOfRot)
        {
            MeshFilter meshFilter = stone.GetComponentInChildren<MeshFilter>();

            /*
            // Saving of original transformations for later
            Vector3 originalPos = stone.transform.position;
            Quaternion originalRot = stone.transform.rotation;
            Vector3 originalScale = stone.transform.localScale;
            */

            //Inicialization of FrNumber
            float frNum = float.MaxValue;

            //Rotating stone and finding the minimal square aligned with xz which the stone falls through
            for (int i = 0; i < numOfRot; i++)
            {
                stone.transform.rotation = Random.rotationUniform;

                Vector2 xzBoundingBox = AxisAlignesXZBox(meshFilter.sharedMesh.vertices, meshFilter.transform.localToWorldMatrix);
                float max = Mathf.Max(xzBoundingBox.x, xzBoundingBox.y);

                //Searching for minimum of all maximums
                if (max < frNum) frNum = max;
            }

            /*
            //Return stones original transformations
            stone.transform.position = originalPos;
            stone.transform.rotation = originalRot;
            stone.transform.localScale = originalScale;
            */

            return frNum;
        }

        public static Vector2 AxisAlignesXZBox(Vector3[] vertices, Matrix4x4 LocToWorld)
        {
            float Xmin = float.MaxValue;
            float Xmax = float.MinValue;
            float Zmin = float.MaxValue;
            float Zmax = float.MinValue;
            
            foreach(Vector3 v in vertices)
            {
                Vector3 p = LocToWorld.MultiplyPoint3x4(v);
                if (p.x < Xmin) Xmin = p.x;
                if (p.x > Xmax) Xmax = p.x;
                if (p.z < Zmin) Zmin = p.z;
                if (p.z > Zmax) Zmax = p.z;
            }

            return new Vector2(Xmax - Xmin, Zmax - Zmin);
        }

        #endregion

        //model properties -- voids and related
        //fraction properties -- grading curve, flat index, shape index
        #region MODEL PROPERTIES
        public static void CountPropertiesOfModel(GameObject ellipsoidsParent, float boxVolume, out string textModelResults)
        {
            //variables inicialization
            ModelProperties mp = ellipsoidsParent.GetComponentInParent<ModelProperties>();
            StoneMeshProperties[] AllStonesProperties = ellipsoidsParent.GetComponentsInChildren<StoneMeshProperties>();
            int noOfStones = AllStonesProperties.Length;
            float stonesVolume = 0f;


            string materialPropertiesText = "FRICTION \n" + "Dynamic: " + ellipsoidsParent.GetComponentInChildren<MeshCollider>().sharedMaterial.dynamicFriction + "\n"
                                                          + "Static: " + ellipsoidsParent.GetComponentInChildren<MeshCollider>().sharedMaterial.staticFriction + "\n"
                                                          + "Bounciness: " + ellipsoidsParent.GetComponentInChildren<MeshCollider>().sharedMaterial.bounciness + "\n" + "\n" + "\n";


            //volume of all stones
            for (int i = 0; i < noOfStones; i++)
            {
                float volume = AllStonesProperties[i].GetVolume();
                stonesVolume += volume;
                //Debug.Log(i + " stone volume: " + volume);

            }

            //counting properties of model
            float voids = (boxVolume - stonesVolume) / boxVolume * 100f;
            mp.NumberOfStones = noOfStones;
            mp.BoxVolume = boxVolume;
            mp.StonesVolume = stonesVolume;
            mp.EmptyVolume = boxVolume - stonesVolume;
            mp.Voids = voids;

            //text -- properties for txt file
            textModelResults = materialPropertiesText + mp.ModelPropertiesToString();

            /*
            Debug.Log("No of ellipsoids: " + noOfStones);
            Debug.Log("Box volume: " + boxVolume);
            Debug.Log("StonesVolume: " + stonesVolume);
            Debug.Log("EmptyVolume: " + (boxVolume - stonesVolume));
            Debug.Log("Voids: " + voids + "%");
            */
        }

        public static void CountPropertiesOfModelFractions(GameObject ellipsoidsParent, List<Fraction> fractions, out string textFractionsResults)
        {
            ModelProperties mp = ellipsoidsParent.GetComponentInParent<ModelProperties>();
            StoneMeshProperties[] AllStonesProperties = ellipsoidsParent.GetComponentsInChildren<StoneMeshProperties>();
            int noOfStones = AllStonesProperties.Length;

            //properties inicialization for all fractions
            mp.FrProperties = new FractionProperties[fractions.Count];

            //inicialization of variables
            float stonesVolume = 0f;

            for(int i = 0; i < fractions.Count; i++)
            {
                mp.FrProperties[i] = new FractionProperties();
                mp.FrProperties[i].FractionName = fractions[i].FractionBoundaries.ToString();
                mp.FrProperties[i].GradingCurve = new GradingCurve(fractions[i].GradingSubfractions.Length);
                mp.FrProperties[i].ShapeIndex = new ShapeIndex(fractions[i].ShapeSubfractions.Length);
                mp.FrProperties[i].FlatIndex = new FlatInex(fractions[i].FlatSubfractions.Length);

                //fractions names
                for (int j = 0; j < fractions[i].GradingSubfractions.Length; j++)
                {
                    mp.FrProperties[i].GradingCurve.FrNames[j] = new string(fractions[i].GradingSubfractions[j].FractionBoundaries.ToString());
                }
                for (int j = 0; j < fractions[i].ShapeSubfractions.Length; j++)
                {
                    mp.FrProperties[i].ShapeIndex.FrNames[j] = new string(fractions[i].ShapeSubfractions[j].FractionBoundaries.ToString());
                }
                for (int j = 0; j < fractions[i].FlatSubfractions.Length; j++)
                {
                    mp.FrProperties[i].FlatIndex.FrNames[j] = new string(fractions[i].FlatSubfractions[j].FractionBoundaries.ToString());
                }
            }

            //summing volumes for grading categories, shape and flat indices of fractions
            for (int i = 0; i < noOfStones; i++)
            {
                float volume = AllStonesProperties[i].GetVolume();
                stonesVolume += volume;

                int frIndex = AllStonesProperties[i].fractionIndex;
                (int, int, int) indGrFlSh = AllStonesProperties[i].indGrFlSh;

                mp.FrProperties[frIndex].VolumeInModel += volume;
                mp.FrProperties[frIndex].GradingCurve.FrVolumes[indGrFlSh.Item1] += volume;
                mp.FrProperties[frIndex].ShapeIndex.FrVolumes[indGrFlSh.Item3] += volume;
                mp.FrProperties[frIndex].FlatIndex.FrVolumes[indGrFlSh.Item2] += volume;

                if(AllStonesProperties[i].IsLong)
                {
                    mp.FrProperties[frIndex].ShapeIndex.LongVolumes[indGrFlSh.Item3] += volume;
                }

                if(AllStonesProperties[i].IsFlat)
                {
                    mp.FrProperties[frIndex].FlatIndex.FlatVolumes[indGrFlSh.Item2] += volume;
                }
            }

            //counting percentage from volumes
            for (int i = 0; i < fractions.Count; i++)
            {
                
                mp.FrProperties[i].VolumePercentageInModel = stonesVolume == 0f ? 0f : mp.FrProperties[i].VolumeInModel / stonesVolume * 100f;
                
                for(int j = 0; j < mp.FrProperties[i].GradingCurve.Percentage.Length; j++)
                {
                    mp.FrProperties[i].GradingCurve.Percentage[j] = mp.FrProperties[i].VolumeInModel == 0f? 0f : mp.FrProperties[i].GradingCurve.FrVolumes[j] / mp.FrProperties[i].VolumeInModel * 100f;
                }

                float LongEllipsoidsVolumeSum = 0f;
                for(int j = 0; j < mp.FrProperties[i].ShapeIndex.LongPercentage.Length; j++)
                {
                    mp.FrProperties[i].ShapeIndex.LongPercentage[j] = mp.FrProperties[i].ShapeIndex.FrVolumes[j] == 0f ? 0f : mp.FrProperties[i].ShapeIndex.LongVolumes[j] / mp.FrProperties[i].ShapeIndex.FrVolumes[j] * 100f;
                    LongEllipsoidsVolumeSum += mp.FrProperties[i].ShapeIndex.LongVolumes[j];
                }

                
                //shape index for "big" fraction
                mp.FrProperties[i].ShapeIndex.FractionShapeIndex = mp.FrProperties[i].VolumeInModel == 0f ? 0f : LongEllipsoidsVolumeSum / mp.FrProperties[i].VolumeInModel * 100f;


                float FlatEllipsoidsVolumeSum = 0f;
                for (int j = 0; j < mp.FrProperties[i].FlatIndex.FlatPercentage.Length; j++)
                {
                    mp.FrProperties[i].FlatIndex.FlatPercentage[j] = mp.FrProperties[i].FlatIndex.FrVolumes[j] == 0f ? 0f : mp.FrProperties[i].FlatIndex.FlatVolumes[j] / mp.FrProperties[i].FlatIndex.FrVolumes[j] * 100f;
                    FlatEllipsoidsVolumeSum += mp.FrProperties[i].FlatIndex.FlatVolumes[j];
                }


                //flat index for "big" fraction
                mp.FrProperties[i].FlatIndex.FractionFlatIndex = mp.FrProperties[i].VolumeInModel == 0f ? 0f : FlatEllipsoidsVolumeSum / mp.FrProperties[i].VolumeInModel * 100f;
            }


            //text -- properties for txt file
            textFractionsResults = mp.FractionPropertiesToString();

        }
    }
    #endregion

}
