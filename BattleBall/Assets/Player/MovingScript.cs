using UnityEngine;
using System.Collections;

public class MovingScript : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;

    public bool IsGrounded = false;

    public Vector3 currentDirection;

    public float maxVelocity;


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
        if (currentMovement != new Vector3(0, 0, 0))
        {
            currentDirection = currentMovement.normalized;
        }
    }

    public void Update()
    {
        if(currentMovement == Vector3.zero)
        {
            SetMovement(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f));
        }
    }

    void FixedUpdate()
    {
        if (IsGrounded)
        {
            if(rb.velocity.magnitude < maxVelocity)
            {
                rb.AddForce(currentMovement* speed, ForceMode.VelocityChange);
            }
                
        }
        currentMovement = Vector3.zero;
    }
}