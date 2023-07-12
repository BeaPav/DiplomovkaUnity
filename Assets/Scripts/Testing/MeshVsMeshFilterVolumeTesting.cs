using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FractionDefinition;
using PropertiesCounter;

public class MeshVsMeshFilterVolumeTesting : MonoBehaviour
{

    [SerializeField] GameObject Ellipsoid;
    [SerializeField] int noOfStonesToGenerate = 0;
    [SerializeField] bool DoneTesting = false;
    [SerializeField] bool Save;

    float[] FractionRatios;
    List<Fraction> Fractions;
    int ActiveFractionIndex;

    float EllipsoidsActualVolume = 0f;




    // Start is called before the first frame update
    void Start()
    {
        FractionRatios = new float[3] { 0.3f, 0.3f, 0.4f };

        Fractions = new List<Fraction>();
        //uvadzane v cm //?
        Fractions.Add(new Fraction((0.4f, 0.8f), //d/D
                                   FractionRatios[0],           //kolko % tejto frakcie miesat

                                   new float[5] { 0.2f, 0.4f, 0.56f, 0.8f, 1.12f },             //OK        //sitovy rozbor - hranice
                                   new float[4] { 0.0809f, 0.3699f, 0.461f, 0.0858f },          //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta

                                   new float[2] { 0.2f, 1.12f },                                //OK        //tvarovy index - hranice
                                   new float[1] { 0.0505f },                                    //OK        //tvarovy index - hodnoty

                                   new float[7] { 0.2f, 0.4f, 0.5f, 0.63f, 0.8f, 1.0f, 1.25f }, //OK        //index plochosti - hranice
                                                                                                //toto je len od sita 0.4, ale to mensie je tam kvoli hraniciam,
                                                                                                //aby sme nevyskocilli out of rangea preto davame hodnotu indexu nula
                                   new float[6] { 0.2f, 0.25f, 0.315f, 0.4f, 0.5f, 0.63f },     //OK        //index plochosti - harfove sita medzery
                                   new float[6] { 0f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f }            //ODHADNUTE //index plochosti - hodnoty
                                   ));

        Fractions.Add(new Fraction((0.8f, 1.6f), //d/D
                                   FractionRatios[1],         //kolko % tejto frakcie miesat

                                   new float[5] { 0.4f, 0.56f, 0.8f, 1.12f, 1.6f },                  //OK        //sitovy rozbor - hranice
                                   new float[4] { 0.0005f, 0.0148f, 0.2388f, 0.7446f },               //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta

                                   new float[2] { 0.4f, 1.6f },                                    //OK        //tvarovy index - hranice
                                   new float[1] { 0.1198f },                                        //OK        //tvarovy index - hodnoty

                                   new float[7] { 0.4f, 0.5f, 0.63f, 0.8f, 1.0f, 1.25f, 1.6f },     //OK        //index plochosti - hranice
                                   new float[6] { 0.25f, 0.315f, 0.4f, 0.5f, 0.63f, 0.8f },         //OK        //index plochosti - harfove sita medzery
                                   new float[6] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f }              //ODHADNUTE //index plochosti - hodnoty
                                   ));

        Fractions.Add(new Fraction((1.6f, 2.2f), //d/D
                                   FractionRatios[2],         //kolko % tejto frakcie miesat

                                   new float[3] { 1.6f, 2.24f, 3.15f },         //OK        //sitovy rozbor - hranice
                                   new float[2] { 0.7672f, 0.2328f },           //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta

                                   new float[2] { 1.6f, 3.15f },                //OK        //tvarovy index - hranice
                                   new float[1] { 0.1725f },                    //OK        //tvarovy index - hodnoty

                                   new float[4] { 1.6f, 2.0f, 2.5f, 3.15f },    //OK        //index plochosti - hranice
                                   new float[3] { 1.0f, 1.25f, 1.6f },          //OK        //index plochosti - harfove sita medzery
                                   new float[3] { 0.1f, 0.1f, 0.1f }            //ODHADNUTE //index plochosti - hodnoty
                                   ));
    }

    // Update is called once per frame
    void Update()
    {


        if (!DoneTesting)
        {

            //Creating a new stone object
            GameObject stone = Instantiate(Ellipsoid, transform.position, Quaternion.identity, transform);
            stone.name = "MeshVsMeshFilterVolumeTesting";
            stone.GetComponent<Rigidbody>().isKinematic = true;

            for (int i = 0; i < noOfStonesToGenerate; i++)
            {
                ActiveFractionIndex = FractionChoice();
                stone.GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(Fractions[ActiveFractionIndex]);
                stone.GetComponent<StoneMeshProperties>().fractionIndex = ActiveFractionIndex;
                EllipsoidsActualVolume += stone.GetComponent<StoneMeshProperties>().GetVolume();
                
                
                float volMeshFilter = stone.GetComponent<StoneMeshProperties>().GetVolume();
                MeshCollider mc = stone.GetComponentInChildren<MeshCollider>();
                Transform mcObj = mc.transform;

                float xScale = mcObj.localScale.x;
                float yScale = mcObj.localScale.y;
                float zScale = mcObj.localScale.z;

                Transform parent = mcObj.parent;

                while (parent != null)
                {
                    xScale *= parent.localScale.x;
                    yScale *= parent.localScale.y;
                    zScale *= parent.localScale.z;
                    parent = parent.parent;
                }

                float volMeshCollider = Prop.VolumeOfMesh(mc.sharedMesh, xScale, yScale, zScale);

                if (volMeshCollider - volMeshFilter != 0)
                {
                    Debug.Log("MeshFilter: " + volMeshFilter + " MeshCollider: " + volMeshCollider);
                    Debug.Log("rozdiel objemov col-filter: " + (volMeshCollider - volMeshFilter));
                    Debug.Log("ake % objemu je coll vzhladom na filter: " + (volMeshCollider / volMeshFilter * 100f));
                }

            }


            /*
            if (Save)
            {
                ModelSavingSystem.SaveTestingModel(transform, "Assets/SavedModels/TestingModels/GeneratingDensityTest",
                                        "Model_" + noOfStonesToGenerate + "stones", false, 1);
            }
            */
            DoneTesting = true;
        }
    }


    public int FractionChoice()
    {
        int index = -1;

        if (Fractions != null && Fractions.Count > 0)
        {
            if (EllipsoidsActualVolume == 0f)
            {
                int j = 0;
                while (j < Fractions.Count && Fractions[j].RequiredVolumePart == 0)
                {
                    j++;
                }

                if (Fractions.Count < j)
                {
                    Debug.LogError("V ellipsoidSpawning mame vsetky frakcie requredVolume 0");
                }

                return j;
            }

            float min = float.MaxValue;

            for (int i = 0; i < Fractions.Count; i++)
            {
                float dif = Fractions[i].ActualFractionVolume / EllipsoidsActualVolume - Fractions[i].RequiredVolumePart;
                if (dif < min)
                {
                    min = dif;
                    index = i;
                }
            }
        }
        return index;
    }
}
