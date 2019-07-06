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
                    
                    // case "Shelf":
                    //     PlacePlantOnShelf(hit);
                    //     break;
                    
                    case "Slot":
                        if (holding != null) // we want to put the plant on that slot
                        {
                            PlacePlantOnSlot(hit);
                        }
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
        var shelf = hit.transform.GetComponent<Shelf>();
        if (holding != null)
        {
            var plant = holding.GetComponent<Plant>();
            holding.parent = null;
            shelf.PlacePlant(holding);
            holding = null;

            //levelGod.PlacePlant(0, plant.index);
        }
    }
    private void PlacePlantOnSlot(RaycastHit hit)
    {
        if (holding != null) // we want to put the plant on that slot
        {
            var plant = holding.GetComponent<Plant>();
            holding.parent = null;
            hit.transform.parent.GetComponent<Shelf>().PlacePlant(holding, hit.transform);
            var slot = hit.transform.GetComponent<Slot>();

            levelGod.PlacePlant(slot, plant.index);
            holding = null;
        }
    }
}
