
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S12_CustomMesh : MonoBehaviour
{
    public enum Shape { Cube, Diamond, Tetrahedron }
    public Shape shape = Shape.Cube;

    void Start()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices;
        int[] triangles;

        if (shape == Shape.Cube)
        {
            vertices = new Vector3[] {
                new(-0.5f,-0.5f,-0.5f), new(0.5f,-0.5f,-0.5f),
                new(0.5f,0.5f,-0.5f), new(-0.5f,0.5f,-0.5f),
                new(-0.5f,-0.5f,0.5f), new(0.5f,-0.5f,0.5f),
                new(0.5f,0.5f,0.5f), new(-0.5f,0.5f,0.5f)
            };
            triangles = new int[] {
                0,2,1, 0,3,2, 4,5,6, 4,6,7,
                0,1,5, 0,5,4, 3,7,6, 3,6,2,
                0,4,7, 0,7,3, 1,2,6, 1,6,5
            };
        }
        else if (shape == Shape.Diamond)
        {
            vertices = new Vector3[] {
                new(0,1,0), new(0.7f,0,0), new(0,0,0.7f),
                new(-0.7f,0,0), new(0,0,-0.7f), new(0,-1,0)
            };
            triangles = new int[] {
                0,2,1, 0,3,2, 0,4,3, 0,1,4,
                5,1,2, 5,2,3, 5,3,4, 5,4,1
            };
        }
        else
        {
            vertices = new Vector3[] {
                new(0,1,0), new(-0.8f,0,0.6f),
                new(0.8f,0,0.6f), new(0,0,-0.8f)
            };
            triangles = new int[] {
                0,1,2, 0,2,3, 0,3,1, 1,3,2
            };
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}
