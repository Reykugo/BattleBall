using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSystemScript : MonoBehaviour {


    public GameObject DashChargerEffect;
    public GameObject ChargeEffect;
    public GameObject DashEffect;

    private ParticleSystem dashChargerEffect;
    private ParticleSystem chargeEffect;
    private ParticleSystem dashEffect;

    public void Init(Color playerColor)
    {
        var mainDashChargerEffect = dashChargerEffect.main;
        mainDashChargerEffect.startColor = playerColor;
        dashChargerEffect.Stop();

        var mainChargeEffect = chargeEffect.main;
        mainChargeEffect.startColor = playerColor;
        chargeEffect.Stop();

        var mainDashEffect = dashEffect.main;
        mainDashEffect.startColor = playerColor;
        dashEffect.Stop();
    }

    public void EnableDashChargerEffect(bool state)
    {
        if (state)
        {
            dashChargerEffect.Play();
        }

        else
        {
            dashChargerEffect.Stop();
        }
    }


    public void EnableChargeEffect(bool state)
    {
        if (state)
        {
            chargeEffect.Play();
        }

        else
        {
            chargeEffect.Stop();
        }
    }

    public void EnableDashEffect(bool state)
    {
        if (state)
        {
            dashEffect.Play();
        }

        else
        {
            dashEffect.Stop();
        }
    }



    // Use this for initialization
    void Awake () {
        dashChargerEffect = DashChargerEffect.GetComponent<ParticleSystem>();
        chargeEffect = ChargeEffect.GetComponent<ParticleSystem>();
        dashEffect = DashEffect.GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        DashChargerEffect.transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
