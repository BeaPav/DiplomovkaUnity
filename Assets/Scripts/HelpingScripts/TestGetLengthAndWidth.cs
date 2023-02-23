using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertiesCounter;

public class TestGetLengthAndWidth : MonoBehaviour
{
    bool b = true;
    int iter = 0;
    Vector3 step;
    // Start is called before the first frame update
    void Start()
    {
        step = new Vector3(0f, 10f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(b)
        {
            b = !b;
            Vector2 v = f.GetLengthAndWidthOfStone(transform.gameObject);
            Debug.Log("Length of cube: " + v.x);
            Debug.Log("Width of cube: " + v.y);
        }

        if (iter % 20 == 0)
        {
            //transform.Rotate(new Vector3(0, 10f, 0));
            transform.eulerAngles = transform.rotation.eulerAngles + step;
            //Physics.SyncTransforms();

        }
        iter++;
    }

    
}
