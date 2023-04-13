using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public static float VolumeOfMesh(MeshFilter mf)
        {
            Transform tr = mf.transform;
            Mesh mesh = mf.mesh;
            float volume = 0;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            //Vector3 T = new Vector3();

            for (int i = 0; i < triangles.Length; i += 3)
            {

                Vector3 v1 = vertices[triangles[i + 0]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];

                float xScale = tr.localScale.x;
                float yScale = tr.localScale.y;
                float zScale = tr.localScale.z;

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
            /*
            T = T / vertices.Length;
            Debug.Log("tazisko: " + T);
            */
            
            return Mathf.Abs(volume);
        }
        #endregion

        #region LENGTH AND WIDTH    
        //COUNTING OF STONE`S    WIDTH = minimal distance of two paralell planes
        //                       LENGTH = maximal distance of two paralell planes
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

        #region FRACTION NUMBER

        public static float FrNumber(GameObject stone, int numOfRot)
        {
            MeshFilter meshFilter = stone.GetComponentInChildren<MeshFilter>();
            Bounds b;

            // Saving of original transformations for later
            Vector3 originalPos = stone.transform.position;
            Quaternion originalRot = stone.transform.rotation;
            Vector3 originalScale = stone.transform.localScale;

            //Inicialization of FrNumber
            float frNum = float.MaxValue;

            //Rotating stone and finding the square witch the stone falls through
            for (int i = 0; i < numOfRot; i++)
            {
                stone.transform.rotation = Random.rotation;
                b = GeometryUtility.CalculateBounds(meshFilter.sharedMesh.vertices, meshFilter.transform.localToWorldMatrix);

                float max = Mathf.Max(b.size.x, b.size.z);

                //Searching for minimum of all maximums
                if (max < frNum) frNum = max;
            }

            //Return stones original transformations
            stone.transform.position = originalPos;
            stone.transform.rotation = originalRot;
            stone.transform.localScale = originalScale;

            return frNum;
        }

        #endregion

    }

}
