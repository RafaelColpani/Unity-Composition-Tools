
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[DisallowMultipleComponent]
[AddComponentMenu("")]
public sealed class RuleOfThirdsOverlay : MonoBehaviour
{
    [Header("Rule of Thirds")]
    [SerializeField]
    private bool showRuleOfThirds = false;
    private Color lineColor = new Color(1f, 1f, 1f, 0.8f);

    [SerializeField, Range(0.5f, 10f)]
    private float lineThickness = 5f;

    [Header("Letterbox")]
    [SerializeField]
    private bool showLetterbox = false;

    [SerializeField, Range(1f, 3.5f)]
    private float letterboxAspectRatio = 2.39f;

    [SerializeField]
    private Color letterboxColor = new Color(0f, 0f, 0f, 0.75f);

    [Header("Action Safe")]
    [SerializeField]
    private bool showActionSafe = false;

    [SerializeField, Range(0.5f, 1f)]
    private float actionSafeSize = 0.95f;

    [SerializeField]
    private Color actionSafeColor =
        new Color(1f, 0f, 0f, 0.9f);

    [Header("Debug")]
    [SerializeField]
    private bool debugLogs = false;

#if UNITY_EDITOR

    private Camera targetCamera;
    private Material lineMaterial;

    private string lastDebugStatus = "Waiting for camera render callback...";

    private bool hasLoggedFirstCallback;
    private bool hasLoggedWrongGameCamera;
    private bool hasLoggedMissingShader;

    // ==========================================
    // PUBLIC PROPERTIES
    // ==========================================
    public bool ShowRuleOfThirds
    {
        get => showRuleOfThirds;
        set => showRuleOfThirds = value;
    }


    public Color LineColor
    {
        get => lineColor;
        set => lineColor = value;
    }

    public float LineThickness
    {
        get => lineThickness;
        set => lineThickness = Mathf.Clamp(value, 0.5f, 10f);
    }

    public bool ShowLetterbox
    {
        get => showLetterbox;
        set => showLetterbox = value;
    }

    public float LetterboxAspectRatio
    {
        get => letterboxAspectRatio;
        set => letterboxAspectRatio = Mathf.Clamp(value, 1f, 3.5f);
    }

    public Color LetterboxColor
    {
        get => letterboxColor;
        set => letterboxColor = value;
    }

    public bool ShowActionSafe
    {
        get => showActionSafe;
        set => showActionSafe = value;
    }

    public float ActionSafeSize
    {
        get => actionSafeSize;
        set => actionSafeSize = Mathf.Clamp(value, 0.5f, 1f);
    }

    public Color ActionSafeColor
    {
        get => actionSafeColor;
        set => actionSafeColor = value;
    }

    public bool DebugLogs
    {
        get => debugLogs;
        set => debugLogs = value;
    }

    public string LastDebugStatus => lastDebugStatus;

    // ==========================================
    // INITIALIZATION
    // ==========================================

    private void OnEnable()
    {
        targetCamera = GetComponent<Camera>();

        Camera.onPostRender += HandleBuiltInPostRender;

        RenderPipelineManager.endCameraRendering +=
            HandleSrpEndCameraRendering;

        DebugLog("Overlay enabled.");

        if (targetCamera == null)
        {
            SetStatus("ERROR: Camera component not found.");

            Debug.LogError(
                "[RuleOfThirds] Camera component not found.",
                this
            );
        }
    }

    private void OnDisable()
    {
        Camera.onPostRender -= HandleBuiltInPostRender;

        RenderPipelineManager.endCameraRendering -=
            HandleSrpEndCameraRendering;

        if (lineMaterial != null)
        {
            DestroyImmediate(lineMaterial);
            lineMaterial = null;
        }

        SetStatus("Overlay disabled.");

        DebugLog("Overlay disabled.");
    }

    // ==========================================
    // BUILT-IN PIPELINE
    // ==========================================

    private void HandleBuiltInPostRender(Camera camera)
    {
        if (GraphicsSettings.currentRenderPipeline != null)
            return;

        TryDrawGuides(camera, "Built-in");
    }

    // ==========================================
    // URP / HDRP PIPELINE
    // ==========================================

