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
        currentMovement = new Vector3(movement.x, 0f, movement.z);

        if (movement != new Vector3(0, 0, 0))
        {
            currentDirection = movement.normalized;
        }
    }

    public void Update()
    {
        if(currentMovement == Vector3.zero)
        {
            SetMovement(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
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