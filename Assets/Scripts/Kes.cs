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
                            // animate/physics?
                            levelGod.PlacePlant(slot);
                            heldObject = null;
                        }

                        if (slot.hasPlant)
                        {
                            if (heldObject == null)
                            {
                                // animate/physics?
                                levelGod.PickupPlant(slot);
                                heldObject = HeldObject.plant;
                            }    

                            if (heldObject == HeldObject.soil)
                            {
                                // animate/physics?
                                levelGod.ChangeSoil(slot);
                                heldObject = null;
                            }
                                
                            if (heldObject == HeldObject.nutrient)
                            {
                                // animate/physics?
                                levelGod.FeedPlant(slot);
                                heldObject = null;
                            }
                        }
                        break;

                    case "Nutrient":
                        if (heldObject == null)
                        {
                            Nutrient nutrient = NutrientAtPosition(hit); 
                            // animate/physics?
                            levelGod.PickupNutrient(nutrient);
                            heldObject = HeldObject.nutrient;
                        }
                        break;

                    case "Soil":
                        if (heldObject == null)
                        {
                            Soil soil = SoilAtPosition(hit); 
                            // animate/physics?
                            levelGod.PickupSoil(soil);
                            heldObject = HeldObject.soil;
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
    private void PickUpPlantFromSlot(RaycastHit hit)
    {
        hit.transform.position = arms.position;
        hit.transform.parent = null;
        hit.transform.parent = arms;
        hit.transform.GetComponent<Rigidbody>().isKinematic = true;
        var plant = hit.transform.GetComponent<Plant>();

        levelGod.PickupPlant(plant);
        holding = hit.transform;
    }

    private void PlacePlantOnSlot(RaycastHit hit)
    {
        if (holding != null)
        {
            var plant = holding.GetComponent<Plant>();
            holding.parent = null;
            hit.transform.parent.GetComponent<Shelf>().PlacePlant(holding, hit.transform);
            var slot = hit.transform.GetComponent<Slot>();

            levelGod.PlacePlant(slot, plant.index);
            holding = null;
        }
    }
    //==================================================================================
}
