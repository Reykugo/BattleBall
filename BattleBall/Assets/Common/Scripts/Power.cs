using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Power : MonoBehaviour{

    protected float PowerTime;
    public enum PowerType { GAZ, METAL, ICE, LIGHTNING, LENGTH };
    public string powerName;

    // Use this for initialization
    void Start()
    {
        Debug.Log("StartCoroutine PowerDuration");
        StartCoroutine(PowerDuration(PowerTime));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator PowerDuration(float timer)
    {
        Debug.Log("timer : " + timer);
        yield return new WaitForSeconds(timer);
        LosePower();
    }

    public void LosePower()
    {
        Debug.Log("Power : " + powerName + "finnished");
        Destroy(this);
        
    }
}
