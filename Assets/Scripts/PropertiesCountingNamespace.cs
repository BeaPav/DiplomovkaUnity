using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropertiesCounter
{
    public static class f
    {
        #region COUNTING OF MESH VOLUME
        private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float v321 = p3.x * p2.y * p1.z;
            float v231 = p2.x * p3.y * p1.z;
            float v312 = p3.x * p1.y * p2.z;
            float v132 = p1.x * p3.y * p2.z;
            float v213 = p2.x * p1.y * p3.z;
            float v123 = p1.x * p2.y * p3.z;

            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }

        public static float VolumeOfMesh(Mesh mesh)
        {
            float volume = 0;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 p1 = vertices[triangles[i + 0]];
                Vector3 p2 = vertices[triangles[i + 1]];
                Vector3 p3 = vertices[triangles[i + 2]];
                volume += SignedVolumeOfTriangle(p1, p2, p3);
            }
            
            
            return Mathf.Abs(volume);
        }
        #endregion

        #region LENGTH AND WIDTH    
        //COUNTING OF STONE`S    WIDTH = minimal distance of two paralell planes
        //                       LENGTH = maximal distance of two paralell planes
        public static Vector2 GetLengthAndWidthOfStone(GameObject stone)
        {
            Renderer meshRenderer = stone.GetComponentInChildren<Renderer>();

            //Saving of original transformations for later
            Vector3 originalPos = stone.transform.position;
            Quaternion originalRot = stone.transform.rotation;
            Vector3 originalScale = stone.transform.localScale;

            //Inicialization of length and width
            float width = Mathf.Min(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y, meshRenderer.bounds.size.z);
            float length = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y, meshRenderer.bounds.size.z);

            //Rotations step to create AABB of actual rotation
            Vector3 zrot = new Vector3(0,0,10);
            Vector3 xrot = new Vector3(10,0,0);

            //Rotating and comparing AABBs in order to find minimal and maximal dimensions posiible
            for (int i = 1; i <= 36; i++)
            {
                for (int j = 1; j <= 36; j++)
                {
                    stone.transform.Rotate(zrot);
                   
                    float min = Mathf.Min(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y, meshRenderer.bounds.size.z);
                    float max = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y, meshRenderer.bounds.size.z);

                    if (min < width)
                        width = min;
                    if (max > length)
                        length = max;
                }

                stone.transform.Rotate(xrot);
            }

            //Return stones original transformations
            stone.transform.position = originalPos;
            stone.transform.rotation = originalRot;
            stone.transform.localScale = originalScale;

            return new Vector2(length, width);
        }
        #endregion


        #region FRACTION NUMBER

        public static float FrNumber(GameObject stone)
        {
            Renderer meshRenderer = stone.GetComponentInChildren<Renderer>();

            // Saving of original transformations for later
            Vector3 originalPos = stone.transform.position;
            Quaternion originalRot = stone.transform.rotation;
            Vector3 originalScale = stone.transform.localScale;

            //Inicialization of FrNumber
            float frNum = float.MaxValue;
            int numOfRot = 20;

            //Rotating stone and finding the square witch the stone falls through
            for (int i = 0; i < numOfRot; i++)
            {
                stone.transform.rotation = Random.rotation;
                float max = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.z);

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
