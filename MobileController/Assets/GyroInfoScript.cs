using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyroInfoScript : MonoBehaviour {

    Text t;
	// Use this for initialization
	void Start () {
        t = GetComponent<Text>();
        t.text = "Test";
	}
	
	// Update is called once per frame
	void Update () {
        t.text = Input.gyro.attitude.ToString() + " : " + Input.acceleration.ToString();

    }
}
