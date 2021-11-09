using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    
    [SerializeField] public float detachDelay = 0.01f;
    [SerializeField] private float respawnDelay;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private GameObject ballPrefab;
    

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;
    private Camera mainCamera;
    private bool isDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    // Update is called once per frame
    void Update()
    {
        //If ball flew out
        if(currentBallRigidbody == null)
        {
            return;
        }

        //Check if player is touching screen
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {   
            if (isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
            return;
        }

        isDragging = true;
        //Change balls state from kinematic to dynamic
        currentBallRigidbody.isKinematic = true;

        //Position of the finger on the screen
        Vector2 TouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(TouchPosition);

        //Move ball to where player clicks
        currentBallRigidbody.position = worldPosition;
    }
    //Make ball fly
    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;
        //Wait for some time before removing ball from spring joint
        //Name of converts name of method into string
        Invoke(nameof(DetachBall), detachDelay);
        
    }

    private void DetachBall()
    {
        //Detach from spring when released
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        //Respawn ball after it's been sent off
        Invoke(nameof(SpawnNewBall), respawnDelay);
    }
    //Create new ball
    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }
}
