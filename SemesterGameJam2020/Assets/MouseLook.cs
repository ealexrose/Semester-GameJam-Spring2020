using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //Speed multiplier for mosue looking around
    public float mouseSensitivity = 100f;
    //Body the ccamera is attached to
    public Transform playerBody;
    //Initial Rotation
    float xRotation = 0f;



    // Start is called before the first frame update
    void Start()
    {
        //Set cursor to be invisible and lock it to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Change in mouse movement between time checks
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        //Calculate vertical rotation(clamped between straight up and straight down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //vertical rotation
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //horizontal rotation done via body
        playerBody.Rotate(Vector3.up * mouseX);


        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }
}
