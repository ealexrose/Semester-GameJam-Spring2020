using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DropLogic : MonoBehaviour
{

    /*===================================================================================================================
     * DropLogic 1.1.0 - moves the piece while in a falling state
     * 
     * Manages the drop and rotational control of falling pieces and should prevent any invalid attempted movement
     * 
     * =================================================================================================================*/

    //spawner is the creator of this piece, reference is used to let it know when it is time to drop another piece
    //rotater is the rotational pivot of the tetronimo, typically offset so that rotations align to the grid
    public GameObject spawner;
    public GameObject rotater;
    public GameObject IconDisplay;
    public GameObject dropDownGhost;


    public Material ghostMaterial;
    //fallSpeed is the speed at which the tetronimo falls, set by spawner
    //unit is the length of the cube in  unity units, makes math easier
    //width is the width of the field, used to not go out of bounds
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

        if (isFalling)
        {

            if (shiftCoolDownTimer == 0)
            {
                if (!isRotating)
                {
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

                if (Input.GetButtonDown("RotateCW") && !isRotating && ValidRotate(true))
                {
                    iconControls.RotateMe(rotSpeedBase);
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
                    Destroy(dropDownGhost);
                    DropDownSearch();
                    isRotating = false;
                }
            }

            if (Input.GetButton("fastFall"))
            {
                fastSpeed = fastFallSpeed;
            }
            else
            {
                fastSpeed = 0;
            }
            if (!isRotating)
            {
                transform.position -= ((fallSpeed + fastSpeed) * Vector3.up * Time.deltaTime);
            }
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
    void DropDownSearch()
    {
        Vector3 homePos = transform.position;
        bool reachedBottom = false;
        do
        {
            foreach (Transform child in rotater.transform)
            {
                Collider[] obstacles = Physics.OverlapBox(child.transform.position, new Vector3((unit / 2) - .01f, (unit / 2) - .01f, (unit / 2) - .01f), child.rotation, groundMask);
                if (obstacles.Length > 0)
                {
                    reachedBottom = true;
                    dropDownGhost = Instantiate(this.gameObject, new Vector3(transform.position.x, Mathf.Ceil(transform.position.y / 5f) * 5f,transform.position.z), Quaternion.identity);
                    foreach (Transform ghostChild in dropDownGhost.GetComponentInChildren<DropLogic>().rotater.transform)
                    {
                       ghostChild.gameObject.GetComponent<Renderer>().material.color = new Color(ghostMaterial.color.r, ghostMaterial.color.g, ghostMaterial.color.b, transperency);
                       ghostChild.GetComponent<BoxCollider>().enabled = false;
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

