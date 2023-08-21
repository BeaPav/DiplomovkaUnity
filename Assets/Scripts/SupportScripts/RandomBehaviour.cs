using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBehaviour : MonoBehaviour
{
    public int noIterationsRandom = 50;
    int[] subintervals;

    // Start is called before the first frame update
    void Start()
    {
        subintervals = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for(int i = 0; i < noIterationsRandom; i++)
        {
            float r = Random.Range(0f, 1f);

            if (r < 0.1f)
                subintervals[0]++;
            else if (r < 0.2f)
                subintervals[1]++;
            else if (r < 0.3f)
                subintervals[2]++;
            else if (r < 0.4f)
                subintervals[3]++;
            else if (r < 0.5f)
                subintervals[4]++;
            else if (r < 0.6f)
                subintervals[5]++;
            else if (r < 0.7f)
                subintervals[6]++;
            else if (r < 0.8f)
                subintervals[7]++;
            else if (r < 0.9f)
                subintervals[8]++;
            else
                subintervals[9]++;
        }
        
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("menej ako 0." + (i + 1) + ": " + subintervals[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
