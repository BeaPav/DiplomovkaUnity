using UnityEngine;

public class ModelProperties : MonoBehaviour
{

    [ReadOnly] public float Voids;

    [ReadOnly] public int NumberOfStones;
    [ReadOnly] public float BoxVolume;
    [ReadOnly] public float StonesVolume;
    [ReadOnly] public float EmptyVolume;

    /// pozoooooooor tu toto chce byt pre kazdu frakciu zvlast!!!!!!!!!!!!!!!!!


    [SerializeField] public FractionProperties[] FrProperties;

}

[System.Serializable]
public class FractionProperties
{

    [ReadOnly] public string FractionName;
    [ReadOnly] public float VolumePercentageInModel;
    [ReadOnly] public float VolumeInModel;

    [SerializeField] public GradingCurve GradingCurve;
    [SerializeField] public ShapeIndex ShapeIndex;
    [SerializeField] public FlatInex FlatIndex;

}

[System.Serializable]
public class GradingCurve
{
    [ReadOnly] public string[] FrNames;
    [ReadOnly] public float[] Percentage;
    [ReadOnly] public float[] FrVolumes;

    public GradingCurve(string[] gradingCurveFrNames, float[] gradingCurvePercentage, float[] gradingCurveVolumes)
    {
        FrNames = gradingCurveFrNames;
        Percentage = gradingCurvePercentage;
        FrVolumes = gradingCurveVolumes;
    }

    public GradingCurve(int noOfGradingSubFractions)
    {
        FrNames = new string[noOfGradingSubFractions];
        Percentage = new float[noOfGradingSubFractions];
        FrVolumes = new float[noOfGradingSubFractions];
    }
}

[System.Serializable]
public class ShapeIndex
{
    [ReadOnly] public float FractionShapeIndex;
    [ReadOnly] public string[] FrNames;
    [ReadOnly] public float[] LongPercentage;
    [ReadOnly] public float[] LongVolumes;
    [ReadOnly] public float[] FrVolumes;

    public ShapeIndex (int noOfShapeSubFractions)
    {
        FrNames = new string[noOfShapeSubFractions];
        LongPercentage = new float[noOfShapeSubFractions];
        LongVolumes = new float[noOfShapeSubFractions];
        FrVolumes = new float[noOfShapeSubFractions];
    }
}


[System.Serializable]
public class FlatInex
{
    [ReadOnly] public float FractionFlatIndex;
    [ReadOnly] public string[] FrNames;
    [ReadOnly] public float[] FlatPercentage;
    [ReadOnly] public float[] FlatVolumes;
    [ReadOnly] public float[] FrVolumes;

    public FlatInex(int noOfFlatSubFractions)
    {
        FrNames = new string[noOfFlatSubFractions];
        FlatPercentage = new float[noOfFlatSubFractions];
        FlatVolumes = new float[noOfFlatSubFractions];
        FrVolumes = new float[noOfFlatSubFractions];
    }
}
