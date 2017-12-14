using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkDiscovery : NetworkDiscovery
{
    public delegate void HostDiscoveryObserver(string fromAddress, string data);
    public HostDiscoveryObserver OnHostDiscovered;

    // Use this for initialization
    void Start()
    {
        bool complete = Initialize();
        if (!complete)
        {
            Debug.Log("port not available");
        }
        //Start listning to broadcast
        StartAsClient();
        OnHostDiscovered += (from, data) =>
        {
            Debug.Log("Received Broadcast from " + from + " with data " + data);
        };
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        OnHostDiscovered(fromAddress, data);
    }

}