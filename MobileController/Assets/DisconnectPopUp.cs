using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DisconnectPopUp : PopUp
{

    public Network.NetworkClientScript net;

    private Text text;
    private Image icon;
    private Button button;

    // Use this for initialization
    void Start()
    {
        text = GetComponentInChildren<Text>();
        //icon = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();
        net.OnDisconnect += () => { Open(); };
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Open()
    {
        text.text = "Disconnected, host is offline...";
        button.gameObject.SetActive(true);

        base.Open();
    }

    public override void Close()
    {


        base.Close();
    }


}
