using UnityEngine;
using System.Collections;

public class MovingScript : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;

    public bool isGrounded = false;

    public float MaxChargePower;

    public float ChargePerSecond;

    private bool makeCharge = false;
    private bool isOnCharge;
    private float chargePower = 0;



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
            rb.AddForce(movement * speed);

            //Used for testing
            if (Input.GetKeyDown("space"))
            {
                if (!isOnCharge)
                {
                    makeCharge = true;
                    StartCoroutine(Charge());
                }
               
            }

            //Used for testing
            if (Input.GetKeyUp("space"))
            {
                makeCharge = false;
               
            }
        }
       
    }

    IEnumerator Charge()
    {
        isOnCharge = true;
        chargePower = 0;
        while (makeCharge)
        {
            if(chargePower <= MaxChargePower)
            {
                chargePower += ChargePerSecond / 4;
                yield return new WaitForSeconds(0.25f); 
            }
            
        }
        
        Vector3 normalizedVelocity = rb.velocity.normalized;
        rb.AddForce(normalizedVelocity * chargePower);
        yield return new WaitForSeconds(0.5f);
        rb.velocity = normalizedVelocity;
        isOnCharge = false;
    }
}