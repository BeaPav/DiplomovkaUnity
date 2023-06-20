using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using FractionDefinition;
using PropertiesCounter;

using UnityEditor;

public class EllipsoidsSpawning : MonoBehaviour
{

    #region VARIABLES
   

    [SerializeField] GameObject Ellipsoid;
    [SerializeField] GameObject EllipsoidParent;

    List<Fraction> Fractions;
    float[] FractionRatios;
    int ActiveFractionIndex;

    /*
    [SerializeField] float DensityOfStoneMaterial = 2600;
    */


    Vector3 SpawnPoint;
    [SerializeField] float SpawnRelativeYOffset;
    [SerializeField] float SpawnRelativeXZOffset;
    float SpawnOffset;

    [SerializeField] float TimePause = 1f;
    float TimeLastSpawn = 0f;

    [HideInInspector] public float BoxVolume;

    public int MaxNumberOfAttemptsToFillTopOfTheBox = 3;
    [SerializeField] int MaxNumerOfDestroyedStones = 50;
    public int NoDestroyedStones = 0;

    [ReadOnly] public bool ProcessPaused;
    [HideInInspector] public float ProcessPausedTime;
    [ReadOnly] public bool ProcessEnded;
    bool PropertiesCalculated = false;

    int iterStonesNames = 0;
    [SerializeField] int folderIterStarter = 0;
    [SerializeField] bool SaveModel = false;

    [ReadOnly] public float EllipsoidsActualVolume = 0f;

    //kg/m^3
    [SerializeField] float DensityOfStoneMaterial;

    int noOfGeneratedStones = 0;
    int errorFractionStones = 0; //ak sa inak zaradi do frakcie 4/8,8/16,16/22
    int errorGrFractionStones = 0; //ak sa inak zaradi do grading v danej frakcii
    int errorShFractionStones = 0; // ak sa inak zaradi do shape v danej frakcii
    int errorFlFractionStones = 0;// ak sa ink zaradi do flat v danej frakcii

    #endregion




