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
            GameObject parent = transform.parent.gameObject;
            transform.parent = null;
            GameObject rocketController = Instantiate(Resources.Load("Prefabs/RocketController")) as GameObject;
            rocketController.transform.position = this.transform.position;
            this.transform.parent = rocketController.transform;
            rocketController.GetComponent<RocketController>().TransferStateFromDecoupling(parent.GetComponentInParent<RocketController>());
        }
        else{
            //Detach children
            //Conserve parent
            GameObject parent = transform.parent.gameObject;
            foreach(Transform child in transform)
            {
                if(child.GetComponent<PhysicsPart>() != null)
                {
                    //Single child
                    child.transform.parent = null;
                    GameObject rocketController = Instantiate(Resources.Load("Prefabs/RocketController")) as GameObject;
                    rocketController.transform.position = child.transform.position;
                    child.transform.parent = rocketController.transform;
                    rocketController.GetComponent<RocketController>().TransferStateFromDecoupling(parent.GetComponentInParent<RocketController>());
                }
            }
            transform.parent = parent.transform;
            
        }
    }
}
