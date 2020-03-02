using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public int clearTarget;
    public int clearCount;

    public void AddLine(int _addedLines)
    {
        clearCount += _addedLines;
        CheckStatus();
    } 

    public void SetClearCount(int _clearCount)
    {
        clearCount = _clearCount;
        CheckStatus();
    }

    void CheckStatus()
    {
        if (clearCount >= clearTarget)
        {
            Destroy(this.gameObject);
        }
        Debug.Log(clearCount + " out of " + clearTarget + " destroyed");
    }

}
