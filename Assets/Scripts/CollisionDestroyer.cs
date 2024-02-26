using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroyer : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.magnitude > 40)
        {
            print("Destroyed because of collision with collider");
            Destroy(this.gameObject);
        }

    }
}
