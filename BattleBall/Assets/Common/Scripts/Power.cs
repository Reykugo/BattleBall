using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Power : MonoBehaviour{

    protected float Time;
    public enum PowerType { GAZ, METAL };
    public string name;
    public static Material baseMaterial;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator PowerDuration(float timer)
    {
        yield return new WaitForSeconds(timer);
        LosePower();
    }

    public void LosePower()
    {
        Debug.Log("Power : " + name + "finnished");
        Destroy(this);
        
    }
}
