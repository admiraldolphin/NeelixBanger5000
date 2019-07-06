using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf: MonoBehaviour
{
    public List<Transform> slots;

    void Start()
    {
        slots = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.tag == "Slot")
            {
                slots.Add(child);
            }
        }
    }

    // we pick where it goes on this one
    public void PlacePlant(Transform plant)
    {
        Transform slot = slots[0].transform;
        var closest = float.MaxValue;

        foreach (var child in slots)
        {
            bool hasPlant = false;
            for (int i = 0; i < child.childCount; i++)
            {
                if (child.tag == "Plant")
                {
                    hasPlant = true;
                    break;
                }
            }

            if (!hasPlant)
            {
                var distance = Vector3.Distance(plant.position, child.position);
                if (distance < closest)
                {
                    slot = child;
                    closest = distance;
                }
            }
        }

        StorePlant(plant, slot);
    }

    private void StorePlant(Transform plant, Transform slot)
    {
        if (!SlotHasPlant(slot))
        {
            var position = slot.position;
            position.y += 0.5f;
            plant.position = position;
            plant.rotation = Quaternion.identity;
            plant.parent = null;
            plant.parent = slot.transform;
        }
    }

    // we put it exactly where we clicked
    public void PlacePlant(Transform plant, Transform spot)
    {
        StorePlant(plant, spot);
    }

    private bool SlotHasPlant(Transform slot)
    {
        for (int i = 0; i < slot.childCount; i++)
        {
            if (slot.tag == "Plant")
            {
                return true;
            }
        }
        return false;
    }
}