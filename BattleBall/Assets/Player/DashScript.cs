using UnityEngine;
using System.Collections;

public class DashScript : MonoBehaviour
{
    public delegate void ChargeObserver(float CurrentCharge);
    public event ChargeObserver OnChargingStarted;
    public event ChargeObserver OnCharging;
    public event ChargeObserver OnFullCharge;
    public delegate void Dashing(Vector3 direction);
    public event Dashing OnDashing;
    public delegate void State();
    public event State OnDashEnd;

    public GroundCheckerScript groundChecker;
    public float MaxChargePower = 30;
    public float ChargePerSecond = 10;
    public float ChargeDuration = 0.5f;
    public float chargePower = 0;

    public bool IsOnDash = false;
    private MovingScript playerMove;
    private Rigidbody rb;
    private bool makeCharge = false;
    private bool isOnCharge = false;
  
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMove= GetComponent<MovingScript>();
    }

    public void StartDash()
    {
        if (!isOnCharge)
        {
            makeCharge = true;
            StartCoroutine(Charge());
        }
    }

    public void ReleaseDash()
    {
        makeCharge = false;
    }

    public void StopDash()
    {
        StopAllCoroutines();
        makeCharge = false;
        IsOnDash = false;
        isOnCharge = false;
        if(OnDashEnd != null)
            OnDashEnd();
    }

    IEnumerator Charge()
    {
        isOnCharge = true;
        chargePower = 0;

        if (OnChargingStarted != null) 
            OnChargingStarted(chargePower);

        while (makeCharge)
        {
            if (chargePower <= MaxChargePower)
            {
                chargePower += ChargePerSecond / 4;
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, 0), 0.1f);
                if(OnCharging != null)
                {
                    OnCharging(chargePower);
                }
            }
            else
            {
                if(OnFullCharge != null)
                {
                    OnFullCharge(MaxChargePower);
                }
            }
            yield return new WaitForSeconds(0.25f);

        }
        IsOnDash = true;
        Vector3 normalizedVelocity = playerMove.currentDirection;
        if(OnDashing != null)
            OnDashing(normalizedVelocity);
        
        rb.AddForce(normalizedVelocity * chargePower, ForceMode.Impulse);

        yield return new WaitForSeconds(ChargeDuration);
        while(rb.velocity.magnitude > playerMove.maxVelocity)
        {  
            rb.velocity = Vector3.Lerp(rb.velocity, normalizedVelocity, 0.2f);
            yield return new WaitForEndOfFrame();

        }

        StopDash();
    }
}
