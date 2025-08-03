using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    [SerializeField, Tooltip("The camera that is parented to this object.")]
    private GameObject attatchedCamera;

    [Header("-- Pivot Position Settings --")]
    [SerializeField, Tooltip("The distance above the ground that the pivot hovers.")]
    private float pivotHeight = 5;
    [SerializeField, Tooltip("The speed the camera moves when controlled by the keyboard.")]
    private float moveSpeed = 1;
    [SerializeField, Tooltip("The amount the camera lerps when controlled by the mouse.")]
    private float cameraLerpSpeed = 5f;
    [SerializeField, Tooltip("The starting position of the pivot object")]
    private Vector2 defaultPivotPostion = Vector2.zero;

    //when using the mouse to travel across the terrain, this is set over which the mouse button is clicked
    private Vector3 mouseDragAnchor;
    
    [Header("-- Pivot Boundaries --")]
    [SerializeField, Tooltip("The far end of the Boundary box.")]
    Vector2 maxBoundaryCorner = new Vector2(100, 100);
    [SerializeField, Tooltip("The other end of the boundary box. Both values should be less than the corresponding value in Max Boundary corner.")]
    Vector2 minBoundaryCorner = new Vector2(-100, -100);

    [Header("-- Pivot Rotation Settings --")]
    [SerializeField, Tooltip("The Maximum angle from the ground.")]
    private float maxAngle = 89.99f;
    [SerializeField, Tooltip("The Minimum angle from the ground.")]
    private float minAngle = 10;
    [SerializeField, Tooltip("The Starting out angle of the pivot.")]
    private float defaultAngle = 45;

    [Header("-- Camera Distance Settings --")]
    [SerializeField, Tooltip("The closest the camera can be to the pivot")]
    private float MinDistance = 10;
    [SerializeField, Tooltip("The furthest the camera can be to the pivot")]
    private float MaxDistance = 500;
    [SerializeField, Tooltip("The camera's starting out distance from the pivot")]
    private float DefaultDistance = 100f;
    [SerializeField, Tooltip("Determines how fast zooming in and out is.")]
    private float zoomAmount = 5;

    //the distance away from the pivot that the camera lerps toward. It changes;
    private float targetCameraDistance = 100;
    
    [Header("-- Reset Settings --")]
    [SerializeField, Tooltip("The key to press to reset the camera's position and rotation.")]
    private KeyCode resetKey = KeyCode.R;
    [SerializeField, Tooltip("How quickly the camera eases back to its default state.")]
    private float resetSpeed = 2f;

    // A reference to the active reset coroutine.
    private Coroutine resetCoroutine;
    
    // The initial state of the pivot captured on start.
    private Quaternion initialRotation;
    
    #region Pivot Movement Function methods
        
    private void Start()
    {
        // Capture the state of the pivot rotation when the game begins.
        initialRotation = transform.rotation;

        // Set the starting zoom distance.
        targetCameraDistance = DefaultDistance;
    }

    /// <summary>
    /// Converts a vector 2 into a point in space on the horizontal plane at y = 0.
    /// </summary>
    private Vector3 ConvertToVector3(Vector2 input)
    {
        return new Vector3(input.x, 0, input.y);
    }
        
    /// <summary>
    /// Moves the pivot point along the horizontal plane by the specified offset.
    /// </summary>
    /// <param name="_offset">The amount to move the pivot by</param>
    void Move(Vector2 _offset)
    {
        Vector3 targetPosition = transform.position + ConvertToVector3(_offset);
        gameObject.transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraLerpSpeed);
    }

    /// <summary>
    /// Rotates the pivot using eulerAngles.
    /// </summary>
    /// <param name="_rotationOffset">subtracts y from pivot's y rotation and adds x to pivot's x rotation.</param>
    void Rotate(Vector2 _rotationOffset)
    {
        Vector3 rotateValue = new Vector3(-_rotationOffset.y, _rotationOffset.x, 0);
        transform.eulerAngles += rotateValue;
    }

    /// <summary>
    /// Moves the pivot's position and rotation to fit the boundaries and the max and min variables.
    /// </summary>
    void EnforceBoundaries()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minBoundaryCorner.x, maxBoundaryCorner.x), transform.position.y, Mathf.Clamp(transform.position.z, minBoundaryCorner.y, maxBoundaryCorner.y));
        transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x, minAngle, maxAngle), transform.eulerAngles.y, transform.eulerAngles.z);
    }
    #endregion

    private void OnValidate()
    {
        gameObject.transform.rotation = Quaternion.Euler(Vector3.right * defaultAngle);
        gameObject.transform.position = new Vector3(defaultPivotPostion.x, pivotHeight, defaultPivotPostion.y);

        // This check stops the code from running on the prefab asset itself.
        // An object in the Project window doesn't belong to a loaded scene.
        if (!gameObject.scene.isLoaded)
        {
            return; // It's a prefab, so exit before causing an error.
        }

        // If the camera is assigned, correctly set its position and parent.
        if (attatchedCamera)
        {
            // This ensures the camera is parented only if it isn't already.
            if (attatchedCamera.transform.parent != gameObject.transform)
                attatchedCamera.transform.parent = gameObject.transform;

            attatchedCamera.transform.localPosition = Vector3.back * DefaultDistance;
        }
    }


    private void OnDrawGizmosSelected()
    {
        //creates the local axes of the pivot at it's position
        Debug.DrawLine(transform.position + transform.forward * 5, transform.position - transform.forward * 5, Color.blue);
        Debug.DrawLine(transform.position + transform.right * 5, transform.position - transform.right * 5, Color.red);
        Debug.DrawLine(transform.position + transform.up * 5, transform.position - transform.up * 5, Color.green);
        //creates a sphere at the position of the mouseDrag anchor
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(mouseDragAnchor, 2);
        //draws the boundary wireframe walls of the pivot.
        Gizmos.color = Color.Lerp(Color.clear, Color.white, 0.5f);
        Gizmos.DrawWireCube((ConvertToVector3((maxBoundaryCorner - minBoundaryCorner) * 0.5f + minBoundaryCorner) + Vector3.up * 100f), new Vector3(maxBoundaryCorner.x - minBoundaryCorner.x, 200, maxBoundaryCorner.y - minBoundaryCorner.y));
        //draws a sphere on the terrain where the pivot is hovering over it.
        Gizmos.DrawSphere(gameObject.transform.position + Vector3.down * pivotHeight, 5);
    }
    
    /// <summary>
    /// Returns the point in space where the cursor is hovering over a collider of an object with the specified layer. 
    /// </summary>
    /// <param name="_layerMask">The layer of the object you want to raycast to.</param>
    private Vector3 MouseRayHitPoint(LayerMask _layerMask)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
        {
            return hit.point;
        }
        return MouseRayHitPoint(0f);
    }

    /// <summary>
    /// Returns the point in space where the cursor is hovering over an infinite, upward facing plane with a specified y height.
    /// </summary>
    /// <param name="TargetHeight">The height in space of the plane.</param>
    private Vector3 MouseRayHitPoint(float TargetHeight)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.up * TargetHeight);
        float distance = 0;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        else return Vector3.zero;
    }
    
    
    
    void Update()
    {
        //stop the camera controls if the game is paused
        if (Time.timeScale == 0)
            return;

        // When the reset key is pressed, start the reset coroutine if it's not already running.
        if (Input.GetKeyDown(resetKey) && resetCoroutine == null)
        {
            resetCoroutine = StartCoroutine(ResetCamera());
        }

        // Do not process any other input while the camera is resetting.
        if (resetCoroutine != null)
        {
            return;
        }
        
        //Set the position of the mouseDrag Anchor when the rightMouse button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            LayerMask ground = LayerMask.GetMask("Ground");
            mouseDragAnchor = MouseRayHitPoint(ground);
        }
        //Lerp the pivot to the position required to keep the cursor over the mouseDragAnchor point when the left mouse button is held down and the mouse is dragged.
        else if (Input.GetMouseButton(1))
        {
            Vector3 mouseOffsetPosition = MouseRayHitPoint(mouseDragAnchor.y);
            //the lerp is nessecary because at certain angles, Move() overshoots, making the pivot jitter back and fourth.
            Move(new Vector2(mouseDragAnchor.x - mouseOffsetPosition.x, mouseDragAnchor.z - mouseOffsetPosition.z));
        }

        //move the pivot around with the keyboard keys if the left mouse button isn't held down
        if (!Input.GetMouseButton(1))
        {
            Vector3 flattenedForwardVector = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Vector3 targetOffset = (transform.right * Input.GetAxisRaw("Horizontal") + flattenedForwardVector * Input.GetAxisRaw("Vertical")) * (moveSpeed * targetCameraDistance);
            transform.position = Vector3.Lerp(transform.position, transform.position + targetOffset, Time.deltaTime * cameraLerpSpeed);
        }

        //if the middle mouse button is pressed, lock the cursor.
        if (Input.GetMouseButtonDown(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        //when the middle mmouse button is held down, rotate the camera by the amount the mouse is moved.
        else if (Input.GetMouseButton(2))
        {
            Vector2 rotation = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Rotate(rotation);
        }
        //when the middle mouse button is released, unlock the cursor.
        else if (Input.GetMouseButtonUp(2))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //zoom in or out when the scroll wheel is scrolled
        if (Input.mouseScrollDelta.y != 0)
        {
            //the division is nessecary so that the distance between scroll intervals is greater the further out you are.
            targetCameraDistance -= targetCameraDistance / (Input.mouseScrollDelta.y * zoomAmount);
            //clamp the targetcamera distance so that it doesn't exceed the limit
            targetCameraDistance = Mathf.Clamp(targetCameraDistance, MinDistance, MaxDistance);
        }
        //lerp the camera to the set distance to make zooming in and out smoother.
        attatchedCamera.transform.localPosition = Vector3.Lerp(attatchedCamera.transform.localPosition, Vector3.back * targetCameraDistance, Time.deltaTime * cameraLerpSpeed);
        
        EnforceBoundaries();
        //set the height of the pivot.
        transform.position = new Vector3(transform.position.x, pivotHeight, transform.position.z);
    }
    
    /// <summary>
    /// Eases the camera pivot and zoom back to their default values over time.
    /// </summary>
    private IEnumerator ResetCamera()
    {
        Vector3 targetPivotPosition = new Vector3(defaultPivotPostion.x, pivotHeight, defaultPivotPostion.y);
        Quaternion targetPivotRotation = Quaternion.Euler(defaultAngle, initialRotation.y, initialRotation.z);
        float targetCamDistance = DefaultDistance;

        Vector3 startingPosition = transform.position;
        Quaternion startingRotation = transform.rotation;
        float startingCamDistance = targetCameraDistance;

        // A higher resetSpeed will make the duration shorter.
        float duration = 1.0f / resetSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            // Use SmoothStep for a nice ease-in and ease-out effect.
            float easedProgress = Mathf.SmoothStep(0, 1, progress);

            // Interpolate pivot position and rotation.
            transform.position = Vector3.Lerp(startingPosition, targetPivotPosition, easedProgress);
            transform.rotation = Quaternion.Slerp(startingRotation, targetPivotRotation, easedProgress);

            // Interpolate camera zoom distance.
            targetCameraDistance = Mathf.Lerp(startingCamDistance, targetCamDistance, easedProgress);
            // Directly set the camera's local position for a smooth zoom during the reset.
            attatchedCamera.transform.localPosition = Vector3.back * targetCameraDistance;

            yield return null;
        }

        // Snap to the final values to ensure precision.
        transform.position = targetPivotPosition;
        transform.rotation = targetPivotRotation;
        targetCameraDistance = targetCamDistance;
        attatchedCamera.transform.localPosition = Vector3.back * targetCameraDistance;

        // Allow input processing again.
        resetCoroutine = null;
    }
}