using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FractionDefinition;
using PropertiesCounter;

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
    [SerializeField] float SpawnPointYOffset;
    [SerializeField] float SpawnRelativeOffset;
    float SpawnOffset;

    [SerializeField] float TimePause = 1f;
    float TimeLastSpawn = 0f;

    [HideInInspector] public float BoxVolume;
    [HideInInspector] public float BoxEmptyVolume;
    [HideInInspector] public float StonesVolume;
    [HideInInspector] public float Voids;

    public int MaxNumberOfAttemptsToFillTopOfTheBox = 3;
    [SerializeField] int MaxNumerOfDestroyedStones = 50;
    public int NoDestroyedStones = 0;

    public bool ProcessPaused;
    [HideInInspector] public float ProcessPausedTime;
    public bool ProcessEnded;
    bool PropertiesCalculated = false;
    
    #endregion




    // Start is called before the first frame update
    void Start()
    {
        #region INITIALIZE VARIABLES

        ProcessPaused = false;

        //BoxVolume = 1f * transform.localScale.x * transform.localScale.y * transform.localScale.z;
        //tu by sa skor hodilo dat naozaj volume of box ratanim aby sa to dalo lahko zamenit
        GameObject boxVolumeObject = GameObject.Find("ObjemNadoby");
        MeshFilter mf = boxVolumeObject.GetComponentInChildren<MeshFilter>();
        BoxVolume = Prop.VolumeOfMesh(mf) * boxVolumeObject.transform.localScale.x * boxVolumeObject.transform.localScale.y * boxVolumeObject.transform.localScale.z;
        Debug.Log("ObjemNadoby: " + BoxVolume);



        BoxEmptyVolume = BoxVolume;
        StonesVolume = 0;

        SpawnPoint = Vector3.zero;
        SpawnPoint.y += transform.localScale.y + SpawnPointYOffset;

        //aj toto nerobit relativny ofset len ku kocke ale k nadobe akehokolvek tvaru alebo dajaky radius tam dat v percentach rozmeru nadoby
        SpawnOffset = transform.localScale.x / 2f * SpawnRelativeOffset;

        Fractions = new List<Fraction>();
        Fractions.Add(new Fraction(new float[2] { 0.8f, 1.6f }, //d/D

                                   new float[3] { 0.8f, 1.12f, 1.6f },   //sitovy rozbor - hranice
                                   new float[2] { 0.4f, 1f },            //sitovy rozbor - zastupenie

                                   new float[3] { 0.8f, 1.12f, 1.6f },  //tvarovy index - hranice
                                   new float[2] { 0f, 0.5f },          //tvarovy index - hodnoty

                                   new float[4] { 0.8f, 1f, 1.25f, 1.6f },  //index plochosti - hranice
                                   new float[3] { 0.5f, 0.63f, 0.8f },      //index plochosti - harfove sita medzery
                                   new float[3] { 0.3f, 0.21f, 0.9f }      //index plochosti - hodnoty
                                   ));
        
        
        FractionRatios = new float[1] { 1f};
        //este by bolo fajne dat vektor, kde by sa postupne prvky FractionRatios scitavali, a na konci je jednotka a z toho by sa potom urcovala frakcia podla pravdepodobnmosti
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        float sumOfRatioList = 0f;
        foreach (float f in FractionRatios)
        {
            sumOfRatioList += f;
        }
        if (sumOfRatioList != 1f)
            Debug.LogError("suma pomerov, ako chceme miesat frakcie nie je jedna");
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
                        //momentalne je prefabom len elipsoid takze toto nie je nahodne, ma to zmysel pri kamenoch z databazy


                        //Creating a new stone object with random position above the box
                        GameObject stone = Instantiate(Ellipsoid, SpawnPoint + new Vector3(x, 0, z), Quaternion.identity, EllipsoidParent.transform);

                        //urcenie aku frakciu ideme primiesat
                        //!!!!!!!!!!!!!!!!!!!!!
                        ActiveFractionIndex = 0;
                        //Fraction activeFraction = Fractions[0];

                        //GenerateEllipsoidObject e = stone.GetComponent<GenerateEllipsoidObject>();
                        //e.GenerateEllipsoid(activeFraction);
                        stone.GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(Fractions[0]);

                        //StoneMeshProperties s = stone.GetComponent<StoneMeshProperties>();
                        //s.ScaleStone(FractionMin, FractionMax);

                        stone.transform.rotation = Random.rotation;
                        stone.GetComponent<Rigidbody>().useGravity = true;

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
            Random.InitState(319);

            StoneMeshProperties[] AllStonesProperties = EllipsoidParent.GetComponentsInChildren<StoneMeshProperties>();
            int noOfEllipsoids = 0;
            for (int i = 0; i < AllStonesProperties.Length; i++)
            {
                noOfEllipsoids++;
                float volume = AllStonesProperties[i].GetVolume();
                StonesVolume += volume;
                BoxEmptyVolume -= volume;
                //Debug.Log(i + " volume: " + volume);

            }

            Voids = BoxEmptyVolume / BoxVolume * 100f;
            Debug.Log("No of ellipsoids: " + noOfEllipsoids);
            Debug.Log("Objem (pocet * objem gule): " + 3.801935f * noOfEllipsoids);
            Debug.Log("Box volume: " + BoxVolume);
            Debug.Log("StonesVolume: " + StonesVolume);
            Debug.Log("EmptyVolume: " + BoxEmptyVolume);
            Debug.Log("Voids: " + Voids + "%");


        }

    }
}
