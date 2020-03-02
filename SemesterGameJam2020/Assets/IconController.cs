using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    public GameObject target;
    public GameObject currentIcon;
    float rotSpeed;
    float rotCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotSpeed));
        rotCount += rotSpeed;
        if (rotCount >= 90)
        {
            rotSpeed = 0;
            rotCount = 0;
        }

    }
    public void Reset()
    {
        Destroy(currentIcon);
        transform.localEulerAngles = Vector3.zero;
    }

    public void RotateMe(float _rotSpeed)
    {
        rotSpeed = _rotSpeed;
    }
    public void SetIcon(GameObject _iconToSet)
    {
        currentIcon = Instantiate(_iconToSet, target.transform);
        currentIcon.transform.localPosition = Vector3.zero;
        currentIcon.GetComponent<DropLogic>().enabled = false;
    }
}
