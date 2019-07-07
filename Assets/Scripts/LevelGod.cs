using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGod: MonoBehaviour
{
    public float TICK_RATE = 2;
    Nutrient[] NUTRIENTS = (Nutrient[])System.Enum.GetValues(typeof(Nutrient));
    Soil[] SOILS = (Soil[])System.Enum.GetValues(typeof(Soil));

    public Slot[] slots;
    public List<Plant> plants = new List<Plant>();

    public Transform soilPrefab;
    public Transform nutrientPrefab;

    public Slot spawnSlot;

    // keeps track of time
    float elapsed = 0;

    // kepts tracking of player holding
    private int heldPlantIndex = -1;
    private int heldNutrientIndex = -1;
    private int heldSoilIndex = -1;

    public Color sick = Color.red;
    public Color dead = Color.black;

    // == GAME SETUP AND LOGISTICS ==

    public GameObject UI;

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

        UI.SetActive(false);
    }

    public void ShowUI(string message)
    {
        var text = UI.GetComponentInChildren<UnityEngine.UI.Text>();
        text.text = message;
        UI.SetActive(true);
    }
    public void HideUI()
    {
        UI.SetActive(false);
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
            bool plantIsWell = plant.PassTime();

            if (!plantIsWell)
            {
                Debug.Log($"Plant {plant.index} has worsened");
                var renderer = plant.GetComponentInChildren<MeshRenderer>();
                renderer.material.color = sick;

                if (!plant.isAlive)
                {
                    renderer.material.color = dead;
                }
                else
                {
                    renderer.material.color = sick;
                }
            }
        }
    }

    public Slot SlotWithPlant(int plantIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].plantIndex == plantIndex)
            {
                return slots[i];
            }
        }
        return null;
    }

    // CHANGES TO BE MADE
    public Plant plantPrefab;
    public void CreatePlant()
    {
        // basically make a new plant based on the template passed in (not done yet)
        // add it to the list of plants
        // plants will start just off by themselves I guess?
        // sure why not
        // later need to start them somewhere a bit more meaningful
        var position = spawnSlot.transform.position;
        position.y += 1;
        var plant = Instantiate(plantPrefab, position, Quaternion.identity);

        plant.index = plants.Count;
        spawnSlot.plantIndex = plant.index;
        
        plants.Add(plant);
    }

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

            Debug.Log(acceptedNutrient);

            if (acceptedNutrient)
            {
                Debug.Log($"Plant {plant.index} has been nourished.");
                // TODO: UpdateSprite(plant); // value asset un-brown if was brown
                plant.GetComponent<MeshRenderer>().material.color = plant.healthyColor;
            }
        }

        heldNutrientIndex = -1;
        // TODO: disappear model to represent it being used up
    }

    // activated plant-holding hand on workstation
    public void ExaminePlantWithWorkstation()
    {
        Plant plant = plants[heldPlantIndex];
        plant.DiscoverProperty();
        // TODO: Unhide a post-it asset;
    }

    // activated plant-holding hand on incinerator
    public void DestroyPlant(Transform trans)
    {
        // to kill something we just remove it from the mappings and then remove the gameobject
        Plant plant = trans.GetComponent<Plant>();
        plants.RemoveAt(plant.index);
        Destroy(plant.gameObject);
        // animate incinerator?
    }

    // activated empty hand on pickup-able object
    public Transform PickupNutrient(int nutrient)
    {
        heldNutrientIndex = nutrient;
        var prefab = Instantiate(nutrientPrefab);
        prefab.tag = "Untagged";
        return prefab;
    }

    // activated empty hand on pickup-able object
    public Transform PickupSoil(int soil)
    {
        heldSoilIndex = soil;
        var prefab = Instantiate(soilPrefab);
        prefab.tag = "Untagged";
        return prefab;
    }

    // activate soil on slot with different soil
    public void ChangeSoil(Slot slot)
    {
        Plant plant = plants[slot.plantIndex];
        Soil soil = SOILS[heldSoilIndex];

        plant.ChangeSoil(soil, slot);
        heldSoilIndex = -1;
        // TODO: disappear model to represent it being used up
    }
}
