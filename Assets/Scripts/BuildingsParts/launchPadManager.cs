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
    public string padName;
    public GameObject button;
    public string operation = "none";
    public bool started = false;
    public bool failed = false;
    public List<string> padLines;
    public List<Guid> connectedContainersPerLine;
    public List<Guid> connectedRocketLines;

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
            //Disconnect rocket from launchpad

            //(WHY TF IS THAT HERE)
            if(ConnectedRocket.GetComponent<PlanetGravity>().possessed == true)
            {
                foreach(Stages stage in ConnectedRocket.GetComponent<Rocket>().Stages)
                {
                    foreach(RocketPart part in stage.Parts)
                    {
                        if(part._partType == "tank")
                        {
                            part.GetComponent<flowController>().origin = null;
                            part.GetComponent<flowController>().selfRate = 0;
                        }
                    }
                }
            }
        }
    }
}
