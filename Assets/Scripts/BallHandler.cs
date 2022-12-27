using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float delayToFree;
    [SerializeField] private float delayToRespawn;
    
    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint2D;
    private Camera mainCamera;
    private bool isDragging;

    private void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (currentBallRigidbody == null) return;
        
        if (Touch.activeTouches.Count == 0)
        {
            if (isDragging)
            {
                LauchBall();
            }
            
            isDragging = false;
            return;
        }

        isDragging = true;
        currentBallRigidbody.isKinematic = true;

        Vector2 touchPositions = new Vector2();
        foreach (Touch touch in Touch.activeTouches)
        {
            touchPositions += touch.screenPosition;
        }

        touchPositions /= Touch.activeTouches.Count;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPositions);

        currentBallRigidbody.position = worldPosition;
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        
        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint2D = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint2D.connectedBody = pivot;
    }

    private void LauchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;
        
        Invoke(nameof(DetachBall), delayToFree);
    }

    private void DetachBall()
    {
        currentBallSpringJoint2D.enabled = false;
        currentBallSpringJoint2D = null;

        Invoke(nameof(SpawnNewBall), delayToRespawn);
    }
}
