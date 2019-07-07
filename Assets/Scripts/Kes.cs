using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeldObject { nutrient, plant, soil, none };

public class Kes: MonoBehaviour
{
    public float movementSpeed = 1;
    public float gravity = 20;
    public float grabDistance = 2;

    //==================================================================================
    public Transform arms;
    public Transform holding;
    //==================================================================================

    public LevelGod levelGod;

    public Transform[] armModels;
    public HeldObject heldObject = HeldObject.none;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        HideArms();
    }

    private void HideArms()
    {
        foreach (var arm in armModels)
        {
            var position = arm.localPosition;
            position.z = -0.5f;
            arm.localPosition = position;
        }
    }
    private void ShowArms()
    {
        foreach (var arm in armModels)
        {
            var position = arm.localPosition;
            position.z = 0.5f;
            arm.localPosition = position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Time.timeScale = 4f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // setting fixedtime to still be 50fps, right?
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // setting fixedtime to still be 50fps, right?
        }
        
        Vector3 rotation = Vector3.zero;
        Vector3 moveDirection = Vector3.zero;
        if (controller.isGrounded)
        {
            rotation = rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * 180 * Time.deltaTime, 0);
            moveDirection = new Vector3(0, 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= movementSpeed;
            transform.Rotate(rotation);
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);

        bool needToShowArms = false;
        if (holding != null)
        {
            needToShowArms = true;
        }

        HandleClick();


        var interacting = CheckUI();

        if (interacting != null)
        {
            needToShowArms = true;
        }

        switch (heldObject)
        {
            case HeldObject.nutrient:
                if(interacting == "Slot")
                {
                    levelGod.ShowUI("Press 'E' to use");
                }
                break;
            case HeldObject.soil:
                if(interacting == "Slot")
                {
                    levelGod.ShowUI("Press 'E' to use");
                }
                break;
            case HeldObject.plant:
                if(interacting == "Slot")
                {
                    levelGod.ShowUI("Press 'E' to put down");
                }
                if (interacting == "Incinerator")
                {
                    levelGod.ShowUI("Press 'E' to INCINERATE!");
                }
                break;
            case HeldObject.none:
                if (interacting == "Plant")
                {
                    levelGod.ShowUI("Press 'E' to pick up");
                }
                break;
        }

        if (needToShowArms)
        {
            ShowArms();
        }
        else
        {
            HideArms();
        }
    }

    private void SlotInteract(Slot slot)
    {
        if (heldObject == HeldObject.plant && !slot.hasPlant)
        {
            PlacePlantOnSlot(slot);
            levelGod.PlacePlant(slot);
            heldObject = HeldObject.none;
            return;
        }

        if (slot.hasPlant)
        {
            if (heldObject == HeldObject.none)
            {
                PickUpPlantFromSlot(slot);
                levelGod.PickupPlant(slot);
                heldObject = HeldObject.plant;
            }    

            if (heldObject == HeldObject.soil)
            {
                PutDownSoil();
                levelGod.ChangeSoil(slot);
                heldObject = HeldObject.none;
            }
                
            if (heldObject == HeldObject.nutrient)
            {
                PutDownNutrient();
                levelGod.FeedPlant(slot);
                heldObject = HeldObject.none;
            }
        }
    }

    private void HandleClick()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Ray laser = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(laser, out hit, grabDistance))
            {
                switch (hit.collider.tag)
                {
                    case "Plant":
                        Slot pSlot = SlotAtPlantPosition(hit);
                        SlotInteract(pSlot);
                        break;

                    case "Slot":
                        Slot slot = SlotAtPosition(hit);
                        SlotInteract(slot);
                        break;

                    case "NutrientRack":
                        int nutrient = NutrientAtPosition(hit); 

                        if (heldObject == HeldObject.none)
                        {
                            PickUpNutrient(nutrient);
                            heldObject = HeldObject.nutrient;
                            return;
                        }

                        if(heldObject == HeldObject.nutrient)
                        {
                            PutDownNutrient();
                            heldObject = HeldObject.none;
                        }
                        break;

                    case "SoilRack":
                        int soil = SoilAtPosition(hit); 

                        if (heldObject == HeldObject.none)
                        {
                            PickUpSoil(soil);
                            heldObject = HeldObject.soil;
                            return;
                        }

                        if (heldObject == HeldObject.soil)
                        {
                            PutDownSoil();
                            heldObject = HeldObject.none;
                        }  
                        break;

                    case "Workstation":
                        if (heldObject == HeldObject.plant)
                        {
                            levelGod.ExaminePlantWithWorkstation();
                        }
                        break;
                    
                    case "Incinerator":
                        if (heldObject == HeldObject.plant)
                        {
                            levelGod.DestroyPlant(holding);
                            holding = null;
                            heldObject = HeldObject.none;
                        }
                        break;

                    default:
                        Debug.Log("Attempting interaction with unknown object.");
                        break;
                }
            }
        }
    }

    // basically if we are within distance of a plant
    // and the cursor is over the top
    // then show the UI for it
    // returns the plant it is over if it is over one
    private string CheckUI()
    {
        Ray laser = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(laser, out hit, grabDistance))
        {
            if (hit.collider.tag == "Plant" || hit.collider.tag == "Slot" || hit.collider.tag == "NutrientRack" || hit.collider.tag == "SoilRack" || hit.collider.tag == "Incinerator")
            {
                return hit.collider.tag;
            }
        }
        levelGod.HideUI();
        return null;
    }

    private Slot SlotAtPosition(RaycastHit hit)
    {
        return hit.transform.GetComponent<Slot>();
    }
    private Slot SlotAtPlantPosition(RaycastHit hit)
    {
        var plant = hit.transform.GetComponent<Plant>();
        return levelGod.SlotWithPlant(plant.index);
    }

    private int NutrientAtPosition(RaycastHit hit)
    {
        var bag = hit.transform.GetComponent<NutrientBag>();
        return bag.contents;
    }

    private int SoilAtPosition(RaycastHit hit)
    {
        var bag = hit.transform.GetComponent<SoilBag>();
        return bag.contents;
    }

    private void PickUpPlantFromSlot(Slot slot)
    {
        // TODO: if plant is child of slot in the scene, then we don't need to ray hit plant to
        // know which plant is in that slot. We can get just get slot.child?
        Plant plant = levelGod.plants[slot.plantIndex];
        plant.transform.position = arms.position;
        plant.transform.parent = null;
        plant.transform.parent = arms;
        plant.transform.GetComponent<Rigidbody>().isKinematic = true;

        holding = plant.transform;
    }

    private void PlacePlantOnSlot(Slot slot)
    {
        holding.transform.parent = null;
        slot.transform.parent.GetComponent<Shelf>().PlacePlant(holding, slot);

        holding = null;
    }

    private void PickUpNutrient(int nutrient)
    {
        // just hide the sprite of that kind of nutrient in the scene
        // appear another one in-hand
        var thing = levelGod.PickupNutrient(nutrient);
        thing.parent = null;
        thing.position = arms.position;
        thing.SetParent(arms);
        holding = thing;
    }

    private void PutDownNutrient()
    {
        // just un-hide the sprite of that kind of nutrient in the scene
        // disappear the ther one in-hand   
        Destroy(holding.gameObject);
    }

    private void PickUpSoil(int soil)
    {
        // just hide the sprite of that kind of soil in the scene
        // appear another one in-hand
        var thing = levelGod.PickupSoil(soil);
        thing.parent = null;
        thing.position = arms.position;
        thing.SetParent(arms);
        holding = thing;
    }

    private void PutDownSoil()
    {
        // disappear the other one in-hand
        Destroy(holding.gameObject);
    }
}
