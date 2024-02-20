using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpReference : MonoBehaviour
{
    public Pump pumpRef;

    MasterManager masterManager;
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        check();
    }

    void check()
    {
        foreach(Pump pump in masterManager.pumpUnlocked)
        {
            if(pump == pumpRef)
            {
                this.gameObject.SetActive(true);
                return;
            }
        }
        this.gameObject.SetActive(false);
    }
}
