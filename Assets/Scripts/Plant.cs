using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Atmosphere { oxygen, other }
public enum Temperature { moderate, cold, hot }
public enum Soil { dirt, ash, sand, mud, bonechar }
public enum LightLevel { normal, dark }
public enum Nutrient { water, blood, acid }

public class Plant: MonoBehaviour
{
    // == CONSTANT PROPERTIES ==

    const int HAPPINESS_THRESHOLD = 200; // how long a plant can be HAPPY without its needs met
    const int LIVINGNESS_THRESHOLD = 300; // how long a plant can be ALIVE without its needs met
    const Slot DEFAULT_ENVIRONMENT = new Slot(); // a limbo slot with default conditions for when being carried around
    // TODO: construct properly ^^

    // default settings for plants
    public Atmosphere atmosphere = Atmosphere.oxygen;
    public Temperature temperature = Temperature.moderate;
    public Soil soil = Soil.dirt;
    public LightLevel lightLevel = LightLevel.normal;
    public Nutrient nutrient = Nutrient.water;

    public int index = -1;

    // == CHANGEABLE PROPERTIES ==

    // how many properties of the plant have been discovered
    public int discoveredProperties = 0;

    // what soil is in its tray
    public Soil traySoil = Soil.dirt;

    // should tick up over time
    public int lastFed = 0;
    public bool lastInCorrectEnvironment = 0;

    // should update on putDown()
    public bool correctEnvironment = false;

    // == COMPUTED PROPERTIES ==

    public bool isHappy
    {
        get { return (lastFed > HAPPINESS_THRESHOLD) &&
                (lastInCorrectEnvironment > HAPPINESS_THRESHOLD); }
    }

    public bool isAlive
    {
        get { return (lastFed > LIVINGNESS_THRESHOLD) &&
                (lastInCorrectEnvironment > LIVINGNESS_THRESHOLD); }
    }

    public bool requirementDiscovered
    {
        get { return discoveredProperties > 0; }
    }

    public bool benefitDiscovered
    {
        get { return discoveredProperties > 1; }
    }

    public bool disbenefitDiscovered
    {
        get { return discoveredProperties > 2; }
    }

    // == FUNCTIONS ==

    public bool PassTime()
    {
        bool plantIsHappy = isHappy;
        bool plantIsAlive = isAlive;

            if (!plant.correctEnvironment)
            {
                lastInCorrectEnvironment += 1;
            }

            lastFed += 1;

            return (plantIsHappy == this.isHappy && plantIsAlive == this.isAlive);
    }

    // plant was putDown() in new slot
    public bool PutDown(Slot slot)
    {
        // check if new conditions match requirements
        correctEnvironment = CheckEnvironment(slot);
        return correctEnvironment;
    }

    public bool ChangeSoil(Soil soil, Slot slot)
    {
        traySoil = soil;

        // check if new conditions match requirements
        correctEnvironment = CheckEnvironment(slot);
        return correctEnvironment;
    }

    // plant was putDown() in new slot or slot changed soil
    public bool CheckEnvironment(Slot slot)
    {
        // check if new conditions match requirements
        correctEnvironment = (slot.atmosphere == this.atmosphere) &&
            (slot.temperature == this.temprature) &&
            (traySoil == this.soil) &&
            (slot.lightLevel == this.lightLevel);

        if (correctEnvironment)
        {
            lastInCorrectEnvironment = 0;
            Debug.Log($"Plant {index} liked the change.");
            return correctEnvironment;
        }
        
        Debug.Log($"Plant {index} did not like the change.");
        return correctEnvironment;
    }

    // discover new property of plant
    public void DiscoverProperty()
    {
        discoveredProperties += 1;
        Debug.Log($"Discovered property {discoveredProperties} of plant {index}.");
    }

    // plant was pickedUp() and is now in limbo
    public void PickUp()
    {
        CheckEnvironment(DEFAULT_ENVIRONMENT);
    }

    // plant was watered with something
    public bool FeedNutrient(Nutrient nutrient)
    {
        if (nutrient == this.nutrient)
        {
            Debug.Log($"Plant {index} liked the nutrient.");
            lastFed = 0;
            return true;
        }

        Debug.Log($"Plant {index} did not like the nutrient.");
        return false;
    }
}
