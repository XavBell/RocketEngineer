using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class launchPadManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public savePath savePathRef = new savePath();
    public MasterManager MasterManager;
    public bool Spawned = false;
    public GameObject ConnectedRocket;
    public outputInputManager oxidizer;
    public outputInputManager fuel;
    public string padName;
    public GameObject button;
    public string operation = "none";
    public bool started = false;
    public bool failed = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager = GMM.GetComponent<MasterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Spawned == false)
        {
            Spawned = true;
        }

        if(ConnectedRocket != null)
        {
            if(ConnectedRocket.GetComponent<PlanetGravity>().possessed == true)
            {
                ConnectedRocket.GetComponent<outputInputManager>().inputParent = null;
                ConnectedRocket = null;
                GetComponent<outputInputManager>().outputParent = null;
            }
        }

        if(operation == "launch")
        {

        }

        if(operation == "staticFire")
        {
            
        }

        if(operation == "WDR")
        {
            
        }
    }
}
