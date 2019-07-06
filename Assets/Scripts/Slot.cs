using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    // this is the index of the plant in the plants list on levelgod
    public int plant = -1;

    // the default slot type, designed to be overwritten
    public Atmosphere atmosphere = Atmosphere.air;
    public Soil soil = Soil.dirt;
    public Temperature temperature = Temperature.moderate;

    // fuck this is terrible...
    public Change[] changes
    {
        get
        {
            var atmosChange = new Change();
            atmosChange.type = ChangeType.atmosphere;
            atmosChange.atmosphere = atmosphere;
            atmosChange.index = plant;

            var soilChange = new Change();
            soilChange.type = ChangeType.soil;
            soilChange.soil = soil;
            soilChange.index = plant;

            var temperatureChange = new Change();
            temperatureChange.type = ChangeType.temperature;
            temperatureChange.temperature = temperature;
            temperatureChange.index = plant;

            Change[] changes = {atmosChange, soilChange, temperatureChange};
            return changes;
        }
    }
}
