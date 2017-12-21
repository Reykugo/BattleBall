using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Power : MonoBehaviour{

    public float PowerTime;

    public enum PowerType { GAZ, METAL, ICE, LIGHTNING, CONFUSION, TORNADO, EXPLOSION, LENGTH };
    public enum PowerTarget { ALL, SELF, OTHERS }
    public enum PowerActivator { AUTOMATIC, MANUAL }

    public string powerName;

    //TODO ugly make it great again.
    public PowerType powerType;
    public PowerTarget powerTarget = PowerTarget.SELF;//Default SELF;
    public PowerActivator powerActivator = PowerActivator.AUTOMATIC;


    public delegate void StateObserver();
    public event StateObserver OnEnd;
    public event StateObserver OnActivation;

    protected bool activated=false;//Activated prevent to run an effect multiple times or allow to run it multiple times with maximum control.

    protected GameObject player;


    void Start()
    {
        
    }

    public void StartEffect()//TODO handle more cases.
    {
        Debug.Log("EFFECT STARTED before was :" + activated);
        player = transform.parent.gameObject;
        if (!activated)
        {
            activated = true;
            if(OnActivation != null)
                OnActivation();
            StartCoroutine(PowerDuration(PowerTime));
        }
    }

    

    public IEnumerator PowerDuration(float timer)
    {
        yield return new WaitForSeconds(timer);
        LosePower();
    }

    public void LosePower()
    {
        if(OnEnd != null)
            OnEnd();
        Destroy(this.gameObject);
    }
}
