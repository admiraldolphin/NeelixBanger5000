using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kes: MonoBehaviour
{
    public float movementSpeed = 1;
    public float gravity = 20;
    public float grabDistance = 2;
    public Transform arms;
    public Transform holding;
    public LevelGod levelGod;

    public Transform[] armModels;

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

        bool needToShowArms = false;
        if (holding != null)
        {
            needToShowArms = true;
        }

        HandleClick();

        var hoveredPlant = ShowUI();
        if (hoveredPlant != null)
        {
            needToShowArms = true;
            // handle the button interaction in here
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("You water the plant");
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("You glare at the plant");
            }
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

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray laser = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(laser, out hit, grabDistance))
            {
                switch (hit.collider.tag)
                {
                    case "Plant":
                        if (holding == null)
                        {
                            PickUpPlant(hit);
                        }
                        else
                        {
                            DropPlant();
                            if (hit.transform != holding)// is it a different plant?
                            {
                                PickUpPlant(hit);
                            }
                        }
                        break;
                    
                    case "Shelf":
                        PlacePlantOnShelf(hit);
                        break;
                    
                    case "Slot":
                        PlacePlantOnSlot(hit);
                        break;

                    default:
                        Debug.Log("shitballs");
                        break;
                }
            }
            else
            {
                DropPlant();
            }
        }
    }

    // basically if we are within distance of a plant
    // and the cursor is over the top
    // then show the UI for it
    // returns the plant it is over if it is over one
    private Plant ShowUI()
    {
        Ray laser = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(laser, out hit, grabDistance))
        {
            if (hit.collider.tag == "Plant")
            {
                var plant = hit.transform.GetComponent<Plant>();

                // show the plant UI then
                levelGod.ShowUI();
                return plant;
            }
        }

        levelGod.HideUI();
        return null;
    }

    private void PickUpPlant(RaycastHit hit)
    {
        hit.transform.position = arms.position;
        hit.transform.parent = null;
        hit.transform.parent = arms;
        hit.transform.GetComponent<Rigidbody>().isKinematic = true;
        var plant = hit.transform.GetComponent<Plant>();

        levelGod.PickupPlant(plant);

        holding = hit.transform;
    }
    private void DropPlant()
    {
        if (holding != null)
        {
            holding.rotation = Quaternion.identity;
            holding.GetComponent<Rigidbody>().isKinematic = false;
            var plant = holding.GetComponent<Plant>();
            levelGod.DropPlant(plant.index);
            holding.parent = null;
            holding = null;
        }
    }
    private void PlacePlantOnShelf(RaycastHit hit)
    {
        if (holding != null)
        {
            var shelf = hit.transform.GetComponent<Shelf>();
            var plant = holding.GetComponent<Plant>();
            
            var slot = shelf.AvailableSpot(holding);
            if (levelGod.PlacePlant(slot, plant.index))
            {
                shelf.PlacePlant(holding, slot);

                holding = null;
            }
        }
    }
    private void PlacePlantOnSlot(RaycastHit hit)
    {
        if (holding != null) // we want to put the plant on that slot
        {
            var plant = holding.GetComponent<Plant>();
            var slot = hit.transform.GetComponent<Slot>();

            if (levelGod.PlacePlant(slot, plant.index))
            {
                holding.parent = null;
                hit.transform.parent.GetComponent<Shelf>().PlacePlant(holding, slot);

                holding = null;
            }
        }
    }
}
