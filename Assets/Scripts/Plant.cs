using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant: MonoBehaviour
{
    public int index = -1;

    // so they start at 0
    // if they reach -3 they die
    // if they reach +3 they do something else..?
    // all numbers made up
    public int happiness = 0;
    public int deadLevel = -3;
    public int superHappy = 3;

    // default settings for plants
    public Soil soil = Soil.dirt;
    public Atmosphere atmosphere = Atmosphere.air;
    public Temperature temperature = Temperature.moderate;

    public Change[] ProcessChange(Change change)
    {
        List<Change> changes = new List<Change>();
        // in here is where we can make plants do all sorts of weird stuff
        // by returning a new change we can keep shit getting real!
        switch(change.type)
        {
            case ChangeType.soil:
                if (change.soil == soil)
                {
                    happiness += 1;
                }
                else
                {
                    happiness -= 1;
                }
                break;
            
            case ChangeType.temperature:
                if (change.temperature == temperature)
                {
                    happiness += 1;
                }
                else
                {
                    happiness -= 1;
                }
                break;
            
            case ChangeType.atmosphere:
                if (change.atmosphere == atmosphere)
                {
                    happiness += 1;
                }
                else
                {
                    happiness -= 1;
                }
                break;
        }
        return changes.ToArray();
    }
    
    public PlantDecalSet plantDecalSet;

    private void Awake() {

        if (plantDecalSet == null) {
            return;
        }

        // TODO: choose a decal object based on input from elsewhere, not here

        var set = Random.Range(0,2) == 0 ? plantDecalSet.flowers : plantDecalSet.fruits;

        var decal = set[Random.Range(0, set.Length)];

        var decalColor = Random.ColorHSV(0,1,0.8f,0.9f, 0.8f, 1f);
        var mainColor = Random.ColorHSV(0,1,0.8f,0.9f, 0.8f, 1f);

        foreach (var renderer in this.GetComponentsInChildren<MeshRenderer>()) {
            renderer.material.color = mainColor;
        }

        foreach (var spawner in GetComponentsInChildren<DecalObjectSpawner>()) {
            spawner.SpawnObject(decal, decalColor);
        }

        this.transform.rotation = Quaternion.Euler(-90,Random.Range(0,360),0);

        
    }
}
