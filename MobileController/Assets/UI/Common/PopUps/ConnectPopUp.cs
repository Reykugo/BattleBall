using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ConnectPopUp : PopUp {

    public Network.NetworkClientScript net;

    private Text text;
    private Image icon;
    private Button button;

    // Use this for initialization
    void Start() {
        text = GetComponentInChildren<Text>();
        //icon = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();
        button.gameObject.SetActive(false);
        net.WillConnect += () => { Open(); };
        net.OnConnect += () => { Close(); };
        net.OnConnectError += ConnectEndedWithErrors;
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ConnectEndedWithErrors(NetworkError error)
    {
        text.GetComponent<WaitingScript>().enabled = false;
        text.text = "Failed to connect : " + "\n" + error;
        button.gameObject.SetActive(true);
    }


    public override void Open()
    {
        text.text = "Connecting";
        text.GetComponent<WaitingScript>().enabled = true;
        button.gameObject.SetActive(false);

        base.Open();
    }

    public override void Close()
    {


        base.Close();
    }


}
