using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenerateEllipsoidsNamespace;

public class GenerateEllipsoid : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Awake()
    {
        genE.GenerateEllipsoid(this.gameObject, 8, 6);
        //!!!!!!!!! treba zratat vsetky vlastnosti objektu typu objemy a podobne
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
