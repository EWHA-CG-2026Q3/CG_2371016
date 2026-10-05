using UnityEngine;

public class S08_DirectTransform : MonoBehaviour
{
    public enum DemoMode
    {
        TranslateThenScale,
        ScaleThenTranslate
    }

    public DemoMode demoMode = DemoMode.TranslateThenScale;

    Vector3[] baseVertices =
    {
        new Vector3(0f, 1f, 0f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 0f),
        new Vector3(0f, 0f, -1f),
        new Vector3(0f, -1f, 0f)
    };

    Vector3 translation = new Vector3(2f, 0f, 0f);
    Vector3 scale = new Vector3(2f, 1f, 1f);

    void Start()
{
    Vector3[] vertices;

    if (demoMode == DemoMode.TranslateThenScale)
    {
        vertices = TranslateThenScale(baseVertices);
    }
    else
    {
        vertices = ScaleThenTranslate(baseVertices);
    }

    int[] triangles =
    {
        0, 1, 2,
        0, 2, 3,
        0, 3, 4,
        0, 4, 1,
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

    GetComponent<MeshRenderer>().material =
        new Material(Shader.Find("Universal Render Pipeline/Lit"));
}

    Vector3[] ApplyTranslation(Vector3[] vertices, Vector3 t)
    {
        Vector3[] result = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            result[i] = vertices[i] + t;
        }

        return result;
    }

    Vector3[] ApplyScale(Vector3[] vertices, Vector3 s)
    {
        Vector3[] result = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            result[i] = new Vector3(
                vertices[i].x * s.x,
                vertices[i].y * s.y,
                vertices[i].z * s.z
            );
        }

        return result;
    }

    Vector3[] TranslateThenScale(Vector3[] vertices)
    {
        Vector3[] translated = ApplyTranslation(vertices, translation);
        return ApplyScale(translated, scale);
    }

    Vector3[] ScaleThenTranslate(Vector3[] vertices)
    {
        Vector3[] scaled = ApplyScale(vertices, scale);
        return ApplyTranslation(scaled, translation);
    }
}