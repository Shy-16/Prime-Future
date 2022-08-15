using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    private Canvas canvas;
    private Camera playerCam;

    //-------------------------------------------------------------------------
    // Initializations
    //-------------------------------------------------------------------------
    void Start()
    {
        canvas = GetComponent<Canvas>();
        playerCam = Camera.main;
        canvas.worldCamera = playerCam;
    }

    //-------------------------------------------------------------------------
    // Update
    //-------------------------------------------------------------------------
    void Update()
    {
        canvas.transform.rotation = playerCam.transform.rotation;
        canvas.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 2, (Vector3.Distance(playerCam.transform.position, transform.position) * .0625f) / 2);
    }

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------

}
