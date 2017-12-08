using UnityEngine;
using System.Collections;

public class GroundCheckerScript : MonoBehaviour
{
    [SerializeField]
    private MovingScript PlayerMove;




    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Field")
        {
            PlayerMove.isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Field")
        {
            PlayerMove.isGrounded = false;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
