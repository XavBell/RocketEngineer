using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class GameManager_Engine : MonoBehaviour
{
   //Specs Float
    public float mass;
    public float thrust;
    public float rate;
    public float maxAngle;

    public TMP_Dropdown turbopumpDropdown;

    public TMP_Dropdown nozzleDropdown;

    public TMP_Dropdown tvcDropdown;

    public savePath savePathRef = new savePath();

    public MasterManager MasterManager = new MasterManager();
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
        {
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
        {
            
        }

    }

    public void backToBuild()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Create()
    {
        Nozzle nozzle = new Nozzle();
        Turbopump turbopump = new Turbopump();
        TVC tvc = new TVC();

        //Turbopump
        string selectedTurbopump = turbopumpDropdown.options[turbopumpDropdown.value].text.ToString();
        if(selectedTurbopump == "TurbopumpRaptor")
        {
            TurbopumpRaptor turbopumpRaptor = new TurbopumpRaptor();

            turbopump.turbopumpName = turbopumpRaptor.turbopumpName;
            turbopump.mass = turbopumpRaptor.mass;
            turbopump.rate = turbopumpRaptor.rate;
            turbopump.thrust = turbopumpRaptor.thrust;
        }

        //Nozzle
        string selectedNozzle = nozzleDropdown.options[nozzleDropdown.value].text.ToString();
        if(selectedNozzle == "NozzleRaptor")
        {
            NozzleRaptor nozzleRaptor = new NozzleRaptor();

            nozzle.nozzleName = nozzleRaptor.nozzleName;
            nozzle.mass = nozzleRaptor.mass;
            nozzle.thrustModifier = nozzleRaptor.thrustModifier;
            nozzle.rateModifier = nozzleRaptor.rateModifier;
        }

        //TVC
        string selectedTVC = tvcDropdown.options[tvcDropdown.value].text.ToString();
        if(selectedTVC == "TVCRaptor")
        {
            TVCRaptor tvcRaptor = new TVCRaptor();

            tvc.TVCName = tvcRaptor.TVCName;
            tvc.mass = tvcRaptor.mass;
            tvc.maxAngle = tvcRaptor.maxAngle; 
        }

        rate = turbopump.rate;
        mass = turbopump.mass;
        thrust = turbopump.thrust;

        mass += nozzle.mass;
        rate *= nozzle.rateModifier;
        thrust *= nozzle.thrustModifier;

        mass += tvc.mass;
        maxAngle = tvc.maxAngle;

        UnityEngine.Debug.Log("Mass: " + mass + " Rate: " + rate + " Thrust: " + thrust + " Max Angle: " + maxAngle );
    }

}
