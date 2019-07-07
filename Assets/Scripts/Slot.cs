using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot: MonoBehaviour
{
    // the index of the plant occupying the slot
    public int plantIndex = -1;

    // default settings for slots
    public Atmosphere atmosphere = Atmosphere.oxygen;
    public Temperature temperature = Temperature.moderate;
    public LightLevel lightLevel = LightLevel.normal;

    // == COMPUTED PROPERTIES ==

    public bool hasPlant
    {
        get { return (plantIndex != -1); }
    }

    public void PlacePlant(Plant plant)
    {
        plant.PutDown(this);
        plantIndex = plant.index;
        Debug.Log($"Plant {plant.index} placed in slot {name}");
    }

    public void RemovePlant(Plant plant)
    {
        plant.PickUp();
        plantIndex = -1;
        Debug.Log($"Plant {plant.index} removed from slot {name}");
    }
}
