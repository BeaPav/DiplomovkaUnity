using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FractionDefinition;

public class AddOneEllipsoid : MonoBehaviour
{
    Fraction f;
    bool b = true;
    // Start is called before the first frame update
    void Start()
    {
        f = new Fraction((0.8f, 1.6f), //d/D
                                   1f,         //kolko % tejto frakcie miesat

                                   new float[5] { 0.4f, 0.56f, 0.8f, 1.12f, 1.6f },                  //OK        //sitovy rozbor - hranice
                                   new float[4] { 0.0005f, 0.0148f, 0.2388f, 0.7446f },               //OK        //sitovy rozbor - zostatok na site - je to "rozklad" 1 - vyjadruje percenta

                                   new float[2] { 0.4f, 1.6f },                                    //OK        //tvarovy index - hranice
                                   new float[1] { 0.1198f },                                        //OK        //tvarovy index - hodnoty

                                   new float[7] { 0.4f, 0.5f, 0.63f, 0.8f, 1.0f, 1.25f, 1.6f },     //OK        //index plochosti - hranice
                                   new float[6] { 0.25f, 0.315f, 0.4f, 0.5f, 0.63f, 0.8f },         //OK        //index plochosti - harfove sita medzery
                                   new float[6] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f }              //ODHADNUTE //index plochosti - hodnoty
                                   );
    }

    // Update is called once per frame
    void Update()
    {
        if (b)
        {
            GetComponent<GenerateEllipsoidObject>().GenerateEllipsoid(f);
            b = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
