using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenerateEllipsoidsNamespace;
using PropertiesCounter;

public class FrNumDeterminationTests : MonoBehaviour
{
    [SerializeField] bool DoTest;
    [SerializeField] int NoSamples;
    bool done = false;
    
    float[] GradingCurveFractions;
    float[] FlatFractions;
    float[] FlatSievesSize;

    int noGradingFractions;

    // Start is called before the first frame update
    void Start()
    {
        GradingCurveFractions = new float[] { 0.4f, 0.56f, 0.8f, 1.12f, 1.6f, 2.24f, 3.15f };
        noGradingFractions = GradingCurveFractions.Length - 1; //-1 lebo intervalov = frakcii je o jedno menej ako je hranic

        FlatFractions = new float[] { 0.4f, 0.5f, 0.63f, 0.8f, 1f, 1.25f, 1.6f, 2f, 2.5f, 3.15f, 4f };
        FlatSievesSize = new float[] { 0.25f, 0.315f, 0.4f, 0.5f, 0.63f, 0.8f, 1f, 1.25f, 1.6f, 2f };

        
    }

    int IndexOfStartOfInterval(float num, float[] intervalBoundaries)
    {
        int i = 0;
        if (num >= intervalBoundaries[intervalBoundaries.Length - 1])
            return 1000;

        while (intervalBoundaries[i + 1] < num)
        {
            i++;
        }
        
        return i;
    }

    // Update is called once per frame
    void Update()
    {
        if(DoTest)
        {
            if (!done)
            {

                //tuto su porovnane cisla ako sa rata frNum cez priemety a frNum podla velkosti poloosi elipsoidu
                
                #region TEST 1
                /*
                for(int i = 0; i < NoSamples; i++)
                {
                    //nahodne polomery (pozor pri vypisovani idu von priemery, tj dvojnasobky)
                    float ellX = Random.Range(0.2f, 1.6f);
                    float ellZ = Random.Range(ellX, 3.15f);
                    float ellY = Random.Range(ellZ, 4.8f);

                    //spravi sa mesh a zrata frNum cez priemety
                    genE.GenerateEllipsoid(this.gameObject, 12, 6, ellX, ellY, ellZ);

                    StoneMeshProperties s = GetComponent<StoneMeshProperties>();
                    s.SetFractionNumber(f.FrNumber(this.gameObject, 20));

                    //frNum sa zrata z 2D dvoch mensich rozmerov - pouzivaju sa priemery (lebo chceme celu stenu ohr obalky nie len polovicu)
                    
                    //frNum zavisi od toho ci je dlhsi rozmer elipsoidu v uhlopriecke stvorcoveho otvoru alebo rovnobezny s hranou, 
                    //to da interval pre frNum -> <2*ellz/sqrt(2),  2*ellZ>
                    float frNum = Random.Range(Mathf.Max(2f * ellZ / Mathf.Sqrt(2), 2*ellX), 2f * ellZ);

                    //vypis
                    //Debug.Log("rozmery: " + 2*ellX + ",  " + 2*ellZ + ",  " + 2*ellY)
                    Debug.Log("rozdiel :" + Mathf.Abs(s.GetFractionNumber() - frNum) +";     frNum z priemetov: " + s.GetFractionNumber() + ",   frNum z 2D: " + frNum);

                    //vymazanie meshu
                    gameObject.GetComponentInChildren<MeshFilter>().mesh = null;
                }
                */
                #endregion
                

                //tu sa urci frakcia, kam chceme kamen zaradit, a v nej frNum nahodne.
                #region TEST 2

                //Potom sa opacnym procesom ako hore vypocita, ake bude  ellZ (stredny rozmer) - nahodnym procesom

                //frNum zavisi od toho ci je dlhsi rozmer elipsoidu v uhlopriecke stvorcoveho otvoru alebo rovnobezny s hranou, 
                //to da interval pre frNum a opacne vieme vytvorit interval aj z frNum pre 2*ellZ -> <frNum, sqrt(2)*frNum>
                for (int i = 0; i < NoSamples; i++)
                {
                    //frakcia (oznacujeme i, chapeme d = GradingCurveIndexes[i] , D = GradingCurveIndexes[i + 1] )
                    //frNum a ellZ
                    int fractionGradingIndex = Random.Range(0, noGradingFractions); 
                    float frNum = Random.Range(GradingCurveFractions[fractionGradingIndex], GradingCurveFractions[fractionGradingIndex + 1]);

                    float ellZ = Random.Range(frNum, Mathf.Sqrt(2) * frNum) / 2f;

                    //teraz sa urci ci bude plochy, resp dlhy alebo nie - 50% pravdepodobnost zatial
                    int plochy = Random.Range(0, 2);
                    int dlhy = Random.Range(0, 2);

                    //vygenerujeme ellX podla plochosti
                    //treba urcit sirku medzery harfoveho sita
                    int fractionFlatIndex = IndexOfStartOfInterval(frNum, FlatFractions);
                    float flatSieveSize = FlatSievesSize[fractionFlatIndex];

                    float ellX = 0f;
                    float ellY = 0f;

                    //vygenerovanie najmensieho rozmeru
                    if (plochy == 1)
                    {
                        //nahodna konstanta pre dolne ohranicenie intervalu - treba zmenit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                         ellX = Random.Range(flatSieveSize / 2f, flatSieveSize) / 2f;
                    }
                    else
                    {
                         ellX = Random.Range(flatSieveSize, frNum) / 2f;
                    }

                    //vygenerovanie najdlhsieho rozmeru
                    if(dlhy == 1)
                    {
                        //nahodny koeficient pre horne ohr intervalu - treba zmenit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                         ellY = Random.Range(3f * 2f * ellX, 5f * 2f * ellX);
                    }
                    else
                    {
                         ellY = Random.Range(2f * ellZ, 3f * 2f * ellX);
                    }




                    //vygenerovanie meshu
                    genE.GenerateEllipsoid(this.gameObject, 12, 6, ellX, ellY, ellZ);

                    //frNum cez priemety
                    StoneMeshProperties s = GetComponent<StoneMeshProperties>();
                    s.SetFractionNumber(Prop.FrNumber(this.gameObject, 30));

                    //ratanie chyby - rozdiel dvoch frNum, resp dvoch indexov kam zaraduje frNum kamen
                    float error = Mathf.Abs(s.GetFractionNumber() - frNum);
                    int fractionIndexFromMesh = IndexOfStartOfInterval(s.GetFractionNumber(), GradingCurveFractions);
                    int errorIndex = Mathf.Abs(fractionGradingIndex - fractionIndexFromMesh);

                    Debug.Log(" rozdiel indexov: " + errorIndex + ";   frIndex dane: " + fractionGradingIndex + " frIndex z meshu: " + fractionIndexFromMesh);
                    Debug.Log(" rozdiel cisel: " + error + "frNum dane: " + frNum + " frNum z meshu: " + s.GetFractionNumber());

                    //vymazanie meshu
                    gameObject.GetComponentInChildren<MeshFilter>().mesh = null;
                }
                #endregion

                done = true;
            }
        }
       
    }
}
