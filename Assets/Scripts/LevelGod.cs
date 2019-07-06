using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGod : MonoBehaviour
{
    const public float TICK_RATE = 2;
    const Array<Nutrient> NUTRIENTS = Enum.GetValues(typeof(Nutrient));
    const Array<Soil> SOILS = Enum.GetValues(typeof(Soil));

    public Slot[] slots;
    public List<Plant> plants = new List<Plant>();
    public List<int> missedPlantIDs = new List<int>(); // mitigate sparse array from removals

    // keeps track of time
    float elapsed = 0;

    // kepts tracking of player holding
    private int heldPlantIndex = -1;
    private int heldNutrientIndex = -1;
    private int heldSoilIndex = -1;

    // == GAME SETUP AND LOGISTICS ==

    // Start is called before the first frame update
    void Start()
    {
        var tags = GameObject.FindGameObjectsWithTag("Slot");
        slots = new Slot[tags.Length];

        for (int i = 0; i < tags.Length; i++)
        {
            var slot = tags[i].GetComponent<Slot>();
            slots[i] = slot;
        }

        CreatePlant();
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed > TICK_RATE)
        {
            elapsed = 0;
            Tick();
        }
    }

    private void Tick()
    {
        foreach (var plant in plants)
        {
            bool changedState = plant.PassTime();

            if (changedState)
            {
                Debug.Log($"Plant {plant.index} has worsened");
                // UpdateSprite(plant); // value asset brown/black as per state
            }
        }
    }

    

    //==================================================================================
    // CHANGES TO BE MADE
    public Plant plantPrefab;
    public void CreatePlant()
    {
        // basically make a new plant based on the template passed in (not done yet)
        // add it to the list of plants
        // plants will start just off by themselves I guess?
        // sure why not
        // later need to start them somewhere a bit more meaningful
        var plant = Instantiate(plantPrefab, transform.position, Quaternion.identity);

        if (missedPlantIDs.Count != 0)
        {
            plant.index = missedPlantIDs[0];
            missedPlantIDs.RemoveAt(0);
        } 
        else
        {
            plant.index = plants.Count;
        }
        
        plants.Add(plant);
    }

    //==================================================================================
    


    // == PLAYER ACTIONS ==

    // activated empty hand on non-empty slot
    public void PickupPlant(Slot slot)
    {
        Plant plant = plants[slot.plantIndex];
        heldPlantIndex = plant.index;
        slot.RemovePlant(plant);
    }
    
    // activated plant-holding hand on empty slot
    public void PlacePlant(Slot slot)
    {
        Plant plant = plants[heldPlantIndex];
        heldPlantIndex = -1;
        slot.PlacePlant(plant);
    }

    // activated food-holding hand on non-empty slot
    public void FeedPlant(Slot slot)
    {
        Plant plant = plants[slot.plantIndex];
        if (plant.isAlive)
        {
            Nutrient nutrient = NUTRIENTS[heldNutrientIndex];
            bool acceptedNutrient = plant.FeedNutrient(nutrient);

            if (acceptedNutrient)
            {
                Debug.Log($"Plant {plant.index} has been nourished.");
                // UpdateSprite(plant); // value asset un-brown if was brown
                // EmitHappyParticles(plant);
            }
        }

        heldNutrientIndex = -1;
        // disappear model to represent it being used up
    }

    // activated plant-holding hand on workstation
    public void ExaminePlantWithWorkstation()
    {
        Plant plant = plants[heldPlantIndex];
        plant.DiscoverProperty()
        // EmitHappyParticles(plant);
    }

    // activated plant-holding hand on incinerator
    public void DestroyPlant(Plant plant)
    {
        // to kill something we just remove it from the mappings and then remove the gameobject
        missedPlantIDs.Add(index);
        Destroy(plant);
    }

    // activated empty hand on pickup-able object
    public void PickupNutrient(Nutrient nutrient)
    {
        heldNutrientIndex = NUTRIENTS.IndexOf(nutrient);
    }

    // activated empty hand on pickup-able object
    public void PickupSoil(Soil soil)
    {
        heldSoilIndex = SOILS.IndexOf(soil);
    }

    // activate soil on slot with different soil
    public void ChangeSoil(Slot slot)
    {
        Plant plant = plants[slot.plantIndex];
        Soil soil = SOILS[heldSoilIndex];

        plant.ChangeSoil(soil, slot);

        heldSoilIndex = -1;
        // disappear model to represent it being used up
    }
}
