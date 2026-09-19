using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S03_CustomPolygonMesh : MonoBehaviour
{
    void Start()
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 1f, 0f),
            new Vector3(-1f, 0.3f, 0f),
            new Vector3(-0.6f, -0.8f, 0f),
            new Vector3(0.6f, -0.8f, 0f),
            new Vector3(1f, 0.3f, 0f)
        };

       int[] triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2,
            0, 4, 3
        };

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().sharedMaterial =
            new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}