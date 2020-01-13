using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChangerScript : MonoBehaviour
{

    public Material material;
    public Material water;

    public void SetUpper1(float f)
    {
        material.SetFloat("_Upper1", f);
    }

    public void SetUpper2(float f)
    {
        material.SetFloat("_Upper2", f);
    }

    public void SetUpper3(float f)
    {
        material.SetFloat("_Upper3", f);
    }

    public void SetUpper4(float f)
    {
        material.SetFloat("_Upper4", f);
    }

    public void SetWaterDepth(float f)
    {
        water.SetFloat("_WaterDepth", f);
    }

    public void SetWaterWave(float f)
    {
        water.SetFloat("_WaveStrength", f);
    }

}
