using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeldObject { nutrient, plant, soil };

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

    public HeldObject heldObject = null;

    private CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
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
        
        Vector3 moveDirection = Vector3.zero;
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= movementSpeed;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            Ray laser = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(laser, out hit, grabDistance))
            {
                switch (hit.collider.tag)
                {
                    case "Slot": // TODO: make Plants ignore Raycast when put down?
                    case "Plant":
                        Slot slot = SlotAtPosition(hit);

                        if (heldObject == HeldObject.plant && !slot.hasPlant)
                        {
                            PlacePlantOnSlot(slot);
                            levelGod.PlacePlant(slot);
                            heldObject = null;
                        }

                        if (slot.hasPlant)
                        {
                            if (heldObject == null)
                            {
                                PickUpPlantFromSlot(slot);
                                levelGod.PickupPlant(slot);
                                heldObject = HeldObject.plant;
                            }    

                            if (heldObject == HeldObject.soil)
                            {
                                PutDownSoil(soil);
                                levelGod.ChangeSoil(slot);
                                heldObject = null;
                            }
                                
                            if (heldObject == HeldObject.nutrient)
                            {
                                PutDownNutrient(nutrient);
                                levelGod.FeedPlant(slot);
                                heldObject = null;
                            }
                        }
                        break;

                    case "Nutrient":
                        Nutrient nutrient = NutrientAtPosition(hit); 

                        if (heldObject == null)
                        {
                            PickUpNutrient(nutrient);
                            levelGod.PickupNutrient(nutrient);
                            heldObject = HeldObject.nutrient;
                        }

                        if(heldObject == HeldObject.nutrient)
                        {
                            PutDownNutrient(nutrient);
                            heldObject = null;
                        }
                        break;

                    case "Soil":
                        Soil soil = SoilAtPosition(hit); 

                        if (heldObject == null)
                        {
                            PickUpSoil(soil);
                            levelGod.PickupSoil(soil);
                            heldObject = HeldObject.soil;
                        }  

                        if (heldObject == HeldObject.soil)
                        {
                            PutDownSoil(soil);
                            heldObject = null;
                        }  
                        break;

                    case "Workstation":
                        if (heldObject == HeldObject.plant)
                        {
                            levelGod.ExaminePlantWithWorkstation();
                        }
                        break;

                    default:
                        Debug.Log("Attempting interaction with unknown object.");
                        break;
                }
            }
        }
    }


    private Slot SlotAtPosition(RayCastHit hit)
    {
        return hit.transform.GetComponent<Slot>();
    }

    private Nutrient NutrientAtPosition(RayCastHit hit)
    {
        return hit.transform.GetComponent<Nutrient>();
    }

    private Soil SoilAtPosition(RayCastHit hit)
    {
        return hit.transform.GetComponent<Soil>();
    }

    //==================================================================================
    private void PickUpPlantFromSlot(Slot slot)
    {
        // TODO: if plant is child of slot in the scene, then we don't need to ray hit plant to
        // know which plant is in that slot. We can get just get slot.child?
        hit.transform.position = arms.position;
        hit.transform.parent = null;
        hit.transform.parent = arms;
        hit.transform.GetComponent<Rigidbody>().isKinematic = true;
        var plant = hit.transform.GetComponent<Plant>();

        levelGod.PickupPlant(plant);
        holding = hit.transform;
    }

    private void PlacePlantOnSlot(Slot slot)
    {
        // TODO: if plant is child of slot in the scene, then we don't need to ray hit plant to
        // know which plant is in that slot. We can get just get slot.child?
        var plant = holding.GetComponent<Plant>();
        holding.parent = null;
        hit.transform.parent.GetComponent<Shelf>().PlacePlant(holding, hit.transform);
        var slot = hit.transform.GetComponent<Slot>();

        levelGod.PlacePlant(slot, plant.index);
        holding = null;
    }

    // TODO: function to pickup nutrient or put it back in 1 single pre-defined spot
    // else you either have to use it up on an occupied slot or carry it around
    private void PickUpNutrient(Nutrient nutrient)
    {
        // just hide the sprite of that kind of nutrient in the scene
        // appear another one in-hand
    }

    private void PutDownNutrient(RaycastHit hit)
    {
        // just un-hide the sprite of that kind of nutrient in the scene
        // disappear the ther one in-hand    
    }

    // TODO: function to pickup soil or put it back in 1 single pre-defined spot
    // else you either have to use it up on an occupied slot or carry it around
    private void PickUpSoil(Soil soil)
    {
        // just hide the sprite of that kind of soil in the scene
        // appear another one in-hand
    }

    private void PutDownSoil(Soil soil)
    {
        // just un-hide the sprite of that kind of soil in the scene
        // disappear the ther one in-hand    
    }

    //==================================================================================
}
