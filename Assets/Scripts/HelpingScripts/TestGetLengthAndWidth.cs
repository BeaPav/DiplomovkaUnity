using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertiesCounter;

public class TestGetLengthAndWidth : MonoBehaviour
{
    bool b = true;

    // Start is called before the first frame update
    void Start()
    {
        
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

        transform.Rotate(new Vector3(0, 2, 0));
    }
}
