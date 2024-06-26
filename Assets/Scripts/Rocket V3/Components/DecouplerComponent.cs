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
            Vector3 transformPosition = transform.position;
            Vector3 eulerAngles = transform.eulerAngles;
            GameObject rocketController = Instantiate(Resources.Load("Prefabs/RocketController")) as GameObject;
            this.transform.parent = rocketController.transform;
            rocketController.GetComponent<RocketController>().TransferStateFromDecoupling(parent.GetComponentInParent<RocketController>());
            rocketController.transform.position = transformPosition;
            rocketController.transform.eulerAngles = eulerAngles;
            this.transform.localPosition = Vector3.zero;
            this.transform.eulerAngles = eulerAngles;
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
                    Vector3 transformPosition = child.transform.position;
                    Vector3 eulerAngles = child.transform.eulerAngles;
                    GameObject rocketController = Instantiate(Resources.Load("Prefabs/RocketController")) as GameObject;
                    child.transform.parent = rocketController.transform;
                    rocketController.GetComponent<RocketController>().TransferStateFromDecoupling(parent.GetComponentInParent<RocketController>());
                    rocketController.transform.position = transformPosition;
                    rocketController.transform.eulerAngles = eulerAngles;
                    child.transform.localPosition = Vector3.zero;
                    child.transform.eulerAngles = eulerAngles;
                }
            }
            transform.parent = parent.transform;
            
        }
    }
}
