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
        return null;
    }
}
