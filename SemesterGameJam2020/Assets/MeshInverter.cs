using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshInverter : MonoBehaviour
{
    public GameObject outside;
    public void InsideOut()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
        this.GetComponent<MeshRenderer>().material = outside.GetComponent<MeshRenderer>().material;
    }
}
