using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class standManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public savePath savePathRef = new savePath();
    public MasterManager MasterManager;
    public bool Spawned = false;
    public GameObject ConnectedTank;
    public outputInputManager output;
    public string standName;
    public GameObject button;
    public float ratio;
    public bool started;
    public bool fuelSufficient = false;
    public TimeManager MyTime;
    public TankStatusTracker tankStatusTracker;
    public bool failed;
    public float startTime;
    public float minThrust;
    public float maxThrust;
    // Start is called before the first frame update
    void Start()
    {
        GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager = GMM.GetComponent<MasterManager>();
        MyTime = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(ConnectedTank != null)
        {
            string substanceType = output.substance;
            ConnectedTank.GetComponent<outputInputManager>().inputParent = output.inputParent;
            output.inputParent.outputParent = ConnectedTank.GetComponent<outputInputManager>();

            if(started == true)
            {
                if(tankStatusTracker == null)
                {
                    tankStatusTracker = new TankStatusTracker();
                    startTime = MyTime.time;
                    Tank tank = ConnectedTank.GetComponent<Tank>();
                }
                
                //For tank you literally just treat it as a regular fuel tank? I guess...
                if(failed == false && tankStatusTracker != null)
                {
                    Tank tank = ConnectedTank.GetComponent<Tank>();

                    tankStatusTracker.times.Add(MyTime.time - startTime);
                    tankStatusTracker.Quantity.Add(tank.GetComponent<outputInputManager>().mass);
                    tankStatusTracker.Pressure.Add(tank.GetComponent<outputInputManager>().internalPressure);
                    tankStatusTracker.Volume.Add(tank.GetComponent<outputInputManager>().volume);
                    tankStatusTracker.state.Add(tank.GetComponent<outputInputManager>().state);

                    if(tank.GetComponent<outputInputManager>().tankState == "broken")
                    {
                        failed = true;
                    }
                }

                if(failed == true && tankStatusTracker != null)
                {
                    //Save results to file and null tracker and save new reliabili
                    started = false;
                    Tank tank = ConnectedTank.GetComponent<Tank>();

                    //Save test to file
                    if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' +  MasterManager.FolderName + "/Tests"))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' +  MasterManager.FolderName + "/Tests");
                    }

                    if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' +  MasterManager.FolderName + "/Tests/" + "TankPressureTests"))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' +  MasterManager.FolderName + "/Tests/" + "TankPressureTests");
                    }

                    string saveName = "/"+ ConnectedTank.GetComponent<RocketPart>()._partName + MyTime.time.ToString() + ".json";

                    if(!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' +  MasterManager.FolderName + "/Tests/" + "TankPressureTests" + saveName))
                    {
                        var jsonString = JsonConvert.SerializeObject(tankStatusTracker);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' +  MasterManager.FolderName + "/Tests/" + "TankPressureTests" + saveName, jsonString);
                    }

                    //Save new engine reliability and maxTime
                    if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + "/" + ConnectedTank.GetComponent<RocketPart>().name + ".json"))
                    {
                        //Probably no data to update here
                        //Indeed
                    }
                    
                    failed = false;
                    tankStatusTracker = null;
                }
            }
        }
    }
}
