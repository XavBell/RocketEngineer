using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecouplerComponent : MonoBehaviour
{
    public bool decoupled = false;
    public bool detachFromParent = false;
    public bool hasDecoupled = false;

    void Update()
    {
        if(decoupled && !hasDecoupled)
        {
            Decouple();
            hasDecoupled = true;
        }
    }

    public void Decouple()
    {
        if(detachFromParent)
        {
            //Separate from parent
            transform.parent = null;
        }else{
            //Detach children
            //Conserve parent
            GameObject parent = transform.parent.gameObject;
            foreach(Transform child in transform)
            {
                if(child.GetComponent<PhysicsPart>() != null)
                {
                    child.transform.parent = null;
                }
            }
            transform.parent = parent.transform;
        }
    }
}
