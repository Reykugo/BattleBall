using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSystemScript : MonoBehaviour {

    public DashScript dashScript;
    public ColorUnifier colorUnifier;

    public GameObject DashChargerEffect;
    public GameObject ChargeEffect;
    public GameObject DashEffect;

    private ParticleSystem dashChargerEffect;
    private ParticleSystem chargeEffect;
    private ParticleSystem dashEffect;

    private GameObject currentDashEffect;
    private Color currentColor;

    public void Awake()
    {
        dashChargerEffect = DashChargerEffect.GetComponent<ParticleSystem>();
        chargeEffect = ChargeEffect.GetComponent<ParticleSystem>();
    }
    public void Start()
    {
        colorUnifier.OnColorChanged += Init;

        dashScript.OnChargingStarted += (charge) =>
        {
            EnableDashChargerEffect(true);
        };
        dashScript.OnFullCharge += (charge) => {
            EnableDashChargerEffect(false);
            EnableChargeEffect(true);
        };

        dashScript.OnDashing += (direction) =>
        {
            EnableDashChargerEffect(false);
            EnableChargeEffect(false);
            if (direction != Vector3.zero)
            {
                CreateDashEffect(direction);
            }
        };

        dashScript.OnDashEnd += DisabledAllEffects;

        Init(colorUnifier.GetColor());
        dashChargerEffect.Stop();
        chargeEffect.Stop();
    }

    void OnDestroy()
    {
        colorUnifier.OnColorChanged -= Init;
        
    }

    public void Init(Color color)
    {
        currentColor = color;

        var mainDashChargerEffect = dashChargerEffect.main;
        mainDashChargerEffect.startColor = color;

        var mainChargeEffect = chargeEffect.main;
        mainChargeEffect.startColor = color;
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
        currentDashEffect.transform.position = transform.position;
        currentDashEffect.transform.rotation = Quaternion.LookRotation(-playerDirection, new Vector3(0.0f, 1.0f, 0.0f));
        dashEffect = currentDashEffect.GetComponent<ParticleSystem>();
        var mainDashEffect = dashEffect.main;
        mainDashEffect.startColor = currentColor;
        dashEffect.Play();
        Destroy(currentDashEffect, 1);
    }

    public void DisabledAllEffects()
    {
        dashChargerEffect.Stop();
        chargeEffect.Stop();
    }

}


