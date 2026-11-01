using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static class BuildS14Scene
{
    const string Root = "Assets/S14";
    const string TexDir = Root + "/Textures";
    const string MatDir = Root + "/Materials";
    const string ScenePath = Root + "/S14_Landscape.unity";

    [MenuItem("S14/Build Landscape Scene")]
    public static void Build()
    {
        EnsureDirs();
        MakeTexture("WoodGrain", new Color32(100, 48, 25, 255), new Color32(176, 104, 53, 255), true);
        MakeTexture("StonePaving", new Color32(66, 83, 85, 255), new Color32(143, 151, 139, 255), false);
        MakeTexture("BrushedBronze", new Color32(83, 65, 41, 255), new Color32(190, 147, 76, 255), true);
        AssetDatabase.Refresh();

        var wood = MakeMaterial("M_Wood", "WoodGrain", new Color(.84f,.76f,.63f), .05f, .28f);
        var stone = MakeMaterial("M_Stone", "StonePaving", Color.white, .12f, .34f);
        var bronze = MakeMaterial("M_Bronze", "BrushedBronze", Color.white, .78f, .72f);
        var jade = MakeMaterial("M_Jade", null, new Color(.03f,.32f,.24f), .58f, .68f);
        var lantern = MakeMaterial("M_LanternGlow", null, new Color(.1f,.5f,.9f), .15f, .48f);
        lantern.EnableKeyword("_EMISSION");
        lantern.SetColor("_EmissionColor", new Color(1.4f, 3.2f, 5.5f, 1));
        EditorUtility.SetDirty(lantern);
        var dark = MakeMaterial("M_DarkWood", null, new Color(.16f,.075f,.035f), .08f, .25f);
        var foliage = MakeMaterial("M_Foliage", null, new Color(.075f,.23f,.14f), 0, .24f);

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(.47f,.57f,.69f);
        RenderSettings.ambientEquatorColor = new Color(.48f,.50f,.42f);
        RenderSettings.ambientGroundColor = new Color(.19f,.21f,.18f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(.63f,.72f,.72f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = .008f;

        var sun = new GameObject("Sun - late afternoon");
        sun.transform.rotation = Quaternion.Euler(48, -32, 0);
        var light = sun.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.35f;
        light.color = new Color(1f,.84f,.62f); light.shadows = LightShadows.Soft;

        // A broad stone platform and an inviting stepping-stone approach.
        Cube("Garden foundation", new Vector3(0,-.24f,0), new Vector3(12,.48f,14), stone);
        for (int i=0;i<8;i++)
            Cube("Approach stone " + (i+1), new Vector3(0,-.005f,-11.5f+i*1.15f), new Vector3(2.35f,.12f,.72f), stone);
        Cube("Approach garden", new Vector3(0,-.32f,-12), new Vector3(12,.45f,5), foliage);

        // Open-sided pavilion: raised deck, carved timber posts, layered eaves and ridge.
        Cube("Pavilion deck", new Vector3(0,.08f,0), new Vector3(8.4f,.30f,6.4f), wood);
        Cube("Deck edge", new Vector3(0,-.03f,0), new Vector3(8.8f,.18f,6.8f), dark);
        float[] xs={-3.35f,3.35f}, zs={-2.35f,2.35f};
        foreach(float x in xs) foreach(float z in zs) {
            Cylinder("Carved cedar pillar", new Vector3(x,1.68f,z), new Vector3(.38f,1.525f,.38f), wood, 12);
            Cube("Pillar foot", new Vector3(x,.52f,z), new Vector3(.62f,.22f,.62f), bronze);
            Cube("Pillar capital", new Vector3(x,3.03f,z), new Vector3(.62f,.18f,.62f), bronze);
        }
        Cube("Roof fascia lower", new Vector3(0,3.23f,0), new Vector3(8.5f,.25f,6.5f), dark);
        Cube("Roof plane", new Vector3(0,3.53f,0), new Vector3(9.1f,.30f,7.1f), wood);
        Cube("Ridge beam", new Vector3(0,3.83f,0), new Vector3(8.4f,.22f,.42f), bronze);
        for(int side=-1;side<=1;side+=2) {
            Cube("Eave trim", new Vector3(side*4.48f,3.43f,0), new Vector3(.14f,.28f,7.25f), bronze);
            Cube("Gable beam", new Vector3(0,3.63f,side*3.34f), new Vector3(8.8f,.20f,.16f), dark);
        }
        // Decorative custom mesh on the gable, plus three distinct S14 mesh props.
        Custom("Gable relief", S14_CustomMesh.Shape.PavilionRelief, new Vector3(0,3.76f,3.43f), new Vector3(1.35f,.55f,.22f), bronze);
        Custom("Jade lantern crystal", S14_CustomMesh.Shape.Crystal, new Vector3(0,1.15f,0), new Vector3(.72f,1.0f,.72f), lantern);
        Custom("Garden marker", S14_CustomMesh.Shape.GardenMarker, new Vector3(-5.0f,.12f,-2.8f), new Vector3(.75f,1.1f,.75f), jade);
        // Primitive material study: different textured URP Lit surfaces.
        Cylinder("Bronze incense vessel", new Vector3(-1.25f,.68f,0), new Vector3(.75f,.62f,.75f), bronze, 20);
        Sphere("Stone garden ornament", new Vector3(1.45f,.58f,-.1f), new Vector3(.95f,.86f,.95f), stone);
        Cube("Wood offering stand", new Vector3(1.5f,.64f,1.35f), new Vector3(1.25f,.55f,.72f), wood);
        Cylinder("Vessel rim", new Vector3(-1.25f,1.06f,0), new Vector3(.88f,.14f,.88f), bronze, 20);
        Sphere("Lantern glow core", new Vector3(0,1.2f,0), new Vector3(.18f,.18f,.18f), lantern);
        var glow = new GameObject("Lantern point light"); glow.transform.position = new Vector3(0,1.25f,0);
        var point = glow.AddComponent<Light>(); point.type=LightType.Point; point.color=new Color(.2f,.55f,1); point.intensity=3.5f; point.range=7;

        // Garden framing with sculpted trees and low shrubs.
        for(int side=-1;side<=1;side+=2) for(int i=0;i<3;i++) {
            float z=-4.2f+i*3.7f; float x=side*5.1f;
            Cylinder("Garden tree trunk", new Vector3(x,1.05f,z), new Vector3(.36f,2.1f,.36f), dark, 10);
            Sphere("Garden tree crown", new Vector3(x,2.35f,z), new Vector3(1.6f,1.7f,1.6f), foliage);
            Sphere("Low garden shrub", new Vector3(side*5.1f,.48f,z+.85f), new Vector3(1.25f,.75f,1.05f), jade);
        }
        for(int i=0;i<5;i++) {
            Cylinder("Path lantern stem", new Vector3((i%2==0?-1:1)*2.25f,.38f,-8.6f+i*.95f), new Vector3(.12f,.38f,.12f), bronze, 10);
            Sphere("Path lantern", new Vector3((i%2==0?-1:1)*2.25f,.88f,-8.6f+i*.95f), new Vector3(.27f,.34f,.27f), lantern);
        }

        // SceneWalker enters from the approach and faces the pavilion opening.
        var walkerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ThirdParty/S13_Pavilion/SceneWalker.prefab");
        if (walkerPrefab != null) {
            var walker = (GameObject)PrefabUtility.InstantiatePrefab(walkerPrefab);
            walker.name="SceneWalker - start outside"; walker.transform.position=new Vector3(0,1f,-10.5f); walker.transform.rotation=Quaternion.identity;
            var playerCam=walker.GetComponentInChildren<Camera>();
            if(playerCam) {
                playerCam.nearClipPlane=.05f; playerCam.farClipPlane=180; playerCam.fieldOfView=68;
                playerCam.clearFlags=CameraClearFlags.Skybox; playerCam.allowHDR=true;
                var playerData=playerCam.GetComponent<UniversalAdditionalCameraData>();
                if(playerData!=null) playerData.renderPostProcessing=true;
            }
        }
        // Separate framing camera, ready to enable for the requested representative still.
        var framing = new GameObject("Composition Camera - still capture");
        framing.transform.position=new Vector3(10.5f,6.2f,-15.5f);
        framing.transform.rotation=Quaternion.LookRotation(new Vector3(0,1.55f,0)-framing.transform.position);
        var fc=framing.AddComponent<Camera>(); fc.fieldOfView=43; fc.nearClipPlane=.1f; fc.farClipPlane=180; fc.enabled=false;
        var fdata=framing.AddComponent<UniversalAdditionalCameraData>(); fdata.renderPostProcessing=true;

        var volumeGo=new GameObject("Global Volume - Bloom"); var volume=volumeGo.AddComponent<Volume>(); volume.isGlobal=true; volume.priority=1;
        var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Root+"/S14_VolumeProfile.asset");
        if(profile==null) profile=ScriptableObject.CreateInstance<VolumeProfile>();
        if(!profile.TryGet<Bloom>(out var bloom)) bloom=profile.Add<Bloom>(true);
        bloom.threshold.Override(1.1f); bloom.intensity.Override(1.25f); bloom.scatter.Override(.72f);
        if(!profile.TryGet<ColorAdjustments>(out var color)) color=profile.Add<ColorAdjustments>(true);
        color.postExposure.Override(.15f); color.contrast.Override(8f); color.saturation.Override(7f);
        volume.sharedProfile=profile;
        if(!AssetDatabase.Contains(profile)) AssetDatabase.CreateAsset(profile, Root+"/S14_VolumeProfile.asset");
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
        Debug.Log("S14_Landscape built: textured primitives, three custom meshes, Bloom, SceneWalker and composition camera.");
    }

    [MenuItem("S14/Capture Composition Camera")]
    public static void CaptureCompositionCamera()
    {
        var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        var go=GameObject.Find("Composition Camera - still capture");
        if(go==null) throw new System.InvalidOperationException("Composition Camera was not found in S14_Landscape.");
        var cam=go.GetComponent<Camera>();
        var old=cam.targetTexture;
        var rt=new RenderTexture(1920,1080,24,RenderTextureFormat.ARGB32);
        cam.targetTexture=rt; cam.Render();
        var previous=RenderTexture.active; RenderTexture.active=rt;
        var image=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);
        image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0); image.Apply();
        File.WriteAllBytes("S14_Composition.png",image.EncodeToPNG());
        RenderTexture.active=previous; cam.targetTexture=old; rt.Release(); Object.DestroyImmediate(rt); Object.DestroyImmediate(image);
        Debug.Log("Wrote S14_Composition.png from the framing camera.");
    }

    [MenuItem("S14/Capture SceneWalker Walkthrough Frames")]
    public static void CaptureWalkthroughFrames()
    {
        const int width=960, height=540, fps=12, seconds=30;
        const string frameDir="/tmp/S14_WalkthroughFrames";
        if(Directory.Exists(frameDir)) Directory.Delete(frameDir,true);
        Directory.CreateDirectory(frameDir);
        EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        var walker=GameObject.Find("SceneWalker - start outside");
        if(walker==null) throw new System.InvalidOperationException("SceneWalker was not found in S14_Landscape.");
        var cam=walker.GetComponentInChildren<Camera>();
        if(cam==null) throw new System.InvalidOperationException("SceneWalker camera was not found.");
        cam.enabled=false;
        var data=cam.GetComponent<UniversalAdditionalCameraData>();
        if(data!=null) data.renderPostProcessing=true;
        var rt=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32);
        cam.targetTexture=rt;
        var previous=RenderTexture.active;
        var image=new Texture2D(width,height,TextureFormat.RGB24,false);
        int count=fps*seconds;
        for(int i=0;i<count;i++)
        {
            float t=(float)i/fps;
            float walk=Mathf.Clamp01(t/22f);
            // A steady outdoor-to-interior walk, then a slow look across the pavilion.
            float x=Mathf.Lerp(0f,.82f,Mathf.SmoothStep(0f,1f,walk));
            walker.transform.SetPositionAndRotation(new Vector3(x,1f,Mathf.Lerp(-12f,-1.5f,walk)), Quaternion.Euler(0,Mathf.Clamp((t-22f)*9.375f,0,75),0));
            cam.Render();
            RenderTexture.active=rt;
            image.ReadPixels(new Rect(0,0,width,height),0,0,false);
            image.Apply(false,false);
            File.WriteAllBytes(Path.Combine(frameDir,"frame"+i.ToString("D4")+".jpg"),image.EncodeToJPG(78));
        }
        RenderTexture.active=previous;
        cam.targetTexture=null;
        rt.Release(); Object.DestroyImmediate(rt); Object.DestroyImmediate(image);
        Debug.Log("Rendered "+count+" SceneWalker camera frames to "+frameDir+".");
    }

    static void EnsureDirs(){ Directory.CreateDirectory(TexDir); Directory.CreateDirectory(MatDir); Directory.CreateDirectory(Root+"/Editor"); }
    static void MakeTexture(string name, Color32 dark, Color32 light, bool grain)
    {
        string path=TexDir+"/T_"+name+".png"; if(File.Exists(path)) return;
        const int size=256; var tex=new Texture2D(size,size,TextureFormat.RGBA32,false); tex.name=name; tex.wrapMode=TextureWrapMode.Repeat; tex.filterMode=FilterMode.Trilinear;
        var rng=new System.Random(name.GetHashCode());
        for(int y=0;y<size;y++) for(int x=0;x<size;x++) {
            float wave=grain ? Mathf.Sin((x+(float)y*.12f)*.16f)*.16f+Mathf.Sin((x+(float)y*.12f)*.47f)*.055f : (Mathf.PerlinNoise(x*.055f,y*.055f)-.5f)*.34f;
            wave+=((float)rng.NextDouble()-.5f)*.08f;
            if(!grain && x%64<2) wave-=.08f;
            tex.SetPixel(x,y,Color.Lerp(dark,light,Mathf.Clamp01(.5f+wave)));
        }
        tex.Apply(); File.WriteAllBytes(path,tex.EncodeToPNG()); Object.DestroyImmediate(tex);
    }
    static Material MakeMaterial(string name,string textureName,Color tint,float metallic,float smoothness)
    {
        string path=MatDir+"/"+name+".mat"; var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat==null) mat=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};
        else mat.shader=Shader.Find("Universal Render Pipeline/Lit");
        mat.SetColor("_BaseColor",tint); mat.SetFloat("_Metallic",metallic); mat.SetFloat("_Smoothness",smoothness);
        if(textureName!=null) mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(TexDir+"/T_"+textureName+".png"));
        if(!AssetDatabase.Contains(mat)) AssetDatabase.CreateAsset(mat,path); EditorUtility.SetDirty(mat); return mat;
    }
    static GameObject Cube(string n,Vector3 p,Vector3 s,Material m){var o=GameObject.CreatePrimitive(PrimitiveType.Cube);o.name=n;o.transform.SetPositionAndRotation(p,Quaternion.identity);o.transform.localScale=s;o.GetComponent<MeshRenderer>().sharedMaterial=m;return o;}
    static GameObject Sphere(string n,Vector3 p,Vector3 s,Material m){var o=GameObject.CreatePrimitive(PrimitiveType.Sphere);o.name=n;o.transform.SetPositionAndRotation(p,Quaternion.identity);o.transform.localScale=s;o.GetComponent<MeshRenderer>().sharedMaterial=m;return o;}
    static GameObject Cylinder(string n,Vector3 p,Vector3 s,Material m,int sides){var o=GameObject.CreatePrimitive(PrimitiveType.Cylinder);o.name=n;o.transform.SetPositionAndRotation(p,Quaternion.identity);o.transform.localScale=s;o.GetComponent<MeshRenderer>().sharedMaterial=m;return o;}
    static GameObject Custom(string n,S14_CustomMesh.Shape shape,Vector3 p,Vector3 s,Material m){var o=new GameObject(n);o.transform.position=p;o.transform.localScale=s;o.AddComponent<MeshFilter>();o.AddComponent<MeshRenderer>().sharedMaterial=m;var gen=o.AddComponent<S14_CustomMesh>();gen.shape=shape;return o;}
}
