using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static class BuildS15Scene
{
    const string Root = "Assets/S15";
    const string ScenePath = Root + "/S15_Landscape.unity";

    [MenuItem("S15/Build Flow Landscape")]
    public static void Build()
    {
        Directory.CreateDirectory(Root);
        if (!AssetDatabase.LoadAssetAtPath<Object>(Root + "/S15_SG_Flow.shadergraph"))
            AssetDatabase.CopyAsset("Assets/ThirdParty/S13_Pavilion/S13_SG_Flow.shadergraph", Root + "/S15_SG_Flow.shadergraph");
        if (!AssetDatabase.LoadAssetAtPath<Material>(Root + "/Mat_S15_Smoke.mat"))
            AssetDatabase.CopyAsset("Assets/ThirdParty/S13_Pavilion/Mat_S13_Smoke.mat", Root + "/Mat_S15_Smoke.mat");
        AssetDatabase.Refresh();

        var flowTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/T_S15_Flow.png");
        var smokeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/T_S15_Smoke.png");
        if (flowTexture == null || smokeTexture == null)
            throw new System.InvalidOperationException("S15 flow and smoke textures must be created before building the scene.");

        var graphAssets = AssetDatabase.LoadAllAssetsAtPath(Root + "/S15_SG_Flow.shadergraph");
        var flowShader = graphAssets.OfType<Shader>().FirstOrDefault();
        if (flowShader == null) flowShader = Shader.Find("Shader Graphs/S15_SG_Flow");
        if (flowShader == null) flowShader = Shader.Find("Shader Graphs/S13_SG_Flow");
        if (flowShader == null) flowShader = Shader.Find("S13_SG_Flow");
        if (flowShader == null)
            throw new System.InvalidOperationException("S15 flow shader asset did not import as a Shader. Imported assets: " + string.Join(", ", graphAssets.Select(a => a.name + " (" + a.GetType().Name + ")")));
        var flowMatPath = Root + "/Mat_S15_SG_Flow.mat";
        var flowMat = AssetDatabase.LoadAssetAtPath<Material>(flowMatPath);
        if (flowMat == null) flowMat = new Material(flowShader) { name = "Mat_S15_SG_Flow" };
        flowMat.shader = flowShader;
        flowMat.SetTexture("_MainTex1", flowTexture);
        flowMat.SetTexture("_MainTex2", flowTexture);
        flowMat.SetTexture("_Texture1", flowTexture);
        flowMat.SetTexture("_Texture2", flowTexture);
        flowMat.SetVector("_Flow1", new Vector4(.075f, .018f, 0f, 0f));
        flowMat.SetVector("_Flow2", new Vector4(-.023f, .045f, 0f, 0f));
        flowMat.SetVector("_Tiling", new Vector4(2.2f, 1.25f, 0f, 0f));
        flowMat.SetFloat("_Blend", .5f);
        if (!AssetDatabase.Contains(flowMat)) AssetDatabase.CreateAsset(flowMat, flowMatPath);
        EditorUtility.SetDirty(flowMat);

        var smokeMat = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Mat_S15_Smoke.mat");
        smokeMat.SetTexture("_BaseMap", smokeTexture);
        smokeMat.SetColor("_BaseColor", new Color(.58f, .66f, .70f, .52f));
        smokeMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        EditorUtility.SetDirty(smokeMat);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var scene = EditorSceneManager.OpenScene("Assets/S14/S14_Landscape.unity", OpenSceneMode.Single);
        var stone = AssetDatabase.LoadAssetAtPath<Material>("Assets/S14/Materials/M_Stone.mat");
        var water = GameObject.CreatePrimitive(PrimitiveType.Quad);
        water.name = "Flowing water channel - S15_SG_Flow";
        water.transform.SetPositionAndRotation(new Vector3(.3f, .62f, -9.0f), Quaternion.Euler(90f, 0f, 0f));
        water.transform.localScale = new Vector3(1.3f, 3.0f, 1f);
        water.GetComponent<MeshRenderer>().sharedMaterial = flowMat;
        var waterCollider = water.GetComponent<Collider>(); if (waterCollider) Object.DestroyImmediate(waterCollider);
        var waterSurface = GameObject.CreatePrimitive(PrimitiveType.Quad);
        waterSurface.name = "Flow texture surface preview";
        waterSurface.transform.SetPositionAndRotation(new Vector3(.3f, .64f, -9.0f), Quaternion.Euler(90f, 0f, 0f));
        waterSurface.transform.localScale = new Vector3(1.26f, 2.96f, 1f);
        var visibleWater = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Mat_S15_Flow_Surface.mat");
        if (visibleWater == null) visibleWater = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "Mat_S15_Flow_Surface" };
        visibleWater.SetTexture("_BaseMap", flowTexture); visibleWater.SetColor("_BaseColor", Color.white);
        visibleWater.SetFloat("_Smoothness", .82f); visibleWater.SetFloat("_Metallic", .18f);
        if (!AssetDatabase.Contains(visibleWater)) AssetDatabase.CreateAsset(visibleWater, Root + "/Mat_S15_Flow_Surface.mat");
        waterSurface.GetComponent<MeshRenderer>().sharedMaterial = visibleWater;
        var waterSurfaceCollider = waterSurface.GetComponent<Collider>(); if (waterSurfaceCollider) Object.DestroyImmediate(waterSurfaceCollider);
        Cube("Channel stone edge left", new Vector3(-.42f, .50f, -9.0f), new Vector3(.14f,.10f,3.2f), stone);
        Cube("Channel stone edge right", new Vector3(1.02f, .50f, -9.0f), new Vector3(.14f,.10f,3.2f), stone);
        Cube("Channel stone edge near", new Vector3(.3f, .50f, -10.52f), new Vector3(1.58f,.10f,.14f), stone);
        Cube("Channel stone edge far", new Vector3(.3f, .50f, -7.48f), new Vector3(1.58f,.10f,.14f), stone);

        var bowl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bowl.name = "Smoke brazier";
        bowl.transform.position = new Vector3(-3.25f, .30f, -3.1f);
        bowl.transform.localScale = new Vector3(.82f,.18f,.82f);
        bowl.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/S14/Materials/M_Bronze.mat");
        var plume = new GameObject("Smoke plume - S15");
        plume.transform.position = new Vector3(-3.25f, .45f, -3.1f);
        var ps = plume.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 7f; main.loop = true; main.prewarm = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(4.8f, 7.0f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(.62f, 1.05f);
        main.startSize = new ParticleSystem.MinMaxCurve(.40f, .74f);
        main.startColor = new Color(.62f,.69f,.72f,.46f);
        main.maxParticles = 180; main.gravityModifier = -.012f; main.simulationSpeed = 1f;
        var emission = ps.emission; emission.rateOverTime = 15f;
        var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 11f; shape.radius = .16f; shape.length = .18f;
        var colorLife = ps.colorOverLifetime; colorLife.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(new[] { new GradientColorKey(new Color(.42f,.48f,.51f),0f), new GradientColorKey(new Color(.72f,.77f,.78f),1f) },
                         new[] { new GradientAlphaKey(0f,0f), new GradientAlphaKey(.38f,.12f), new GradientAlphaKey(.23f,.72f), new GradientAlphaKey(0f,1f) });
        colorLife.color = gradient;
        var sizeLife = ps.sizeOverLifetime; sizeLife.enabled = true;
        sizeLife.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f,.52f), new Keyframe(1f,1.8f)));
        var renderer = plume.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        var smokeShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        var visibleSmoke = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Mat_S15_Smoke_Particles.mat");
        if (visibleSmoke == null) visibleSmoke = new Material(smokeShader) { name = "Mat_S15_Smoke_Particles" };
        visibleSmoke.SetTexture("_BaseMap", smokeTexture); visibleSmoke.SetColor("_BaseColor", new Color(.72f,.78f,.8f,.58f));
        visibleSmoke.SetFloat("_Surface", 1f); visibleSmoke.SetFloat("_Blend", 0f); visibleSmoke.SetFloat("_SrcBlend", 5f);
        visibleSmoke.SetFloat("_DstBlend", 10f); visibleSmoke.SetFloat("_ZWrite", 0f); visibleSmoke.renderQueue = 3000;
        visibleSmoke.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); visibleSmoke.SetOverrideTag("RenderType", "Transparent");
        if (!AssetDatabase.Contains(visibleSmoke)) AssetDatabase.CreateAsset(visibleSmoke, Root + "/Mat_S15_Smoke_Particles.mat");
        renderer.material = visibleSmoke;
        renderer.sortMode = ParticleSystemSortMode.OldestInFront;
        var plumeMaterial = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Mat_S15_Smoke_Puffs.mat");
        if (plumeMaterial == null) plumeMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "Mat_S15_Smoke_Puffs" };
        plumeMaterial.SetColor("_BaseColor", new Color(.54f,.61f,.64f,.24f));
        plumeMaterial.SetFloat("_Surface",1f); plumeMaterial.SetFloat("_Blend",0f); plumeMaterial.SetFloat("_SrcBlend",5f);
        plumeMaterial.SetFloat("_DstBlend",10f); plumeMaterial.SetFloat("_ZWrite",0f); plumeMaterial.renderQueue=3000;
        plumeMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); plumeMaterial.SetOverrideTag("RenderType","Transparent");
        if (!AssetDatabase.Contains(plumeMaterial)) AssetDatabase.CreateAsset(plumeMaterial, Root + "/Mat_S15_Smoke_Puffs.mat");
        for (int i=0;i<6;i++)
        {
            var puff=GameObject.CreatePrimitive(PrimitiveType.Sphere); puff.name="Smoke puff "+(i+1);
            puff.transform.position=new Vector3(.15f + Mathf.Sin(i*1.7f)*.18f,.85f+i*.42f,-5.2f);
            float size=.46f+i*.09f; puff.transform.localScale=new Vector3(size,size*1.12f,size);
            puff.GetComponent<MeshRenderer>().sharedMaterial=plumeMaterial;
            var puffCollider=puff.GetComponent<Collider>(); if(puffCollider) Object.DestroyImmediate(puffCollider);
        }

        var walker = GameObject.Find("SceneWalker - start outside");
        if (walker != null)
        {
            walker.transform.SetPositionAndRotation(new Vector3(0f,1f,-11.5f), Quaternion.identity);
            var playerCamera = walker.GetComponentInChildren<Camera>();
            if (playerCamera) { playerCamera.fieldOfView = 72f; playerCamera.clearFlags = CameraClearFlags.Skybox; }
        }
        var composition = GameObject.Find("Composition Camera - still capture");
        if (composition != null)
        {
            composition.transform.position = new Vector3(-10.5f,10.5f,-14.8f);
            composition.transform.rotation = Quaternion.LookRotation(new Vector3(-.6f,2.2f,-2.7f) - composition.transform.position);
            var cam = composition.GetComponent<Camera>(); cam.fieldOfView = 46f;
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        Debug.Log("S15_Landscape built with the copied flow graph, side-channel water and smoke plume.");
    }

    [MenuItem("S15/Capture Composition Screenshot")]
    public static void CaptureScreenshot()
    {
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var composition = GameObject.Find("Composition Camera - still capture");
        var cam = composition.GetComponent<Camera>();
        var smoke = GameObject.Find("Smoke plume - S15").GetComponent<ParticleSystem>();
        smoke.Play(); smoke.Simulate(3.5f, true, true, false);
        var rt = new RenderTexture(1920,1080,24,RenderTextureFormat.ARGB32);
        cam.targetTexture = rt; cam.Render();
        var previous = RenderTexture.active; RenderTexture.active = rt;
        var image = new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);
        image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0); image.Apply();
        File.WriteAllBytes("S15_Composition.png",image.EncodeToPNG());
        RenderTexture.active = previous; cam.targetTexture = null; rt.Release();
        Object.DestroyImmediate(rt); Object.DestroyImmediate(image);
        Debug.Log("Wrote S15_Composition.png from the composition camera.");
    }

    [MenuItem("S15/Capture SceneWalker Walkthrough Frames")]
    public static void CaptureWalkthroughFrames()
    {
        const int width=960,height=540,fps=12,seconds=30;
        const string frameDir="/tmp/S15_WalkthroughFrames";
        if(Directory.Exists(frameDir)) Directory.Delete(frameDir,true);
        Directory.CreateDirectory(frameDir);
        EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        var walker=GameObject.Find("SceneWalker - start outside");
        var cam=walker.GetComponentInChildren<Camera>(); cam.enabled=false;
        var smoke=GameObject.Find("Smoke plume - S15").GetComponent<ParticleSystem>();
        var rt=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32); cam.targetTexture=rt;
        var previous=RenderTexture.active; var image=new Texture2D(width,height,TextureFormat.RGB24,false);
        for(int i=0;i<fps*seconds;i++)
        {
            float t=(float)i/fps, walk=Mathf.Clamp01(t/22f);
            float x=Mathf.Lerp(0f,1.2f,Mathf.SmoothStep(0f,1f,walk));
            walker.transform.SetPositionAndRotation(new Vector3(x,1f,Mathf.Lerp(-12f,-1.5f,walk)),Quaternion.identity);
            smoke.Play(); smoke.Simulate(t,true,true,false);
            cam.Render(); RenderTexture.active=rt;
            image.ReadPixels(new Rect(0,0,width,height),0,0,false); image.Apply(false,false);
            File.WriteAllBytes(Path.Combine(frameDir,"frame"+i.ToString("D4")+".jpg"),image.EncodeToJPG(78));
        }
        RenderTexture.active=previous; cam.targetTexture=null; rt.Release();
        Object.DestroyImmediate(rt); Object.DestroyImmediate(image);
        Debug.Log("Rendered "+fps*seconds+" S15 SceneWalker frames to "+frameDir+".");
    }

    static GameObject Cube(string n,Vector3 p,Vector3 s,Material m)
    {
        var o=GameObject.CreatePrimitive(PrimitiveType.Cube); o.name=n; o.transform.position=p; o.transform.localScale=s;
        o.GetComponent<MeshRenderer>().sharedMaterial=m; return o;
    }
}
