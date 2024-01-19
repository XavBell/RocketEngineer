using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroyer : MonoBehaviour
{
    public Collision collision;
    void OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude > 30)
        {
            Destroy(collision.collider.gameObject);
        }
        if(this.transform.parent.GetComponent<Rocket>() != null)
        {
            Destroy(this.gameObject.transform.parent);
        }
        Destroy(this.gameObject);
        
    }
}
