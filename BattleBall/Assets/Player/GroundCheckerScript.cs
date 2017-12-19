using UnityEngine;
using System.Collections;

public class GroundCheckerScript : MonoBehaviour
{
    public bool IsGrounded;

    private void OnTriggerEnter(Collider other)
    {
       
    }

    private void OnTriggerStay(Collider other)
    {
        IsGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
    }
}
