using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

[InitializeOnLoad]
public static class Tutorial2SceneBuilder
{
    private const string Root = "Assets/Tutorial2";
    private const string PrefabsPath = Root + "/Prefabs";
    private const string MaterialsPath = Root + "/Materials";
    private const string ScenePath = "Assets/Scenes/Tutorial2.unity";

    static Tutorial2SceneBuilder()
    {
        EditorApplication.delayCall += CreateSceneOnFirstImport;
    }

    [MenuItem("Tutorial 2/Create Demo Scene")]
    public static void CreateDemoScene()
    {
        BuildAll();
        EditorUtility.DisplayDialog("Tutorial 2", "Сцена, куст и три фруктовых префаба созданы. Откройте Tutorial2 и нажмите Play.", "OK");
    }

    private static void CreateSceneOnFirstImport()
    {
        if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (!File.Exists(ScenePath))
            BuildAll();
    }

    private static void BuildAll()
    {
        EnsureFolders();

        Material leaves = CreateMaterial("Leaves", new Color(0.16f, 0.48f, 0.19f));
        Material wood = CreateMaterial("Wood", new Color(0.34f, 0.19f, 0.09f));
        Material marker = CreateMaterial("Branch Point", new Color(1f, 0.72f, 0.12f));
        Material apple = CreateMaterial("Apple Red", new Color(0.78f, 0.08f, 0.07f));
        Material orange = CreateMaterial("Orange", new Color(1f, 0.38f, 0.04f));
        Material pear = CreateMaterial("Pear Green", new Color(0.55f, 0.76f, 0.16f));
        Material fruitStem = CreateMaterial("Fruit Stem", new Color(0.24f, 0.13f, 0.06f));
        Material ground = CreateMaterial("Ground", new Color(0.36f, 0.58f, 0.28f));
        PhysicMaterial groundPhysics = CreateGroundPhysics();

        GameObject bushPrefab = CreateBushPrefab(leaves, wood, marker);
        GameObject[] fruits =
        {
            CreateFruitPrefab("Apple", apple, fruitStem, new Vector3(0.36f, 0.42f, 0.36f), true),
            CreateFruitPrefab("Orange", orange, fruitStem, new Vector3(0.38f, 0.38f, 0.38f), false),
            CreateFruitPrefab("Pear", pear, fruitStem, new Vector3(0.32f, 0.45f, 0.32f), true)
        };

        BuildScene(bushPrefab, fruits, ground, groundPhysics);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void EnsureFolders()
    {
        CreateFolderIfMissing("Assets", "Tutorial2");
        CreateFolderIfMissing(Root, "Prefabs");
        CreateFolderIfMissing(Root, "Materials");
        CreateFolderIfMissing("Assets", "Scenes");
    }

    private static void CreateFolderIfMissing(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
    }

    private static Material CreateMaterial(string name, Color color)
    {
        string path = MaterialsPath + "/" + name.Replace(" ", "") + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.name = name;
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static PhysicMaterial CreateGroundPhysics()
    {
        const string path = MaterialsPath + "/GroundPhysics.physicMaterial";
        PhysicMaterial physics = AssetDatabase.LoadAssetAtPath<PhysicMaterial>(path);
        if (physics == null)
        {
            physics = new PhysicMaterial("GroundPhysics");
            AssetDatabase.CreateAsset(physics, path);
        }
        physics.dynamicFriction = 0.7f;
        physics.staticFriction = 0.8f;
        physics.bounciness = 0.12f;
        physics.frictionCombine = PhysicMaterialCombine.Average;
        physics.bounceCombine = PhysicMaterialCombine.Minimum;
        EditorUtility.SetDirty(physics);
        return physics;
    }

    private static GameObject CreateBushPrefab(Material leaves, Material wood, Material markerMaterial)
    {
        const string path = PrefabsPath + "/Bush.prefab";
        GameObject root = new GameObject("Bush");
        root.AddComponent<SpawnPoint>().pointName = "Bush root";

        AddPrimitive("Main trunk", PrimitiveType.Cylinder, root.transform,
            new Vector3(0f, 1.05f, 0f), new Vector3(0.22f, 1.05f, 0.22f), wood);

        AddBranch(root.transform, "Branch left", new Vector3(0f, 1.05f, 0f), new Vector3(-0.95f, 2.15f, -0.05f), wood);
        AddBranch(root.transform, "Branch right", new Vector3(0f, 1.15f, 0f), new Vector3(0.95f, 2.25f, 0.04f), wood);
        AddBranch(root.transform, "Branch back", new Vector3(0f, 1.2f, 0f), new Vector3(0.05f, 2.55f, 0.85f), wood);
        AddBranch(root.transform, "Branch front", new Vector3(0f, 1.25f, 0f), new Vector3(-0.08f, 2.45f, -0.9f), wood);

        AddPrimitive("Crown center", PrimitiveType.Sphere, root.transform, new Vector3(0f, 2.65f, 0f), new Vector3(1.05f, 1.1f, 1.0f), leaves);
        AddPrimitive("Crown left", PrimitiveType.Sphere, root.transform, new Vector3(-0.7f, 2.25f, 0f), new Vector3(0.9f, 0.9f, 0.85f), leaves);
        AddPrimitive("Crown right", PrimitiveType.Sphere, root.transform, new Vector3(0.68f, 2.35f, 0.05f), new Vector3(0.9f, 0.9f, 0.85f), leaves);
        AddPrimitive("Crown back", PrimitiveType.Sphere, root.transform, new Vector3(0f, 2.35f, 0.62f), new Vector3(0.85f, 0.9f, 0.8f), leaves);
        AddPrimitive("Crown top", PrimitiveType.Sphere, root.transform, new Vector3(0f, 3.25f, 0f), new Vector3(0.8f, 0.8f, 0.8f), leaves);

        Vector3[] points =
        {
            new Vector3(-1.08f, 2.2f, -0.05f),
            new Vector3(1.08f, 2.3f, 0.04f),
            new Vector3(0.05f, 2.62f, 0.98f),
            new Vector3(-0.08f, 2.52f, -1.02f),
            new Vector3(-0.55f, 3.18f, 0.15f),
            new Vector3(0.55f, 3.28f, -0.12f)
        };

        for (int i = 0; i < points.Length; i++)
        {
            GameObject point = new GameObject("BranchSpawnPoint_" + (i + 1));
            point.transform.SetParent(root.transform, false);
            point.transform.localPosition = points[i];
            point.AddComponent<SpawnPoint>().pointName = "Branch " + (i + 1);
            GameObject pin = AddPrimitive("Point marker", PrimitiveType.Sphere, point.transform, Vector3.zero,
                Vector3.one * 0.16f, markerMaterial);
            Collider collider = pin.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
        }

        return SavePrefabAndDestroy(root, path);
    }

    private static GameObject CreateFruitPrefab(string name, Material fruitMaterial, Material stemMaterial, Vector3 bodyScale, bool withLeaf)
    {
        string path = PrefabsPath + "/" + name + ".prefab";
        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.name = name;
        root.transform.localScale = bodyScale;
        root.GetComponent<Renderer>().sharedMaterial = fruitMaterial;
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.mass = 0.3f;
        body.drag = 0.02f;
        body.angularDrag = 0.08f;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        AddPrimitive("Stem", PrimitiveType.Cylinder, root.transform, new Vector3(0f, 0.52f, 0f),
            new Vector3(0.055f, 0.15f, 0.055f), stemMaterial);
        if (withLeaf)
        {
            GameObject leaf = AddPrimitive("Leaf", PrimitiveType.Sphere, root.transform, new Vector3(0.13f, 0.62f, 0f),
                new Vector3(0.23f, 0.07f, 0.11f), fruitMaterial);
            leaf.transform.localRotation = Quaternion.Euler(0f, 0f, -25f);
        }

        return SavePrefabAndDestroy(root, path);
    }

    private static void AddBranch(Transform parent, string name, Vector3 start, Vector3 end, Material material)
    {
        Vector3 delta = end - start;
        GameObject branch = AddPrimitive(name, PrimitiveType.Cylinder, parent, (start + end) * 0.5f,
            new Vector3(0.09f, delta.magnitude * 0.5f, 0.09f), material);
        branch.transform.localRotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
    }

    private static GameObject AddPrimitive(string name, PrimitiveType type, Transform parent, Vector3 localPosition,
        Vector3 localScale, Material material)
    {
        GameObject child = GameObject.CreatePrimitive(type);
        child.name = name;
        child.transform.SetParent(parent, false);
        child.transform.localPosition = localPosition;
        child.transform.localScale = localScale;
        if (material != null) child.GetComponent<Renderer>().sharedMaterial = material;
        return child;
    }

    private static GameObject SavePrefabAndDestroy(GameObject root, string path)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static void BuildScene(GameObject bushPrefab, GameObject[] fruitPrefabs, Material groundMaterial, PhysicMaterial groundPhysics)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 55f;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = new Vector3(0f, 9f, -15f);
        cameraObject.transform.LookAt(new Vector3(0f, 1.2f, 0f));

        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = "Ground Plane";
        plane.transform.position = Vector3.zero;
        plane.transform.localScale = new Vector3(2f, 1f, 2f);
        plane.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        plane.GetComponent<Collider>().material = groundPhysics;

        Transform[] groundPoints = new Transform[6];
        Vector3[] pointPositions =
        {
            new Vector3(-6f, 0.05f, -3.5f), new Vector3(-2.4f, 0.05f, -3.5f),
            new Vector3(2.4f, 0.05f, -3.5f), new Vector3(6f, 0.05f, -3.5f),
            new Vector3(-4.2f, 0.05f, 2.3f), new Vector3(4.2f, 0.05f, 2.3f)
        };
        for (int i = 0; i < pointPositions.Length; i++)
        {
            GameObject point = new GameObject("BushSpawnPoint_" + (i + 1));
            point.transform.position = pointPositions[i];
            point.AddComponent<SpawnPoint>().pointName = "Ground " + (i + 1);
            groundPoints[i] = point.transform;
        }

        GameObject managerObject = new GameObject("GameManager");
        Tutorial2GameManager manager = managerObject.AddComponent<Tutorial2GameManager>();
        SerializedObject serializedManager = new SerializedObject(manager);
        SerializedProperty bushes = serializedManager.FindProperty("bushPrefabs");
        bushes.arraySize = 1;
        bushes.GetArrayElementAtIndex(0).objectReferenceValue = bushPrefab;
        SerializedProperty fruits = serializedManager.FindProperty("fruitPrefabs");
        fruits.arraySize = fruitPrefabs.Length;
        for (int i = 0; i < fruitPrefabs.Length; i++)
            fruits.GetArrayElementAtIndex(i).objectReferenceValue = fruitPrefabs[i];
        SerializedProperty points = serializedManager.FindProperty("bushSpawnPoints");
        points.arraySize = groundPoints.Length;
        for (int i = 0; i < groundPoints.Length; i++)
            points.GetArrayElementAtIndex(i).objectReferenceValue = groundPoints[i];
        serializedManager.ApplyModifiedPropertiesWithoutUndo();

        CreateUI(manager);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    }

    private static void CreateUI(Tutorial2GameManager manager)
    {
        GameObject canvasObject = new GameObject("Demo UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);

        GameObject panel = new GameObject("Button Panel", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup));
        panel.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 24f);
        panelRect.sizeDelta = new Vector2(520f, 76f);
        panel.GetComponent<Image>().color = new Color(0.08f, 0.12f, 0.08f, 0.78f);

        HorizontalLayoutGroup layout = panel.GetComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 10, 10);
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        CreateButton(panel.transform, "Spawn Bush", manager, manager.SpawnBush);
        CreateButton(panel.transform, "Spawn Fruit", manager, manager.SpawnFruit);
        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        eventSystem.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
    }

    private static void CreateButton(Transform parent, string label, Tutorial2GameManager manager, UnityAction action)
    {
        GameObject buttonObject = new GameObject(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        buttonObject.GetComponent<RectTransform>().sizeDelta = new Vector2(220f, 54f);
        Image image = buttonObject.GetComponent<Image>();
        image.color = label == "Spawn Bush" ? new Color(0.18f, 0.48f, 0.22f) : new Color(0.76f, 0.34f, 0.08f);
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        UnityEventTools.AddPersistentListener(button.onClick, action);

        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Text text = textObject.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 22;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.raycastTarget = false;
    }
}