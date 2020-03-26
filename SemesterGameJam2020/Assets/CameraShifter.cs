using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShifter : MonoBehaviour
{
    // homePos is the position of the camera from the first person point of view
    // positionOffset is the relative position from which the third person camera is located form the play field.
    //  The X and Z are used as global positions, the Y position is the player height or the offset height, whichever is higher
    public Vector3 homePos;
    public Vector3 positionOffset;

    //The body the camera is attached to in first person, used to interpret homePos consistantly in case the body moves during the duration of the camera change
    public GameObject body;

    //Scripts for movement and camera control so that relevant functions can be freezed when they shouldn't be available
    public PlayerMovement movementController;
    public MouseLook viewController;

    //The time taken to transition between views
    public float moveTime;
    
    //The Angle the camera was at before moving to the third person view
    Quaternion homeAngles;
    
    //time passed during current transition
    float timeCount;

    //Boolean value showing if a camera transition has been queued to happen
    public bool zoomOutQueue;
    public bool zoomInQueue;
    
    //State of camera
    public bool zoomOut;
    public bool zoomIn;
    public int cameraState;

    //vector calculated using current position and offset
    Vector3 targetVector;

    // Start is called before the first frame update
    void Start()
    {
        //Store default offset and set base camera state
        homePos = transform.localPosition;
        cameraState = 0;

    }

    /* Camera State
     * 1  - Currently Zoomed out, check to see if readt to zoom in
     * 0  - Currently Zoomed in, check to see if ready to zoom out
     * -1 - Currently Zooming Out, continue until complete
     * -2 - Currently Zooming in, continue until complete
     */
     //Camera movement is done by lerping between the start position and the calculated target vector over the amount of time


    // Update is called once per frame
    void Update()
    {
        //If the zoom button is pushed or released at an appropriate point queue either a zoom in or out
        if (Input.GetButtonDown("zoomKey") && cameraState == 0)
        {
            zoomOutQueue = true;
        }
        if (Input.GetButtonUp("zoomKey") && (cameraState != -2))
        {
            zoomInQueue = true;
        }

        //Boolean State Machine
        if (cameraState == 1)
        {
            if (zoomInQueue)
            {
                timeCount = 0;
                zoomIn = true;
                zoomInQueue = false;
                cameraState = -2;
            }
        }

        if (cameraState == 0)
        {
            if (zoomOutQueue)
            {
                homePos = transform.localPosition;
                timeCount = 0;
                homeAngles = transform.rotation;
                targetVector = new Vector3(positionOffset.x, Mathf.Max(transform.position.y, positionOffset.y), positionOffset.z);
                zoomOutQueue = false;
                zoomOut = true;
                cameraState = -1;
            }
        }

        if (cameraState == -1)
        {
            if (zoomOut)
            {
                movementController.frozen = true;
                viewController.enabled = false;
                transform.position = Vector3.Slerp(transform.TransformPoint(homePos), targetVector, timeCount / moveTime);
                timeCount++;
                transform.rotation = Quaternion.Slerp(homeAngles, Quaternion.identity, timeCount / moveTime);
                if (timeCount >= moveTime)
                {
                    zoomOut = false;
                    cameraState = 1;
                }
            }
        }

        if (cameraState == -2)
        {
            if (zoomIn)
            {
                timeCount++;
                transform.position = Vector3.Slerp(targetVector, body.transform.position + homePos, timeCount / moveTime);           
                transform.rotation = Quaternion.Slerp(Quaternion.identity, homeAngles, timeCount / moveTime);
                Debug.Log(body.transform.position);
                if (timeCount >= moveTime)
                {
                    zoomIn = false;
                    movementController.frozen = false;
                    viewController.enabled = true;
                    cameraState = 0;
                }
            }
        }

    }
}