    private void HandleSrpEndCameraRendering(
        ScriptableRenderContext context,
        Camera camera)
    {
        if (GraphicsSettings.currentRenderPipeline == null)
            return;

        TryDrawGuides(camera, "SRP");
    }

    // ==========================================
    // VALIDATE CAMERA
    // ==========================================

    private void TryDrawGuides(
        Camera camera,
        string pipeline)
    {
        if (!enabled || !gameObject.activeInHierarchy)
            return;

        if (camera == null)
            return;

        if (camera.cameraType != CameraType.Game)
            return;

        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        if (targetCamera == null || camera != targetCamera)
        {
            if (!hasLoggedWrongGameCamera && debugLogs)
            {
                hasLoggedWrongGameCamera = true;

                Debug.LogWarning(
                    "[RuleOfThirds] Another Game camera rendered. " +
                    "Check Target Camera.",
                    this
                );
            }

            return;
        }

        if (!hasLoggedFirstCallback)
        {
            hasLoggedFirstCallback = true;

            DebugLog(
                $"Render callback received. Pipeline: {pipeline}, " +
                $"Camera: {camera.name}, " +
                $"Resolution: {camera.pixelWidth}x{camera.pixelHeight}"
            );
        }

        if (DrawGuides(camera))
        {
            SetStatus(
                $"Overlay DRAWN | Pipeline: {pipeline} | " +
                $"Camera: {camera.name}"
            );
        }
    }

    // ==========================================
    // CALCULATE LETTERBOX FRAME
    // ==========================================

    private Rect GetContentFrame(Camera camera)
    {
        // Full camera area when Letterbox is disabled.
        if (!showLetterbox)
            return new Rect(0f, 0f, 1f, 1f);

        float screenAspect =
            (float)camera.pixelWidth /
            Mathf.Max(1, camera.pixelHeight);

        float targetAspect = letterboxAspectRatio;

        // Target aspect is wider than the Game View.
        // Create horizontal cinematic bars.
        if (targetAspect > screenAspect)
        {
            float frameHeight = screenAspect / targetAspect;

            float yOffset = (1f - frameHeight) * 0.5f;

            return new Rect(
                0f,
                yOffset,
                1f,
                frameHeight
            );
        }

        // Target aspect is narrower than the Game View.
        // Create vertical bars.
        float frameWidth = targetAspect / screenAspect;

        float xOffset = (1f - frameWidth) * 0.5f;

        return new Rect(
            xOffset,
            0f,
            frameWidth,
            1f
        );
    }

    // ==========================================
    // DRAW GUIDES
    // ==========================================

    private bool DrawGuides(Camera camera)
    {
        if (lineMaterial == null)
            lineMaterial = CreateLineMaterial();

        if (lineMaterial == null)
            return false;

        if (!lineMaterial.SetPass(0))
        {
            SetStatus("ERROR: Material.SetPass failed.");
            return false;
        }

        int width = Mathf.Max(1, camera.pixelWidth);
        int height = Mathf.Max(1, camera.pixelHeight);

        float thicknessX = lineThickness / width;
        float thicknessY = lineThickness / height;

        float halfThicknessX = thicknessX * 0.5f;
        float halfThicknessY = thicknessY * 0.5f;

        // Active image area, accounting for Letterbox.
        Rect frame = GetContentFrame(camera);

        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);

        // ======================================
        // RULE OF THIRDS
        // ======================================

        if (showRuleOfThirds)
        {
            GL.Color(lineColor);

            float vertical1 = frame.x + frame.width / 3f;
            float vertical2 = frame.x + frame.width * 2f / 3f;

            float horizontal1 = frame.y + frame.height / 3f;
            float horizontal2 = frame.y + frame.height * 2f / 3f;

            DrawRect(
                vertical1 - halfThicknessX,
                frame.y,
                vertical1 + halfThicknessX,
                frame.yMax
            );

            DrawRect(
                vertical2 - halfThicknessX,
                frame.y,
                vertical2 + halfThicknessX,
                frame.yMax
            );

            DrawRect(
                frame.x,
                horizontal1 - halfThicknessY,
                frame.xMax,
                horizontal1 + halfThicknessY
            );

            DrawRect(
                frame.x,
                horizontal2 - halfThicknessY,
                frame.xMax,
                horizontal2 + halfThicknessY
            );
        }

