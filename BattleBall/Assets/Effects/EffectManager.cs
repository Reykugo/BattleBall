using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    private Power currentPower;

    public void ActivateCurrentPower()
    {
        if(currentPower != null && currentPower.powerActivator == Power.PowerActivator.MANUAL)
        {
            currentPower.StartEffect();
        }
    }

    public void SetPower(Power power)
    {
        if(currentPower != null)
        {
            Destroy(currentPower.gameObject);
        }
        currentPower = power;
        currentPower.OnEnd += OnPowerEnd;
        if(currentPower.powerActivator == Power.PowerActivator.AUTOMATIC)
        {
            currentPower.StartEffect();
        }
    }

    private void OnPowerEnd()
    {
        currentPower.OnEnd -= OnPowerEnd;
        currentPower = null;
    }
}
