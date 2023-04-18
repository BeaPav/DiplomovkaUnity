using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelProperties : MonoBehaviour
{
    [ReadOnly] public float BoxVolume;
    [ReadOnly] public int NumberOfStones;
    [ReadOnly] public float StonesVolume;
    [ReadOnly] public float EmptyVolume;
    [ReadOnly] public float Voids;
}
