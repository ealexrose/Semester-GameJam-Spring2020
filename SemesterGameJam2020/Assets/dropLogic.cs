using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropLogic : MonoBehaviour
{

    /*===================================================================================================================
     * DropLogic 1.0.0 - moves the piece while in a falling state
     * 
     * Manages the drop and rotational control of falling pieces and should prevent any invalid attempted movement
     * 
     * =================================================================================================================*/

    //spawner is the creator of this piece, reference is used to let it know when it is time to drop another piece
    //rotater is the rotational pivot of the tetronimo, typically offset so that rotations align to the grid
    public GameObject spawner;
    public GameObject rotater;


    //fallSpeed is the speed at which the tetronimo falls, set by spawner
    //unit is the length of the cube in  unity units, makes math easier
    //width is the width of the field, used to not go out of bounds
    public float fallSpeed;
    public float unit;
    public float width;

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

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in rotater.transform)
        {
            if (child.GetComponent<FallCheck>().Check())
            {
                foreach (Transform children in rotater.transform)
                {
                    children.transform.gameObject.layer = 8;
                }
                spawner.GetComponent<pieceSpawner>().LineCheck();
                isFalling = false;
                spawner.GetComponent<pieceSpawner>().spawnFlag = true;
                transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y * 2) / 2, transform.position.z);
                Destroy(this);
            }
        }

        if (isFalling)
        {

            if (shiftCoolDownTimer == 0)
            {
                if (!isRotating)
                {
                    if (Input.GetButtonDown("LeftShift") && (ValidMovement(-(unit * (dimensions.w + 0.5f)))))
                    {
                        transform.position -= (Vector3.right * unit);
                        shiftCoolDownTimer = shiftCoolDown;
                        //Debug.DrawRay(rotater.transform.position, Vector3.left * dimensions.w * unit, Color.red, 3);
                    }
                    if (Input.GetButtonDown("RightShift") && (ValidMovement(unit * (dimensions.y + 0.5f))))
                    {
                        transform.position += (Vector3.right * unit);
                        shiftCoolDownTimer = shiftCoolDown;
                    }
                }

                if (Input.GetButtonDown("RotateCW") && !isRotating && ValidRotate(true))
                {
                    rotSpeed = rotSpeedBase;
                    rotCount = 0;
                    isRotating = true;
                    dimensions = new Vector4(dimensions.w, dimensions.x, dimensions.y, dimensions.z);
                }
            }
            else if(shiftCoolDownTimer >= 0 ){
                shiftCoolDownTimer -= 1;
            }

            if (isRotating)
            {
                rotater.transform.Rotate(new Vector3(0, 0, rotSpeed));
                rotCount += rotSpeed;
                if(rotCount >= 90)
                {
                    isRotating = false;
                }
            }

            if (!isRotating)
            {
                transform.position -= (fallSpeed * Vector3.up * Time.deltaTime);
            }
         }

        bool ValidMovement(float _xMov)
        {
            float newPos = rotater.transform.position.x + _xMov;
            Vector3 locCheck = (new Vector3(newPos, rotater.transform.position.y, 0));
            Debug.DrawRay(locCheck, Vector3.up * dimensions.x * unit, Color.green, 3);
            Debug.DrawRay(locCheck, Vector3.down * dimensions.z * unit, Color.green, 3);
            if ((newPos <= -(width * unit / 2)) || (newPos >= (width * unit / 2)))
            {
                return false;

            } else
                
            {
                foreach (Transform child in rotater.transform)
                {
                    if(Physics.CheckSphere(child.position + (Vector3.right * unit * Mathf.Sign(_xMov)), 1.5f, groundMask))
                    {
                        return false;
                    }
                }
            }


                return true;
        }

    }

    bool ValidRotate(bool _dir)
    {
        Vector3 futureRightBound = transform.position + (unit * Vector3.right * dimensions.x);
        Vector3 futureLeftBound = transform.position + (unit * Vector3.left * dimensions.z);
        if (_dir)
        {

            if ((futureLeftBound.x <= -(width * unit / 2)) || (futureRightBound.x >= (width * unit / 2)))
            {
                return false;
            }

        }
        return true;
    }
}
