using UnityEngine;
using System.Collections;

public class FollowPositionScript : MonoBehaviour
{

    [SerializeField]
    public Transform ObjectToFollow;

    [SerializeField]
    private bool X; //if object can follow on this position axis

    [SerializeField]
    private bool Y; //if object can follow on this position axis

    [SerializeField]
    private bool Z; //if object can follow on this position axis

    [SerializeField]
    private Vector3 Offset;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = transform.position.x;
        float yAxis = transform.position.y;
        float zAxis = transform.position.z;
        if (X)
        {
            xAxis = ObjectToFollow.position.x;
        }
        if (Y)
        {
            yAxis = ObjectToFollow.position.y;
        }
        if (Z)
        {
            zAxis = ObjectToFollow.position.z;
        }

        transform.position = new Vector3(xAxis + Offset.x, yAxis + Offset.y, zAxis + Offset.z);
    }

}
