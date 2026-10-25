
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class S11_GenerateDiamond : MonoBehaviour
{
    void Awake()
    {
        Mesh mesh = new Mesh();
        mesh.name = "GeneratedDiamond";

        // 위쪽 꼭짓점, 아래쪽 꼭짓점, 가운데 네 모서리
        Vector3[] vertices =
        {
            new Vector3(0,  1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0.7f, 0, 0),
            new Vector3(0, 0, 0.7f),
            new Vector3(-0.7f, 0, 0),
            new Vector3(0, 0, -0.7f)
        };

        int[] triangles =
        {
            0, 2, 3,  0, 3, 4,  0, 4, 5,  0, 5, 2,
            1, 3, 2,  1, 4, 3,  1, 5, 4,  1, 2, 5
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
