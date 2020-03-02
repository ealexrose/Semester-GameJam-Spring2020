using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public float riseSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.localScale += Vector3.up * riseSpeed;
    }
}
