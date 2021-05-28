using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Handness
{ 
}

[RequireComponent(typeof(LineRenderer))]
public class VRLocomotion : MonoBehaviour
{


    [Header("Smooth Movement")]
    public Transform playerCamera;
    public float playerSpeed = 5f;

    [Header("Teleport")]
    public Transform xrRig;
    public string handness = "Right";

    [Header("Smooth Reticle")]
    public Transform teleportReticle;

    // Internal Vars
    private LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        InitializeLineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
        Rotate();
        SmoothMovement();
    }

    private void InitializeLineRenderer()
    {
        // Get Line Renderer
        lr = GetComponent<LineRenderer>();

        // Turn it off
        lr.enabled = false;

        // Set the total number of points in the line
        lr.positionCount = 2;

    }

    private void SmoothMovement()
    {
        Vector3 forwardDir = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z);
        Vector3 rightDir = new Vector3(playerCamera.right.x, 0, playerCamera.right.z);

        forwardDir.Normalize();
        rightDir.Normalize();

        // Forward and backwards
        transform.position +=
            -forwardDir *
            playerSpeed *
            Input.GetAxis("XRI_" + handness + "_Primary2DAxis_Vertical") *
            Time.deltaTime;

        // Right and Left
        transform.position +=
            rightDir *
            playerSpeed *
            Input.GetAxis("XRI_" + handness + "_Primary2DAxis_Horizontal") *
            Time.deltaTime;
    }

    private void HandleRaycast()
    {
        // Create RayCast
        Ray ray = new Ray(transform.position, transform.forward);
        //RaycastHit hitInfo = new RaycastHit();

        // If hit the raycast returns true
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Turn on Line Renderer and Reticle if we hit somthing
            lr.enabled = true;
            teleportReticle.gameObject.SetActive(true);

            // Check to see if hit valid target
            bool validTarget = hitInfo.collider.tag == "Ground";

            // Set color to blue if valid target, gray otherwise
            Color lrColor = validTarget ? Color.cyan : Color.gray;

            lr.startColor = lrColor;
            lr.endColor = lrColor;

            // Old Straight line set positions
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hitInfo.point);

            // Check for input if is a valid target
            if ( validTarget == true && Input.GetButtonDown(handness + "_Trigger"))
            {
                // Use coroutine to teleport user
                xrRig.position = hitInfo.point;
            }
        }
        // Code comes here if the raycast did not hit anythihng
        else
        {
            lr.enabled = false;
            teleportReticle.gameObject.SetActive(false);
        }
    }

    private void Rotate()
    {
        // Check if button is clicked
        if (Input.GetButtonDown(handness + "_StickClick"))
        {
            // Determine rotation direction
            float rot = Input.GetAxis(handness + "_Joystick") > 0 ? 30 : -30;

            // Rotate user
            xrRig.transform.Rotate(0, rot, 0);
        }
    }

}
