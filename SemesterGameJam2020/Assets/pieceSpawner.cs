using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pieceSpawner : MonoBehaviour
{
    public List<GameObject> tetronimos;
    public GameObject lineManager;

    public float unitSpace;
    public float fieldLength;
    public float fieldHeight;
    public float baseSpeed;


    public bool spawnFlag = true;

    // Update is called once per frame
    void Update()
    {
        if (spawnFlag)
        {
            spawnTetronimo();
            spawnFlag = false;
        }
    }

    void spawnTetronimo()
    {
        float spawnPos = unitSpace * (fieldLength / 2);

        GameObject spawnedPiece = Instantiate(tetronimos[(int)Mathf.Round(Random.Range(0, tetronimos.Count))], transform.position + Vector3.right*spawnPos, Quaternion.identity);
        spawnedPiece.GetComponent<dropLogic>().spawner = this.gameObject;
        spawnedPiece.GetComponent<dropLogic>().width = fieldLength;
        spawnedPiece.GetComponent<dropLogic>().fallSpeed = baseSpeed;
    }

    public void LineCheck()
    {
        lineManager.GetComponent<lineManager>().lineCheck(fieldLength, fieldHeight, unitSpace);
    }
}
