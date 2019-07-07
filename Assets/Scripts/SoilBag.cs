using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilBag : MonoBehaviour
{
    public Soil soil = Soil.dirt;

    private Soil[] soils = (Soil[])System.Enum.GetValues(typeof(Soil));
    public int contents
    {
        get
        {
            for (int i = 0; i < soils.Length; i++)
            {
                if (soils[i] == soil)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
