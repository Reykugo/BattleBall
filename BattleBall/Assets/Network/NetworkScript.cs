using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkScript : NetworkDiscovery { 

    public GlobalConfig GConfig = new GlobalConfig();


    public ConnectionConfig ConnectConfig = new ConnectionConfig();

    private HostTopology topology;

	// Use this for initialization
	void Start () {
        StartAsClient();
        NetworkTransport.Init(GConfig);
        topology = new HostTopology(ConnectConfig, 4);
        hostId = NetworkTransport.AddHost(topology, 8080);

	}

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        NetworkManager.singleton.networkAddress = fromAddress;
        NetworkManager.singleton.StartClient();
    }
}
