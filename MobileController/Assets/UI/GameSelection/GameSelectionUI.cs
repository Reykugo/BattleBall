using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectionUI : FlowStep {

    public GameObject WaitingPopUp;
    public Network.NetworkClientScript net;

    private Dictionary<string, GameObject> gamesFound = new Dictionary<string, GameObject>();
    private ClientNetworkDiscovery clientDiscovery;

    private GamesContainer container;
    private GameStateManager gameStateManager;

    public string testIP = "10.92.2.204";
    public string TestRoomName = "Testing";

	// Use this for initialization

    void Awake()
    {
        gameStateManager = GetComponentInParent<GameStateManager>();
        clientDiscovery = GetComponent<ClientNetworkDiscovery>();
        container = GetComponentInChildren<GamesContainer>();
    }
	void Start () {
        AddHost("::ffff:"+testIP, TestRoomName);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddHost(string fromAddress, string data)
    {
        WaitingPopUp.SetActive(false);

        String pattern = @"::.*:(.*)";// TODO change regex to allow 0 -> 2 colons and 0 -> 1 for the last colon
        var matches = Regex.Matches(fromAddress, pattern);
        string ipV4 = "";
        if (matches.Count == 1)
        {
            ipV4 = matches[0].Groups[1].Value;
        }
        string[] hostInfos = data.Split(":".ToCharArray());
        container.Add(hostInfos[0], ipV4, (hostInfos.Length == 2) ? hostInfos[1] : "N/A");
    }

    void OnEnable()
    {
        WaitingPopUp.SetActive(true);
        net.OnConnect += gameStateManager.TransitToLobby;
        clientDiscovery.OnHostDiscovered += AddHost;
    }
    void OnDisable()
    {
        container.Clear();
        net.OnConnect -= gameStateManager.TransitToLobby;
    }
}
