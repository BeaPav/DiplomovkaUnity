using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEditor;

using FractionDefinition;
using PropertiesCounter;


// chyba tu este grading curve vysledny pre porovnanie 


public class FrNumTesting : MonoBehaviour
{

    [SerializeField] GameObject Ellipsoid;
    [SerializeField] string Fraction = "[4,8]";
    [SerializeField] int noOfStonesToGenerate = 0;
    [SerializeField] int noRotations;
    [SerializeField] float meshScaleFactor;
    [SerializeField] bool WriteInConsole = false;
    [SerializeField] bool DoneTesting = false;
    [SerializeField] bool Save;


    Dictionary<string, int> FractionIndex;
    List<Fraction> Fractions;
    Fraction ActiveFraction;
    Fraction RotFrNumFraction;

    //pocetnost
    [ReadOnly] public float errorFrNumDifference;       //rozdiel dvoch ziskanych frNum
    [ReadOnly] public int errorBiggerFraction = 0;      //ak by sa zaradil do vacsej ako aktualna (4/8,8/16,16/22), pricom je to vacsie ako nadsit
    [ReadOnly] public int errorSmallerFraction = 0;     //ak by sa zaradil do mensej ako aktualna (4/8,8/16,16/22), pricom je to mensie ako podsit
    [ReadOnly] public int errorGrFractionStones = 0;    //ak sa inak zaradi do grading v danej frakcii
    [ReadOnly] public int errorShFractionStones = 0;    //ak sa inak zaradi do shape v danej frakcii
    [ReadOnly] public int errorFlFractionStones = 0;    //ak sa inak zaradi do flat v danej frakcii

    [ReadOnly] public float errorBiggerFractionPercentage = 0;      //% ak by sa zaradil do vacsej ako aktualna (4/8,8/16,16/22), pricom je to vacsie ako nadsit
    [ReadOnly] public float errorSmallerFractionPercentage = 0;     //% ak by sa zaradil do mensej ako aktualna (4/8,8/16,16/22), pricom je to mensie ako podsit
    [ReadOnly] public float errorGrFractionStonesPercentage = 0;    //% ak sa inak zaradi do grading v danej frakcii
    [ReadOnly] public float errorShFractionStonesPercentage = 0;    //% ak sa inak zaradi do shape v danej frakcii
    [ReadOnly] public float errorFlFractionStonesPercentage = 0;    //% ak sa inak zaradi do flat v danej frakcii


    //objem
    [ReadOnly] public float GeneratedVolume = 0f;
    [ReadOnly] public float GeneratedVolumeInRotFraction = 0f;

    [ReadOnly] public float errorBiggerFractionVolume = 0;      //ak by sa zaradil do vacsej ako aktualna (4/8,8/16,16/22), pricom je to vacsie ako nadsit
    [ReadOnly] public float errorSmallerFractionVolume = 0;     //ak by sa zaradil do mensej ako aktualna (4/8,8/16,16/22), pricom je to mensie ako podsit
    [ReadOnly] public float errorGrFractionStonesVolume = 0;    //ak sa inak zaradi do grading v danej frakcii
    [ReadOnly] public float errorShFractionStonesVolume = 0;    //ak sa inak zaradi do shape v danej frakcii
    [ReadOnly] public float errorFlFractionStonesVolume = 0;    //ak sa inak zaradi do flat v danej frakcii

    [ReadOnly] public float errorBiggerFractionVolumePercentage = 0;      //% ak by sa zaradil do vacsej ako aktualna (4/8,8/16,16/22), pricom je to vacsie ako nadsit
    [ReadOnly] public float errorSmallerFractionVolumePercentage = 0;     //% ak by sa zaradil do mensej ako aktualna (4/8,8/16,16/22), pricom je to mensie ako podsit
    [ReadOnly] public float errorGrFractionStonesVolumePercentage = 0;    //% ak sa inak zaradi do grading v danej frakcii
    [ReadOnly] public float errorShFractionStonesVolumePercentage = 0;    //% ak sa inak zaradi do shape v danej frakcii
    [ReadOnly] public float errorFlFractionStonesVolumePercentage = 0;    //% ak sa inak zaradi do flat v danej frakcii


