using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVCReference : MonoBehaviour
{
    public TVC tvcRef;
    MasterManager masterManager;

    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        check();
    }

    void check()
    {
        bool found = false;
        foreach(TVC tvc in masterManager.tvcUnlocked)
        {
            if(tvc == tvcRef)
            {
                this.gameObject.SetActive(true);
                found = true;
                return;
            }
        }

        if(found == false)
        {
            this.gameObject.SetActive(false);
        }
    }
}
