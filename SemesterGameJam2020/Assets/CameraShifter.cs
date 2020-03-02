using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShifter : MonoBehaviour
{

    public Vector3 homePos;
    Quaternion homeAngles;
    public Vector3 positionOffset;
    public GameObject body;
    public PlayerMovement movementController;
    public MouseLook viewController;
    public float moveTime;
    float timeCount;
    bool zoomOutQueue;
    bool zoomInQueue;
    bool zoomOut;
    bool zoomIn;
    int cameraState;
    Vector3 targetVector;
    // Start is called before the first frame update
    void Start()
    {
        homePos = transform.localPosition;
        cameraState = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("zoomKey") && cameraState == 0)
        {
            zoomOutQueue = true;
        }
        if (Input.GetButtonUp("zoomKey"))
        {
            zoomInQueue = true;
        }

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
        if(cameraState == -2)
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
        if(cameraState == -1) {
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
    }
}
