using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertiesCounter;

public class StoneMeshProperties : MonoBehaviour
{
    public float Volume;
    public float FractionNumber;
    public float Length;
    public float Width;
    public bool IsLong;
    public bool IsFlat;

    public bool EllGrFrIndexIsMoreRotGrFrIndex = false;
    public Vector3 axes;
    public (int, int, int) indGrFlSh;

    //Mesh StoneMesh;

    #region METHODS FOR GET & SET PROPERTIES 

    public void SetIsLong(bool b)
    {
        IsLong = b;
    }

    public bool GetIsLong()
    {
        return IsLong;
    }

    public void SetIsFlat(bool b)
    {
        IsFlat = b;
    }

    public bool GetIsFlat()
    {
        return IsFlat;
    }

    public void SetVolume(float v)
    {
        Volume = v;
    }
    
    public float GetVolume()
    {
        return Volume;
    }

    public void SetWidth(float w)
    {
        Width = w;
    }

    public float GetWidth()
    {
        return Width;
    }

    public void SetLength(float l)
    {
        Length = l;
    }

    public float GetLength()
    {
        return Length;
    }

    public void SetFractionNumber(float frNum)
    {
        FractionNumber = frNum;
    }

    public float GetFractionNumber()
    {
        return FractionNumber;
    }

    #endregion

    //METHOD TO SCALE STONE`S MESH AND ITS PROPERTIES
    public void ScaleStone(float ScaleMin, float ScaleMax)
    {
        if (ScaleMin > ScaleMax)
        {
            (ScaleMin, ScaleMax) = (ScaleMax, ScaleMin);
        }
        
        //Scaling in order FractionNumber belongs to interval [FractionMin,FractionMax]
        float scaleFactor = Random.Range(ScaleMin, ScaleMax)/FractionNumber;

        transform.localScale *= scaleFactor;

        Volume *= scaleFactor * scaleFactor * scaleFactor;
        FractionNumber *= scaleFactor;
        Length *= scaleFactor;
        Width *= scaleFactor;

        GetComponent<Rigidbody>().mass *= scaleFactor * scaleFactor * scaleFactor;

    }



}
