using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public List<GameObject> tetronimos;
    public GameObject lineManager;
    public GameObject CurrentPieceIcon;

    public float unit;
    public float fieldLength;
    public float fieldHeight;
    public float baseSpeed;


    public bool spawnFlag = true;

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

    void SpawnTetronimo()
    {
        float spawnPos = unit * (fieldLength / 2);

        GameObject spawnedPiece = Instantiate(tetronimos[(int)Mathf.Round(Random.Range(-.49f, tetronimos.Count + 0.3f))], transform.position + Vector3.right*spawnPos, Quaternion.identity);
        spawnedPiece.GetComponent<DropLogic>().spawner = this.gameObject;
        spawnedPiece.GetComponent<DropLogic>().IconDisplay = CurrentPieceIcon;
        spawnedPiece.GetComponent<DropLogic>().width = fieldLength;
        spawnedPiece.GetComponent<DropLogic>().fallSpeed = baseSpeed;
        spawnedPiece.GetComponent<DropLogic>().fastFallSpeed = baseSpeed*4;
        spawnedPiece.GetComponent<DropLogic>().transperency = 0.2f;

    }

    public void LineCheck()
    {
        lineManager.GetComponent<LineManager>().LineCheck(fieldLength, fieldHeight, unit);
    }
}
