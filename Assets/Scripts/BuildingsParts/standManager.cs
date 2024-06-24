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
using System.Linq;

public class standManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public savePath savePathRef = new savePath();
    public MasterManager MasterManager;
    public bool Spawned = false;
    public GameObject ConnectedTank;
    public container output;
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

            if(started == true)
            {
                if(tankStatusTracker == null)
                {
                    tankStatusTracker = new TankStatusTracker();
                    startTime = (float)MyTime.time;
                }
                
                //For tank you literally just treat it as a regular fuel tank? I guess...
                if(failed == false && tankStatusTracker != null)
                {
                    TankComponent tank = ConnectedTank.GetComponent<TankComponent>();

                    tankStatusTracker.times.Add((float)(MyTime.time - startTime));
                    tankStatusTracker.Quantity.Add(tank.GetComponent<container>().mass);
                    tankStatusTracker.Pressure.Add(tank.GetComponent<container>().internalPressure);
                    tankStatusTracker.Volume.Add(tank.GetComponent<container>().volume);
                    tankStatusTracker.state.Add(tank.GetComponent<container>().state);
                }

                if(failed == true && tankStatusTracker != null)
                {
                    logData();
                }
            }
        }
    }

    public void logData()
    {
        //Save results to file and null tracker and save new reliabili
        started = false;
        TankComponent tank = ConnectedTank.GetComponent<TankComponent>();

        //Save test to file
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests");
        }

        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests/" + "TankPressureTests"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests/" + "TankPressureTests");
        }

        string saveName = "/" + ConnectedTank.GetComponent<PhysicsPart>().name + MyTime.time.ToString() + ".json";

        if (!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests/" + "TankPressureTests" + saveName))
        {
            var jsonString = JsonConvert.SerializeObject(tankStatusTracker);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests/" + "TankPressureTests" + saveName, jsonString);
        }

        if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder +  ConnectedTank.GetComponent<PhysicsPart>().path + ".json"))
        {
            saveTank saveObject = new saveTank();
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + ConnectedTank.GetComponent<PhysicsPart>().path + ".json");
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);
            saveObject = loadedTank;
            //Save previous unchanged value
            saveObject.path = savePathRef.tankFolder;
            saveObject.tankName = loadedTank.tankName;
            saveObject.mass = loadedTank.mass;

            //Updated Value
            if(tankStatusTracker != null)
            {
                saveObject.maxRecPressure = tankStatusTracker.Pressure.Min();
            }
            saveObject.cost = tank.GetComponent<PhysicsPart>().cost;
            saveObject.tested = true;

            var jsonString1 = JsonConvert.SerializeObject(saveObject);
            Debug.Log("SAVING TO " + Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder +  ConnectedTank.GetComponent<PhysicsPart>().path + ".json");
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder +  ConnectedTank.GetComponent<PhysicsPart>().path + ".json", jsonString1);
        }

        failed = false;
        tankStatusTracker = null;
    }
}