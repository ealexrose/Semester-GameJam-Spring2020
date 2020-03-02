using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{

    public List<GameObject> barriers;

    public float checkRadius = 0.2f;
    public LayerMask blockMask;


    public int LineCheck(float _width, float _height, float _unit)
    {
        //transform.localPosition = Vector3.zero;
        //Loop on each row, and for each row loop on each column, check to see if there is a block at that location
        for (int i = 0; i < _height; i++)
        {
            transform.position = transform.position - (Vector3.up * _unit);
            int countedBlocks = 0;
            for (int j = 0; j < _width; j++)
            {
                transform.localPosition = transform.localPosition + (Vector3.right * _unit);
                if (!GroundCheck())
                {
                    Debug.DrawRay(transform.position, transform.up * 1f, Color.red, 3);
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.up * 1f, Color.green, 3);
                    countedBlocks += 1;
                }
            }

            transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
           // Debug.Log("Counted " + countedBlocks + " Blocks out of " + _width + " on row " + (_height - i));
            if (countedBlocks == _width)
            {
                int pass = i;
                LineDestroy(pass, _width, _unit);

            }
            //Debug.Log(i);
        }

        //Reset position
        transform.localPosition = Vector3.zero;

        return 0;
    }

    void LineDestroy(int _depth, float _width, float _unit)
    {

        float heightHold = transform.localPosition.y;
        Debug.Log("destroy row " + (_depth));
        //destroy line
        for (int j = 0; j < _width; j++)
        {
            transform.localPosition = transform.localPosition + (Vector3.right * _unit);
            Collider[] hitCollider = Physics.OverlapSphere(transform.position, checkRadius, blockMask);
            Destroy(hitCollider[0].gameObject);
            Debug.DrawRay(transform.position, transform.up * 1f, Color.black, 3);

        }
        foreach (GameObject barrier in barriers)
        {
            if(barrier!= null)
            {
                barrier.GetComponent<BarrierController>().AddLine(1);
            }
            
        }

        //move every line above downwards
        transform.localPosition = new Vector3(0, -_unit * (_depth), 0);
        Debug.DrawRay(transform.position, -transform.right * 1f, Color.magenta, 3);

        for (int k = 0; k <= _depth; k++)
        {
 
            for (int m = 0; m < _width; m++)
            {
                transform.localPosition = transform.localPosition + (Vector3.right * _unit);
                Collider[] hitCollider = Physics.OverlapSphere(transform.position, checkRadius, blockMask);
                if (hitCollider.Length > 0)
                {
                   hitCollider[0].transform.position -= Vector3.up * _unit;
                   Debug.DrawRay(transform.position, transform.right * 1f, Color.yellow, 3);
                }


            }
            transform.position = transform.position + (Vector3.up * _unit);
            transform.localPosition = new Vector3(0, transform.localPosition.y, 0);


        }
        Debug.Log(heightHold);
        transform.localPosition = new Vector3(0, heightHold, 0);
        Debug.DrawRay(transform.position, -transform.up * 1f, Color.blue, 3);

    }

    bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, checkRadius, blockMask);
    }
}
