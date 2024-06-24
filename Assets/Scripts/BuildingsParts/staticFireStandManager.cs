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
    public container oxidizer;
    public container fuel;

    public flowController oxidizerController;
    public flowController fuelController;
    public string standName;
    public GameObject button;
    public float ratio;
    public bool started = false;
    public bool stopped = true;
    public bool fuelSufficient = true;
    public bool oxidizerSufficient = true;
    public TimeManager MyTime;
    public EngineStaticFireTracker engineStaticFireTracker;
    public bool failed = false;
    public float startTime;
    public float minThrust;
    public float maxThrust;

    public float fuelFlowEngine;
    public float ratioEngine;
    public float perFrameConsumedOxidizer;
    public float perFrameConsumedFuel;
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
        if(ConnectedEngine != null)
        {
            if(started == true)
            {
                if(engineStaticFireTracker == null)
                {
                    engineStaticFireTracker = new EngineStaticFireTracker();
                    startTime = (float)MyTime.time;
                    ConnectedEngine.GetComponent<EngineComponent>().active = true;
                    fuelFlowEngine = ConnectedEngine.GetComponent<EngineComponent>().maxFuelFlow;
                    ratioEngine = ConnectedEngine.GetComponent<EngineComponent>().usedPropellant.oxidizerToFuelRatio;
                    ConnectedEngine.GetComponent<EngineComponent>().maxFuelFlow = 0;
                    perFrameConsumedOxidizer = fuelFlowEngine * ratioEngine;
                    perFrameConsumedFuel = fuelFlowEngine * (1/ratioEngine);
                }

                if(failed == false && engineStaticFireTracker != null && oxidizer.moles > 0 && fuel.moles > 0 && oxidizer.state == "liquid" && fuel.state == "liquid")
                {
                    EngineComponent engine = ConnectedEngine.GetComponent<EngineComponent>();
                    float outThrust = engine.produceThrust(1).magnitude * MyTime.deltaTime;
                    if(oxidizer.moles - perFrameConsumedOxidizer/oxidizer.substance.MolarMass * MyTime.deltaTime < 0)
                    {
                        oxidizer.moles = 0;
                        oxidizerSufficient = false;
                    }else{
                        oxidizer.moles -= perFrameConsumedOxidizer*1000/oxidizer.substance.MolarMass * MyTime.deltaTime;
                    }
                    if(fuel.moles - perFrameConsumedFuel/fuel.substance.MolarMass * MyTime.deltaTime < 0)
                    {
                        fuel.moles = 0;
                        fuelSufficient = false;
                    }else{
                        fuel.moles -= perFrameConsumedFuel*1000/fuel.substance.MolarMass * MyTime.deltaTime;
                    }
                    engineStaticFireTracker.thrusts.Add(outThrust);
                    engineStaticFireTracker.times.Add((float)(MyTime.time - startTime));
                    engineStaticFireTracker.fuelQty.Add(fuel.mass);
                    engineStaticFireTracker.oxidizerQty.Add(oxidizer.mass);
                    if(engine.operational == false)
                    {
                        failed = true;
                    }
                }

                if((failed == true || fuelSufficient == false || oxidizerSufficient == false || stopped == true || (started == true && ConnectedEngine.GetComponent<EngineComponent>().active == false)) && engineStaticFireTracker != null)
                {
                    //Save results to file and null tracker and save new reliabili
                    started = false;
                    EngineComponent engine = ConnectedEngine.GetComponent<EngineComponent>();
                    float reliabilityToAdd = ((float)(MyTime.time - startTime))/engine.maxTime * 0.05f;
                    if((MyTime.time-startTime) > engine.maxTime)
                    {
                        engine.maxTime = (float)(MyTime.time - startTime);
                    }

                    if((engine.reliability + reliabilityToAdd) <= 1f)
                    {
                        engine.reliability += reliabilityToAdd;
                    }

                    //Save test to file
                    if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests"))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests");
                    }

                    if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests/" + "StaticFireEngine"))
                    {
                        Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests/" + "StaticFireEngine");
                    }

                    string saveName = "/"+ ConnectedEngine.GetComponent<PhysicsPart>().path + MyTime.time.ToString() + ".json";

                    if(!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' +  MasterManager.FolderName + "/Tests/" + "StaticFireEngine" + saveName))
                    {
                        var jsonString = JsonConvert.SerializeObject(engineStaticFireTracker);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + "/Tests/" + "StaticFireEngine" + saveName, jsonString);
                    }

                    //Save new engine reliability and maxTime
                    if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder  + ConnectedEngine.GetComponent<PhysicsPart>().path + ".json"))
                    {
                        saveEngine saveObject = new saveEngine();
                        var jsonString = JsonConvert.SerializeObject(saveObject);
                        jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + ConnectedEngine.GetComponent<PhysicsPart>().path  + ".json");
                        saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);

                        EngineComponent part = ConnectedEngine.GetComponent<EngineComponent>();
                        saveObject = loadedEngine;
                        //Save previous unchanged value
                        saveObject.path = savePathRef.engineFolder;
                        saveObject.engineName = loadedEngine.engineName;
                        saveObject.thrust_s = engine.maxThrust;
                        saveObject.mass_s = engine.GetComponent<PhysicsPart>().mass;
                        saveObject.rate_s = fuelFlowEngine;

                        saveObject.nozzleName_s = engine._nozzleName;
                        saveObject.turbineName_s = engine._turbineName;
                        saveObject.pumpName_s = engine._pumpName;
                        
                        //Updated Value
                        saveObject.reliability = engine.reliability;
                        saveObject.maxTime = engine.maxTime;
                        saveObject.cost = engine.GetComponent<PhysicsPart>().cost;

                        var jsonString1 = JsonConvert.SerializeObject(saveObject);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + ConnectedEngine.GetComponent<PhysicsPart>().path  + ".json", jsonString1);
                    }

                    stopped = true;
                    engineStaticFireTracker = null;
                    oxidizerSufficient = true;
                    fuelSufficient = true;
                }
            }

            if(started == false)
            {
                stopped = true;
                oxidizerSufficient = true;
                fuelSufficient = true;
            }
        }
    }
}
