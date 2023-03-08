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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(DoTest)
        {
            if (!done)
            { 
                for(int i = 0; i < NoSamples; i++)
                {
                    //nahodne polomery (pozor pri vypisovani idu von priemery, tj dvojnasobky)
                    float ellX = Random.Range(1f, 4f);
                    float ellZ = Random.Range(ellX, 5f);
                    float ellY = Random.Range(ellZ, 6f);

                    //spravi sa mesh a zrata frNum cez priemety
                    genE.GenerateEllipsoid(this.gameObject, 12, 6, ellX, ellY, ellZ);

                    StoneMeshProperties s = GetComponent<StoneMeshProperties>();
                    s.SetFractionNumber(f.FrNumber(this.gameObject, 20));

                    //frNum sa zrata z 2D dvoch mensich rozmerov - pouzivaju sa priemery (lebo chceme celu stenu ohr obalky nie len polovicu)
                    float frNum = Random.Range(Mathf.Max(2f * ellZ / Mathf.Sqrt(2), 2*ellX), 2f * ellZ);

                    //vypis
                    Debug.Log("rozmery: " + 2*ellX + ",  " + 2*ellZ + ",  " + 2*ellY + ";     frNum z priemetov: " + s.GetFractionNumber() + ",   frNum z 2D: " + frNum);

                    //vymazanie meshu
                    gameObject.GetComponentInChildren<MeshFilter>().mesh = null;
                }
                
                done = true;
            }
        }
       
    }
}
