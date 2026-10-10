
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class S09_Shear : MonoBehaviour
{
    public float k = 1.4f;

    public static Matrix4x4 ShearMatrixRaw(float k)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix[0, 1] = k;
        return matrix;
    }

    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = Instantiate(filter.sharedMesh);
        mesh.name = "ShearedDiamond";

        Vector3[] vertices = mesh.vertices;
        Matrix4x4 shear = ShearMatrixRaw(k);

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = shear.MultiplyPoint3x4(vertices[i]);

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        filter.sharedMesh = mesh;

        Vector3 top = new Vector3(0.5f, 1f, 0.5f);
        Debug.Log($"k={k}, top={shear.MultiplyPoint3x4(top)}");
    }
}