using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kes: MonoBehaviour
{
    public float movementSpeed = 1;
    public float jumpSpeed = 1;
    public float gravity = 20;
    public float grabDistance = 2;
    public Transform arms;
    public Transform plant;

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

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            if (plant == null)
            {
                Ray laser = Camera.main.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(laser.origin, laser.direction, Color.red, 50);
                RaycastHit hit;
                if (Physics.Raycast(laser, out hit, grabDistance))
                {
                    if (hit.collider.tag == "Plant")
                    {
                        if (plant == null)
                        {
                            hit.transform.position = arms.position;
                            hit.transform.parent = arms;
                            hit.transform.GetComponent<Rigidbody>().isKinematic = true;

                            plant = hit.transform;
                        }
                    }
                }
            }
            else
            {
                plant.GetComponent<Rigidbody>().isKinematic = false;
                plant.parent = null;
                plant = null;
            }
        }
    }
}
