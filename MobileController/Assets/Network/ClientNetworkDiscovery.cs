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
        StartCoroutine("StartBroadcasting");
    }

    void OnDisable()
    {
        StopCoroutine("StartBroadcasting");
        if (running)
            StopBroadcast();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if(OnHostDiscovered != null)
            OnHostDiscovered(fromAddress, data);
    }

    IEnumerator StartBroadcasting()
    {
        //Try to initialize.
        bool complete = Initialize();

        yield return new WaitForSeconds(0.5f);

        if (complete && !running)
            StartAsClient();

        yield return null;
    }

}