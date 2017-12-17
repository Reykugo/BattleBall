using UnityEngine;
using System.Collections;

public class DashScript : MonoBehaviour
{
    public float MaxChargePower = 30;

    public float ChargePerSecond = 10;

    public float ChargeDuration = 0.5f;


    public DashSystemScript DashSystem;

    public bool IsOnDash = false;
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

    public void StartDash(bool isDashing)
    {
        if (!isOnCharge)
        {
            makeCharge = isDashing;
            StartCoroutine(Charge());
        }
    }

    public void StopDash(bool isDashing)
    {
        makeCharge = isDashing;
    }

    // Update is called once per frame
    void Update()
    {
        //Used for testing
        if (playerMove.IsGrounded && Input.GetKeyDown("space"))
        {
            StartDash(true);
        }

        //Used for testing
        if (Input.GetKeyUp("space"))
        {
            StopDash(false);
        }
    }



    IEnumerator Charge()
    {
        isOnCharge = true;
        chargePower = 0;

        DashSystem.EnableDashChargerEffect(true);
        while (makeCharge)
        {
            if (chargePower <= MaxChargePower)
            {
                chargePower += ChargePerSecond / 4;
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, 0), 0.1f);
            }
            else
            {
                DashSystem.EnableDashChargerEffect(false);
                DashSystem.EnableChargeEffect(true);               

            }
            yield return new WaitForSeconds(0.25f);

        }
        DashSystem.EnableChargeEffect(false);
        DashSystem.EnableDashEffect(true);
        Vector3 normalizedVelocity = GetComponent<MovingScript>().currentDirection;

        IsOnDash = true;
        rb.AddForce(normalizedVelocity * chargePower, ForceMode.Impulse);
        yield return new WaitForSeconds(ChargeDuration);
        while(rb.velocity.magnitude > GetComponent<MovingScript>().maxVelocity)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, normalizedVelocity, 0.2f);
            yield return new WaitForEndOfFrame();

        }

        IsOnDash = false;
        isOnCharge = false;
    }
}