        // ======================================
        // ACTION SAFE
        // ======================================

        if (showActionSafe)
        {
            GL.Color(actionSafeColor);

            // Calculate the rectangular safe area.
            // The percentage applies independently
            // to the frame width and height.

            float insetX =
                frame.width * (1f - actionSafeSize) * 0.5f;

            float insetY =
                frame.height * (1f - actionSafeSize) * 0.5f;

            float xMin = frame.xMin + insetX;
            float xMax = frame.xMax - insetX;

            float yMin = frame.yMin + insetY;
            float yMax = frame.yMax - insetY;

            DrawRectBorder(
                xMin,
                yMin,
                xMax,
                yMax,
                thicknessX,
                thicknessY
            );
        }
        // ======================================
        // LETTERBOX BARS
        // ======================================

        if (showLetterbox)
        {
            GL.Color(letterboxColor);

            // Bottom bar.
            if (frame.yMin > 0f)
            {
                DrawRect(
                    0f,
                    0f,
                    1f,
                    frame.yMin
                );
            }

            // Top bar.
            if (frame.yMax < 1f)
            {
                DrawRect(
                    0f,
                    frame.yMax,
                    1f,
                    1f
                );
            }

            // Left bar.
            if (frame.xMin > 0f)
            {
                DrawRect(
                    0f,
                    frame.yMin,
                    frame.xMin,
                    frame.yMax
                );
            }

            // Right bar.
            if (frame.xMax < 1f)
            {
                DrawRect(
                    frame.xMax,
                    frame.yMin,
                    1f,
                    frame.yMax
                );
            }
        }

        GL.End();

        GL.PopMatrix();

        return true;
    }

    // ==========================================
    // DRAW RECTANGLE
    // ==========================================

    private static void DrawRect(
        float xMin,
        float yMin,
        float xMax,
        float yMax)
    {
        GL.Vertex3(xMin, yMin, 0f);
        GL.Vertex3(xMax, yMin, 0f);
        GL.Vertex3(xMax, yMax, 0f);
        GL.Vertex3(xMin, yMax, 0f);
    }

    // ==========================================
    // DRAW RECTANGLE BORDER
    // ==========================================

    private static void DrawRectBorder(
        float xMin,
        float yMin,
        float xMax,
        float yMax,
        float thicknessX,
        float thicknessY)
    {
        // Top.
        DrawRect(
            xMin,
            yMax - thicknessY,
            xMax,
            yMax
        );

        // Bottom.
        DrawRect(
            xMin,
            yMin,
            xMax,
            yMin + thicknessY
        );

        // Left.
        DrawRect(
            xMin,
            yMin,
            xMin + thicknessX,
            yMax
        );

        // Right.
        DrawRect(
            xMax - thicknessX,
            yMin,
            xMax,
            yMax
        );
    }

    // ==========================================
    // CREATE MATERIAL
    // ==========================================

    private Material CreateLineMaterial()
    {
        Shader shader =
            Shader.Find("Hidden/Internal-Colored");

        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        if (shader == null)
        {
            if (!hasLoggedMissingShader)
            {
                hasLoggedMissingShader = true;

                Debug.LogError(
                    "[RuleOfThirds] No compatible Shader found.",
                    this
                );
            }

            return null;
        }

        Material material = new Material(shader)
        {
            hideFlags = HideFlags.HideAndDontSave
        };

        material.SetInt(
            "_SrcBlend",
            (int)BlendMode.SrcAlpha
        );

        material.SetInt(
            "_DstBlend",
            (int)BlendMode.OneMinusSrcAlpha
        );

        material.SetInt(
            "_Cull",
            (int)CullMode.Off
        );

        material.SetInt("_ZWrite", 0);

        material.SetInt(
            "_ZTest",
            (int)CompareFunction.Always
        );

        DebugLog($"Material created: {shader.name}");

        return material;
    }

    // ==========================================
    // DEBUG
    // ==========================================

    private void SetStatus(string message)
    {
        lastDebugStatus = message;
    }

    private void DebugLog(string message)
    {
        if (debugLogs)
        {
            Debug.Log(
                "[RuleOfThirds] " + message,
                this
            );
        }
    }

#endif
}
