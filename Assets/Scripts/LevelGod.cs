using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGod : MonoBehaviour
{
    public Slot[] slots;
    public List<Plant> plants = new List<Plant>();

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

    public Plant plantPrefab;
    public void CreatePlant()
    {
        // basically make a new plant based on the template passed in (not done yet)
        // add it to the list of plants
        // plants will start just off by themselves I guess?
        // sure why not
        // later need to start them somewhere a bit more meaningful
        var plant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
        var index = plants.Count;
        plant.index = index;
        plants.Add(plant);
    }

    private List<Change> changeList = new List<Change>();
    private void FlagChanges(Change[] changes)
    {
        changeList.AddRange(changes);
    }
    private void FlagChange(Change change)
    {
        changeList.Add(change);
    }

    private void Tick()
    {
        // giving each plant the slots attributes
        foreach (var slot in slots)
        {
            // if you have a plant you flag it needing changes
            if (slot.plant != -1)
            {
                Debug.Log($"{slot.name} holds plant {plants[slot.plant].name}");
                FlagChanges(slot.changes);
            }
        }

        Debug.Log($"flagged {changeList.Count} changes");

        // processing each change
        while (changeList.Count > 0)
        {
            var change = changeList[0];
            var result = plants[change.index].ProcessChange(change);

            if (result != null && result.Length > 0)
            {
                FlagChanges(result);
            }

            changeList.RemoveAt(0);
        }
        changeList.Clear();// in theory they are all gone by now but it never hurts to make sure
    }

    public void PickupPlant(Plant plant)
    {
        Debug.Log("picked up plant");

        // go through the slots and find the plants owner
        // squish that out
        foreach (var slot in slots)
        {
            if (slot.plant == plant.index)
            {
                slot.plant = -1;
            }
        }

        // also pin the plant to your arms now
        // or should that remain the job of Kes?
    }
    public void KillPlant(Plant plant)
    {
        // to kill something we just remove it from the mappings and then remove the gameobject
        // this does mean the plants list will evetually just fill up with nothin
        // that feels ok to me
        foreach (var slot in slots)
        {
            if (slot.plant == plant.index)
            {
                slot.plant = -1;
            }
        }

        Destroy(plant);
    }
    public void PlacePlant(Slot slot, int plant)
    {
        slot.plant = plant;
        Debug.Log("Placed plants");
    }
    public void DropPlant(int plant)
    {
        Debug.Log($"Dropping the plant at {plant}");
    }

    float elapsed = 0;
    public float TickRate = 2;
    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed > TickRate)
        {
            elapsed = 0;
            Tick();
        }
    }
}

public enum Atmosphere { air, weird }
public enum Soil { dirt, ash }
public enum Temperature {moderate, cold, hot}

public enum ChangeType {atmosphere, soil, temperature}
public enum ChangeGroup {slot, plant}
public class Change
{
    public ChangeType type = ChangeType.atmosphere;
    public ChangeGroup group = ChangeGroup.plant;
    public int index = 0;
    
    // these are the defaults
    public Atmosphere atmosphere = Atmosphere.air;
    public Soil soil = Soil.dirt;
    public Temperature temperature = Temperature.moderate;
}
