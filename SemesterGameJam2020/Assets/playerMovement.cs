using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float wallJumpPush;
    public float horizontalVelocity;
    public float verticalVelocity;
    public float deceleration = 0.01f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public LayerMask fallingMask;
    RaycastHit wallScanner;

    Vector3 velocity;
    bool isGrounded;
    int maxJumps = 3;
    int doubleJumps;
    // Update is called once per frame
    void Update()
    {
        isGrounded = (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) || Physics.CheckSphere(groundCheck.position, groundDistance, fallingMask));

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");


        Vector3 move = (transform.right * (x) + transform.forward * (z + verticalVelocity)+ new Vector3(horizontalVelocity,0,verticalVelocity));

        if (Mathf.Abs(horizontalVelocity) > 0) {
            horizontalVelocity -= Mathf.Sign(horizontalVelocity) * deceleration;
            if (Mathf.Abs(horizontalVelocity) < 0.02f)
            {
                horizontalVelocity = 0;
            }
         }
        if (Mathf.Abs(verticalVelocity) > 0)
        {
            verticalVelocity -= Mathf.Sign(verticalVelocity) * deceleration;
            if (Mathf.Abs(verticalVelocity) < 0.02f)
            {
                verticalVelocity = 0;
            }
        }

        controller.Move(move * speed * Time.deltaTime);
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            doubleJumps = maxJumps;
            velocity.y = -.2f;
        }
        if (jump && isGrounded)
        {
            velocity.y = jumpHeight;
        } else if(jump && Physics.Raycast(groundCheck.position,transform.forward,out wallScanner,1f) && doubleJumps >0) 
        {
            Debug.Log("got here");
            
           // if (wallScanner.transform.gameObject.layer == groundMask)
            //{
                horizontalVelocity = -transform.forward.x * wallJumpPush;
                verticalVelocity = -transform.forward.z * wallJumpPush;
                doubleJumps--;
                velocity.y = jumpHeight;
           // }
            
        }
        Debug.Log(transform.forward);
        Debug.DrawRay(transform.position, transform.forward, Color.black);
        controller.Move(velocity);
    }
}
