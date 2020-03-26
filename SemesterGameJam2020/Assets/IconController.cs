using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    //Point that the icon will get parented to
    public GameObject target;
    //Currently held icon
    public GameObject currentIcon;

    //Speed at which to rotate object to keep accurate display of rotating tetromino
    float rotSpeed;
    float rotCount;

    // Update is called once per frame
    void Update()
    {
        //Rotation happens every turn, but values only increase if rotSpeed is greater than 0.
        //If the counted value of the cumultive rotation(rotCount) exceeds 90 then stop rotating and reset movement values
        transform.Rotate(new Vector3(0, 0, rotSpeed));
        rotCount += rotSpeed;
        if (rotCount >= 90)
        {
            rotSpeed = 0;
            rotCount = 0;
        }
    }

    //Prepare for next icon by destroying current one and resetting the rotation
    public void Reset()
    {
        Destroy(currentIcon);
        transform.localEulerAngles = Vector3.zero;
    }
    //Start rotation of an object, when rotSpeed is changed to a value greater than 0
    public void RotateMe(float _rotSpeed)
    {
        rotSpeed = _rotSpeed;
    }
    //Create a new icon as a copy of a passed gameobject, disable the logic script on the attached gameobject so that it can be controlled by this script
    public void SetIcon(GameObject _iconToSet)
    {
        currentIcon = Instantiate(_iconToSet, target.transform);
        currentIcon.transform.localPosition = Vector3.zero;
        currentIcon.GetComponent<DropLogic>().enabled = false;
    }
}
