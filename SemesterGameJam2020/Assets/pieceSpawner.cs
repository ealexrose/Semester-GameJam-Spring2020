using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    //Library of pieces that can be spawned
    public List<GameObject> tetronimos;
    
    //References to the Object that checks the grid for completed lines and second camera derivative that functions as a UI overlay
    public GameObject lineManager;
    public GameObject CurrentPieceIcon;

    //Standard length of block
    public float unit;
    //Length of play space in block units
    public float fieldLength;
    //Height of play space in block units
    public float fieldHeight;
    //Base Speed that pieces falla t
    public float baseSpeed;
    public float transperency;

    //Whether the next piece is readty to be spawned
    public bool spawnFlag = true;

    int[] idArray;
    int currentID;
    void Start()
    {
        idArray = InitIDArray(tetronimos.Count);
        idArray = ShuffleOrder(idArray);
        currentID = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (spawnFlag)
        {
            LineCheck();
            SpawnTetronimo();
            spawnFlag = false;
        }

    }
    //Spawns a tetonimo at a point determined to be half the field away
    void SpawnTetronimo()
    {
        //determine halfway point
        float spawnPos = unit * (fieldLength / 2);

        //Instantiate Next Piece in order
        GameObject spawnedPiece = Instantiate(tetronimos[idArray[currentID]], transform.position + Vector3.right*spawnPos, Quaternion.identity);

        //Move up in order, if order is complete, shuffle array and move to the beggining
        currentID++;
        if (currentID == idArray.Length)
        {
            currentID = 0;
            idArray = ShuffleOrder(idArray);
        }
        //Assign Spawned piece values
        spawnedPiece.GetComponent<DropLogic>().spawner = this.gameObject;
        spawnedPiece.GetComponent<DropLogic>().IconDisplay = CurrentPieceIcon;
        spawnedPiece.GetComponent<DropLogic>().width = fieldLength;
        spawnedPiece.GetComponent<DropLogic>().fallSpeed = baseSpeed;
        spawnedPiece.GetComponent<DropLogic>().fastFallSpeed = baseSpeed*4;
        spawnedPiece.GetComponent<DropLogic>().transperency = transperency;

    }

    //Extends the Line Check that will be requested by a dropped piece sinc ethe dropped piece is only aware of this script
    public void LineCheck()
    {
        lineManager.GetComponent<LineManager>().LineCheck(fieldLength, fieldHeight, unit);
    }

    //Create a sequential integer array from 0 to _size
    public int[] InitIDArray(int _size)
    {
        int[] newIDArray = new int[_size];
        for (int i = 0; i < _size; i++)
        {
            newIDArray[i] = i;
        }
        return newIDArray;
    }

    //Takes an integer array, creates an array of equal size filled with -1's, these demarkate unfilled values, then goes in order of values from original array and attempts
    //To place it in the new array, if the array position it trys to place at is filled it trys the next position in the array and repeats until it finds an empty spot
    //Then returns that new array
    public int[] ShuffleOrder(int[] _listToShuffle)
    {
        int listSize = _listToShuffle.Length;
        int[] shuffledList = new int[_listToShuffle.Length];
        for (int i = 0; i < listSize; i++)
        {
            shuffledList[i] = -1;
        }

        for (int i = 0; i < listSize; i++)
        {
            bool placeFound = false;
            int firstPlace = (int)Random.Range(0, listSize-1);
            do
            {
                if (shuffledList[firstPlace] == -1)
                {
                    shuffledList[firstPlace] = _listToShuffle[i];
                    placeFound = true;
                }
                else if (firstPlace >= listSize-1)
                {
                    firstPlace = 0;
                }
                else
                {
                    firstPlace++;
                }
            } while (placeFound == false);
        }

        for (int i = 0; i < listSize; i++)
        {
            Debug.Log(shuffledList[i]);
        }

        return shuffledList;
    }
}
