using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Cinemachine;
using TMPro;

public class GameController : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    [Header("Gameplay Stuff")]
    public Image[] crosshair = new Image[4];

    public CinemachineVirtualCamera vCam;
    public Canvas pauseCanvas;
    public Canvas hudCanvas;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sliderReadout;

    public CinemachinePOV povComp;
    //private MouseLook look;
    private PlayerAttack attack;

    private float uiBloomScalar = 9f;

    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    private void OnEnable()
    {
        Application.targetFrameRate = 300;
    }

    private void Start()
    {
        // Old camera system
        //look = MouseLook._instance;
        povComp = vCam.GetCinemachineComponent<CinemachinePOV>();
        attack = PlayerAttack._instance;
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    private void Update()
    {
        // ----- Pause stuff -----
        if (Input.GetKeyUp(KeyCode.P))
        {
            pauseCanvas.enabled = !pauseCanvas.enabled;
            hudCanvas.enabled = !hudCanvas.enabled;
            attack.enabled = !attack.enabled;
            //look.enabled = !look.enabled;
            if (pauseCanvas.enabled)
            {
                povComp.m_HorizontalAxis.m_MaxSpeed = 0;
                povComp.m_VerticalAxis.m_MaxSpeed = 0;
            }
            else
            {
                povComp.m_HorizontalAxis.m_MaxSpeed = 1.5f * sensitivitySlider.value;
                povComp.m_VerticalAxis.m_MaxSpeed = 1.5f * sensitivitySlider.value;
            }
            

            if (pauseCanvas.enabled)
            {
                //Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                //Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if (pauseCanvas.enabled)
            sliderReadout.text = sensitivitySlider.value.ToString("F2");

        // ----- Gameplay stuff -----
        float maxCrosshairGap = uiBloomScalar * attack.curWeapon.maxBloom;

        // This is the worst way to do anything ever. fix this shit.
        crosshair[0].rectTransform.localPosition = new Vector3(0, 5 + (maxCrosshairGap * (1 - attack.SetAccuracy)), 0);
        crosshair[1].rectTransform.localPosition = new Vector3(5 + (maxCrosshairGap * (1 - attack.SetAccuracy)), 0, 0);
        crosshair[2].rectTransform.localPosition = new Vector3(0, -5 - (maxCrosshairGap * (1 - attack.SetAccuracy)), 0);
        crosshair[3].rectTransform.localPosition = new Vector3(-5 - (maxCrosshairGap * (1 - attack.SetAccuracy)), 0, 0);
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------
    public void AdjustSensitivity()
    {
        // Old camera system
        //MouseLook.mouseSensitivity = 200f * sensitivitySlider.value;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
