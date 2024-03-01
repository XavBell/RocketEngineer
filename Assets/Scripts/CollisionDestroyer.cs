using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroyer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.magnitude > 40)
        {
            FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destruction due to collision with another object");
        }

    }
}
