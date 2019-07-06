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

    public Change[] ProcessChange(Change change)
    {
        List<Change> changes = new List<Change>();
        switch (change.type)
        {
            case ChangeType.atmosphere:
                var atmosChange = Change.AtmosphereChange(plant, change.atmosphere);
                atmosChange.fromIndex = change.toIndex;
                changes.Add(atmosChange);
                Debug.Log("atmos change on slot");

                atmosphere = change.atmosphere;
                break;
            
            case ChangeType.soil:
                var soilChange = Change.SoilChange(plant, change.soil);
                soilChange.fromIndex = change.toIndex;
                changes.Add(soilChange);
                Debug.Log("soil change on slot");

                soil = change.soil;
                break;
            
            case ChangeType.temperature:
                var temperatureChange = Change.TemperatureChange(plant, change.temperature);
                temperatureChange.fromIndex = change.toIndex;
                changes.Add(temperatureChange);
                Debug.Log("temp change on slot");

                temperature = change.temperature;
                break;
        }

        return changes.ToArray();
    }

    // fuck this is terrible...
    public Change[] changes
    {
        get
        {
            var atmosChange = new Change();
            atmosChange.type = ChangeType.atmosphere;
            atmosChange.atmosphere = atmosphere;
            atmosChange.toIndex = plant;

            var soilChange = new Change();
            soilChange.type = ChangeType.soil;
            soilChange.soil = soil;
            soilChange.toIndex = plant;

            var temperatureChange = new Change();
            temperatureChange.type = ChangeType.temperature;
            temperatureChange.temperature = temperature;
            temperatureChange.toIndex = plant;

            Change[] changes = {atmosChange, soilChange, temperatureChange};
            return changes;
        }
    }
}
