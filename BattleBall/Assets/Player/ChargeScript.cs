using UnityEngine;
using System.Collections;

public class ChargeScript : MonoBehaviour
{
    public float MaxChargePower = 30;

    public float ChargePerSecond = 10;

    public float ChargeDuration = 0.5f;

    public bool IsCharge = false;
    private bool makeCharge = false;
    private bool isOnCharge;
  
    public float chargePower = 0;

    private PlayerScript player;
    private MovingScript playerMove;

    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        player = this.GetComponent<PlayerScript>();
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

        //chargeParticles.GetComponent<ParticleSystem.ColorOverLifetimeModule>().color =  
        player.particleSystem.Play();
        while (makeCharge)
        {
            if (chargePower <= MaxChargePower)
            {
                chargePower += ChargePerSecond / 4;
                Debug.Log(chargePower);
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, 0), 0.1f);
                ParticleSystem.ShapeModule s = player.particleSystem.shape;
                s.radius = Mathf.Lerp(1, 0.01f, (chargePower/MaxChargePower));
                var main = player.particleSystem.main;
            }
            else
            {
                ParticleSystem.ShapeModule s = player.particleSystem.shape;
                s.radius = 0.01f; 
            }
            yield return new WaitForSeconds(0.25f);

        }
        ParticleSystem.ShapeModule shape = player.particleSystem.shape;
        shape.radius = 1;
        player.particleSystem.Stop();
        Vector3 normalizedVelocity = GetComponent<MovingScript>().currentDirection;
        /*Vector3 normalizedVelocity = rb.velocity.normalized;*/

        IsCharge = true;
        rb.AddForce(normalizedVelocity * chargePower, ForceMode.Impulse);
        yield return new WaitForSeconds(ChargeDuration);
        while(rb.velocity.magnitude > GetComponent<MovingScript>().maxVelocity)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, normalizedVelocity, 0.2f);
            yield return new WaitForEndOfFrame();

        }

        IsCharge = false;
        isOnCharge = false;
    }
}
