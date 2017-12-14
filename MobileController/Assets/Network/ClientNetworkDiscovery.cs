using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkDiscovery : NetworkDiscovery
{
    public delegate void HostDiscoveryObserver(string fromAddress, string data);
    public HostDiscoveryObserver OnHostDiscovered;

    // Use this for initialization
    void Awake()
    {
        
       
    }

    void OnEnable()
    {
        bool complete = Initialize();
        if (!complete)
        {
            Debug.Log("port not available");
        }
        //Start listning to broadcast
        StartAsClient();
    }

    void OnDisable()
    {
        StopBroadcast();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if(OnHostDiscovered != null)
            OnHostDiscovered(fromAddress, data);
    }

}