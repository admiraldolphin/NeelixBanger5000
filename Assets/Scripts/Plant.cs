using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Atmosphere { oxygen, other }
public enum Temperature { moderate, cold, hot }
public enum Soil { dirt, ash, sand, tar, bonechar }
public enum LightLevel { normal, dark }
public enum Nutrient { water, blood, acid }

public class Plant: MonoBehaviour
{
    // == CONSTANT PROPERTIES ==

    const int HAPPINESS_THRESHOLD = 10; // how long a plant can be HAPPY without its needs met
    const int LIVINGNESS_THRESHOLD = 20; // how long a plant can be ALIVE without its needs met

    public Color sickColor = new Color(167f/255f,114f/255f,0f);
    public Color deadColor = Color.black;

    public MeshRenderer traySoilRenderer;


    // default settings for plants
    public Atmosphere atmosphere = Atmosphere.oxygen;
    public Temperature temperature = Temperature.moderate;
    public Soil soil = Soil.dirt;
    public LightLevel lightLevel = LightLevel.normal;
    public Nutrient nutrient = Nutrient.water;

    public Color healthyColor = Color.green;

    public LevelGod levelGod;

    public int index = -1;

    // == CHANGEABLE PROPERTIES ==

    // how many properties of the plant have been discovered
    public int discoveredProperties = 0;

    // what soil is in its tray
    public Soil traySoil = Soil.dirt;

    // should tick up over time
    public int lastFed = 0;
    public int lastInCorrectEnvironment = 0;

    // should update on putDown()
    public bool correctEnvironment = false;

    private GameObject plantBaseInstance;

    // == COMPUTED PROPERTIES ==

    public bool isHappy
    {
        get
        {
            return (lastFed < HAPPINESS_THRESHOLD) && (lastInCorrectEnvironment < HAPPINESS_THRESHOLD); 
        }
    }

    public bool isAlive
    {
        get 
        { 
            return (lastFed < LIVINGNESS_THRESHOLD) && (lastInCorrectEnvironment < LIVINGNESS_THRESHOLD); 
        }
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
        if (!correctEnvironment)
        {
            lastInCorrectEnvironment += 1;
        }

        lastFed += 1;

        UpdateHealthAppearance();

        return (isHappy && isAlive);
    }

    private void UpdateHealthAppearance()
    {
        var baseRenderer = plantBaseInstance.GetComponent<MeshRenderer>();
        
        if (isAlive) {
            if (isHappy) {
                baseRenderer.material.color = healthyColor;                
            } else {
                baseRenderer.material.color = sickColor;
            }
        } else {
            baseRenderer.material.color = deadColor;
        }
    }

    // plant was putDown() in new slot
    public bool PutDown(Slot slot)
    {
        // check if new conditions match requirements
        correctEnvironment = CheckEnvironment(slot);
        return correctEnvironment;
    }

    public bool ChangeSoil(Soil soil, Slot slot, Color soilColor)
    {
        traySoil = soil;

        traySoilRenderer.material.color = soilColor;

        // check if new conditions match requirements
        correctEnvironment = CheckEnvironment(slot);
        return correctEnvironment;
    }

    // plant was putDown() in new slot or slot changed soil
    public bool CheckEnvironment(Slot slot)
    {
        // check if new conditions match requirements
        correctEnvironment = (slot.atmosphere == this.atmosphere) &&
            (slot.temperature == this.temperature) &&
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
    public PlantDecalSet plantDecalSet;

    private void Awake() 
    {
        levelGod = GameObject.FindObjectOfType<LevelGod>();
        
        var bases = plantDecalSet.bases;
        var plantBasePrefab = bases[UnityEngine.Random.Range(0, bases.Length)];

        Soil[] randoSoil = (Soil[])System.Enum.GetValues(typeof(Soil));
        LightLevel[] randoLight = (LightLevel[])System.Enum.GetValues(typeof(LightLevel));
        var requiremenSoil = randoSoil[UnityEngine.Random.Range(0, randoSoil.Length)];
        var requiremenLight = randoLight[UnityEngine.Random.Range(0, randoLight.Length)];

        soil = requiremenSoil;
        lightLevel = requiremenLight;

        if (plantDecalSet == null) 
        {
            return;
        }

        plantBaseInstance = Instantiate(plantBasePrefab);
        plantBaseInstance.transform.SetParent(this.transform, true);
        plantBaseInstance.transform.position = this.transform.position;

        healthyColor = UnityEngine.Random.ColorHSV(0,1,0.8f,0.9f, 0.8f, 1f);
        
        // TODO: choose a decal object based on input from elsewhere, not here

        var set = UnityEngine.Random.Range(0,2) == 0 ? plantDecalSet.flowers : plantDecalSet.fruits;

        var decal = set[UnityEngine.Random.Range(0, set.Length)];

        var decalColor = UnityEngine.Random.ColorHSV(0,1,0.8f,0.9f, 0.8f, 1f);
        
        foreach (var renderer in plantBaseInstance.GetComponentsInChildren<MeshRenderer>()) 
        {
            renderer.material.color = healthyColor;
        }

        foreach (var spawner in GetComponentsInChildren<DecalObjectSpawner>()) 
        {
            spawner.SpawnObject(decal, decalColor);
        }
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
        CheckEnvironment(levelGod.spawnSlot);
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
