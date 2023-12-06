using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
    public GameObject rb;
    // Start is called before the first frame update
   void OnTriggerEnter(Collider other)
   {
        if(other.gameObject.GetComponent<Rocket>() != null)
        {
            rb.SetActive(true);
        }

   }

   void OnTriggerExit(Collider other)
   {
        if(other.gameObject.GetComponent<Rocket>() != null)
        {
            rb.SetActive(false);
        }

   }
}
