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


        //vráti index (dolna hranica na oznaceni) náhodnej cistej frakcie podla grading curve percentage
        public int GradingChoice()
        {
            float tmp = Random.Range(0f, 1f);
            int indexOfFraction = IndexOfLowerBundary(tmp, GradingFractionPercentage);
            if (indexOfFraction == 1000)
                Debug.Log("nahodne cislo [0,1] je vacsie ako hranice GradingFractionPercentage");
            return indexOfFraction;
        }
        public bool IsFlat(float frNum)
        {
            int indexOfFlatFraction = IndexOfLowerBundary(frNum, FlatFractionBoundaries);
            float flatIndex = FlatFractionPercentage[indexOfFlatFraction];
            float tmp = Random.Range(0f, 1f);
            if (tmp <= flatIndex) return true;
            return false;
            //tu by asi bolo treba este vratit ze aka je medzera sita
        }
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