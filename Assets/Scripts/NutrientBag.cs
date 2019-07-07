using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientBag : MonoBehaviour
{
    public Nutrient nutrient = Nutrient.water;

    private Nutrient[] nutrients = (Nutrient[])System.Enum.GetValues(typeof(Nutrient));
    public int contents
    {
        get
        {
            for (int i = 0; i < nutrients.Length; i++)
            {
                if (nutrients[i] == nutrient)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
