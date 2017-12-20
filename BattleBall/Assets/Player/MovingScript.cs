using UnityEngine;
using System.Collections;

public class MovingScript : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;

    public GroundCheckerScript groundChecker;

    public Vector3 currentDirection;

    public float maxVelocity;

    public float movementModifier = 1f;


    private Vector3 currentMovement = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetMovement(Vector3 movement)
    {
        movement.x *= 2;
        movement.y *= 2;
        if(movement.x < 0.01 && movement.x > 0.01)
        {
            movement.x = 0;
        }
        if(movement.y < 0.01 && movement.y > -0.01)
        {
            movement.y = 0;
        }

        currentMovement = new Vector3(movement.x, 0f, movement.y);
        currentMovement *= movementModifier;
        if (currentMovement != new Vector3(0, 0, 0))
        {
            currentDirection = currentMovement.normalized;
        }
    }

    void FixedUpdate()
    {
        if (groundChecker.IsGrounded)
        {
            if(rb.velocity.magnitude < maxVelocity)
            {
                rb.AddForce(currentMovement* speed, ForceMode.VelocityChange);
            }
                
        }
        groundChecker.IsGrounded = false;
        currentMovement = Vector3.zero;
    }
}