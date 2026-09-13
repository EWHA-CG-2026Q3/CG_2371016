using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S04_PyramidMesh : MonoBehaviour
{
    void Start()
    {
        // 정점 5개
        // 0~3: 사각형 밑면의 네 모서리
        // 4: 피라미드 꼭짓점
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1f, 0f, -1f), // 0
            new Vector3( 1f, 0f, -1f), // 1
            new Vector3( 1f, 0f,  1f), // 2
            new Vector3(-1f, 0f,  1f), // 3
            new Vector3( 0f, 1.5f, 0f) // 4 꼭짓점
        };

        // 삼각형 6개
        // 밑면 2개 + 옆면 4개
        int[] triangles = new int[]
        {
            // 밑면: 삼각형 2개
            0, 1, 2,
            0, 2, 3,

            // 옆면: 삼각형 4개
            0, 4, 1,
            1, 4, 2,
            2, 4, 3,
            3, 4, 0
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