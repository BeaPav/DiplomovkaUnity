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

    #endregion




    // Start is called before the first frame update
    void Start()
    {
        #region INITIALIZE VARIABLES

        ProcessPaused = false;

        //BoxVolume sa urci pomocou urcenia objemu telesa, ktore sa nezobrazuje ale vyplna objem
        Transform boxVolumeObject = transform.Find("BoxVolume");
        BoxVolume = Prop.VolumeOfMesh(boxVolumeObject.GetComponent<MeshFilter>());
        Debug.Log("ObjemNadoby: " + BoxVolume);



        SpawnPoint = transform.position;
        SpawnPoint.y = 2f * transform.localScale.y + 2f * transform.localScale.y * SpawnRelativeYOffset;

        //aj toto nerobit relativny ofset len ku kocke ale k nadobe akehokolvek tvaru alebo dajaky radius tam dat v percentach rozmeru nadoby
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
        //uvadzane v cm
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


                        //urcenie aku frakciu ideme primiesat podla zelaneho pomeru
                        ActiveFractionIndex = FractionChoice();

                       
                        stone.GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(Fractions[ActiveFractionIndex]);

                        stone.transform.rotation = Random.rotation;
                        stone.GetComponent<Rigidbody>().useGravity = true;

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
}
