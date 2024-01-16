using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maxBuildAreaTank : MonoBehaviour
{
    MasterManager masterManager;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        this.transform.localScale = new Vector2(masterManager.maxTankBuildSizeX, masterManager.maxTankBuildSizeY)*2;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
