using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Unity predefined Character controller that will handle movement inputs
    public CharacterController controller;

    //Base speed values for moving jumping and falling
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public int maxJumps = 3;

    //Force applied when walljumping away from a wall
    public float wallJumpPush;
    
    //The speed of the player in different directions at any given moment
    public float horizontalVelocity;
    public float verticalVelocity;
    //How quickly a player loses momentum
    public float deceleration = 0.01f;
    //Whether the player is frozen by anotehr script
    public bool frozen;

    //Location of bottom of character that will check if the player is on the ground, and the distance to check to see if there is ground
    public Transform groundCheck;
    public float groundDistance = 0.2f;

    //Layer masks to detect appropriate objects
    public LayerMask groundMask;
    public LayerMask fallingMask;
    public LayerMask playableZone;
    public LayerMask lava;

    //Storage device for raycast to see if a walljump is possible
    RaycastHit wallScanner;
    
    //internal values keeping track of moving values and states
    Vector3 velocity;
    bool isGrounded;
    int doubleJumps;
    float x;
    float z;
    bool jump;
    // Update is called once per frame
    void Update()
    {
        //check to see if the player is on the ground or on a alling piece and save that value for use in the rest of the update
        isGrounded = (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask) || Physics.CheckSphere(groundCheck.position, groundDistance, fallingMask));
        //Detect whether input values should be taken at all, when control is taken away frozen will be  true
        if (!frozen)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            jump = Input.GetButtonDown("Jump");
        }//If the player is frozen, gradually slow their movements down until eventually they reach zero
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
        //calcualte a movement vector based on inputs
        Vector3 move = (transform.right * (x) + transform.forward * (z + verticalVelocity)+ new Vector3(horizontalVelocity,0,verticalVelocity));
        
        //Slow player down based on deceleration value
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
        //Pass movement vector adjsuted for time and desired speed to the player controller
        controller.Move(move * speed * Time.deltaTime);
        //apply change to vertical velocity based on gravity value
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            //If there is a falling block close to the palyers head move them down immedietly so that they don't collide with the falling piece
            if (Physics.Raycast(transform.position,Vector3.up, 1.5f, fallingMask))
            {
                velocity.y  = -.3f;
            }
        }
        else
        {
            //If th eplayer is on the ground reset their double jump
            doubleJumps = maxJumps;
            velocity.y = -.2f;
        }
        //If on the ground and the jump key is pressed immedietly set the player vertical speed to bet the jump speed
        if (jump && isGrounded)
        {
            velocity.y = jumpHeight;
        }
        //If the player is not on the ground and is facing a wall 
        else if(jump && Physics.Raycast(groundCheck.position,transform.forward,out wallScanner,1f) && doubleJumps > 0) 
        {
                horizontalVelocity = -transform.forward.x * wallJumpPush;
                verticalVelocity = -transform.forward.z * wallJumpPush;
                doubleJumps--;
                velocity.y = jumpHeight;
            
        }
        // detect if the player is in a valid area of play, if not restart the level.This is considered a death.
        if (!ValidZone())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        //Wall Jump debugging ray
        Debug.DrawRay(transform.position, transform.forward, Color.black);

        //Vertical movement sent to movement controller
        controller.Move(velocity);
        
    }
    //detect if the player is in a valid area of play
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
