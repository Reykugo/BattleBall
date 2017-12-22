using UnityEngine;
using System.Collections;

public class GroundCheckerScript : MonoBehaviour
{
    public bool IsGrounded;

    private void OnTriggerStay(Collider other)
    {
        IsGrounded = true;
    }
}
