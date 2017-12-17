using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDebug : MonoBehaviour {

    GameUI gameUI;
    public Text dashingState;
    public Text accel;

	// Use this for initialization
	void Start () {
        gameUI = GetComponent<GameUI>();
        gameUI.OnDash += UpdateDashing;
	}
	
	// Update is called once per frame
	void Update () {
        accel.text = Input.acceleration.ToString();
	}

    void UpdateDashing(bool dashing)
    {
        dashingState.text = dashing.ToString();
    }
}
