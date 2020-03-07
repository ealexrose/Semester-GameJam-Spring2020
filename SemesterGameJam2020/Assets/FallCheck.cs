using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCheck : MonoBehaviour
{
    public bool topCheck;
    public bool rightCheck;
    public bool downCheck;
    public bool leftCheck;
    public float baseRadius = 2.6f;
    public MeshRenderer childRender;
    public MeshInverter meshInverter;
    RaycastHit hitCheck;
    //Check all the given directions for ground, if ground is found return true
    bool GroundCast(Vector3 _dir)
    {
        if (Physics.Raycast(transform.position, _dir, out hitCheck, baseRadius))
        {
            if (hitCheck.transform.gameObject.layer == 8)
            {
               //Debug.Log(hitCheck.transform.gameObject.name + " " + _dir);
                return true;
            }
        }
        Debug.DrawRay(transform.position, _dir * baseRadius, Color.red);

        return false;
    }

    public bool Check()
    {
        if (topCheck)
        {
            if (GroundCast(transform.up))
            {
                return true;
            }
        }
        if (rightCheck)
        {
            if (GroundCast(transform.right))
            {
                return true;
            }
        }
        if (downCheck)
        {
            if (GroundCast(-transform.up))
            {
                return true;
            }
        }
        if (leftCheck)
        {
            if (GroundCast(-transform.right))
            {
                return true;
            }
        }
        return false;
    }
    public void ChildRender(bool renderMode)
    {
        childRender.enabled = renderMode;
        meshInverter.InsideOut();
    }
}
