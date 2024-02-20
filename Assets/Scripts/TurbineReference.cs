using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurbineReference:MonoBehaviour
{
    public Turbine turbineRef;
    MasterManager masterManager;
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        check();

    }

    void check()
    {
        foreach(Turbine turbine in masterManager.turbineUnlocked)
        {
            if(turbine == turbineRef)
            {
                this.gameObject.SetActive(true);
                return;
            }
        }
        this.gameObject.SetActive(false);
    }
}
