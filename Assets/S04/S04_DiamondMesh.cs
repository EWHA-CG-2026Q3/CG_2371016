using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S04_DiamondMesh : MonoBehaviour
{
    void Start()
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 1f, 0f),    // 0: 위
            new Vector3(-1f, 0f, 0f),   // 1: 왼쪽
            new Vector3(0f, 0f, 1f),    // 2: 앞
            new Vector3(1f, 0f, 0f),    // 3: 오른쪽
            new Vector3(0f, 0f, -1f),   // 4: 뒤
            new Vector3(0f, -1f, 0f)    // 5: 아래
        };

        int[] triangles = new int[]
        {
            // 위쪽 4개
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,

            // 아래쪽 4개
            5, 2, 1,
            5, 3, 2,
            5, 4, 3,
            5, 1, 4
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