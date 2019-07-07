using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf: MonoBehaviour
{
    public List<Slot> slots;

    void Start()
    {
        slots = new List<Slot>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.tag == "Slot")
            {
                var slot = child.GetComponent<Slot>();
                slots.Add(slot);
            }
        }
    }

    // we pick where it goes on this one
    // returns the slot that was selected
    public Slot AvailableSpot(Transform plant)
    {
        Slot slot = slots[0];
        var closest = float.MaxValue;

        foreach (var point in slots)
        {
            if (!point.hasPlant)
            {
                var distance = Vector3.Distance(plant.position, point.transform.position);
                if (distance < closest)
                {
                    slot = point;
                    closest = distance;
                }
            }
        }

        return slot;
    }

    // this method assumes you have already checked you CAN store a plant here
    private void StorePlant(Transform plant, Slot slot)
    {
        var position = slot.transform.position;
        position.y += 0.5f;
        plant.position = position;
        plant.rotation = Quaternion.identity;

        plant.parent = null;
        //plant.parent = slot.transform;
        plant.SetParent(slot.transform);
        Debug.Log($"parent: {plant.transform.parent.name}");
    }

    // we put it exactly where we clicked
    public void PlacePlant(Transform plant, Slot spot)
    {
        StorePlant(plant, spot);
    }

    // need to make it so that shelves can change the properties of their slots properties
    // basiclly we want a global "change the settings please" mode
    // that can then be hooked up to the UI and we can show it as needed
}