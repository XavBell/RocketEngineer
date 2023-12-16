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

public class staticFireStandManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public savePath savePathRef = new savePath();
    public MasterManager MasterManager;
    public bool Spawned = false;
    public GameObject ConnectedEngine;
    public outputInputManager oxidizer;
    public outputInputManager fuel;
    public string standName;
    public GameObject button;
    public float ratio;
    public bool started;
    public bool fuelSufficient = false;
    public bool oxidizerSufficient = false;
    public TimeManager MyTime;
    public EngineStaticFireTracker engineStaticFireTracker;
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
    void Update()
    {
        if(ConnectedEngine != null)
        {
            string oxidizerType = oxidizer.substance;
            string fuelType = fuel.substance;

            if(oxidizerType == "LOX" && fuelType == "kerosene")
            {
                ratio = 2.56f;
            }

            if(started == true)
            {
                if(engineStaticFireTracker == null)
                {
                    engineStaticFireTracker = new EngineStaticFireTracker();
                    startTime = MyTime.time;
                    Engine engine = ConnectedEngine.GetComponent<Engine>();
                    minThrust = engine._thrust * 0.7f;
                    maxThrust = engine._thrust * 1.3f;
                }

                if(failed == false && (fuelSufficient == true && oxidizerSufficient == true) && engineStaticFireTracker != null)
                {
                    Engine engine = ConnectedEngine.GetComponent<Engine>();
                    bool withinThrustRange;
                    float outThrust = engine.CalculateOutputThrust(out withinThrustRange) * MyTime.deltaTime;

                    engineStaticFireTracker.thrusts.Add(outThrust);
                    engineStaticFireTracker.times.Add(MyTime.time - startTime);
                    engineStaticFireTracker.fuelQty.Add(fuel.mass);
                    engineStaticFireTracker.oxidizerQty.Add(oxidizer.mass);

                    if(withinThrustRange == false)
                    {
                        failed = true;
                    }
                }

                if((failed == true || fuelSufficient == false || oxidizerSufficient == false) && engineStaticFireTracker != null)
                {
                    //Save results to file and null tracker and save new reliabili
                    started = false;
                    Engine engine = ConnectedEngine.GetComponent<Engine>();
                    float reliabilityToAdd = (MyTime.time - startTime)/engine.maxTime * 0.01f;
                    if((MyTime.time-startTime) > engine.maxTime)
                    {
                        engine.maxTime = MyTime.time - startTime;
                    }

                    if((engine.reliability + reliabilityToAdd) <= 1f)
                    {
                        engine.reliability += reliabilityToAdd;
                    }

                    //Save test to file
                    if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + "Tests"))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + "Tests");
                    }

                    if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + "Tests/" + "StaticFireEngine"))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + "Tests/" + "StaticFireEngine");
                    }

                    string saveName = "/"+ ConnectedEngine.GetComponent<Part>().partName + MyTime.time.ToString() + ".json";

                    if(!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + "Tests/" + "StaticFireEngine" + saveName))
                    {
                        var jsonString = JsonConvert.SerializeObject(engineStaticFireTracker);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + "Tests/" + "StaticFireEngine" + saveName, jsonString);
                    }

                    //Save new engine reliability and maxTime
                    if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + "/" + ConnectedEngine.GetComponent<Part>().partName + ".json"))
                    {
                        saveEngine saveObject = new saveEngine();
                        Part part = ConnectedEngine.GetComponent<Part>();
                        //Save previous unchanged value
                        saveObject.path = savePathRef.engineFolder;
                        saveObject.engineName = part.partName;
                        saveObject.thrust_s = engine._thrust;
                        saveObject.mass_s = part.mass;
                        saveObject.rate_s = engine._rate;
                        saveObject.tvcSpeed_s = engine._tvcSpeed;
                        saveObject.tvcMaxAngle_s = engine._maxAngle;

                        saveObject.tvcName_s = engine._tvcName;
                        saveObject.nozzleName_s = engine._nozzleName;
                        saveObject.turbineName_s = engine._turbineName;
                        saveObject.pumpName_s = engine._pumpName;
                        
                        //Updated Value
                        saveObject.reliability = engine.reliability;
                        saveObject.maxTime = engine.maxTime;

                        var jsonString = JsonConvert.SerializeObject(saveObject);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json", jsonString);
                    }
                    
                    failed = false;
                    engineStaticFireTracker = null;
                }
            }
        }
    }
}
