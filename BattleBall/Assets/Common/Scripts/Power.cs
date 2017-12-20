using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Power : MonoBehaviour{

    public float PowerTime;
    public enum PowerType { GAZ, METAL, ICE, LIGHTNING, CONFUSION, LENGTH };
    public string powerName;

    protected GameObject player;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartEffect()
    {
        player = this.transform.parent.gameObject;
        Debug.Log("StartCoroutine PowerDuration");
        StartCoroutine(PowerDuration(PowerTime));
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
        Destroy(this.gameObject);
        
    }
}
