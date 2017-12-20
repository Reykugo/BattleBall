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

    private GameObject currentDashEffect;

    private Color playerColor;

    public void Init(Color pColor)
    {
        playerColor = pColor;
        var mainDashChargerEffect = dashChargerEffect.main;
        mainDashChargerEffect.startColor = playerColor;
        dashChargerEffect.Stop();

        var mainChargeEffect = chargeEffect.main;
        mainChargeEffect.startColor = playerColor;
        chargeEffect.Stop();

        
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

    public void CreateDashEffect(Vector3 playerDirection)
    {

        currentDashEffect = Instantiate<GameObject>(DashEffect);
        currentDashEffect.transform.position = this.transform.position;
        currentDashEffect.transform.rotation = Quaternion.LookRotation(-playerDirection, new Vector3(0.0f, 1.0f, 0.0f));
        dashEffect = currentDashEffect.GetComponent<ParticleSystem>();
        var mainDashEffect = dashEffect.main;
        mainDashEffect.startColor = playerColor;
            
        dashEffect.Play();
        Destroy(currentDashEffect, 1);
    }


    public void DisabledAllEffects()
    {
        dashChargerEffect.Stop();
        chargeEffect.Stop();
    }


    // Use this for initialization
    void Awake () {
        dashChargerEffect = DashChargerEffect.GetComponent<ParticleSystem>();
        chargeEffect = ChargeEffect.GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {

    }
}
