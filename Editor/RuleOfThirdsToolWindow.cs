
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class RuleOfThirdsToolWindow : EditorWindow
{
    [SerializeField]
    private Camera targetCamera;

    private string lastObservedDebugStatus;

    // ==========================================
    // MENU
    // ==========================================

    [MenuItem("Tools/Composition/Rule of Thirds")]
    private static void OpenWindow()
    {
        RuleOfThirdsToolWindow window =
            GetWindow<RuleOfThirdsToolWindow>();

        window.titleContent =
            new GUIContent("Composition Tools");

        window.minSize =
            new Vector2(340f, 430f);

        window.Show();
    }

    // ==========================================
    // INITIALIZATION
    // ==========================================

    private void OnEnable()
    {
        if (targetCamera == null)
            targetCamera = FindDefaultCamera();

        EditorApplication.update += CheckDebugStatus;
    }

    private void OnDisable()
    {
        EditorApplication.update -= CheckDebugStatus;
    }

    // ==========================================
    // DEBUG STATUS
    // ==========================================

    private void CheckDebugStatus()
    {
        if (targetCamera == null)
            return;

        RuleOfThirdsOverlay overlay =
            targetCamera.GetComponent<RuleOfThirdsOverlay>();

        string currentStatus =
            overlay != null
            ? overlay.LastDebugStatus
            : string.Empty;

        if (currentStatus != lastObservedDebugStatus)
        {
            lastObservedDebugStatus = currentStatus;
            Repaint();
        }
    }

    // ==========================================
    // EDITOR UI
    // ==========================================

    private void OnGUI()
    {
        EditorGUILayout.Space(8f);

        EditorGUILayout.LabelField(
            "Composition Tools",
            EditorStyles.boldLabel
        );

        EditorGUILayout.HelpBox(
            "Composition guides for the Unity Game View.",
            MessageType.Info
        );

        EditorGUILayout.Space(8f);

        // ======================================
        // CAMERA
        // ======================================

        targetCamera = (Camera)EditorGUILayout.ObjectField(
            "Target Camera",
            targetCamera,
            typeof(Camera),
            true
        );

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Find Main Camera"))
        {
            targetCamera = Camera.main != null
                ? Camera.main
                : FindDefaultCamera();
        }

        if (GUILayout.Button("Use Selected Camera"))
        {
            if (Selection.activeGameObject != null)
            {
                Camera selected =
                    Selection.activeGameObject.GetComponent<Camera>();

                if (selected != null)
                    targetCamera = selected;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (targetCamera == null)
        {
            EditorGUILayout.HelpBox(
                "Please assign a Camera.",
                MessageType.Warning
            );

            return;
        }

        EditorGUILayout.Space(8f);

        RuleOfThirdsOverlay overlay =
            targetCamera.GetComponent<RuleOfThirdsOverlay>();

        // ======================================
        // ENABLE TOOL
        // ======================================

        if (overlay == null)
        {
            if (GUILayout.Button(
                "Enable Composition Overlay",
                GUILayout.Height(32f)))
            {
                overlay =
                    Undo.AddComponent<RuleOfThirdsOverlay>(
                        targetCamera.gameObject
                    );

                if (overlay != null)
                {
                    overlay.DebugLogs = false;

                    Debug.Log(
                        "[RuleOfThirds] Composition overlay enabled.",
                        targetCamera
                    );
                }

                RefreshViews();
            }

            return;
        }

        // ======================================
        // GENERAL SETTINGS
        // ======================================

        EditorGUILayout.LabelField(
            "General Settings",
            EditorStyles.boldLabel
        );

        EditorGUI.BeginChangeCheck();

        bool overlayEnabled = EditorGUILayout.Toggle(
            "Enable Overlay",
            overlay.enabled
        );

        EditorGUILayout.Space(5f);

        // ======================================
        // RULE OF THIRDS
        // ======================================

        EditorGUILayout.LabelField(
            "Rule of Thirds",
            EditorStyles.boldLabel
        );

        bool showRuleOfThirds = EditorGUILayout.Toggle(
            "Enable Rule of Thirds",
        overlay.ShowRuleOfThirds
);

        Color lineColor = EditorGUILayout.ColorField(
            "Line Color",
            overlay.LineColor
        );

        float lineThickness = EditorGUILayout.Slider(
            "Line Thickness",
            overlay.LineThickness,
            0.5f,
            10f
        );

        // ======================================
        // LETTERBOX
        // ======================================

        EditorGUILayout.Space(8f);

        EditorGUILayout.LabelField(
            "Letterbox",
            EditorStyles.boldLabel
        );

        bool showLetterbox = EditorGUILayout.Toggle(
            "Enable Letterbox",
            overlay.ShowLetterbox
        );

        using (new EditorGUI.DisabledScope(!showLetterbox))
        {
            float aspectRatio = EditorGUILayout.Slider(
                "Aspect Ratio",
                overlay.LetterboxAspectRatio,
                1f,
                3.5f
            );

            // Common presets.
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("16:9"))
                aspectRatio = 16f / 9f;

            if (GUILayout.Button("1.85:1"))
                aspectRatio = 1.85f;

            if (GUILayout.Button("2.35:1"))
                aspectRatio = 2.35f;

            if (GUILayout.Button("2.39:1"))
                aspectRatio = 2.39f;

            EditorGUILayout.EndHorizontal();

            overlay.LetterboxAspectRatio = aspectRatio;

            Color letterboxColor = EditorGUILayout.ColorField(
                "Bar Color",
                overlay.LetterboxColor
            );

            overlay.LetterboxColor = letterboxColor;
        }

        // ======================================
        // ACTION SAFE
        // ======================================

        EditorGUILayout.Space(8f);

        EditorGUILayout.LabelField(
            "Action Safe",
            EditorStyles.boldLabel
        );

        bool showActionSafe = EditorGUILayout.Toggle(
            "Enable Action Safe",
            overlay.ShowActionSafe
        );

        float actionSafeSize =
            overlay.ActionSafeSize * 100f;

        Color actionSafeColor =
            overlay.ActionSafeColor;

        using (new EditorGUI.DisabledScope(!showActionSafe))
        {
            actionSafeSize = EditorGUILayout.Slider(
                "Safe Area (%)",
                actionSafeSize,
                50f,
                100f
            );

            actionSafeColor = EditorGUILayout.ColorField(
                "Guide Color",
                actionSafeColor
            );
        }

        // ======================================
        // DEBUG
        // ======================================

        EditorGUILayout.Space(8f);

        EditorGUILayout.LabelField(
            "Debug",
            EditorStyles.boldLabel
        );

        bool debugLogs = EditorGUILayout.Toggle(
            "Debug Logs",
            overlay.DebugLogs
        );

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(
                overlay,
                "Change Composition Settings"
            );

            overlay.enabled = overlayEnabled;

            overlay.ShowRuleOfThirds = showRuleOfThirds;
            overlay.LineColor = lineColor;
            overlay.LineThickness = lineThickness;

            overlay.ShowLetterbox = showLetterbox;

            overlay.ShowActionSafe = showActionSafe;
            overlay.ActionSafeSize = actionSafeSize / 100f;
            overlay.ActionSafeColor = actionSafeColor;

            overlay.DebugLogs = debugLogs;

            EditorUtility.SetDirty(overlay);

            RefreshViews();
        }

        // ======================================
        // DEBUG STATUS
        // ======================================

        EditorGUILayout.Space(4f);

        EditorGUILayout.HelpBox(
            overlay.LastDebugStatus,
            overlay.LastDebugStatus.Contains("DRAWN")
                ? MessageType.Info
                : MessageType.Warning
        );

        // ======================================
        // ACTION BUTTONS
        // ======================================

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Refresh"))
            RefreshViews();

        if (GUILayout.Button("Select Camera"))
        {
            Selection.activeGameObject =
                targetCamera.gameObject;

            EditorGUIUtility.PingObject(
                targetCamera.gameObject
            );
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5f);

        if (GUILayout.Button("Remove Composition Overlay"))
        {
            Undo.DestroyObjectImmediate(overlay);

            RefreshViews();
        }
    }

    // ==========================================
    // FIND CAMERA
    // ==========================================

    private static Camera FindDefaultCamera()
    {
        if (Selection.activeGameObject != null)
        {
            Camera selected =
                Selection.activeGameObject.GetComponent<Camera>();

            if (selected != null)
                return selected;
        }

        if (Camera.main != null)
            return Camera.main;

        return Object.FindFirstObjectByType<Camera>();
    }

    // ==========================================
    // REFRESH EDITOR
    // ==========================================

    private static void RefreshViews()
    {
        EditorApplication.QueuePlayerLoopUpdate();

        SceneView.RepaintAll();
    }
}

#endif
