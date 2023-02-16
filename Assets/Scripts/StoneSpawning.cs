using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertiesCounter;

public class StoneSpawning : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] List<GameObject> Prefabs;
    [SerializeField] GameObject StoneParent;

    [SerializeField] float DensityOfStoneMaterial = 2600;
    float[] MassOfStones;
    float[] GradingCurveIndexes;
    float[] GradingCurveVolumes;

    [SerializeField] float SpawnPointYOffset;
    Vector3 SpawnPoint;

    [SerializeField] float SpawnRelativeOffset;
    float SpawnOffset;

    [SerializeField] float ScaleMin;
    [SerializeField] float ScaleMax;


    [SerializeField] float TimePause = 1f;
    float TimeLastSpawn = 0f;

    public float BoxVolume;
    public float BoxEmptyVolume;
    public float StonesVolume;
    public float Voids;

    public int MaxNumberOfAttemptsToFillTopOfTheBox = 3;
    [SerializeField] int MaxNumerOfDestroyedStones = 50;
    public int NoDestroyedStones = 0;

    public bool ProcessPaused;
    public bool ProcessEnded;
    bool PropertiesCalculated = false;
    int noPrefabs;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region INITIALIZE VARIABLES

        noPrefabs = Prefabs.Count;
        MassOfStones = new float[noPrefabs];

        ProcessPaused = false;

        BoxVolume = 1f * transform.localScale.x * transform.localScale.y * transform.localScale.z;
        BoxEmptyVolume = BoxVolume;
        StonesVolume = 0;

        SpawnPoint = Vector3.zero;
        SpawnPoint.y += transform.localScale.y + SpawnPointYOffset;

        SpawnOffset = transform.localScale.x / 2f * SpawnRelativeOffset;

        GradingCurveIndexes = new float[] { 0.2f, 0.4f, 0.56f, 0.8f, 1.12f, 1.6f, 2.24f, 3.15f };
        GradingCurveVolumes = new float[GradingCurveIndexes.Length + 1];

        #endregion

        #region INITIALIZE STONE`S PROPERTIES
        /*
        //Counting of essential mesh properties (length, width, volume) for each mesh prototype
        for (int i = 0; i < noPrefabs; i++)
        {
            Prefabs[i].SetActive(true);

            Vector2 lengthWidth = f.GetLengthAndWidthOfStone(Prefabs[i]);
            float volume = f.VolumeOfMesh(Prefabs[i].transform.GetComponentInChildren<MeshFilter>().sharedMesh);
            float frNum = f.FrNumber(Prefabs[i], 20);

            MassOfStones[i] = volume * DensityOfStoneMaterial;

            StoneMeshProperties s = Prefabs[i].GetComponent<StoneMeshProperties>();
            s.SetVolume(volume);
            s.SetLength(lengthWidth.x);
            s.SetWidth(lengthWidth.y);
            s.SetFractionNumber(frNum);
            Prefabs[i].SetActive(false);

            //Debug.Log(i + ": length:" + s.GetLength() + " width: " + s.GetWidth() + " frNum: " 
            //            + s.GetFractionNumber() +  " volume: " + s.GetVolume() + " mass: " + MassOfStones[i]);
        }
        */
        #endregion



    }


    // Update is called once per frame
    void Update()
    {
        if (NoDestroyedStones < MaxNumerOfDestroyedStones)
        {
            if (Time.time > TimeLastSpawn + TimePause)
            {
                //Randomness of spawning point and mesh prototype
                float x = Random.Range(-1f, 1f) * SpawnOffset;
                float z = Random.Range(-1f, 1f) * SpawnOffset;
                int prefabIndex = Random.Range(0, noPrefabs - 1);

                //Creating a new stone with random rotation, scale and position above the box
                GameObject stone = Instantiate(Prefabs[prefabIndex], SpawnPoint + new Vector3(x, 0, z), Random.rotation, StoneParent.transform);
                stone.SetActive(true);
                //stone.GetComponent<Rigidbody>().mass = MassOfStones[prefabIndex];
                StoneMeshProperties s = stone.GetComponent<StoneMeshProperties>();
                s.ScaleStone(ScaleMin, ScaleMax);

                TimeLastSpawn = Time.time;
            }
        }
        else
        {
            ProcessPaused = true;
        }

        //Calculation of propertiesof completed model
        if(ProcessEnded & !PropertiesCalculated)
        {
            Debug.Log("zaciatok ratania properties");
            PropertiesCalculated = true;
            Random.InitState(319);

            StoneMeshProperties[] AllStonesProperties = StoneParent.GetComponentsInChildren<StoneMeshProperties>();
            for (int i = 0; i < AllStonesProperties.Length; i++)
            {
                float volume = AllStonesProperties[i].GetVolume();
                StonesVolume += volume;
                BoxEmptyVolume -= volume;
                //Debug.Log(i + " volume: " + volume);

                //znovu sa vygeneruje nahodne fraction number pre ziskanie grading curve, povodne sa neprepisuje
                //float frNum = f.FrNumber(AllStonesProperties[i].transform.gameObject, 10);

                Renderer meshRenderer = AllStonesProperties[i].gameObject.GetComponentInChildren<Renderer>();
                float frNum = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.z);

                if (frNum < GradingCurveIndexes[0])
                {
                    GradingCurveVolumes[0] += volume;
                }
                else if(GradingCurveIndexes[GradingCurveIndexes.Length - 1] < frNum)
                {
                    GradingCurveVolumes[GradingCurveIndexes.Length] += volume;
                }
                else
                {
                    for(int j = 1; j < GradingCurveIndexes.Length; j++)
                    {
                        if(frNum < GradingCurveIndexes[j])
                        {
                            GradingCurveVolumes[j] += volume;
                            break;
                        }
                    }
                }

                
            }

            Voids = BoxEmptyVolume / BoxVolume;
            Debug.Log("StonesVolume: " + StonesVolume);
            Debug.Log("EmptyVolume: " + BoxEmptyVolume);
            Debug.Log("Voids: " + Voids);



            Debug.Log("GradingCurve: ");
            Debug.Log("[  - " + GradingCurveIndexes[0] + "] : " + GradingCurveVolumes[0] / StonesVolume);
            for (int k = 1; k<GradingCurveVolumes.Length-1;k++)
            {
                Debug.Log("[" + GradingCurveIndexes[k-1] + " - " + GradingCurveIndexes[k] + "]: "  + GradingCurveVolumes[k] / StonesVolume);
            }
            Debug.Log("[" + GradingCurveIndexes[GradingCurveIndexes.Length-1] + " - ]: " + GradingCurveVolumes[0] / StonesVolume);
        }
      
    }
}
