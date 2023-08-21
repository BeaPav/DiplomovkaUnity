using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertiesCounter;
using System.IO;
using UnityEditor;



/* For convex aproximate decomposition for colliders of stones
 * this package is needed https://github.com/Unity-Technologies/VHACD/tree/mesh-generator
*/

public class StoneSpawning : MonoBehaviour
{


    #region VARIABLES
    [SerializeField] List<GameObject> Prefabs;
    int noPrefabs;
    [SerializeField] GameObject StoneParent;

    //kg/cm^3
    [SerializeField] float DensityOfStoneMaterial = 0.00263f;
    //float[] MassOfStones;
    /*
    float[] GradingCurveIndexes;
    float[] GradingCurveVolumes;
    */

Vector3 SpawnPoint;
    [SerializeField] float SpawnRelativeYOffset;
    [SerializeField] float SpawnRelativeXZOffset;
    float SpawnOffset;

    
    [SerializeField] float FractionMin;
    [SerializeField] float FractionMax;
    
    

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

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region INITIALIZE VARIABLES

        noPrefabs = Prefabs.Count;
        //MassOfStones = new float[noPrefabs];

        ProcessPaused = false;

        //BoxVolume sa urci pomocou urcenia objemu telesa, ktore sa nezobrazuje ale vyplna objem
        Transform boxVolumeObject = transform.Find("BoxVolume");
        BoxVolume = Prop.VolumeOfEllipsoidMesh(boxVolumeObject.GetComponent<MeshFilter>());
        Debug.Log("ObjemNadoby: " + BoxVolume);

        SpawnPoint = transform.position;
        SpawnPoint.y += 2f * transform.localScale.y + 2f * transform.localScale.y * SpawnRelativeYOffset;
        SpawnOffset = Mathf.Min(transform.localScale.x, transform.localScale.z) * SpawnRelativeXZOffset;
        

        /*
        GradingCurveIndexes = new float[] { 0.2f, 0.4f, 0.56f, 0.8f, 1.12f, 1.6f, 2.24f, 3.15f };
        GradingCurveVolumes = new float[GradingCurveIndexes.Length + 1];
        */

        #endregion

        #region INITIALIZE STONE`S PROPERTIES - !!!!!!!!!!!!treba vymazat
        
        //Counting of essential mesh properties (length, width, volume) for each mesh prototype
        for (int i = 0; i < noPrefabs; i++)
        {
            Prefabs[i].SetActive(true);

            Vector2 lengthWidth = Prop.LengthAndWidthOfStone(Prefabs[i]);
            float volume = Prop.VolumeOfEllipsoidMesh(Prefabs[i].transform.GetComponentInChildren<MeshFilter>());
            float frNum = Prop.FrNumber(Prefabs[i], 20);

            //MassOfStones[i] = volume * DensityOfStoneMaterial;

            StoneMeshProperties s = Prefabs[i].GetComponent<StoneMeshProperties>();
            s.SetVolume(volume);
            s.SetLength(lengthWidth.x);
            s.SetWidth(lengthWidth.y);
            s.SetFractionNumber(frNum);
            Prefabs[i].SetActive(false);

            //Debug.Log(i + ": length:" + s.GetLength() + " width: " + s.GetWidth() + " frNum: " 
            //            + s.GetFractionNumber() +  " volume: " + s.GetVolume() + " mass: " + MassOfStones[i]);
        }
        
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
                        //toto  ma to zmysel pri kamenoch z databazy
                        int prefabIndex = Random.Range(0, noPrefabs - 1);

                        //Creating a new stone with random rotation, scale and position above the box
                        GameObject stone = Instantiate(Prefabs[prefabIndex], SpawnPoint + new Vector3(x, 0, z), Quaternion.identity, StoneParent.transform);
                        
                        stone.name = "Stone" + iterStonesNames;
                        iterStonesNames++;

                        stone.SetActive(true);

                        stone.transform.rotation = Random.rotation;
                        StoneMeshProperties s = stone.GetComponent<StoneMeshProperties>();
                        s.ScaleStone(FractionMin, FractionMax);

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
        else if(!PropertiesCalculated)
        {
            Debug.Log("zaciatok ratania properties");
            PropertiesCalculated = true;
            
            //Prop.CountPropertiesOfModel(StoneParent, BoxVolume, out string textModelResults);
            //Prop.CountPropertiesOfModelFractions(StoneParent, Fractions, out string textFractionsResults);
            //tu ani nemame fractions

            #region COSI S GRADING, MOZNO TO NETREBA -- ZAKOMENTOVANE
            //tu neviem co sa deje
            //nejak ratanie grading curve alebo cosi toho stylu
            /*
            Random.InitState(319);
            StoneMeshProperties[] AllStonesProperties = StoneParent.GetComponentsInChildren<StoneMeshProperties>();
            for (int i = 0; i < AllStonesProperties.Length; i++)
            {
                
                //znovu sa vygeneruje nahodne fraction number pre ziskanie grading curve, povodne sa neprepisuje
                //float frNum = f.FrNumber(AllStonesProperties[i].transform.gameObject, 10);
                
                //tu neviem ci som neriesila cosi v ratani frNum a ineho ze na bounds mam vlastne ratanie a nepouziva sa nic z Unity
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
            
            Debug.Log("GradingCurve: ");
            Debug.Log("[  - " + GradingCurveIndexes[0] + "] : " + GradingCurveVolumes[0] / StonesVolume);
            for (int k = 1; k<GradingCurveVolumes.Length-1;k++)
            {
                Debug.Log("[" + GradingCurveIndexes[k-1] + " - " + GradingCurveIndexes[k] + "]: "  + GradingCurveVolumes[k] / StonesVolume);
            }
            Debug.Log("[" + GradingCurveIndexes[GradingCurveIndexes.Length-1] + " - ]: " + GradingCurveVolumes[0] / StonesVolume);
            */
            #endregion

            if (SaveModel)
            {
                ModelSavingSystem.SaveModel(StoneParent.transform, folderIterStarter, "Assets/SavedModels/StoneModels",(1f,1f,1f),
                            "", false, false);
            }
        }

    }


}
