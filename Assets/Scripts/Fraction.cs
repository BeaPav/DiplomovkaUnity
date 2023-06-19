using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FractionDefinition
{
    public class Fraction
    {
        public (float, float) FractionBoundaries;
        public float ActualFractionVolume = 0f;
        public float RequiredVolumePart;
        public float LongVolume = 0f;
        public float FlatVolume = 0f;
        public float FlatSieveSize;
        public Fraction[] GradingSubfractions;
        public Fraction[] ShapeSubfractions;
        public Fraction[] FlatSubfractions;


        #region CONSTRUCTOR
        public Fraction((float,float) frBoundaries, float requiredVolumePart, float[] grFrBoundaries, float[] grFrPercentage,
                        float[] shFrBoundaries, float[] shFrPercentage,
                        float[] flFrBoundaries, float[] flSieveSizes, float[] flFrPercentage)
        {
            FractionBoundaries = frBoundaries;
            RequiredVolumePart = requiredVolumePart;

            if (grFrBoundaries.Length - grFrPercentage.Length != 1)
                Debug.LogError("zla dlzka vektora pri gradingBoundaries a gradingPercentage");

            GradingSubfractions = new Fraction[grFrPercentage.Length];
            for(int i = 0; i < grFrPercentage.Length; i++)
            {
                float d = grFrBoundaries[i];
                float D = grFrBoundaries[i + 1];
                GradingSubfractions[i] = new Fraction((d, D), grFrPercentage[i]);
            }

            if(shFrBoundaries.Length - shFrPercentage.Length != 1)
                Debug.LogError("zla dlzka vektora pri shapeBoundaries a shapePercentage");

            ShapeSubfractions = new Fraction[shFrPercentage.Length];
            for (int i = 0; i < shFrPercentage.Length; i++)
            {
                float d = shFrBoundaries[i];
                float D = shFrBoundaries[i + 1];
                ShapeSubfractions[i] = new Fraction((d, D), shFrPercentage[i]);
            }


            if(flFrBoundaries.Length - flFrPercentage.Length != 1)
                Debug.LogError("zla dlzka vektora pri flatBoundaries a flatPercentage");
            if(flFrBoundaries.Length - flSieveSizes.Length != 1)
                Debug.LogError("zla dlzka vektora pri flatBoundaries a flatSieveSizes");

            FlatSubfractions = new Fraction[flFrPercentage.Length];
            for (int i = 0; i < flFrPercentage.Length; i++)
            {
                float d = flFrBoundaries[i];
                float D = flFrBoundaries[i + 1];
                FlatSubfractions[i] = new Fraction((d, D), flFrPercentage[i], flSieveSizes[i]);
            }

        }
        public Fraction((float, float) frBoundaries)
        {
            FractionBoundaries = frBoundaries;
        }
        
        public Fraction((float, float) frBoundaries, float reqVolume, float flSieveSize, Fraction[] grSubfr, Fraction[] shSubfr, Fraction[] flatSubfr)
        {
            FractionBoundaries = frBoundaries;
            RequiredVolumePart = reqVolume;
            FlatSieveSize = flSieveSize;
            GradingSubfractions = grSubfr;
            ShapeSubfractions = shSubfr;
            FlatSubfractions = flatSubfr;
        }
        public Fraction((float, float) frBoundaries, float reqVolume, float flSieveSize = 0f)
        {
            FractionBoundaries = frBoundaries;
            RequiredVolumePart = reqVolume;
            FlatSieveSize = flSieveSize;
        }
#endregion



        //vr�ti index  cistej frakcie podla grading curve, ktorej chceme vygenerovat kamen
        //vybera sa ta, ktorej zastupenie celkoveho objemu sa najviac vzdaluje od pozadovaneho
        public int GradingChoice()
        {
            int index = -1;
            if (GradingSubfractions != null && GradingSubfractions.Length > 0)
            {
                if (ActualFractionVolume == 0f)
                    return 0;

                float min = float.MaxValue;
                
                for (int i = 0; i < GradingSubfractions.Length; i++)
                {
                    float dif = GradingSubfractions[i].ActualFractionVolume / ActualFractionVolume - GradingSubfractions[i].RequiredVolumePart;
                    if (dif < min)
                    {
                        min = dif;
                        index = i;
                    }
                }
            }
            return index;

        }
        
        // vrati index zodpovedajucej frakcie (pre zoznamy plochych podfrakcii), ci je plochy a zodpovedajucu medzeru harfoveho sita
        // ak je aktualny index plochosti frakcie (podiel objemu plochych a vsetkych) menej ako pozadovany, tak generujeme plochy kamen
        public (int, bool, float) IsFlat(float frNum)
        {
            int flIndex = IndexFromFractionVector(frNum, FlatSubfractions);
            if (FlatSubfractions[flIndex].ActualFractionVolume == 0)
            {
                if (FlatSubfractions[flIndex].RequiredVolumePart != 0)
                    return (flIndex, true, FlatSubfractions[flIndex].FlatSieveSize);
                return (flIndex, false, FlatSubfractions[flIndex].FlatSieveSize);
            }

            float dif = FlatSubfractions[flIndex].FlatVolume / FlatSubfractions[flIndex].ActualFractionVolume - FlatSubfractions[flIndex].RequiredVolumePart;

            if (dif < 0)
                return (flIndex, true, FlatSubfractions[flIndex].FlatSieveSize);

            return (flIndex, false, FlatSubfractions[flIndex].FlatSieveSize);
            
        }

        //vrati index zodpovedajucej frakcie (pre zoznamy dlhych podfrakcii), ci je dlhy
        // ak je aktualny tvarovy index frakcie (podiel objemu dlhych a vsetkych) menej ako pozadovany, tak generujeme dlhy kamen
        public (int, bool) IsLong(float frNum)
        {
            int shIndex = IndexFromFractionVector(frNum, ShapeSubfractions);
            if(ShapeSubfractions[shIndex].ActualFractionVolume == 0)
            {
                if (ShapeSubfractions[shIndex].RequiredVolumePart != 0)
                    return (shIndex, true);
                return (shIndex, false);
            }

            float dif = ShapeSubfractions[shIndex].LongVolume / ShapeSubfractions[shIndex].ActualFractionVolume - ShapeSubfractions[shIndex].RequiredVolumePart;

            if (dif < 0)
                return (shIndex, true);

            return (shIndex, false);

        }


        public void ActualizeVolume(float volume, (int, int, int) indGrFlSh, (bool, bool) shapeFlatLong)
        {
            //pridanie elipsoidu do aktualnej frakcie (4/8, 8/16, 16/22)
            ActualFractionVolume += volume;

            //zaratanie objemu do konkretnej ciary zrnitosti v danej frakcii
            if (GradingSubfractions == null)
                Debug.LogError("chceme pridat volume do neexistujuceho grading subfraction");
            else
            {
                GradingSubfractions[indGrFlSh.Item1].ActualFractionVolume += volume;
            }

            //pridanie objemu elipsoida do konkretnej podfrakcie pre celkovu hmotnost v zoznamoch plochych a tiez priratanie plocheho ak je to treba
            if (FlatSubfractions == null)
                Debug.LogError("chceme pridat volume do neexistujuceho flat subfraction");
            else
            {
                FlatSubfractions[indGrFlSh.Item2].ActualFractionVolume += volume;
                if (shapeFlatLong.Item1)
                {
                    FlatSubfractions[indGrFlSh.Item2].FlatVolume += volume;
                    FlatVolume += volume;
                }
            }

            //zaratanie objemu do celkovej hmotnosti podfrakcie v zoznamoch pre dlhe a priratanie objemu k dlhym ak je to treba
            if (ShapeSubfractions == null)
                Debug.LogError("chceme pridat volume do neexistujuceho shape subfraction");
            else
            {
                ShapeSubfractions[indGrFlSh.Item3].ActualFractionVolume += volume;
                if (shapeFlatLong.Item2)
                {
                    ShapeSubfractions[indGrFlSh.Item3].LongVolume += volume;
                    LongVolume += volume;
                }
            }

        }

        int IndexFromFractionVector(float num, Fraction[] frac)
        {
            int i = 0;
            if (frac[frac.Length - 1].FractionBoundaries.Item2 < num)
                //Debug.Log("prekrocenie hranic pri zaradovani do frakcie");
                return 1000;

            while (frac[i].FractionBoundaries.Item2 < num)
            {
                i++;
            }

            return i;
        }
    }
}