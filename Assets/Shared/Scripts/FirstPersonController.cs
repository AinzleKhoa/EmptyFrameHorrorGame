// CHANGE LOG
// 
// CHANGES || version 1.0.2
// "Fixed Rigidbody.velocity obsolete warning by using linearVelocity. Updated to New Input System."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Added for Modern Input

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{
    private Rigidbody rb;

    #region Camera Movement Variables
    public Camera playerCamera;
    public float fov = 60f;
    public bool invertCamera = false;
    public bool cameraCanMove = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;

    // Crosshair
    public bool lockCursor = true;
    public bool crosshair = true;
    public Sprite crosshairImage;
    public Color crosshairColor = Color.white;

    // Internal Variables
    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Image crosshairObject;

    #region Camera Zoom Variables
    public bool enableZoom = true;
    public bool holdToZoom = false;
    public KeyCode zoomKey = KeyCode.Mouse1; // Note: KeyCode still used for Editor, but logic uses InputSystem
    public float zoomFOV = 30f;
    public float zoomStepTime = 5f;
    private bool isZoomed = false;
    #endregion
    #endregion

    #region Movement Variables
    public bool playerCanMove = true;
    public float walkSpeed = 5f;
    public float maxVelocityChange = 10f;
    private bool isWalking = false;

    #region Sprint
    public bool enableSprint = true;
    public bool unlimitedSprint = false;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float sprintSpeed = 7f;
    public float sprintDuration = 5f;
    public float sprintCooldown = .5f;
    public float sprintFOV = 80f;
    public float sprintFOVStepTime = 10f;

    // Sprint Bar
    public bool useSprintBar = true;
    public bool hideBarWhenFull = true;
    public Image sprintBarBG;
    public Image sprintBar;
    public float sprintBarWidthPercent = .3f;
    public float sprintBarHeightPercent = .015f;

    private CanvasGroup sprintBarCG;
    private bool isSprinting = false;
    private float sprintRemaining;
    private float sprintBarWidth;
    private float sprintBarHeight;
    private bool isSprintCooldown = false;
    private float sprintCooldownReset;
    #endregion

    #region Jump
    public bool enableJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpPower = 5f;
    private bool isGrounded = false;
    #endregion

    #region Crouch
    public bool enableCrouch = true;
    public bool holdToCrouch = true;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchHeight = .75f;
    public float speedReduction = .5f;
    private bool isCrouched = false;
    private Vector3 originalScale;
    #endregion
    #endregion

    #region Head Bob
    public bool enableHeadBob = true;
    public Transform joint;
    public float bobSpeed = 10f;
    public Vector3 bobAmount = new Vector3(.15f, .05f, 0f);
    private Vector3 jointOriginalPos;
    private float timer = 0;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Essential for FP Controllers

        crosshairObject = GetComponentInChildren<Image>();

        playerCamera.fieldOfView = fov;
        originalScale = transform.localScale;
        jointOriginalPos = joint.localPosition;

        if (!unlimitedSprint)
        {
            sprintRemaining = sprintDuration;
            sprintCooldownReset = sprintCooldown;
        }
    }

    void Start()
    {
        if (lockCursor) Cursor.lockState = CursorLockMode.Locked;

        if (crosshair)
        {
            if (crosshairObject != null)
            {
                crosshairObject.sprite = crosshairImage;
                crosshairObject.color = crosshairColor;
            }
        }
        else if (crosshairObject != null)
        {
            crosshairObject.gameObject.SetActive(false);
        }

        #region Sprint Bar Setup
        sprintBarCG = GetComponentInChildren<CanvasGroup>();
        if (useSprintBar && sprintBarBG != null && sprintBar != null)
        {
            sprintBarBG.gameObject.SetActive(true);
            sprintBar.gameObject.SetActive(true);

            sprintBarWidth = Screen.width * sprintBarWidthPercent;
            sprintBarHeight = Screen.height * sprintBarHeightPercent;

            sprintBarBG.rectTransform.sizeDelta = new Vector3(sprintBarWidth, sprintBarHeight, 0f);
            sprintBar.rectTransform.sizeDelta = new Vector3(sprintBarWidth - 2, sprintBarHeight - 2, 0f);

            if (hideBarWhenFull && sprintBarCG != null) sprintBarCG.alpha = 0;
        }
        #endregion
    }

    private void Update()
    {
        #region Camera Logic (Modern Input)
        if (cameraCanMove)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw = transform.localEulerAngles.y + mouseDelta.x * mouseSensitivity * 0.1f;

            if (!invertCamera) pitch -= mouseDelta.y * mouseSensitivity * 0.1f;
            else pitch += mouseDelta.y * mouseSensitivity * 0.1f;

            pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

            transform.localEulerAngles = new Vector3(0, yaw, 0);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
        #endregion

        #region Zoom Logic
        if (enableZoom)
        {
            bool zoomInput = Mouse.current.rightButton.isPressed;

            if (holdToZoom) isZoomed = zoomInput && !isSprinting;
            else if (Mouse.current.rightButton.wasPressedThisFrame && !isSprinting) isZoomed = !isZoomed;

            float targetFOV = isZoomed ? zoomFOV : (isSprinting ? sprintFOV : fov);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, zoomStepTime * Time.deltaTime);
        }
        #endregion

        #region Sprint Logic
        if (enableSprint)
        {
            if (isSprinting)
            {
                isZoomed = false;
                if (!unlimitedSprint)
                {
                    sprintRemaining -= Time.deltaTime;
                    if (sprintRemaining <= 0) { isSprinting = false; isSprintCooldown = true; }
                }
            }
            else
            {
                sprintRemaining = Mathf.Clamp(sprintRemaining + Time.deltaTime, 0, sprintDuration);
            }

            if (isSprintCooldown)
            {
                sprintCooldown -= Time.deltaTime;
                if (sprintCooldown <= 0) isSprintCooldown = false;
            }
            else sprintCooldown = sprintCooldownReset;

            if (useSprintBar && !unlimitedSprint && sprintBar != null)
            {
                sprintBar.transform.localScale = new Vector3(sprintRemaining / sprintDuration, 1f, 1f);
            }
        }
        #endregion

        #region Jump & Crouch Input
        if (enableJump && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded) Jump();

        if (enableCrouch)
        {
            if (holdToCrouch)
            {
                if (Keyboard.current.leftCtrlKey.wasPressedThisFrame) { isCrouched = false; Crouch(); }
                else if (Keyboard.current.leftCtrlKey.wasReleasedThisFrame) { isCrouched = true; Crouch(); }
            }
            else if (Keyboard.current.leftCtrlKey.wasPressedThisFrame) Crouch();
        }
        #endregion

        CheckGround();
        if (enableHeadBob) HeadBob();
    }

    void FixedUpdate()
    {
        #region Movement Logic (Using linearVelocity)
        if (playerCanMove)
        {
            Vector2 moveInput = new Vector2(
                (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0)
            );

            isWalking = moveInput.magnitude > 0 && isGrounded;
            bool isSprintingInput = Keyboard.current.leftShiftKey.isPressed && isWalking && sprintRemaining > 0 && !isSprintCooldown;

            float currentSpeed = isSprintingInput ? sprintSpeed : walkSpeed;
            isSprinting = isSprintingInput;

            Vector3 targetVelocity = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y)) * currentSpeed;

            // FIX: Using linearVelocity instead of velocity
            Vector3 velocity = rb.linearVelocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Handle Sprint Bar Alpha
            if (hideBarWhenFull && useSprintBar && sprintBarCG != null)
            {
                if (isSprinting) sprintBarCG.alpha = Mathf.MoveTowards(sprintBarCG.alpha, 1, 5 * Time.deltaTime);
                else if (sprintRemaining >= sprintDuration) sprintBarCG.alpha = Mathf.MoveTowards(sprintBarCG.alpha, 0, 3 * Time.deltaTime);
            }
        }
        #endregion
    }

    private void CheckGround()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
        isGrounded = Physics.Raycast(origin, Vector3.down, out _, 0.75f);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(0f, jumpPower, 0f, ForceMode.Impulse);
            isGrounded = false;
        }
        if (isCrouched && !holdToCrouch) Crouch();
    }

    private void Crouch()
    {
        if (isCrouched)
        {
            transform.localScale = originalScale;
            walkSpeed /= speedReduction;
            isCrouched = false;
        }
        else
        {
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);
            walkSpeed *= speedReduction;
            isCrouched = true;
        }
    }

    private void HeadBob()
    {
        if (isWalking)
        {
            float speedMultiplier = isSprinting ? (bobSpeed + sprintSpeed) : (isCrouched ? bobSpeed * speedReduction : bobSpeed);
            timer += Time.deltaTime * speedMultiplier;
            joint.localPosition = jointOriginalPos + new Vector3(Mathf.Sin(timer) * bobAmount.x, Mathf.Sin(timer) * bobAmount.y, Mathf.Sin(timer) * bobAmount.z);
        }
        else
        {
            timer = 0;
            joint.localPosition = Vector3.Lerp(joint.localPosition, jointOriginalPos, Time.deltaTime * bobSpeed);
        }
    }
}

