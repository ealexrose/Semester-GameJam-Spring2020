using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DropLogic : MonoBehaviour
{

    /*===================================================================================================================
     * DropLogic 1.2.0 - moves the piece while in a falling state
     * 
     * Manages the drop and rotational control of falling pieces and should prevent any invalid attempted movement
     * Creates a ghost outline at the piece destination
     * TODO Quick drop functionality
     * =================================================================================================================*/

    //spawner is the creator of this piece, reference is used to let it know when it is time to drop another piece
    //rotater is the rotational pivot of the tetronimo, typically offset so that rotations align to the grid
    public GameObject spawner;
    public GameObject rotater;
    public GameObject IconDisplay;
    public GameObject dropDownGhost;


    public Material ghostMaterial;
    //fallSpeed is the speed at which the tetronimo falls, set by spawner
    //fastFallSpeed is the speed of the fall when pressing the fast fall button, also set by the spawner
    //unit is the length of the cube in  unity units, makes math easier
    //width is the width of the field, used to not go out of bounds
    //transperency is the visibility of the solid portion of the piece in the ghost outline
    public float fallSpeed;
    public float fastFallSpeed;
    float fastSpeed;
    public float unit;
    public float width;
    public float transperency;

    //rotSpeedBase is the base rotational speed when changing setup, should go evenly into 90 for now
    //groundMask is a layer mask for ground to identify if future movements are invalid to prevent pieces moving in to each other
    //rotSpeed is the current rotational speed
    //rotCount keeps track of the amount of degrees rotated on a given rotation
    public float rotSpeedBase = 1.5f;
    public LayerMask groundMask;
    float rotSpeed;
    float rotCount;

    //Dimensions of the piece relative to the rotater. <x,y,z,w> = Top, Right, Left, and Right
    public Vector4 dimensions;

    //shiftCoolDown is the Time in frames between actions taken
    //shiftCoolDownTimer keeps track of where the actual cooldown timer is
    public float shiftCoolDown = 5;
    float shiftCoolDownTimer = 0;

    //keeps track of the states of the tetronimo
    bool isFalling = true;
    bool isRotating = false;

    IconController iconControls;
    void Start()
    {
        iconControls  = IconDisplay.GetComponent<IconController>();
        iconControls.SetIcon(this.gameObject);
        DropDownSearch();
        //ghostMaterial = rotater.GetComponentInChildren<Transform>().GetComponent<Renderer>().material;
        //ghostMaterial.color = new Color(ghostMaterial.color.r, ghostMaterial.color.g, ghostMaterial.color.b, transperency);
        Debug.Log(ghostMaterial + " " + ghostMaterial.color);
    }

    // Update is called once per frame
    void Update()
    {
        //Check to see fi any of the pieces of the Tetromino have touched the ground yet, if they have stop the piece, move it to align with the grid,
        //Destroy its ghost, set the spawner to spawn the next piece, and destroy this script
        foreach (Transform child in rotater.transform)
        {
            if (child.GetComponent<FallCheck>().Check())
            {
                iconControls.Reset();
                foreach (Transform children in rotater.transform)
                {
                    children.transform.gameObject.layer = 8;
                }
                isFalling = false;
                spawner.GetComponent<PieceSpawner>().spawnFlag = true;
                transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y / 5f) * 5f, transform.position.z);
                Destroy(dropDownGhost);
                Destroy(this);
            }
        }

        //While the piece is falling
        if (isFalling)
        {
            //If enough time has passed between actions, check to see if the player wants to shift the Tetromino or rotate
            if (shiftCoolDownTimer == 0)
            {
                //If the Tetromino is not rotating, check for shifts
                if (!isRotating)
                {
                    //In the case of moving left or right, destroy the current ghost, then move the piece and place a new ghost
                    //TODO Change it so that it moves the locaiton of an already generated ghost instead of spawning a new one everytime to save memory
                    if (Input.GetButtonDown("LeftShift") && (ValidMovement(-(unit * (dimensions.w + 0.5f)))))
                    {
                        Destroy(dropDownGhost);
                        transform.position -= (Vector3.right * unit);
                        DropDownSearch();
                        shiftCoolDownTimer = shiftCoolDown;
                    }
                    if (Input.GetButtonDown("RightShift") && (ValidMovement(unit * (dimensions.y + 0.5f))))
                    {
                        Destroy(dropDownGhost);
                        transform.position += (Vector3.right * unit);
                        DropDownSearch();
                        shiftCoolDownTimer = shiftCoolDown;
                    }
                }

                //If the way is clear rotate the piece at the speed given when it spawned until the rotation is complete
                if (Input.GetButtonDown("RotateCW") && !isRotating && ValidRotate(true))
                {
                    iconControls.RotateMe(rotSpeedBase);
                    rotSpeed = rotSpeedBase;
                    rotCount = 0;
                    isRotating = true;
                    //Sets new dimensions by rotating teh vectro as well
                    dimensions = new Vector4(dimensions.w, dimensions.x, dimensions.y, dimensions.z);
                    
                }
            }//If the cooldown ahs not finished tick it down
            else if(shiftCoolDownTimer >= 0 ){
                shiftCoolDownTimer -= 1;
            }
            //If a rotation is in progress, rotate and then check to see if rotation is complete, if it is, destroy the ghost and create a new one
            if (isRotating)
            {
                rotater.transform.Rotate(new Vector3(0, 0, rotSpeed));
                rotCount += rotSpeed;
                if(rotCount >= 90)
                {
                    Destroy(dropDownGhost);
                    DropDownSearch();
                    isRotating = false;
                }
            }
            //IF the fastfall button is pressed, make the Tetromino fall at its fast fall speed
            if (Input.GetButton("fastFall"))
            {
                fastSpeed = fastFallSpeed;
            }
            else
            {
                fastSpeed = 0;
            }
            //If the Tetromino is not rotating, have it continure to fall
            if (!isRotating)
            {
                transform.position -= ((fallSpeed + fastSpeed) * Vector3.up * Time.deltaTime);
            }
         }


    }
    //briefly Check a sphere from an offset of each piece of a tetromino at designated new location
    //If there is any overlap with an obstacle do not move
    bool ValidMovement(float _xMov)
    {
        float newPos = rotater.transform.position.x + _xMov;
        Vector3 locCheck = (new Vector3(newPos, rotater.transform.position.y, 0));
        Debug.DrawRay(locCheck, Vector3.up * dimensions.x * unit, Color.green, 3);
        Debug.DrawRay(locCheck, Vector3.down * dimensions.z * unit, Color.green, 3);
        if ((newPos <= -(width * unit / 2)) || (newPos >= (width * unit / 2)))
        {
            return false;

        }
        else

        {
            foreach (Transform child in rotater.transform)
            {
                if (Physics.CheckSphere(child.position + (Vector3.right * unit * Mathf.Sign(_xMov)), 1.5f, groundMask))
                {
                    return false;
                }
            }
        }


        return true;
    }

    //Rotate the piece to a couple critical angles and check if there is any overlap with other objects, if there is return that it is an invalid rotation
    //Return to previous rotation
    bool ValidRotate(bool _dir)
    {
        float halfExtent = (unit / 2) - .01f;
        rotSpeed = 30;
        rotCount = 0;
        do
        {
            rotater.transform.Rotate(new Vector3(0, 0, rotSpeed));
            rotCount += rotSpeed;
            foreach(Transform child in rotater.transform)
            {
                Collider[] obstacles = Physics.OverlapBox(child.transform.position, new Vector3(halfExtent, (unit / 2) - .01f, (unit / 2) - .01f), child.rotation, groundMask);
                if (obstacles.Length > 0)
                {
                    rotater.transform.Rotate(new Vector3(0, 0, -rotCount));
                    return false;                   
                }
            }
        } while (rotCount <= 90);
        rotater.transform.Rotate(new Vector3(0, 0, -rotCount));
        return true;
    }

    //Search straight down until the bottom of the playfield is found, place a ghost at that location
    void DropDownSearch()
    {
        //Where the piece starts, saved so that the location can be returned to
        Vector3 homePos = transform.position;
        bool reachedBottom = false;
        //Loop until the bottom is found, once the bottom is found create a ghost a copy of the tetormino
        //Save that tetromino in memory and then disable attached scripts. Then change the material to be the ghost material
        do
        {
            //Check each piece of the tetromino to see if it intersects with the ground
            foreach (Transform child in rotater.transform)
            {
                Collider[] obstacles = Physics.OverlapBox(child.transform.position, new Vector3((unit / 2) - .01f, (unit / 2) - .01f, (unit / 2) - .01f), child.rotation, groundMask);
                //if it does interesect do the ghost setup and exit the loop
                if (obstacles.Length > 0)
                {
                    reachedBottom = true;
                    dropDownGhost = Instantiate(this.gameObject, new Vector3(transform.position.x, Mathf.Ceil(transform.position.y / 5f) * 5f,transform.position.z), Quaternion.identity);
                    foreach (Transform ghostChild in dropDownGhost.GetComponentInChildren<DropLogic>().rotater.transform)
                    {
                       ghostChild.gameObject.GetComponent<Renderer>().material.color = new Color(ghostMaterial.color.r, ghostMaterial.color.g, ghostMaterial.color.b, transperency);
                       ghostChild.GetComponent<BoxCollider>().enabled = false;
                       ghostChild.GetComponent<FallCheck>().ChildRender(true);
                       Debug.Log(ghostChild);
                    }
                    dropDownGhost.GetComponent<DropLogic>().enabled = false;
                    transform.position = homePos;
                    return;
                }
            }
            transform.position -= new Vector3(0, unit, 0);
        } while (!reachedBottom);
    }

}

