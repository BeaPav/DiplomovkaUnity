using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FractionDefinition
{
    public class Fraction
    {
        public float[] FractionBoundaries;

        public float[] GradingFractionBoundaries;
        public float[] GradingFractionPercentage;

        public float[] ShapeFractionBoundaries;
        public float[] ShapeFractionPercentage;

        public float[] FlatFractionBoundaries;
        public float[] FlatSieveSizes;
        public float[] FlatFractionPercentage;

        //constructor
        public Fraction(float[] fractionBoundaries, float[] gradingFractionBoundaries, float[] gradingFractionPercentage,
                        float[] shapeFractionBoundaries, float[] shapeFractionPercentage,
                        float[] flatFractionBoundaries, float[] flatSieveSizes, float[] flatFractionPercentage)
        {
            if (fractionBoundaries.Length != 2)
                Debug.LogError("zla dlzka vektora hranic, oznacedie d/D");

            FractionBoundaries = fractionBoundaries;

            if (gradingFractionBoundaries.Length - gradingFractionPercentage.Length != 1)
                Debug.LogError("zla dlzka vektora pri gradingBoundaries a gradingPercentage");

            GradingFractionBoundaries = gradingFractionBoundaries;
            GradingFractionPercentage = gradingFractionPercentage;

            if(shapeFractionBoundaries.Length - shapeFractionPercentage.Length != 1)
                Debug.LogError("zla dlzka vektora pri shapeBoundaries a shapePercentage");

            ShapeFractionBoundaries = shapeFractionBoundaries;
            ShapeFractionPercentage = shapeFractionPercentage;

            if(flatFractionBoundaries.Length - flatFractionPercentage.Length != 1)
                Debug.LogError("zla dlzka vektora pri flatBoundaries a flatPercentage");
            if(flatFractionBoundaries.Length - flatSieveSizes.Length != 1)
                Debug.LogError("zla dlzka vektora pri flatBoundaries a flatSieveSizes");

            FlatFractionBoundaries = flatFractionBoundaries;
            FlatSieveSizes = flatSieveSizes;
            FlatFractionPercentage = flatFractionPercentage;
        }
        public Fraction(float[] fractionBoundaries)
        {
            if (fractionBoundaries.Length != 2)
                Debug.LogError("zla dlzka vektora hranic, oznacedie d/D");

            FractionBoundaries = fractionBoundaries;
        }




        //vráti index (dolna hranica na oznaceni) náhodnej cistej frakcie podla grading curve percentage
        public int GradingChoice()
        {
            float tmp = Random.Range(0f, 1f);
            int indexOfFraction = IndexOfLowerBundary(tmp, GradingFractionPercentage);
            if (indexOfFraction == 1000)
                Debug.LogError("nahodne cislo [0,1] je vacsie ako hranice GradingFractionPercentage");
            return indexOfFraction;
        }
        
        // vrati ci je plochy a zodpovedajucu medzeru harfoveho sita
        public (bool, float) IsFlat(float frNum)
        {
            int indexOfFlatFraction = IndexOfLowerBundary(frNum, FlatFractionBoundaries);
            float flatIndex = FlatFractionPercentage[indexOfFlatFraction];
            float tmp = Random.Range(0f, 1f);
            if (tmp <= flatIndex) return (true, FlatSieveSizes[indexOfFlatFraction]) ;
            return (false, FlatSieveSizes[indexOfFlatFraction]);
        }
        
        //vrati ci je dlhy
        public bool IsLong(float frNum)
        {
            int indexOfShapeFraction = IndexOfLowerBundary(frNum, ShapeFractionBoundaries);
            float shapeIndex = ShapeFractionPercentage[indexOfShapeFraction];
            float tmp = Random.Range(0f, 1f);
            if (tmp <= shapeIndex) return true;
            return false;
        }



        int IndexOfLowerBundary(float num, float[] intervalBoundaries)
        {
            int i = 0;
            if (num >= intervalBoundaries[intervalBoundaries.Length - 1])
                //Debug.Log("prekrocenie hranic pri zaradovani do frakcie");
                return 1000;

            while (intervalBoundaries[i + 1] < num)
            {
                i++;
            }

            return i;
        }
    }
}