    [ReadOnly] public string[] GradingCurveFrNames;
    [ReadOnly] public float[] GradingCurveEllPercentage;
    [ReadOnly] public float[] GradingCurveEllVolumes;
    [ReadOnly] public float[] GradingCurveRotPercentage;
    [ReadOnly] public float[] GradingCurveRotVolumes;

    public int nesediGrAEllJeViacAkoRot = 0;
    public float nesediGrAEllJeViacAkoRotPercentage;

    // Start is called before the first frame update
    void Start()
    {
        #region FRACTIONS INICIALIZATION
        float[] FractionRatios = new float[3] { 0f, 0f, 0f};
        FractionIndex = new Dictionary<string, int>()
                { {"[4,8]", 0 }, {"[8,16]", 1 }, {"[16,22]", 2 } };
        FractionRatios[FractionIndex[Fraction]] = 1f;

        Fractions = new List<Fraction>();
        //uvadzane v cm //?
        Fractions.Add(new Fraction((0.4f, 0.8f), //d/D
                                   FractionRatios[0],           //kolko % tejto frakcie miesat
                                   
                                   new float[5] { 0.2f, 0.4f, 0.56f, 0.8f, 1.12f },             //OK        //sitovy rozbor - hranice
                                   new float[4] { 0.0809f, 0.3699f, 0.461f, 0.0858f },          //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta
                                   /*
                                   new float[3] { 0.4f, 0.6f, 0.8f },   //sitovy rozbor - hranice
                                   new float[2] { 0.8f, 0.2f },
                                   */



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
                                   /*
                                   new float[3] { 0.8f, 1.12f, 1.6f },   //sitovy rozbor - hranice
                                   new float[2] { 0.4f, 0.6f },
                                   */


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
        #endregion

        ActiveFraction = Fractions[FractionIndex[Fraction]];

        #region RotFrNumFraction INICIALIZATION
        if (FractionIndex[Fraction] == 0)
        {
            RotFrNumFraction = new Fraction((0.4f, 0.8f), //d/D
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
                                   );
        }
        else if(FractionIndex[Fraction] == 1)
        {
            RotFrNumFraction = new Fraction((0.8f, 1.6f), //d/D
                                   FractionRatios[1],         //kolko % tejto frakcie miesat


                                   new float[5] { 0.4f, 0.56f, 0.8f, 1.12f, 1.6f },                  //OK        //sitovy rozbor - hranice
                                   new float[4] { 0.0005f, 0.0148f, 0.2388f, 0.7446f },               //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta
                                   
                                   new float[2] { 0.4f, 1.6f },                                    //OK        //tvarovy index - hranice
                                   new float[1] { 0.1198f },                                        //OK        //tvarovy index - hodnoty

                                   new float[7] { 0.4f, 0.5f, 0.63f, 0.8f, 1.0f, 1.25f, 1.6f },     //OK        //index plochosti - hranice
                                   new float[6] { 0.25f, 0.315f, 0.4f, 0.5f, 0.63f, 0.8f },         //OK        //index plochosti - harfove sita medzery
                                   new float[6] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f }              //ODHADNUTE //index plochosti - hodnoty
                                   );
        }
        else
        {
            RotFrNumFraction = new Fraction((1.6f, 2.2f), //d/D
                                   FractionRatios[1],         //kolko % tejto frakcie miesat

                                   new float[3] { 1.6f, 2.24f, 3.15f },         //OK        //sitovy rozbor - hranice
                                   new float[2] { 0.7672f, 0.2328f },           //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta

                                   new float[2] { 1.6f, 3.15f },                //OK        //tvarovy index - hranice
                                   new float[1] { 0.1725f },                    //OK        //tvarovy index - hodnoty

                                   new float[4] { 1.6f, 2.0f, 2.5f, 3.15f },    //OK        //index plochosti - hranice
                                   new float[3] { 1.0f, 1.25f, 1.6f },          //OK        //index plochosti - harfove sita medzery
                                   new float[3] { 0.1f, 0.1f, 0.1f }            //ODHADNUTE //index plochosti - hodnoty
                                   );
        }
        #endregion

    }

    // Update is called once per frame
    void Update()
    {


        if (!DoneTesting)
        {

            //Creating a new stone object
            GameObject stone = Instantiate(Ellipsoid, transform.position, Quaternion.identity, transform);
            stone.name = "EllipsoidFrNumTesting";
            stone.GetComponent<Rigidbody>().isKinematic = true;

            for (int i = 0; i < noOfStonesToGenerate; i++)
            {
                stone.GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(ActiveFraction, meshScaleFactor);
                

                StoneMeshProperties s = stone.GetComponent<StoneMeshProperties>();
                float volume = s.GetVolume();
                GeneratedVolume += volume;
                float frNumEllipsoid = s.GetFractionNumber();
                float frNumRotation = Prop.FrNumber(stone, noRotations);

                //ratanie chyby - rozdiel dvoch frNum
                //errorFrNumDifference = Mathf.Abs(frNumEllipsoid - frNumRotation);
                //if (WriteInConsole) Debug.Log("rozdiel dvoch frNum: " + errorFrNumDifference);


                //ak prekroci frNum cez rotacie najmensiu/najvacsiu hranicu pri grading - zaradili by sme ho do inej frakcie, tj ani nevysetrujeme dalej
                bool fractionError = false;
                if (ActiveFraction.GradingSubfractions[ActiveFraction.GradingSubfractions.Length - 1].FractionBoundaries.Item2 < frNumRotation)
                {
                    fractionError = true;
                    errorBiggerFraction++;
                    errorBiggerFractionVolume += volume;
                    //Debug.Log("Zaradili by sme kamen do vacsej frakcie");
                }
                else if (ActiveFraction.GradingSubfractions[0].FractionBoundaries.Item1 > frNumRotation)
                {
                    fractionError = true;
                    errorSmallerFraction++;
                    errorSmallerFractionVolume += volume;
                    //Debug.Log("Zaradili by sme kamen do mensej frakcie");
                }



                if (!fractionError)
                {
                    //ratanie chyby - rozdiel dvoch dvoch indexov kam zaraduje frNum kamen v danej frakcii pre grading, shape a flat
                    int frGrIndexEllipsoid = IndexFromFractionVector(frNumEllipsoid, ActiveFraction.GradingSubfractions);
                    int frGrIndexRotation = IndexFromFractionVector(frNumRotation, ActiveFraction.GradingSubfractions);

                    int frShIndexEllipsoid = IndexFromFractionVector(frNumEllipsoid, ActiveFraction.ShapeSubfractions);
                    int frShIndexRotation = IndexFromFractionVector(frNumRotation, ActiveFraction.ShapeSubfractions);

                    int frFlIndexEllipsoid = IndexFromFractionVector(frNumEllipsoid, ActiveFraction.FlatSubfractions);
                    int frFlIndexRotation = IndexFromFractionVector(frNumRotation, ActiveFraction.FlatSubfractions);

                    int errorGrIndex = Mathf.Abs(frGrIndexEllipsoid - frGrIndexRotation);
                    if (errorGrIndex != 0)
                    {
                        errorGrFractionStones++;
                        errorGrFractionStonesVolume += volume;
                        /*
                        //toto bolo pri rieseni problemu ze frNumRot zaraduje kamene do mensej frakcie ako frNumEll, 
                        //ktora teraz vyjadruje najmensi stvorec kade kamen prepadne, tj, frNumRot by malo byt viac... 
                        //Asi chyba pri vypoctoch a numerike

                        if (frGrIndexEllipsoid > frGrIndexRotation)
                        {
                            nesediGrAEllJeViacAkoRot++;
                            GameObject kamen = Instantiate(stone, transform.position, Quaternion.identity, transform);
                            kamen.GetComponent<StoneMeshProperties>().EllGrFrIndexIsMoreRotGrFrIndex = true;
                            Debug.Log("EllFrNum " + frNumEllipsoid + " RotFrNum " + frNumRotation);
                            Debug.Log("EllGrIndex " + frGrIndexEllipsoid + " RotGrIndex " + frGrIndexRotation);
                            Debug.Log("axes " + kamen.GetComponent<StoneMeshProperties>().axes);
                        }
                        */
                    }


                    int errorShIndex = Mathf.Abs(frShIndexEllipsoid - frShIndexRotation);
                    if (errorShIndex != 0)
                    {
                        errorShFractionStones++;
                        errorShFractionStonesVolume += volume;
                    }

                    int errorFlIndex = Mathf.Abs(frFlIndexEllipsoid - frFlIndexRotation);
                    if (errorFlIndex != 0)
                    {
                        errorFlFractionStones++;
                        errorFlFractionStonesVolume += volume; 
                    }

                    //////////////////////////este tto doriesit
                     RotFrNumFraction.ActualizeVolume(s.GetVolume(), (frGrIndexRotation, frFlIndexRotation, frShIndexRotation), (s.GetIsFlat(), s.GetIsLong()));
                    GeneratedVolumeInRotFraction += volume;
                }

            }

            //pocetnost
            errorBiggerFractionPercentage = errorBiggerFraction / (float)noOfStonesToGenerate * 100;
            errorSmallerFractionPercentage = errorSmallerFraction / (float)noOfStonesToGenerate * 100;
            int errorFractionStones = errorBiggerFraction + errorSmallerFraction;
            int notErrorFractionStones = noOfStonesToGenerate - errorFractionStones;

            errorGrFractionStonesPercentage = errorGrFractionStones / (float)notErrorFractionStones * 100;
            errorShFractionStonesPercentage = errorShFractionStones / (float)notErrorFractionStones * 100;
            errorFlFractionStonesPercentage = errorFlFractionStones / (float)notErrorFractionStones * 100;

            nesediGrAEllJeViacAkoRotPercentage = nesediGrAEllJeViacAkoRot / (float)errorGrFractionStones * 100;


            //objemy
            errorBiggerFractionVolumePercentage = errorBiggerFractionVolume / GeneratedVolume * 100;
            errorSmallerFractionVolumePercentage = errorSmallerFractionVolume / GeneratedVolume * 100;

            float errorFractionStonesVolume = errorBiggerFractionVolume + errorSmallerFractionVolume;
            double notErrorFractionStonesVolume = (double)GeneratedVolume - (double)errorFractionStonesVolume;


            errorGrFractionStonesVolumePercentage = errorGrFractionStonesVolume / (float)notErrorFractionStonesVolume * 100f;
            errorShFractionStonesVolumePercentage = errorShFractionStonesVolume / (float)notErrorFractionStonesVolume * 100f;
            errorFlFractionStonesVolumePercentage = errorFlFractionStonesVolume / (float)notErrorFractionStonesVolume * 100f;




            //toto sa rata ze zo vsetkeho co bolo vygenerovane, tu nam to takto nevadi, v modeli treba ratat len z kamenov v nadobe
            ActiveFraction.GradingCurve(out GradingCurveFrNames, out GradingCurveEllPercentage, out GradingCurveEllVolumes, out string TextGradingEllipsoidResults);
            RotFrNumFraction.GradingCurve(out GradingCurveRotPercentage, out GradingCurveRotVolumes, out string TextGradingRotResults);

            string textResults = "FrNum TESTING \n\n\n" + "ELL GRADING CURVE\n\n" + TextGradingEllipsoidResults + "\n\n\n" + "ROT GRADING CURVE\n\n" + TextGradingRotResults + "\n\n\n";

            textResults += "CHYBY V POCETNOSTI \n\n\n";
            textResults += "ERROR BIGGER FRACTION % \t" + errorBiggerFractionPercentage + "\n";
            textResults += "ERROR SMALLER FRACTION % \t" + errorSmallerFractionPercentage + "\n";
            textResults += "ERROR GRADING FRACTION % \t" + errorGrFractionStonesPercentage + "\n";
            textResults += "ERROR FLAT FRACTION % \t" + errorFlFractionStonesPercentage + "\n";
            textResults += "ERROR SHAPE FRACTION % \t" + errorShFractionStonesPercentage + "\n\n\n";

            textResults += "CHYBY V OBJEMOCH \n\n\n";
            textResults += "ERROR BIGGER FRACTION % \t" + errorBiggerFractionVolumePercentage + "\n";
            textResults += "ERROR SMALLER FRACTION % \t" + errorSmallerFractionVolumePercentage + "\n";
            textResults += "ERROR GRADING FRACTION % \t" + errorGrFractionStonesVolumePercentage + "\n";
            textResults += "ERROR FLAT FRACTION % \t" + errorFlFractionStonesVolumePercentage + "\n";
            textResults += "ERROR SHAPE FRACTION % \t" + errorShFractionStonesVolumePercentage + "\n\n\n";





            if (WriteInConsole)
            {
                Debug.Log("aktualna chyba zaradovania do frakcie:");
                Debug.Log("mensia frakcia %: " + errorSmallerFractionPercentage + " zle: " + errorSmallerFraction + " vsetky: " + noOfStonesToGenerate);
                Debug.Log("vacsia frakcia %: " + errorBiggerFractionPercentage + " zle: " + errorBiggerFraction + " vsetky: " + noOfStonesToGenerate);
                Debug.Log("zle grading %: " + errorGrFractionStonesPercentage + " zle: " + errorGrFractionStones + " vsetky: " + notErrorFractionStones);
                Debug.Log("zle shape %: " + errorShFractionStonesPercentage + " zle: " + errorShFractionStones + " vsetky: " + notErrorFractionStones);
                Debug.Log("zle flat %: " + errorFlFractionStonesPercentage + " zle: " + errorFlFractionStones + " vsetky: " + notErrorFractionStones);

                Debug.Log("Grading:");


                for (int i = 0; i < ActiveFraction.GradingSubfractions.Length; i++)
                {
                    Debug.Log("d/D:  " + GradingCurveFrNames[i] +
                             "  reqVol:  " + (ActiveFraction.GradingSubfractions[i].RequiredVolumePart) + "%" +
                             "  EllfrNum:  " + (GradingCurveEllPercentage[i]) + "%" +
                             "  EllfrNum:  " + (GradingCurveRotPercentage[i]) + "%");
                }
            }

            if (Save)
            {
                if (!Directory.Exists("Assets/SavedModels/TestingModels/FrNumTest/Fraction" + Fraction + "/Scale" + meshScaleFactor))
                {
                    Directory.CreateDirectory("Assets/SavedModels/TestingModels/FrNumTest/Fraction" + Fraction + "/Scale" + meshScaleFactor);
                    AssetDatabase.Refresh();
                }

                ModelSavingSystem.SaveTestingModel(transform, "Assets/SavedModels/TestingModels/FrNumTest/Fraction" + Fraction + "/Scale" + meshScaleFactor,
                                        "Model_" + Fraction + "_" + noOfStonesToGenerate + "stones_" + noRotations + "rotations_" + meshScaleFactor + "Scale",textResults, false, 1,
                                        "Stones" + noOfStonesToGenerate + "_Rotations" + noRotations + "_Scale" + meshScaleFactor);
            }

            DoneTesting = true;

        }
    }

    //urcenie indexu zo zoznamu frakcii podla zaradenia cez frNum
    int IndexFromFractionVector(float num, Fraction[] frac)
    {
        int i = 0;
        if (frac[frac.Length - 1].FractionBoundaries.Item2 < num)
        {
            Debug.Log("prekrocenie hranic pri zaradovani do frakcie");
            return 1000;
        }

        while (frac[i].FractionBoundaries.Item2 < num)
        {
            i++;
        }

        return i;
    }
}
