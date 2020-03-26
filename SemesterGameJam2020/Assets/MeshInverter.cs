using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshInverter : MonoBehaviour
{
    public bool invertOnStart;

    void Start()
    {
        if (invertOnStart)
        {
            InsideOut();
        }
    }
    //Object to flip inside out
    public GameObject outside;
    public void InsideOut()
    {
        //Flip object inside out **ONLY WORKS ON SIMPLE GEOMETRY**
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        this.GetComponent<MeshRenderer>().material = outside.GetComponent<MeshRenderer>().material;
    }
}
