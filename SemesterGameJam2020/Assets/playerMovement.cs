using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float wallJumpPush;
    public float horizontalVelocity;
    public float verticalVelocity;
    public float deceleration = 0.01f;
    public bool frozen;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public LayerMask fallingMask;
    public LayerMask playableZone;
    public LayerMask lava;
    RaycastHit wallScanner;

    Vector3 velocity;
    bool isGrounded;
    int maxJumps = 3;
    int doubleJumps;
    float x;
    float z;
    bool jump;
    // Update is called once per frame
    void Update()
    {
        isGrounded = (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) || Physics.CheckSphere(groundCheck.position, groundDistance, fallingMask));
        if (!frozen)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            jump = Input.GetButtonDown("Jump");
        }
        else
        {
            if (Mathf.Abs(x) > 0.01)
            {
                x -= 1 * Time.deltaTime * Mathf.Sign(x);
            }
            else
            {
                x = 0;
            }
            if (Mathf.Abs(z) > 0.01)
            {
                z -= 1 * Time.deltaTime * Mathf.Sign(z);
            }
            else
            {
                z = 0;
            }
        }

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
            if (Physics.Raycast(transform.position,Vector3.up, 1.5f, fallingMask))
            {
                velocity.y  = -.3f;
            }
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

        if (!ValidZone())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        Debug.DrawRay(transform.position, transform.forward, Color.black);


        controller.Move(velocity);
        
    }

    bool ValidZone()
    {
        bool inGame = Physics.CheckSphere(transform.position, 1f, playableZone);
        bool fallFix = Physics.CheckSphere(transform.position, 3f, fallingMask);
        bool inBlock = Physics.CheckSphere(transform.position, 0.01f, groundMask);
        bool inLava = Physics.CheckSphere(transform.position, 0.01f, lava,QueryTriggerInteraction.Collide);
        if ((inGame || fallFix) && !inBlock && !inLava)
        {

            return true;
        }
        else
        {
            return false;
        }
    }
}
