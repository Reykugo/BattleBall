using UnityEngine;
using System.Collections;

public class ChargeScript : MonoBehaviour
{
    public float MaxChargePower;

    public float ChargePerSecond;

    private bool makeCharge = false;
    private bool isOnCharge;
    private float chargePower = 0;

    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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


    IEnumerator Charge()
    {
        isOnCharge = true;
        chargePower = 0;
        while (makeCharge)
        {
            if (chargePower <= MaxChargePower)
            {
                chargePower += ChargePerSecond / 4;
                yield return new WaitForSeconds(0.15f);
            }

        }

        Vector3 normalizedVelocity = rb.velocity.normalized;
        rb.AddForce(normalizedVelocity * chargePower, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        rb.velocity = normalizedVelocity;
        isOnCharge = false;
    }
}
