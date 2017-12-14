using UnityEngine;
using System.Collections;

public class MovingScript : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;

    public bool isGrounded = false;

    public Vector3 currentDirection;

    public float maxVelocity;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isGrounded)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            currentDirection = movement.normalized;
            if(rb.velocity.magnitude < maxVelocity)
            {
                rb.AddForce(movement * speed, ForceMode.VelocityChange);
            }
                
        }
       
    }
}