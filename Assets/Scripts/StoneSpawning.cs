using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertiesCounter;

public class StoneSpawning : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] List<GameObject> Prefabs;
    [SerializeField] GameObject StoneParent;

    [SerializeField] Vector3 SpawnPoint;
    [SerializeField] float SpawnOffset;

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
        ProcessPaused = false;

        BoxVolume = 1f;
        BoxEmptyVolume = BoxVolume;
        StonesVolume = 0;

        #endregion

        #region INITIALIZE STONE`S PROPERTIES
        //Counting of essential mesh properties (length, width, volume) for each mesh prototype
        for (int i = 0; i < noPrefabs; i++)
        {
            Prefabs[i].SetActive(true);

            Vector2 lengthWidth = f.GetLengthAndWidthOfFragment(Prefabs[i]);
            float volume = f.VolumeOfMesh(Prefabs[i].transform.GetComponentInChildren<MeshFilter>().sharedMesh);

            StoneMeshProperties s = Prefabs[i].GetComponent<StoneMeshProperties>();
            s.SetVolume(volume);
            s.SetLength(lengthWidth.x);
            s.SetWidth(lengthWidth.y);
            Prefabs[i].SetActive(false);

            //Debug.Log(i + ": length:" + s.GetLength() + " width: " + s.GetWidth() + " volume: " + s.GetVolume());
        }
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
            PropertiesCalculated = true;

            StoneMeshProperties[] AllStonesProperties = StoneParent.GetComponentsInChildren<StoneMeshProperties>();
            for (int i = 0; i < AllStonesProperties.Length; i++)
            {
                float volume = AllStonesProperties[i].GetVolume();
                StonesVolume += volume;
                BoxEmptyVolume -= volume;
                //Debug.Log(i + " volume: " + volume);
            }

            Voids = BoxEmptyVolume / BoxVolume;
            Debug.Log("StonesVolume: " + StonesVolume);
            Debug.Log("EmptyVolume: " + BoxEmptyVolume);
            Debug.Log("Voids: " + Voids);
            
        }
      
    }
}
