using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    //How fast the lava Rises
    public float riseSpeed;

    //Rise the lava up each frame by the rise speed
    void Update()
    {
        transform.localScale += Vector3.up * riseSpeed;
    }
}
