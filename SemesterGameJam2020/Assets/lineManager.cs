using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    /* Debug RayCast Lines
     * Red      - No Block Found
     * Green    - Block Found
     * Black    - Block Here designated for destruction
     * Yellow   - Block Here detected and moved Down
     * Blue     - Position where Line Check resumed after destruction *
     */

    //List of game objects that care about the amount of lines cleared
    public GameObject barrier;
    
    //The radius for which to check for each piece. Not the distance between points on the grid.
    public float checkRadius = 0.2f;
    //The Layer that the blocks are on.
    public LayerMask blockMask;

    //Check to see if there are completed lines of length _width starting from the current position and looping for intervals of _unity down to _unity*_height away.
    //For each line, checks a sphere with radius checkRadius for overlap with a tetornimo piece.

    public int LineCheck(float _width, float _height, float _unit)
    {
        //Loop on each row, and for each row loop on each column, check to see if there is a block at that location
        //For each row keeps a count of the amount of blocks in the row, if there are blocks equal to the width, call the line deletion function and hen resume.
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

            if (countedBlocks == _width)
            {
                int pass = i;
                LineDestroy(pass, _width, _unit);

            }
        }

        //Reset position
        transform.localPosition = Vector3.zero;

        //For future use with line clear effects
        return 0;
    }

    //Destroys the row of blocks at the current height, then for each row above move all blocks down one space.
    //Then return back to the location where this function was initially called
    void LineDestroy(int _depth, float _width, float _unit)
    {
        //Stores starting height
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

        //Add a line to each of the remaining barriers

         barrier.GetComponent<BarrierController>().AddLine(1);

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
        transform.localPosition = new Vector3(0, heightHold, 0);
        Debug.DrawRay(transform.position, -transform.up * 1f, Color.blue, 3);

    }

    //Shorthand Function to check if there is a block at hte lineManager's current location
    bool GroundCheck()
    {
        return Physics.CheckSphere(transform.position, checkRadius, blockMask);
    }
}
