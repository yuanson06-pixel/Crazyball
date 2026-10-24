using System;
using System.Linq;
using Crazyball.CourseKit.Coins;
using Crazyball.CourseKit.Core;
using Crazyball.CourseKit.Input;
using Crazyball.CourseKit.Obstacles;
using Crazyball.CourseKit.Player;
using Crazyball.CourseKit.UI;
using Crazyball.CourseKit.Upgrades;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CrazyballCompleteEditorLayout
{
    private const string Root = "Assets/Crazyball";
    private const string MainScene = Root + "/Course/Scenes/Crazyball_Main.unity";
    private const string CourseScene = Root + "/Course/Scenes/Crazyball_Course.unity";
    private const string InputPath = Root + "/Resources/Complete/CrazyballCompleteInput.asset";
    private static readonly Color Navy = Hex("101629");
    private static readonly Color Cyan = Hex("24D5E8");
    private static readonly Color Pink = Hex("F0559B");
    private static readonly Color Gold = Hex("FFC857");
    private static TMP_FontAsset font;

    [MenuItem("Crazyball/Build Complete Scenes")]
    public static void Apply()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Exit Play Mode before building the Crazyball scenes.");
            return;
        }

        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Root + "/Resources/Complete/Fonts/Crazyball SDF.asset");
        EnsureGameplayActions();
        EnsurePinballUpgradeAssets();
        BuildMainScene();
        BuildCourseScene();
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(MainScene, true),
            new EditorBuildSettingsScene(CourseScene, true)
        };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene(MainScene, OpenSceneMode.Single);
        Debug.Log("Crazyball Complete: both scenes, runtime references, UI, and Build Settings are ready.");
    }

    private static void BuildMainScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(new Vector3(0, 7, -10), new Vector3(28, 0, 0));
        CreateLight();

        GameObject visualRoot = new GameObject("Menu Visuals");
        for (int i = 0; i < 14; i++)
        {
            PrimitiveType type = i % 3 == 0 ? PrimitiveType.Sphere : PrimitiveType.Cube;
            GameObject shape = GameObject.CreatePrimitive(type);
            shape.name = "NeonShape_" + (i + 1).ToString("00");
            shape.transform.SetParent(visualRoot.transform);
            float angle = i * Mathf.PI * 2f / 14f;
            shape.transform.position = new Vector3(Mathf.Sin(angle) * 7.5f, UnityEngine.Random.Range(-1f, 5f), 4 + Mathf.Cos(angle) * 3f);
            shape.transform.localScale = Vector3.one * UnityEngine.Random.Range(.25f, .85f);
            SetMaterial(shape, i % 2 == 0 ? Cyan : Pink);
            UnityEngine.Object.DestroyImmediate(shape.GetComponent<Collider>());
        }

        Canvas canvas = CreateCanvas("Main UI");
        Image bg = Panel(canvas.transform, "Background", Navy, Vector2.zero, Vector2.one);
        bg.transform.SetAsFirstSibling();
        bg.color = new Color(Navy.r, Navy.g, Navy.b, .90f);

        TMP_Text eyebrow = Text(canvas.transform, "Eyebrow", "FIVE STAGES  •  ONE PERFECT RUN", 22, Cyan, TextAlignmentOptions.Center);
        Place(eyebrow.rectTransform, new Vector2(.15f, .76f), new Vector2(.85f, .84f));
        TMP_Text title = Text(canvas.transform, "Title", "CRAZY\nBALL", 96, Color.white, TextAlignmentOptions.Center);
        title.fontStyle = FontStyles.Bold;
        Place(title.rectTransform, new Vector2(.15f, .43f), new Vector2(.85f, .77f));
        TMP_Text subtitle = Text(canvas.transform, "Subtitle", "DODGE  •  COLLECT  •  UPGRADE", 24, new Color(1, 1, 1, .72f), TextAlignmentOptions.Center);
        Place(subtitle.rectTransform, new Vector2(.15f, .37f), new Vector2(.85f, .43f));

        CompleteMainMenuController controller = new GameObject("Main Menu Controller").AddComponent<CompleteMainMenuController>();
        TMP_Text progress = Text(canvas.transform, "Progress", "UPGRADE CORES  0\nBEST CHECKPOINT  0 / 5", 20, Gold, TextAlignmentOptions.Center);
        Place(progress.rectTransform, new Vector2(.28f, .20f), new Vector2(.72f, .31f));
        Set(controller, "progressText", progress);

        Button play = Button(canvas.transform, "Play Button", "START RUN", Cyan);
        Place(play.GetComponent<RectTransform>(), new Vector2(.34f, .09f), new Vector2(.66f, .18f));
        UnityEventTools.AddPersistentListener(play.onClick, controller.Play);
        TMP_Text footer = Text(canvas.transform, "Footer", "DRAG / HOLD TO MOVE    •    ESC TO PAUSE", 15, new Color(1, 1, 1, .45f), TextAlignmentOptions.Center);
        Place(footer.rectTransform, new Vector2(.15f, .02f), new Vector2(.85f, .07f));
        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, MainScene);
    }

    private static void BuildCourseScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Camera camera = CreateCamera(new Vector3(0, 9.8f, -8.5f), new Vector3(48, 0, 0));
        camera.fieldOfView = 52;
        CreateLight();

        GameObject environment = new GameObject("Environment");
        GameObject floor = Primitive("Runway", PrimitiveType.Cube, new Vector3(0, -.3f, 1), new Vector3(7, .5f, 20), Navy, environment.transform);
        Primitive("Left Rail", PrimitiveType.Cube, new Vector3(-3.55f, .15f, 1), new Vector3(.18f, .8f, 20), Cyan, environment.transform);
        Primitive("Right Rail", PrimitiveType.Cube, new Vector3(3.55f, .15f, 1), new Vector3(.18f, .8f, 20), Pink, environment.transform);
        Primitive("Top Deflector", PrimitiveType.Cube, new Vector3(0, .15f, 10.8f), new Vector3(7.2f, .8f, .18f), Cyan, environment.transform);
        for (int z = -7; z <= 10; z += 2)
            Primitive("Lane Marker " + z, PrimitiveType.Cube, new Vector3(0, -.015f, z), new Vector3(.06f, .03f, 1.1f), new Color(1, 1, 1, .18f), environment.transform);

        GameObject systems = new GameObject("Runtime Systems");
        RunCoinWallet wallet = systems.AddComponent<RunCoinWallet>();
        ObstacleSpawner spawner = systems.AddComponent<ObstacleSpawner>();
        InGameUpgradeManager upgrades = systems.AddComponent<InGameUpgradeManager>();
        UpgradeEffectDispatcher dispatcher = systems.AddComponent<UpgradeEffectDispatcher>();
        PlayerPointerInput input = systems.AddComponent<PlayerPointerInput>();

        Set(spawner, "obstacleBlockPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Resources/Complete/Prefabs/PF_ObstacleBlock.prefab"));
        Set(spawner, "coinPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Resources/Complete/Prefabs/PF_Coin.prefab"));
        Set(upgrades, "wallet", wallet);
        Set(upgrades, "catalog", new[]
        {
            AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(Root + "/Resources/Complete/Upgrades/SO_SmallBall.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(Root + "/Resources/Complete/Upgrades/SO_Shield.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(Root + "/Resources/Complete/Upgrades/SO_QuickControl.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(Root + "/Resources/Complete/Upgrades/SO_PinballDamage.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(Root + "/Resources/Complete/Upgrades/SO_PinballSpeed.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(Root + "/Resources/Complete/Upgrades/SO_PinballSize.asset"),
            AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(Root + "/Resources/Complete/Upgrades/SO_PlayerWidth.asset")
        });
        Set(input, "moveAction", FindActionReference("Gameplay", "Move"));
        Set(input, "launchAction", FindActionReference("Gameplay", "Launch"));
        Set(input, "pauseAction", FindActionReference("Gameplay", "Pause"));

        GameObject player = Primitive("Player Paddle", PrimitiveType.Cube, new Vector3(0, .45f, -5.2f), new Vector3(1.55f, .7f, .65f), Color.white, null);
        Rigidbody body = player.AddComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeAll;
        body.useGravity = false;
        body.isKinematic = true;
        PlayerBallController playerController = player.AddComponent<PlayerBallController>();
        PlayerSurvival survival = player.AddComponent<PlayerSurvival>();
        RunCoinCollector collector = player.AddComponent<RunCoinCollector>();
        PlayerScaleUpgrade scale = player.AddComponent<PlayerScaleUpgrade>();
        PlayerResponsivenessUpgrade response = player.AddComponent<PlayerResponsivenessUpgrade>();
        PlayerWidthUpgrade widthUpgrade = player.AddComponent<PlayerWidthUpgrade>();
        Set(playerController, "pointerInput", input);
        Set(playerController, "scaleUpgrade", scale);
        Set(playerController, "widthUpgrade", widthUpgrade);
        Set(collector, "wallet", wallet);
        Set(response, "controller", playerController);
        Set(spawner, "player", playerController);
        Set(spawner, "survival", survival);

        GameObject ball = Primitive("Pinball", PrimitiveType.Sphere, new Vector3(0, .45f, -4.4f), Vector3.one * .48f, Gold, null);
        ball.AddComponent<Rigidbody>();
        PinballProjectile pinball = ball.AddComponent<PinballProjectile>();
        Set(pinball, "player", playerController);
        Set(pinball, "survival", survival);
        Set(playerController, "pinball", pinball);
        PinballUpgradeReceiver damageUpgrade = AddPinballReceiver(ball, pinball, UpgradeEffectType.PinballDamage);
        PinballUpgradeReceiver speedUpgrade = AddPinballReceiver(ball, pinball, UpgradeEffectType.PinballSpeed);
        PinballUpgradeReceiver sizeUpgrade = AddPinballReceiver(ball, pinball, UpgradeEffectType.PinballSize);
        Set(dispatcher, "upgradeManager", upgrades);
        Set(dispatcher, "receiverComponents", new MonoBehaviour[] { survival, scale, response, widthUpgrade, damageUpgrade, speedUpgrade, sizeUpgrade });

        Canvas canvas = CreateCanvas("HUD");
        Image topBar = Panel(canvas.transform, "Top Bar", new Color(.035f, .055f, .11f, .94f), new Vector2(0, .88f), Vector2.one);
        TMP_Text stage = Text(topBar.transform, "Stage", "STAGE 1 / 5", 27, Color.white, TextAlignmentOptions.Left);
        Place(stage.rectTransform, new Vector2(.04f, .12f), new Vector2(.29f, .88f));
        TMP_Text coins = Text(topBar.transform, "Coin", "COIN  0", 27, Gold, TextAlignmentOptions.Right);
        Place(coins.rectTransform, new Vector2(.71f, .12f), new Vector2(.96f, .88f));
        RunCoinText coinText = coins.gameObject.AddComponent<RunCoinText>();
        Set(coinText, "wallet", wallet);
        Set(coinText, "label", coins);
        Set(coinText, "prefix", "COIN  ");
        TMP_Text health = Text(topBar.transform, "RunHealthText", "HP  3 / 3    SHIELD  0", 20, Pink, TextAlignmentOptions.Center);
        Place(health.rectTransform, new Vector2(.3f, .12f), new Vector2(.7f, .88f));
        RunHealthText healthText = health.gameObject.AddComponent<RunHealthText>();
        Set(healthText, "survival", survival);
        Set(healthText, "label", health);
        TMP_Text message = Text(canvas.transform, "Message", "", 40, Cyan, TextAlignmentOptions.Center);
        Place(message.rectTransform, new Vector2(.1f, .67f), new Vector2(.9f, .84f));
        TMP_Text controls = Text(canvas.transform, "Controls", "A / D  MOVE     SPACE  LAUNCH", 17, new Color(1, 1, 1, .55f), TextAlignmentOptions.Center);
        Place(controls.rectTransform, new Vector2(.15f, .015f), new Vector2(.85f, .06f));

        Image bossHealthPanel = Panel(canvas.transform, "Boss Health", new Color(.035f, .055f, .11f, .96f), new Vector2(.27f, .80f), new Vector2(.73f, .875f));
        bossHealthPanel.raycastTarget = false;
        TMP_Text bossName = Text(bossHealthPanel.transform, "Boss Name", "BOSS", 18, Pink, TextAlignmentOptions.Left);
        Place(bossName.rectTransform, new Vector2(.04f, .52f), new Vector2(.58f, .94f));
        TMP_Text bossHealthValue = Text(bossHealthPanel.transform, "Health Value", "0 / 0", 16, Color.white, TextAlignmentOptions.Right);
        Place(bossHealthValue.rectTransform, new Vector2(.6f, .52f), new Vector2(.96f, .94f));
        GameObject sliderObject = new GameObject("Health Bar", typeof(RectTransform), typeof(Slider));
        sliderObject.transform.SetParent(bossHealthPanel.transform, false);
        Place(sliderObject.GetComponent<RectTransform>(), new Vector2(.04f, .14f), new Vector2(.96f, .42f));
        Image sliderBackground = Panel(sliderObject.transform, "Background", new Color(.18f, .2f, .28f, 1f), Vector2.zero, Vector2.one);
        sliderBackground.raycastTarget = false;
        Image sliderFill = Panel(sliderObject.transform, "Fill", Pink, Vector2.zero, Vector2.one);
        sliderFill.raycastTarget = false;
        Slider bossSlider = sliderObject.GetComponent<Slider>();
        bossSlider.fillRect = sliderFill.rectTransform;
        bossSlider.targetGraphic = sliderFill;
        bossSlider.direction = Slider.Direction.LeftToRight;
        bossSlider.interactable = false;

        Image leftBar = Panel(canvas.transform, "Left Bar", new Color(.035f, .055f, .11f, .90f), new Vector2(.015f, .16f), new Vector2(.235f, .82f));
        leftBar.raycastTarget = false;
        TMP_Text leftTitle = Text(leftBar.transform, "Title", "RUN UPGRADES", 19, Cyan, TextAlignmentOptions.Center);
        Place(leftTitle.rectTransform, new Vector2(.07f, .87f), new Vector2(.93f, .97f));
        TMP_Text upgradeList = Text(leftBar.transform, "Upgrade List", "NO UPGRADES", 17, Color.white, TextAlignmentOptions.TopLeft);
        Place(upgradeList.rectTransform, new Vector2(.09f, .07f), new Vector2(.91f, .84f));

        Image rightBar = Panel(canvas.transform, "Right Bar", new Color(.035f, .055f, .11f, .90f), new Vector2(.765f, .16f), new Vector2(.985f, .82f));
        rightBar.raycastTarget = false;
        TMP_Text rightTitle = Text(rightBar.transform, "Title", "RUN PROGRESS", 19, Pink, TextAlignmentOptions.Center);
        Place(rightTitle.rectTransform, new Vector2(.07f, .87f), new Vector2(.93f, .97f));
        TMP_Text progress = Text(rightBar.transform, "Progress", "CURRENT STAGE\n1 / 5\n\nCHECKPOINTS LEFT\n5", 19, Color.white, TextAlignmentOptions.Top);
        Place(progress.rectTransform, new Vector2(.08f, .08f), new Vector2(.92f, .83f));

        Image upgradePanel = Panel(canvas.transform, "Checkpoint Upgrade Panel", new Color(.025f, .035f, .075f, .985f), new Vector2(.12f, .16f), new Vector2(.88f, .82f));
        Text(upgradePanel.transform, "Heading", "CHECKPOINT REACHED", 34, Cyan, TextAlignmentOptions.Center);
        Place(upgradePanel.transform.GetChild(0).GetComponent<RectTransform>(), new Vector2(.08f, .82f), new Vector2(.92f, .96f));
        Button[] choiceButtons = new Button[3];
        Image[] choiceIcons = new Image[3];
        TMP_Text[] choiceLabels = new TMP_Text[3];
        for (int i = 0; i < 3; i++)
        {
            float x0 = .055f + i * .315f;
            choiceButtons[i] = Button(upgradePanel.transform, "Choice " + (i + 1), "UPGRADE", i == 1 ? Pink : Cyan);
            Place(choiceButtons[i].GetComponent<RectTransform>(), new Vector2(x0, .28f), new Vector2(x0 + .26f, .76f));
            choiceLabels[i] = choiceButtons[i].GetComponentInChildren<TMP_Text>();
            choiceLabels[i].fontSize = 18;
            Place(choiceLabels[i].rectTransform, new Vector2(.06f, .06f), new Vector2(.94f, .53f));
            choiceIcons[i] = Panel(choiceButtons[i].transform, "Upgrade Icon", Color.white, new Vector2(.24f, .56f), new Vector2(.76f, .94f));
            choiceIcons[i].raycastTarget = false;
            choiceIcons[i].preserveAspect = true;
        }
        Button skip = Button(upgradePanel.transform, "Skip", "SKIP UPGRADE", new Color(.26f, .29f, .38f));
        Place(skip.GetComponent<RectTransform>(), new Vector2(.35f, .07f), new Vector2(.65f, .20f));
        CheckpointUpgradePanel panelComponent = upgradePanel.gameObject.AddComponent<CheckpointUpgradePanel>();
        Set(panelComponent, "panel", upgradePanel.gameObject);
        Set(panelComponent, "choiceButtons", choiceButtons);
        Set(panelComponent, "choiceIcons", choiceIcons);
        Set(panelComponent, "choiceLabels", choiceLabels);
        Set(panelComponent, "skipButton", skip);
        upgradePanel.gameObject.SetActive(false);

        Image gameOver = Panel(canvas.transform, "Run Complete Panel", new Color(.025f, .035f, .075f, .98f), new Vector2(.22f, .27f), new Vector2(.78f, .72f));
        TMP_Text result = Text(gameOver.transform, "Result", "RUN OVER", 52, Pink, TextAlignmentOptions.Center);
        Place(result.rectTransform, new Vector2(.08f, .57f), new Vector2(.92f, .9f));
        Button retry = Button(gameOver.transform, "Retry", "RUN AGAIN", Cyan);
        Place(retry.GetComponent<RectTransform>(), new Vector2(.2f, .15f), new Vector2(.8f, .4f));
        gameOver.gameObject.SetActive(false);

        CompleteGameController game = systems.AddComponent<CompleteGameController>();
        Set(game, "wallet", wallet); Set(game, "spawner", spawner); Set(game, "playerController", playerController);
        Set(game, "playerSurvival", survival); Set(game, "upgrades", upgrades); Set(game, "upgradePanel", panelComponent);
        Set(game, "gameOverPanel", gameOver.gameObject);
        CompleteHudPresenter hud = canvas.gameObject.AddComponent<CompleteHudPresenter>();
        Set(hud, "gameController", game);
        Set(hud, "upgradeManager", upgrades);
        Set(hud, "spawner", spawner);
        Set(hud, "stageLabel", stage);
        Set(hud, "statusLabel", message);
        Set(hud, "upgradeLabel", upgradeList);
        Set(hud, "progressLabel", progress);
        Set(hud, "bossHealthPanel", bossHealthPanel.gameObject);
        Set(hud, "bossNameLabel", bossName);
        Set(hud, "bossHealthLabel", bossHealthValue);
        Set(hud, "bossHealthSlider", bossSlider);
        bossHealthPanel.gameObject.SetActive(false);
        UnityEventTools.AddPersistentListener(retry.onClick, game.BeginRun);
        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, CourseScene);
    }

    private static Camera CreateCamera(Vector3 position, Vector3 euler)
    {
        GameObject go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        Camera cam = go.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Hex("080B16");
        go.transform.SetPositionAndRotation(position, Quaternion.Euler(euler));
        go.AddComponent<AudioListener>();
        return cam;
    }

    private static void CreateLight()
    {
        Light light = new GameObject("Directional Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        light.color = new Color(.72f, .85f, 1f);
        light.transform.rotation = Quaternion.Euler(50, -25, 0);
        RenderSettings.ambientLight = new Color(.19f, .22f, .32f);
    }

    private static Canvas CreateCanvas(string name)
    {
        Canvas canvas = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = .5f;
        return canvas;
    }

    private static void CreateEventSystem()
    {
        GameObject go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        InputSystemUIInputModule ui = go.GetComponent<InputSystemUIInputModule>();
        ui.actionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
    }

    private static Image Panel(Transform parent, string name, Color color, Vector2 min, Vector2 max)
    {
        Image image = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        image.transform.SetParent(parent, false);
        image.color = color;
        Place(image.rectTransform, min, max);
        return image;
    }

    private static TMP_Text Text(Transform parent, string name, string value, float size, Color color, TextAlignmentOptions alignment)
    {
        TextMeshProUGUI text = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        text.transform.SetParent(parent, false);
        text.text = value;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        if (font != null) text.font = font;
        Place(text.rectTransform, Vector2.zero, Vector2.one);
        return text;
    }

    private static Button Button(Transform parent, string name, string label, Color color)
    {
        Image image = Panel(parent, name, color, Vector2.zero, Vector2.one);
        Button button = image.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = Color.Lerp(color, Color.white, .2f);
        colors.pressedColor = Color.Lerp(color, Color.black, .2f);
        button.colors = colors;
        TMP_Text text = Text(image.transform, "Label", label, 24, Navy, TextAlignmentOptions.Center);
        text.fontStyle = FontStyles.Bold;
        return button;
    }

    private static GameObject Primitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent);
        go.transform.position = position;
        go.transform.localScale = scale;
        SetMaterial(go, color);
        return go;
    }

    private static void SetMaterial(GameObject go, Color color)
    {
        string key = ColorUtility.ToHtmlStringRGBA(color);
        string path = Root + "/Resources/Complete/Materials/MAT_Complete_" + key + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = color;
            AssetDatabase.CreateAsset(material, path);
        }
        go.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void Place(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = min; rect.anchorMax = max;
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
    }

    private static void Set(UnityEngine.Object target, string property, UnityEngine.Object value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(property).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void Set(UnityEngine.Object target, string property, UnityEngine.Object[] values)
    {
        SerializedObject serialized = new SerializedObject(target);
        SerializedProperty array = serialized.FindProperty(property);
        array.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void Set(UnityEngine.Object target, string property, string value)
    {
        SerializedObject serialized = new SerializedObject(target);
        serialized.FindProperty(property).stringValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static InputActionReference FindActionReference(string map, string action)
    {
        InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
        InputAction found = asset.FindAction(map + "/" + action, true);
        return AssetDatabase.LoadAllAssetsAtPath(InputPath).OfType<InputActionReference>().FirstOrDefault(reference => reference.action != null && reference.action.id == found.id);
    }

    private static void EnsureGameplayActions()
    {
        InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
        InputActionMap gameplay = asset.FindActionMap("Gameplay", true);
        InputAction move = gameplay.FindAction("Move");
        if (move == null)
        {
            move = gameplay.AddAction("Move", InputActionType.Value);
            move.expectedControlType = "Axis";
            move.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/a")
                .With("Positive", "<Keyboard>/d");
            move.AddBinding("<Gamepad>/leftStick/x");
        }

        InputAction launch = gameplay.FindAction("Launch");
        if (launch == null)
        {
            launch = gameplay.AddAction("Launch", InputActionType.Button, "<Keyboard>/space");
            launch.AddBinding("<Gamepad>/buttonSouth");
        }

        EnsureActionReference(asset, move);
        EnsureActionReference(asset, launch);
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(InputPath, ImportAssetOptions.ForceUpdate);
    }

    private static PinballUpgradeReceiver AddPinballReceiver(GameObject target, PinballProjectile projectile, UpgradeEffectType effectType)
    {
        PinballUpgradeReceiver receiver = target.AddComponent<PinballUpgradeReceiver>();
        Set(receiver, "projectile", projectile);
        SerializedObject serialized = new SerializedObject(receiver);
        serialized.FindProperty("effectType").enumValueIndex = (int)effectType;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        return receiver;
    }

    private static void EnsurePinballUpgradeAssets()
    {
        CreateUpgrade("SO_PinballDamage", "ingame.pinball.damage", "Heavy Ball",
            "Deal more damage to walls and bosses.", UpgradeEffectType.PinballDamage,
            new[] { 3, 5, 7 }, new[] { 2f, 3f, 4f });
        CreateUpgrade("SO_PinballSpeed", "ingame.pinball.speed", "Velocity Core",
            "Increase pinball travel speed.", UpgradeEffectType.PinballSpeed,
            new[] { 2, 4, 6 }, new[] { 13f, 15f, 18f });
        CreateUpgrade("SO_PinballSize", "ingame.pinball.size", "Wide Impact",
            "Increase pinball size and hit coverage.", UpgradeEffectType.PinballSize,
            new[] { 2, 4, 6 }, new[] { 1.15f, 1.3f, 1.5f });
        CreateUpgrade("SO_PlayerWidth", "ingame.player.width", "Wide Paddle",
            "Increase the player's horizontal catch width.", UpgradeEffectType.PlayerWidth,
            new[] { 3, 5, 7 }, new[] { 1.2f, 1.4f, 1.65f });
    }

    private static void CreateUpgrade(string assetName, string id, string displayName, string description,
        UpgradeEffectType effectType, int[] costs, float[] values)
    {
        string path = Root + "/Resources/Complete/Upgrades/" + assetName + ".asset";
        UpgradeDefinition definition = AssetDatabase.LoadAssetAtPath<UpgradeDefinition>(path);
        if (definition == null)
        {
            definition = ScriptableObject.CreateInstance<UpgradeDefinition>();
            AssetDatabase.CreateAsset(definition, path);
        }

        SerializedObject serialized = new SerializedObject(definition);
        serialized.FindProperty("upgradeId").stringValue = id;
        serialized.FindProperty("displayName").stringValue = displayName;
        serialized.FindProperty("description").stringValue = description;
        serialized.FindProperty("category").enumValueIndex = (int)UpgradeCategory.InGame;
        serialized.FindProperty("effectType").enumValueIndex = (int)effectType;
        serialized.FindProperty("maxLevel").intValue = values.Length;
        SetIntArray(serialized.FindProperty("costs"), costs);
        SetFloatArray(serialized.FindProperty("effectValues"), values);
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(definition);
    }

    private static void SetIntArray(SerializedProperty property, int[] values)
    {
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).intValue = values[i];
    }

    private static void SetFloatArray(SerializedProperty property, float[] values)
    {
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) property.GetArrayElementAtIndex(i).floatValue = values[i];
    }

    private static void EnsureActionReference(InputActionAsset asset, InputAction action)
    {
        bool exists = AssetDatabase.LoadAllAssetsAtPath(InputPath).OfType<InputActionReference>()
            .Any(reference => reference.action != null && reference.action.id == action.id);
        if (exists) return;

        InputActionReference reference = InputActionReference.Create(action);
        reference.name = action.actionMap.name + "_" + action.name;
        AssetDatabase.AddObjectToAsset(reference, asset);
        EditorUtility.SetDirty(reference);
    }

    private static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString("#" + value, out Color color);
        return color;
    }
}
