using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public static MouseLook _instance;

    [Header("Camera Control Variables")]
    public static float mouseSensitivity;
    private float startingSensitivity = 100f;

    [Header("Cursor Variables")]
    public Texture2D cursorTexture;

    [Header("Object References")]
    public Transform playerRoot;
    public FirstPersonMovement playerMovement;

    private Quaternion currentCameraRotation;
    private float xRotation = 0f;

    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    private void OnEnable()
    {
        _instance = this;
    }

    private void Start()
    {
        mouseSensitivity = startingSensitivity;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.ForceSoftware);
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    private void Update()
    {
        //----------Regular Camera Rotation----------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        playerRoot.Rotate(Vector3.up * mouseX);

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        currentCameraRotation.eulerAngles = new Vector3(xRotation, currentCameraRotation.eulerAngles.y, currentCameraRotation.eulerAngles.z);
        transform.localRotation = currentCameraRotation;
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------


}
