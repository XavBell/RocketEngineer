using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outputInputManager : MonoBehaviour
{
    public GameObject input;
    public GameObject output;

    public GameObject attachedInput;
    public GameObject attachedOutput;

    public GameObject inputParent;
    public GameObject outputParent;

    //Rate is in unit/s
    public float selfRate;
    public float rate;

    public float quantity = 0;

    public bool log = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateParents();
        setRate();
        fuelTransfer();
        DebugLog();
    }
    
    void updateParents()
    {
        if(!inputParent  && attachedInput)
        {
            inputParent = attachedInput.transform.parent.gameObject;
        }
        
        if(!outputParent && attachedOutput)
        {
            outputParent = attachedOutput.transform.parent.gameObject;
        }
    }

    void setRate()
    {
        if(inputParent)
        {
            rate = inputParent.GetComponent<outputInputManager>().rate;
        }

        if(!inputParent && quantity != 0)
        {
            rate = selfRate;
        }
    }

    void fuelTransfer()
    {
        //Fix reverse flow
        if(outputParent)
        {
           if(quantity - rate * Time.deltaTime >= 0)
           {
                float variation;
                variation = rate * Time.deltaTime;
                outputParent.GetComponent<outputInputManager>().quantity -=  variation;
           }
        }

        if(inputParent)
        {
            quantity += inputParent.GetComponent<outputInputManager>().rate*Time.deltaTime;
        }
    }

    void DebugLog()
    {
        if(log == true)
        {
            Debug.Log(quantity);
        }
    }
}
