using UnityEngine;

/// <summary>Small edit-mode mesh generator used by the S14 scene's hand-built props.</summary>
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public sealed class S14_CustomMesh : MonoBehaviour
{
    public enum Shape { Crystal, PavilionRelief, GardenMarker }
    public Shape shape;

    void OnEnable() => Rebuild();
    void OnValidate() => Rebuild();

    void Rebuild()
    {
        var filter = GetComponent<MeshFilter>();
        if (filter == null) return;
        var mesh = new Mesh { name = "S14 " + shape };
        switch (shape)
        {
            case Shape.Crystal:
                mesh.vertices = new[] {
                    new Vector3(0,1,0), new Vector3(.52f,.28f,0), new Vector3(0,.28f,.52f),
                    new Vector3(-.52f,.28f,0), new Vector3(0,.28f,-.52f), new Vector3(0,-.75f,0)
                };
                mesh.triangles = new[] { 0,2,1, 0,3,2, 0,4,3, 0,1,4, 5,1,2, 5,2,3, 5,3,4, 5,4,1 };
                break;
            case Shape.PavilionRelief:
                mesh.vertices = new[] {
                    new Vector3(-.8f,0,-.35f),new Vector3(0,.7f,-.35f),new Vector3(.8f,0,-.35f),
                    new Vector3(-.8f,0,.35f),new Vector3(0,.7f,.35f),new Vector3(.8f,0,.35f),
                    new Vector3(0,.25f,-.35f),new Vector3(0,.25f,.35f)
                };
                mesh.triangles = new[] { 0,1,6, 1,2,6, 3,7,4, 7,5,4, 0,3,4, 0,4,1, 1,4,5, 1,5,2, 2,5,3, 2,3,0 };
                break;
            default:
                mesh.vertices = new[] {
                    new Vector3(-.5f,0,-.5f),new Vector3(.5f,0,-.5f),new Vector3(.5f,0,.5f),new Vector3(-.5f,0,.5f),
                    new Vector3(0,1.35f,0),new Vector3(0,.35f,0)
                };
                mesh.triangles = new[] { 0,1,5,1,2,5,2,3,5,3,0,5,0,4,1,1,4,2,2,4,3,3,4,0 };
                break;
        }
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        filter.sharedMesh = mesh;
    }
}
