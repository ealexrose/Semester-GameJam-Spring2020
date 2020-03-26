using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    //The amount of lines needed to clear this barrier
    public List<int> clearTarget;
    public List<int> spaceClaimed;
    //The amount of lines that have currently been cleared. Fed to by the Line Manager
    public int clearCount;
    public int clearIndex;
    //Increase the amount of cleared lines by some number
    public void AddLine(int _addedLines)
    {
        clearIndex = 0;
        clearCount += _addedLines;
        CheckStatus();
    } 

    //Set the clear count to some number 
    public void SetClearCount(int _clearCount)
    {
        clearCount = _clearCount;
        CheckStatus();
    }

    //Checks the clear count against the targetted number, if it's greater then destroy the barrier.
    void CheckStatus()
    {
        if (clearCount >= clearTarget[clearIndex])
        {
            transform.localScale -= spaceClaimed[clearIndex] * Vector3.up;
            clearIndex++;
        }
        //Debug tool to keep track of the amount of lines cleared during play.
        //Debug.Log(clearCount + " out of " + clearTarget + " destroyed");
    }

}