    // Start is called before the first frame update
    void Start()
    {
        #region INITIALIZE VARIABLES

        //zmena gravitacneho zrychlenia kvoli jednotkam, z metrov ideme a cm
        //Physics.gravity = new Vector3(0f, -980f, 0f);

        ProcessPaused = false;

        //BoxVolume sa urci pomocou urcenia objemu telesa, ktore sa nezobrazuje ale vyplna objem
        Transform boxVolumeObject = transform.Find("BoxVolume");
        BoxVolume = Prop.VolumeOfMesh(boxVolumeObject.GetComponent<MeshFilter>());
        Debug.Log("ObjemNadoby: " + BoxVolume);



        SpawnPoint = transform.position;
        SpawnPoint.y += 2f * transform.localScale.y + 2f * transform.localScale.y * SpawnRelativeYOffset;
        SpawnOffset = transform.localScale.x * SpawnRelativeXZOffset;

        //pomery v akych miesame
        FractionRatios = new float[2] { 0.3f, 0.7f };

        float sumOfRatioList = 0f;
        foreach (float f in FractionRatios)
        {
            sumOfRatioList += f;
        }
        if (sumOfRatioList != 1f)
            Debug.LogError("suma pomerov, ako chceme miesat frakcie nie je jedna");

        Fractions = new List<Fraction>();
        //uvadzane v cm //?
        Fractions.Add(new Fraction(( 0.4f, 0.8f ), //d/D
                                   FractionRatios[0],           //chceme 30% tejto frakcie miesat

                                   new float[3] { 0.4f, 0.6f, 0.8f },   //sitovy rozbor - hranice
                                   new float[2] { 0.8f, 0.2f },            //sitovy rozbor - zastupenie - nechce to byt nascitane do jednotky, je to rozklad 1

                                   new float[3] { 0.4f, 0.5f, 0.8f },  //tvarovy index - hranice
                                   new float[2] { 0.25f, 0.1f },          //tvarovy index - hodnoty

                                   new float[4] { 0.4f, 0.5f, 0.6f, 0.8f },  //index plochosti - hranice
                                   new float[3] { 0.25f, 0.3f, 0.4f },      //index plochosti - harfove sita medzery
                                   new float[3] { 0.2f, 0.13f, 0.1f }      //index plochosti - hodnoty
                                   ));

        Fractions.Add(new Fraction((0.8f, 1.6f), //d/D
                                   FractionRatios[1],         //chceme 70% tejto frakcie miesat

                                   new float[3] { 0.8f, 1.12f, 1.6f },   //sitovy rozbor - hranice
                                   new float[2] { 0.4f, 0.6f },            //sitovy rozbor - zastupenie  - nechce to byt nascitane do jednotky, je to rozklad 1

                                   new float[3] { 0.8f, 1.12f, 1.6f },  //tvarovy index - hranice
                                   new float[2] { 0.3f, 0.2f },          //tvarovy index - hodnoty

                                   new float[4] { 0.8f, 1f, 1.25f, 1.6f },  //index plochosti - hranice
                                   new float[3] { 0.5f, 0.63f, 0.8f },      //index plochosti - harfove sita medzery
                                   new float[3] { 0.3f, 0.13f, 0.1f }      //index plochosti - hodnoty
                                   ));
        
        


        if (FractionRatios.Length != Fractions.Count)
            Debug.LogError("ina dlzka vektorov pre frakcie a vektora pomerov pre miesanie frakcii");

        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        if (!ProcessEnded)
        {
            if (!ProcessPaused)
            {
                if (NoDestroyedStones < MaxNumerOfDestroyedStones)
                {
                    if (Time.time > TimeLastSpawn + TimePause)
                    {
                        //Randomness of spawning point and mesh prototype
                        float x = Random.Range(-1f, 1f) * SpawnOffset;
                        float z = Random.Range(-1f, 1f) * SpawnOffset;


                        //Creating a new stone object with random position above the box
                        GameObject stone = Instantiate(Ellipsoid, SpawnPoint + new Vector3(x, 0, z), Quaternion.identity, EllipsoidParent.transform);
                        stone.name = "Ellipsoid" + iterStonesNames;
                        iterStonesNames++;
                        noOfGeneratedStones++;

                        //urcenie aku frakciu ideme primiesat podla zelaneho pomeru
                        ActiveFractionIndex = FractionChoice();

                       
                        stone.GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(Fractions[ActiveFractionIndex]);



                        #region TESTY ZARADENIA DO FRAKCIE
                        /////////////////////////////////////////////////////////////////////////////////////////
                        //testovanie a porovnanie zaradenia do frakcie podla frNum z elipsoidu a podla priemetu
                        //mozno este dat to inde a porovnavat aj ci sme dobre vramci frakcie a potom az podfrakcie sledovat
                        int noRotations = 250;
                        StoneMeshProperties s = stone.GetComponent<StoneMeshProperties>();
                        float frNumEllipsoid = s.GetFractionNumber();
                        float frNumRotation = Prop.FrNumber(stone, noRotations);
                        //ratanie chyby - rozdiel dvoch frNum
                        //float error = frNumEllipsoid - frNumRotation;

                        //zaradenie do frakcie
                        int fractionRotation = IndexFromFractionVector(frNumRotation, Fractions.ToArray());
                        int fractionEllipsoid = ActiveFractionIndex;
                        //ratanie chyby - rozdiel frakcii
                        int fractionError = Mathf.Abs(fractionEllipsoid - fractionRotation);

                        if (fractionError == 0)
                        {
                            //ratanie chyby - rozdiel dvoch dvoch indexov kam zaraduje frNum kamen v danej frakcii pre grading, shape a flat
                            int frGrIndexEllipsoid = IndexFromFractionVector(frNumEllipsoid, Fractions[ActiveFractionIndex].GradingSubfractions);
                            int frGrIndexRotation = IndexFromFractionVector(frNumRotation, Fractions[ActiveFractionIndex].GradingSubfractions);
                            int frShIndexEllipsoid = IndexFromFractionVector(frNumEllipsoid, Fractions[ActiveFractionIndex].ShapeSubfractions);
                            int frShIndexRotation = IndexFromFractionVector(frNumRotation, Fractions[ActiveFractionIndex].ShapeSubfractions);
                            int frFlIndexEllipsoid = IndexFromFractionVector(frNumEllipsoid, Fractions[ActiveFractionIndex].FlatSubfractions);
                            int frFlIndexRotation = IndexFromFractionVector(frNumRotation, Fractions[ActiveFractionIndex].FlatSubfractions);

                            int errorGrIndex = Mathf.Abs(frGrIndexEllipsoid - frGrIndexRotation);
                            if (errorGrIndex != 0) errorGrFractionStones++;

                            int errorShIndex = Mathf.Abs(frShIndexEllipsoid - frShIndexRotation);
                            if (errorShIndex != 0) errorShFractionStones++;

                            int errorFlIndex = Mathf.Abs(frFlIndexEllipsoid - frFlIndexRotation);
                            if (errorFlIndex != 0) errorFlFractionStones++;

                            /*
                            Debug.Log("rozdiel frakcii (4/8,8/16,16/22): " + fractionError);
                            Debug.Log("(pre active fraction a jej grading) rozdiel indexov: " + errorGrIndex + ";   frGrIndexEllipsoid: " + frGrIndexEllipsoid + " frGrIndex z meshu: " + frGrIndexRotation);
                            Debug.Log("(pre active fraction a jej shapeInd) rozdiel indexov: " + errorShIndex + ";   frShIndexEllipsoid: " + frShIndexEllipsoid + " frShIndex z meshu: " + frShIndexRotation);
                            Debug.Log("(pre active fraction a jej FlatInd) rozdiel indexov: " + errorFlIndex + ";   frFlIndexEllipsoid: " + frFlIndexEllipsoid + " frFlIndex z meshu: " + frFlIndexRotation);
                            Debug.Log("rozdiel frNum: " + error + "frNumEllipsoid: " + frNumEllipsoid + " frNum z meshu: " + frNumRotation);
                            */
                        }
                        else
                        {
                            errorFractionStones++;
                        }

                        Debug.Log("aktualna chyba zaradovania do frakcie:");
                        Debug.Log("zla frakcia %: " + errorFractionStones / (float)noOfGeneratedStones * 100 + " zle: " + errorFractionStones + " vsetky: " + noOfGeneratedStones);
                        Debug.Log("zle grading %: " + errorGrFractionStones / ((float)noOfGeneratedStones - errorFractionStones) * 100 + " zle: " + errorGrFractionStones + " vsetky: " + (noOfGeneratedStones - errorFractionStones));
                        Debug.Log("zle shape %: " + errorShFractionStones / ((float)noOfGeneratedStones - errorFractionStones) * 100 + " zle: " + errorShFractionStones + " vsetky: " + (noOfGeneratedStones - errorFractionStones));
                        Debug.Log("zle flat %: " + errorFlFractionStones / ((float)noOfGeneratedStones - errorFractionStones) * 100 + " zle: " + errorFlFractionStones + " vsetky: " + (noOfGeneratedStones - errorFractionStones));

                        #endregion


                        stone.transform.rotation = Random.rotation;
                        stone.GetComponent<Rigidbody>().useGravity = true;


                        //hlupo mera hmotnost
                        stone.GetComponent<Rigidbody>().mass = DensityOfStoneMaterial * stone.GetComponent<StoneMeshProperties>().GetVolume();

                        EllipsoidsActualVolume += stone.GetComponent<StoneMeshProperties>().GetVolume();

                        TimeLastSpawn = Time.time;




                    }
                }
                else
                {
                    ProcessPaused = true;
                    ProcessPausedTime = Time.time;
                }
            }
        }
        //Calculation of propertiesof completed model
        else if (!PropertiesCalculated)
        {
            Debug.Log("zaciatok ratania properties");
            PropertiesCalculated = true;

            Prop.CountPropertiesOfModel(EllipsoidParent, BoxVolume);

            if(SaveModel)
                ModelSavingSystem.SaveModel(EllipsoidParent.transform, folderIterStarter, "Assets/SavedModels/EllipsoidModels", true);

        }

    }



    //vráti index frakcie podla pomeru v ktorom miesame, ktorej chceme vygenerovat kamen
    //vybera sa ta, ktorej zastupenie celkoveho objemu sa najviac vzdaluje od pozadovaneho
    public int FractionChoice()
    {
        int index = -1;
        
        if (Fractions != null && Fractions.Count > 0)
        {
            if (EllipsoidsActualVolume == 0f)
            {
                return 0;
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

    //urcenie indexu zo zoznamu frakcii podla zaradenia cez frNum
    int IndexFromFractionVector(float num, Fraction[] frac)
    {
        int i = 0;
        if (frac[frac.Length - 1].FractionBoundaries.Item2 < num)
            //Debug.Log("prekrocenie hranic pri zaradovani do frakcie");
            return 1000;

        while (frac[i].FractionBoundaries.Item2 < num)
        {
            i++;
        }

        return i;
    }

}
