using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static class BuildS17Abs
{
    const string Root = "Assets/S17_Abs";
    const string ScenePath = Root + "/S17_Texture_Abs.unity";

    [MenuItem("S17 Abs/Build UV Texture Scene")]
    public static void Build()
    {
        AssetDatabase.Refresh();
        var shader = Shader.Find("CG/S17_MyTextureAbs");
        if (shader == null) throw new System.InvalidOperationException("CG/S17_MyTextureAbs shader did not compile.");
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/T_S17_Grid.png");
        if (texture == null) throw new System.InvalidOperationException("S17 grid texture is missing.");

        var materialPath = Root + "/Mat_S17_MyTextureAbs.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null) material = new Material(shader) { name = "Mat_S17_MyTextureAbs" };
        material.shader = shader;
        material.SetTexture("_BaseMap", texture);
        material.SetTextureScale("_BaseMap", Vector2.one);
        material.SetTextureOffset("_BaseMap", Vector2.zero);
        if (!AssetDatabase.Contains(material)) AssetDatabase.CreateAsset(material, materialPath);
        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientLight = new Color(.72f, .74f, .78f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        var cameraObject = new GameObject("Game Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(-.35f, 0f, -10f);
        cameraObject.transform.rotation = Quaternion.identity;
        var camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 2.8f;
        camera.nearClipPlane = .1f;
        camera.farClipPlane = 100f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.13f, .16f, .21f, 1f);
        camera.allowHDR = true;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.AddComponent<UniversalAdditionalCameraData>();

        var lightObject = new GameObject("Directional Light");
        lightObject.transform.rotation = Quaternion.Euler(40f, -30f, 0f);
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;

        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "Quad - UV quadrant remap";
        quad.transform.position = new Vector3(-2.1f, 0f, 0f);
        quad.transform.localScale = new Vector3(3.6f, 3.6f, 1f);
        quad.GetComponent<MeshRenderer>().sharedMaterial = material;

        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "Sphere - same material UV comparison";
        sphere.transform.position = new Vector3(2.05f, 0f, 0f);
        sphere.transform.localScale = Vector3.one * 2.05f;
        sphere.GetComponent<MeshRenderer>().sharedMaterial = material;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("S17_Texture_Abs scene built with Quad, Sphere and Mat_S17_MyTextureAbs.");
    }

    [MenuItem("S17 Abs/Capture Game View Screenshot")]
    public static void CaptureScreenshot()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var cameraObject = GameObject.Find("Game Camera");
        if (cameraObject == null) throw new System.InvalidOperationException("Game Camera is missing.");
        var camera = cameraObject.GetComponent<Camera>();
        var target = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = target;
        camera.Render();
        var old = RenderTexture.active;
        RenderTexture.active = target;
        var image = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
        image.Apply();
        File.WriteAllBytes("S17_Abs_GameView.png", image.EncodeToPNG());
        RenderTexture.active = old;
        camera.targetTexture = null;
        target.Release();
        Object.DestroyImmediate(target);
        Object.DestroyImmediate(image);
        Debug.Log("Wrote S17_Abs_GameView.png from the Game Camera.");
    }
}