#region Custom Editor
#if UNITY_EDITOR
[CustomEditor(typeof(FirstPersonController)), InitializeOnLoadAttribute]
public class FirstPersonControllerEditor : Editor
{
    FirstPersonController fpc;
    SerializedObject SerFPC;

    private void OnEnable()
    {
        fpc = (FirstPersonController)target;
        SerFPC = new SerializedObject(fpc);
    }

    public override void OnInspectorGUI()
    {
        SerFPC.Update();

        EditorGUILayout.Space();
        GUILayout.Label("Modular First Person Controller", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("Fixed & Modernized version 1.0.2", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

        #region Camera Setup
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Camera Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 });
        fpc.playerCamera = (Camera)EditorGUILayout.ObjectField("Camera", fpc.playerCamera, typeof(Camera), true);
        fpc.fov = EditorGUILayout.Slider("Field of View", fpc.fov, 10, 120);
        fpc.cameraCanMove = EditorGUILayout.ToggleLeft("Enable Camera Rotation", fpc.cameraCanMove);

        if (fpc.cameraCanMove)
        {
            EditorGUI.indentLevel++;
            fpc.invertCamera = EditorGUILayout.ToggleLeft("Invert Y Axis", fpc.invertCamera);
            fpc.mouseSensitivity = EditorGUILayout.Slider("Sensitivity", fpc.mouseSensitivity, 0.1f, 10f);
            fpc.maxLookAngle = EditorGUILayout.Slider("Max Look Angle", fpc.maxLookAngle, 40, 90);
            EditorGUI.indentLevel--;
        }

        fpc.lockCursor = EditorGUILayout.ToggleLeft("Lock Cursor", fpc.lockCursor);
        fpc.crosshair = EditorGUILayout.ToggleLeft("Auto Crosshair", fpc.crosshair);
        if (fpc.crosshair)
        {
            EditorGUI.indentLevel++;
            fpc.crosshairImage = (Sprite)EditorGUILayout.ObjectField("Sprite", fpc.crosshairImage, typeof(Sprite), false);
            fpc.crosshairColor = EditorGUILayout.ColorField("Color", fpc.crosshairColor);
            EditorGUI.indentLevel--;
        }
        #endregion

        #region Movement Setup
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Movement Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 });

        fpc.playerCanMove = EditorGUILayout.ToggleLeft("Enable Movement", fpc.playerCanMove);
        if (fpc.playerCanMove)
        {
            fpc.walkSpeed = EditorGUILayout.Slider("Walk Speed", fpc.walkSpeed, 1, 20);

            EditorGUILayout.Space();
            GUILayout.Label("Sprint Settings", EditorStyles.boldLabel);
            fpc.enableSprint = EditorGUILayout.ToggleLeft("Enable Sprint", fpc.enableSprint);
            if (fpc.enableSprint)
            {
                EditorGUI.indentLevel++;
                fpc.unlimitedSprint = EditorGUILayout.ToggleLeft("Unlimited Sprint", fpc.unlimitedSprint);
                fpc.sprintSpeed = EditorGUILayout.Slider("Sprint Speed", fpc.sprintSpeed, fpc.walkSpeed, 30);
                if (!fpc.unlimitedSprint)
                {
                    fpc.sprintDuration = EditorGUILayout.Slider("Duration", fpc.sprintDuration, 1, 20);
                    fpc.sprintCooldown = EditorGUILayout.Slider("Cooldown", fpc.sprintCooldown, 0.1f, 10);
                }
                fpc.useSprintBar = EditorGUILayout.ToggleLeft("Use UI Bar", fpc.useSprintBar);
                if (fpc.useSprintBar)
                {
                    fpc.sprintBarBG = (Image)EditorGUILayout.ObjectField("Bar BG", fpc.sprintBarBG, typeof(Image), true);
                    fpc.sprintBar = (Image)EditorGUILayout.ObjectField("Bar Fill", fpc.sprintBar, typeof(Image), true);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            GUILayout.Label("Jump & Crouch", EditorStyles.boldLabel);
            fpc.enableJump = EditorGUILayout.ToggleLeft("Enable Jump", fpc.enableJump);
            if (fpc.enableJump) fpc.jumpPower = EditorGUILayout.Slider("Jump Power", fpc.jumpPower, 1, 20);

            fpc.enableCrouch = EditorGUILayout.ToggleLeft("Enable Crouch", fpc.enableCrouch);
            if (fpc.enableCrouch)
            {
                EditorGUI.indentLevel++;
                fpc.holdToCrouch = EditorGUILayout.ToggleLeft("Hold To Crouch", fpc.holdToCrouch);
                fpc.crouchHeight = EditorGUILayout.Slider("Crouch Height", fpc.crouchHeight, 0.1f, 1f);
                fpc.speedReduction = EditorGUILayout.Slider("Speed %", fpc.speedReduction, 0.1f, 1f);
                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Head Bob
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Label("Head Bob Setup", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 13 });
        fpc.enableHeadBob = EditorGUILayout.ToggleLeft("Enable Head Bob", fpc.enableHeadBob);
        if (fpc.enableHeadBob)
        {
            fpc.joint = (Transform)EditorGUILayout.ObjectField("Camera Joint", fpc.joint, typeof(Transform), true);
            fpc.bobSpeed = EditorGUILayout.Slider("Bob Speed", fpc.bobSpeed, 1, 30);
            fpc.bobAmount = EditorGUILayout.Vector3Field("Bob Amount", fpc.bobAmount);
        }
        #endregion

        if (GUI.changed)
        {
            EditorUtility.SetDirty(fpc);
            Undo.RecordObject(fpc, "FPC Change");
            SerFPC.ApplyModifiedProperties();
        }
    }
}
#endif
#endregion