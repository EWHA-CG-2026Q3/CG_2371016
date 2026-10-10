
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S09_DiamondMesh : MonoBehaviour
{
    void Awake()
    {
        Vector3[] vertices =
        {
            new Vector3(0f, 1f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 0f, -1f),
            new Vector3(0f, -1f, 0f)
        };

        int[] triangles =
        {
            0, 2, 1, 0, 3, 2,
            0, 4, 3, 0, 1, 4,
            5, 1, 2, 5, 2, 3,
            5, 3, 4, 5, 4, 1
        };

        Mesh mesh = new Mesh();
        mesh.name = "S09_DiamondMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;

        var renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = new Material(
            Shader.Find("Universal Render Pipeline/Lit")
        );
    }
}