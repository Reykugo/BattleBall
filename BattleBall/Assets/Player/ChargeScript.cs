using UnityEngine;
using System.Collections;

public class ChargeScript : MonoBehaviour
{
    public float MaxChargePower = 30;

    public float ChargePerSecond = 10;

    public float ChargeDuration = 0.5f;

    private bool makeCharge = false;
    private bool isOnCharge;
    private float chargePower = 0;

    private MovingScript playerMove;

    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMove= GetComponent<MovingScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //Used for testing
        if (playerMove.IsGrounded)
        {
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
            if (chargePower <= MaxChargePower)
            {
                chargePower += ChargePerSecond / 4;
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, 0), 0.1f);
            }
            yield return new WaitForSeconds(0.15f);

        }
        Vector3 normalizedVelocity = GetComponent<MovingScript>().currentDirection;
        /*Vector3 normalizedVelocity = rb.velocity.normalized;*/

        rb.AddForce(normalizedVelocity * chargePower, ForceMode.Impulse);
        yield return new WaitForSeconds(ChargeDuration);
        while(rb.velocity.magnitude > GetComponent<MovingScript>().maxVelocity)
        {
            rb.velocity -= normalizedVelocity;
            yield return new WaitForEndOfFrame();

        }

        
        isOnCharge = false;
    }
}
