using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using FractionDefinition;
using PropertiesCounter;

public class EllipsoidsSpawning : MonoBehaviour
{

    #region VARIABLES
   

    [SerializeField] GameObject Ellipsoid;
    [SerializeField] GameObject EllipsoidParent;

    List<Fraction> Fractions;
    [SerializeField] float[] FractionRatios;
    int ActiveFractionIndex;


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


    
    [SerializeField] float DensityOfStoneMaterial; //kg/m^3

    [SerializeField] bool SaveModel = false;
    [SerializeField] int iterStonesNames = 0;
    [SerializeField] int folderIterStarter = 0;
    
    string SavePath = "Assets/SavedModels/EllipsoidModels";

    [ReadOnly] public float EllipsoidsActualVolume = 0f;


    [SerializeField] public bool ProcessPaused;
    [HideInInspector] public float ProcessPausedTime;
    
    [SerializeField] public bool ProcessEnded;
    [SerializeField] bool PropertiesCalculated = false;



    [SerializeField] bool ErrorCounting = false;
    [SerializeField] public int noOfGeneratedStones = 0;

    #endregion




    // Start is called before the first frame update
    void Start()
    {
        #region INITIALIZE VARIABLES


        ProcessPaused = false;

        //BoxVolume -- according to box-volume object (not rendered)
        Transform boxVolumeObject = transform.Find("BoxVolume");
        BoxVolume = Prop.VolumeOfEllipsoidMesh(boxVolumeObject.GetComponent<MeshFilter>());
        //Debug.Log("ObjemNadoby: " + BoxVolume);


        SpawnPoint = transform.position;
        SpawnPoint.y += 2f * transform.localScale.y + 2f * transform.localScale.y * SpawnRelativeYOffset;
        SpawnOffset = Mathf.Min(transform.localScale.x, transform.localScale.z) * SpawnRelativeXZOffset;

        //control of defined ratios
        float sumOfRatioList = 0f;
        foreach (float f in FractionRatios)
        {
            sumOfRatioList += f;
        }
        if (sumOfRatioList != 1f)
            Debug.LogError("suma pomerov, ako chceme miesat frakcie nie je jedna");

        #endregion

        #region FRACTIONS INICIALIZATION

        Fractions = new List<Fraction>();
        //cm
        Fractions.Add(new Fraction(( 0.4f, 0.8f ), //d/D
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
                                   new float[4] { 0.0005f, 0.0148f,0.2388f, 0.7446f},               //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta

                                   new float[2] { 0.4f,  1.6f },                                    //OK        //tvarovy index - hranice
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
        #endregion

        

    }

    // Update is called once per frame
    void Update()
    {
        if (!ProcessEnded)
        {
            if (!ProcessPaused)
            {
                
                if (NoDestroyedStones < MaxNumerOfDestroyedStones)  //end criterion
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

                        //Selecting fraction according to fraction ratios
                        ActiveFractionIndex = FractionChoice();
                       
                        //Generating mesh of ellipsoid and defining its other properties
                        stone.GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(Fractions[ActiveFractionIndex]);
                        stone.GetComponent<StoneMeshProperties>().fractionIndex = ActiveFractionIndex;
                        stone.transform.rotation = Random.rotationUniform;
                        stone.GetComponent<Rigidbody>().useGravity = true;
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
            //Debug.Log("zaciatok ratania properties");
            PropertiesCalculated = true;

            Prop.CountPropertiesOfModel(EllipsoidParent, BoxVolume, out string textModelResults);
            Prop.CountPropertiesOfModelFractions(EllipsoidParent, Fractions, out string textFractionsResults);

            //saving
            if(SaveModel)
                ModelSavingSystem.SaveModel(EllipsoidParent.transform, folderIterStarter, SavePath, (FractionRatios[0], FractionRatios[1], FractionRatios[2]),
                                            textModelResults + textFractionsResults, false, false);

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
                int j = 0;
                while(j < Fractions.Count && Fractions[j].RequiredVolumePart == 0)
                {
                    j++;
                }

                if(Fractions.Count < j)
                {
                    Debug.LogError("V ellipsoidSpawning mame vsetky frakcie requredVolume 0");
                }

                return j;
            }

            float min = float.MaxValue;

            for (int i = 0; i < Fractions.Count; i++)
            {
                float dif = Fractions[i].ActualFractionVolume / EllipsoidsActualVolume - Fractions[i].RequiredVolumePart;
                if (dif < min && Fractions[i].RequiredVolumePart != 0f)
                {
                    min = dif;
                    index = i;
                }
            }
        }
        return index;
    }


}
