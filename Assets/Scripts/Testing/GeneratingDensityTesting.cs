using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using FractionDefinition;
using PropertiesCounter;

public class GeneratingDensityTesting : MonoBehaviour
{

    [SerializeField] GameObject Ellipsoid;
    [SerializeField] int noOfStonesToGenerate = 0;
    [SerializeField] bool DoneTesting = false;
    [SerializeField] bool Save;

    float[] FractionRatios;
    List<Fraction> Fractions;
    int ActiveFractionIndex;

    [ReadOnly] public float EllipsoidsActualVolume = 0f;

    [SerializeField] public FractionProperties[] FrProperties;




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

        if (FractionRatios.Length != Fractions.Count)
            Debug.LogError("ina dlzka vektorov pre frakcie a vektora pomerov pre miesanie frakcii");

        //inicializacia vlastnosti frakcii
        FrProperties = new FractionProperties[Fractions.Count];

        for (int i = 0; i < Fractions.Count; i++)
        {
            FrProperties[i] = new FractionProperties();
            FrProperties[i].FractionName = Fractions[i].FractionBoundaries.ToString();
            FrProperties[i].GradingCurve = new GradingCurve(Fractions[i].GradingSubfractions.Length);
            FrProperties[i].ShapeIndex = new ShapeIndex(Fractions[i].ShapeSubfractions.Length);
            FrProperties[i].FlatIndex = new FlatInex(Fractions[i].FlatSubfractions.Length);

            //// mena dame do funkcii ako je to v grading
            /*
            //mena pre hranice frakcii
            for (int j = 0; j < Fractions[i].GradingSubfractions.Length; j++)
            {
                FrProperties[i].GradingCurve.FrNames[j] = new string(Fractions[i].GradingSubfractions[j].FractionBoundaries.ToString());
            }
            for (int j = 0; j < Fractions[i].ShapeSubfractions.Length; j++)
            {
                FrProperties[i].ShapeIndex.FrNames[j] = new string(Fractions[i].ShapeSubfractions[j].FractionBoundaries.ToString());
            }
            for (int j = 0; j < Fractions[i].FlatSubfractions.Length; j++)
            {
                FrProperties[i].FlatIndex.FrNames[j] = new string(Fractions[i].FlatSubfractions[j].FractionBoundaries.ToString());
            }
            */
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (!DoneTesting)
        {

            //Creating a new stone object
            GameObject stone = Instantiate(Ellipsoid, transform.position, Quaternion.identity, transform);
            stone.name = "EllGeneratingDensityTesting";
            stone.GetComponent<Rigidbody>().isKinematic = true;

            for (int i = 0; i < noOfStonesToGenerate; i++)
            {
                ActiveFractionIndex = FractionChoice();
                stone.GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(Fractions[ActiveFractionIndex]);
                stone.GetComponent<StoneMeshProperties>().fractionIndex = ActiveFractionIndex;
                EllipsoidsActualVolume += stone.GetComponent<StoneMeshProperties>().GetVolume();

            }

            string textResults = "";
            string textTMPResults;
            for(int i = 0; i< Fractions.Count; i++)
            {
                Fractions[i].GradingCurve(out FrProperties[i].GradingCurve.FrNames, out FrProperties[i].GradingCurve.Percentage, out FrProperties[i].GradingCurve.FrVolumes, out textTMPResults);
                Fractions[i].ShapeIndex(out FrProperties[i].ShapeIndex.FrNames, out FrProperties[i].ShapeIndex.LongPercentage, out FrProperties[i].ShapeIndex.LongVolumes, 
                                        out FrProperties[i].ShapeIndex.FrVolumes, out FrProperties[i].ShapeIndex.FractionShapeIndex, out textTMPResults);
                Fractions[i].FlatIndex(out FrProperties[i].FlatIndex.FrNames, out FrProperties[i].FlatIndex.FlatPercentage, out FrProperties[i].FlatIndex.FlatVolumes,
                                        out FrProperties[i].FlatIndex.FrVolumes, out FrProperties[i].FlatIndex.FractionFlatIndex, out textTMPResults);
                FrProperties[i].VolumeInModel = Fractions[i].ActualFractionVolume;
                FrProperties[i].VolumePercentageInModel = EllipsoidsActualVolume == 0f ? 0f : Fractions[i].ActualFractionVolume / EllipsoidsActualVolume * 100f;

                textTMPResults = FrProperties[i].ResultsToString();
                textResults += textTMPResults + "\n\n\n";
            }


            
            if (Save)
            {
                ModelSavingSystem.SaveTestingModel(transform, "Assets/SavedModels/TestingModels/GeneratingDensityTest",
                                        "Model_" + noOfStonesToGenerate + "stones", textResults, false, 1);
            }
            
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
