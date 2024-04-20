using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


public class Engine : RocketPart
{ 
    public List<Nozzle> nozzleReferences = new List<Nozzle>();
    public List<Turbine> turbineReferences = new List<Turbine>();
    public List<Pump> pumpReferences = new List<Pump>();
    public savePath savePathRef = new savePath();
    private MasterManager MasterManager;
    public float _thrust;
    public float _rate;
    public float _tvcSpeed;
    public float _maxAngle;

    public string _tvcName;
    public string _nozzleName;
    public string _pumpName;
    public string _turbineName;
    public int stageNumber;
    //Reliability is between 0 and 1
    public float reliability;
    public float maxTime;
    public bool active = false;
    public bool operational = true;

    public bool willFail = false;
    public float timeOfFail;
    public float outReadThrust;
    public bool willExplode = false;
    public bool lackFuel = false; //Used between Stages control and ParticleController

    public void activate()
    {
        active = true;
    }

    void Start()
    {
        MasterManager = FindObjectOfType<MasterManager>();
        InitializeSprite();
        InitializeFail();
    }

    public void InitializeSprite()
    {
        foreach(Nozzle nozzle in nozzleReferences)
        {
            if(nozzle.nozzleName == _nozzleName)
            {
                this.GetComponentInChildren<autoSpritePositionner>().nozzle.GetComponent<SpriteRenderer>().sprite = nozzle.sprite;
            }
        }

        foreach(Pump pump in pumpReferences)
        {
            if(pump.pumpName == _pumpName)
            {
                this.GetComponentInChildren<autoSpritePositionner>().pump.GetComponent<SpriteRenderer>().sprite = pump.sprite;
            }
        }

        foreach(Turbine turbine in turbineReferences)
        {
            if(turbine.turbineName == _turbineName)
            {
                this.GetComponentInChildren<autoSpritePositionner>().turbine.GetComponent<SpriteRenderer>().sprite = turbine.sprite;
            }
        }
    }

    public void InitializeFail()
    {
        float percentageOfThrust = Random.Range(reliability, 2-reliability);
        float outThrust = _thrust * percentageOfThrust;
        float minThrust = _thrust * 0.8f;
        float maxThrust = _thrust * 1.2f;
        if(outThrust < minThrust || outThrust > maxThrust)
        {
            willFail = true;
            if(percentageOfThrust < 0.5f || percentageOfThrust > 1.5f)
            {
                willExplode = true;
            }
            timeOfFail = maxTime * percentageOfThrust;
            if(timeOfFail < 2)
            {
                timeOfFail = 2;
            }
        }else{
            willFail = false;
        }
    }

    public float CalculateOutputThrust(double time, out bool fail)
    {
        float percentageOfThrust = Random.Range(0.99f, 1.01f);
        float outThrust = _thrust * percentageOfThrust;
        if(willFail == true && timeOfFail <= time)
        {
            outThrust = 0;
            fail = true;
            if(willExplode == true)
            {
                if(this.gameObject.transform.parent.GetComponent<staticFireStandManager>() != null)
                {
                    this.gameObject.transform.parent.GetComponent<staticFireStandManager>().failed = false;
                    this.gameObject.transform.parent.GetComponent<staticFireStandManager>().started = false;
                    this.gameObject.transform.parent.GetComponent<staticFireStandManager>().stopped = true;
                    
                }
                if(this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>() != null)
                {
                    //WHen on a rocket
                    GameObject toDestroy = this.gameObject.transform.parent.gameObject;
                    FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destroyed due to engine failure");
                    //Add 5% reliability
                    if(reliability + 0.05f <= 1f)
                    {
                        reliability += 0.05f;
                        saveReliability();
                    }else{
                        reliability = 1f;
                        saveReliability();
                    }
                    Destroy(toDestroy);
                }else if(explosion != null){
                    //When on a static fire stand
                    explosion.transform.parent = null;
                    explosion.Play();
                    FindObjectOfType<StaticFireViewer>().Terminate();
                    FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destroyed due to engine failure");
                    //Add 5% reliability
                    if(reliability + 0.05f <= 1f)
                    {
                        reliability += 0.05f;
                        saveReliability();
                    }else{
                        reliability = 1f;
                        saveReliability();
                    }
                    Destroy(this.gameObject);
                }
            }
        }else if(willFail == false){
            fail = false;
        }else if(willFail == true && timeOfFail >= time){
            fail = false;
        }else{
            fail = false;
        }
        outReadThrust = outThrust;
        return outThrust;
    }

    public void saveReliability()
    {
        //Save new engine reliability and maxTime
        if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + "/" + this.GetComponent<Engine>()._partName + ".json"))
        {
            saveEngine saveObject = new saveEngine();
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + "/" + this.GetComponent<Engine>()._partName + ".json");
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);

            RocketPart part = this.GetComponent<RocketPart>();
            Engine engine = this.GetComponent<Engine>();
            saveObject = loadedEngine;
            //Save previous unchanged value
            saveObject.path = savePathRef.engineFolder;
            saveObject.engineName = part._partName;
            saveObject.thrust_s = engine._thrust;
            saveObject.mass_s = engine._partMass;
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
            saveObject.cost = engine._partCost;

            var jsonString1 = JsonConvert.SerializeObject(saveObject);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + "/" + this.GetComponent<Engine>()._partName + ".json", jsonString1);
        }
        //Apply new reliability to all rockets
        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
        var rocketNamesFiles = info.GetFiles();
        List<string> rocketNames = new List<string>();
        foreach (var file in rocketNamesFiles)
        {
            rocketNames.Add(file.Name);
        }
        foreach (var rocket in rocketNames)
        {
            if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + "/" + rocket))
            {
                savecraft saveObject = new savecraft();
                var jsonString = JsonConvert.SerializeObject(saveObject);
                jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + "/" + rocket);
                savecraft loadedRocket = JsonConvert.DeserializeObject<savecraft>(jsonString);

                saveObject = loadedRocket;
                int i = 0;
                foreach (string engine1 in saveObject.engineName)
                {
                    if (engine1 == this.GetComponent<Engine>()._partName)
                    {
                        saveObject.reliability[i] = this.GetComponent<Engine>().reliability;
                    }
                    i++;
                }

                var jsonString1 = JsonConvert.SerializeObject(saveObject);
                System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + "/" + rocket, jsonString1);
            }


        }
    }
}
