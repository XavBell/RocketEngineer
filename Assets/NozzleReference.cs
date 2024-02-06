using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NozzleReference : MonoBehaviour
{
    public Nozzle nozzleRef;
    MasterManager masterManager;
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        check();
    }

    void check()
    {
        foreach(Nozzle nozzle in masterManager.nozzleUnlocked)
        {
            if(nozzle == nozzleRef)
            {
                this.gameObject.SetActive(true);
                return;
            }
        }
        this.gameObject.SetActive(false);
    }
}
