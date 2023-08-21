using UnityEngine;

public class ModelProperties : MonoBehaviour
{

    [ReadOnly] public float Voids;

    [ReadOnly] public int NumberOfStones;
    [ReadOnly] public float BoxVolume;
    [ReadOnly] public float StonesVolume;
    [ReadOnly] public float EmptyVolume;

    [SerializeField] public FractionProperties[] FrProperties;

    public string ResultsToString()
    {
        string textResults = "";
        textResults += ModelPropertiesToString();
        
        
        if (FrProperties == null)
            return textResults;

        textResults += FractionPropertiesToString();

        return textResults;
    }

    public string ModelPropertiesToString()
    {
        string textResults = "MODEL PROPERTIES" + "\n\n";
        textResults += "Box volume:     " + BoxVolume.ToString() + "\n";
        textResults += "Number of ellipsoids:   " + NumberOfStones.ToString() + "\n";
        textResults += "EllipsoidsVolume:   " + StonesVolume.ToString() + "\n";
        textResults += "EmptyVolume:    " + (EmptyVolume).ToString() + "\n\n";
        textResults += "VOIDS:  " + Voids.ToString() + "%" + "\n\n";

        return textResults;
    }

    public string FractionPropertiesToString()
    {
        string textResults = "\n" + "FRACTIONS PROPERTIES" + "\n";
        for (int i = 0; i<FrProperties.Length; i++)
        {
            textResults += FrProperties[i].ResultsToString();
        }

        return textResults;
    }
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

    public string ResultsToString()
    {
        string s = "\n\n" + "PROPERTIES OF FRACTION " + FractionName + "\n\n";

        s += "VolumePercentageInModel" + "\t" + VolumePercentageInModel.ToString() + "\n";
        s += "VolumeInModel" + "\t" + VolumeInModel.ToString() + "\n";

        s += GradingCurve.ResultsToString();
        s += ShapeIndex.ResultsToString();
        s += FlatIndex.ResultsToString();

        return s;

    }

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

    public string ResultsToString()
    {
        string s = "\n" + "GRADING CURVE \n\n";

        s += "Fractions" + "\t";
        for(int i = 0; i < FrNames.Length; i++)
        {
            s += FrNames[i] + "\t";
        }
        s += "\n";

        s += "Percentage" + "\t";
        for (int i = 0; i < Percentage.Length; i++)
        {
            s += Percentage[i] + "\t";
        }
        s += "\n";

        s += "FrVolumes" + "\t";
        for (int i = 0; i < FrVolumes.Length; i++)
        {
            s += FrVolumes[i] + "\t";
        }
        s += "\n";

        return s;
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

    public string ResultsToString()
    {
        string s = "\n" + "SHAPE INDEX \n\n";

        s += "Fractions" + "\t";
        for (int i = 0; i < FrNames.Length; i++)
        {
            s += FrNames[i] + "\t";
        }
        s += "\n";

        s += "LongPercentage" + "\t";
        for (int i = 0; i < LongPercentage.Length; i++)
        {
            s += LongPercentage[i] + "\t";
        }
        s += "\n";

        s += "LongVolumes" + "\t";
        for (int i = 0; i < LongVolumes.Length; i++)
        {
            s += LongVolumes[i] + "\t";
        }
        s += "\n";

        s += "FrVolumes" + "\t";
        for (int i = 0; i < FrVolumes.Length; i++)
        {
            s += FrVolumes[i] + "\t";
        }
        s += "\n";

        s += "\n" + "FractionShapeIndex" + "\t" + FractionShapeIndex.ToString();
        s += "\n\n";

        return s;
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

    public string ResultsToString()
    {
        string s = "\n" + "FLAT INDEX \n\n";


        s += "Fractions" + "\t";
        for (int i = 0; i < FrNames.Length; i++)
        {
            s += FrNames[i] + "\t";
        }
        s += "\n";

        s += "FlatPercentage" + "\t";
        for (int i = 0; i < FlatPercentage.Length; i++)
        {
            s += FlatPercentage[i] + "\t";
        }
        s += "\n";

        s += "FlatVolumes" + "\t";
        for (int i = 0; i < FlatVolumes.Length; i++)
        {
            s += FlatVolumes[i] + "\t";
        }
        s += "\n";

        s += "FrVolumes" + "\t";
        for (int i = 0; i < FrVolumes.Length; i++)
        {
            s += FrVolumes[i] + "\t";
        }
        s += "\n";


        s += "\n"+ "FractionFlatIndex" + "\t" + FractionFlatIndex.ToString();
        s += "\n\n";

        return s;
    }
}
