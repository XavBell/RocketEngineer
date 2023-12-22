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
    public float speed;
    public string pumpName;
    public string turbineName;
    public string nozzleName;
    public string TVCName;
    public TMP_InputField savePath;
    public string saveName;

    public TMP_Dropdown pumpDropdown;
    public TMP_Dropdown turbineDropdown;

    public TMP_Dropdown nozzleDropdown;

    public TMP_Dropdown tvcDropdown;

    public TMP_Dropdown engineDropdown;

    public savePath savePathRef = new savePath();

    public MasterManager MasterManager = new MasterManager();

    public GameObject MainPanel;
    public GameObject CreatorPanel;
    public GameObject DataPanel;

    //Text field for Data Viewer
    public TMP_Text engineName;
    public TMP_Text engineExpectedThrust;
    public TMP_Text engineMaximumRunTime;
    public TMP_Text engineEstimatedReliability;
    public TMP_Text engineMassFlowRate;
    public TMP_Text engineMass;

    //Text field for Creator
    public TMP_Text engineMass_C;
    public TMP_Text engineThurst_C;
    public TMP_Text engineFlowRate_C;
    public TMP_Text engineReliability_C;


    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
            initializeEngineInFolder();
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
        {
            UpdateValues();
        }

    }


    public void backToBuild()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void BackToMain()
    {
        MainPanel.SetActive(true);
        CreatorPanel.SetActive(false);
        DataPanel.SetActive(false);
    }

    public void EnterCreator()
    {
        MainPanel.SetActive(false);
        CreatorPanel.SetActive(true);
        DataPanel.SetActive(false);
    }

    public void EnterData()
    {
        MainPanel.SetActive(false);
        CreatorPanel.SetActive(false);
        DataPanel.SetActive(true);
    }

    public void UpdateValues()
    {
        Nozzle nozzle = new Nozzle();
        Pump pump = new Pump();
        Turbine turbine = new Turbine();
        TVC tvc = new TVC();

        string selectedTurbine = turbineDropdown.options[turbineDropdown.value].text.ToString();
        string selectedPump = pumpDropdown.options[pumpDropdown.value].text.ToString();
        string selectedNozzle = nozzleDropdown.options[nozzleDropdown.value].text.ToString();
        string selectedTVC = tvcDropdown.options[tvcDropdown.value].text.ToString();

        setTurbine(selectedTurbine, turbine);
        setPump(selectedPump, pump);
        setNozzle(selectedNozzle, nozzle);
        setTVC(selectedTVC, tvc);
        setValues(tvc, nozzle, turbine, pump);
    }

    public void Create()
    {
        Nozzle nozzle = new Nozzle();
        Pump pump = new Pump();
        Turbine turbine = new Turbine();
        TVC tvc = new TVC();

        string selectedTurbine = turbineDropdown.options[turbineDropdown.value].text.ToString();
        string selectedPump = pumpDropdown.options[pumpDropdown.value].text.ToString();
        string selectedNozzle = nozzleDropdown.options[nozzleDropdown.value].text.ToString();
        string selectedTVC = tvcDropdown.options[tvcDropdown.value].text.ToString();

        setTurbine(selectedTurbine, turbine);
        setPump(selectedPump, pump);
        setNozzle(selectedNozzle, nozzle);
        setTVC(selectedTVC, tvc);
        setValues(tvc, nozzle, turbine, pump);

        if(savePath.text != null)
        {
            save(selectedTVC, selectedNozzle, selectedPump, selectedTurbine);
        }
    }

    public void setNozzle(string selectedNozzle, Nozzle nozzle)
    {
        if(selectedNozzle == "NozzleRaptor")
        {
            NozzleRaptor nozzleRaptor = new NozzleRaptor();

            nozzle.nozzleName = nozzleRaptor.nozzleName;
            nozzle.mass = nozzleRaptor.mass;
            nozzle.thrustModifier = nozzleRaptor.thrustModifier;
            nozzle.rateModifier = nozzleRaptor.rateModifier;
        }

        if(selectedNozzle == "NozzleSmall")
        {
            NozzleSmall nozzleSmall = new NozzleSmall();

            nozzle.nozzleName = nozzleSmall.nozzleName;
            nozzle.mass = nozzleSmall.mass;
            nozzle.thrustModifier = nozzleSmall.thrustModifier;
            nozzle.rateModifier = nozzleSmall.rateModifier;
        }
    }

    public void setTurbine(string selectedTurbine, Turbine turbine)
    {
        if(selectedTurbine == "TurbineRaptor")
        {
            TurbineRaptor turbineRaptor = new TurbineRaptor();

            turbine.turbineName = turbineRaptor.turbineName;
            turbine.mass = turbineRaptor.mass;
            turbine.rate = turbineRaptor.rate;
            turbine.thrustModifier = turbineRaptor.thrustModifier;
        }

        if(selectedTurbine == "TurbineSmall")
        {
            TurbineSmall turbineSmall = new TurbineSmall();

            turbine.turbineName = turbineSmall.turbineName;
            turbine.mass = turbineSmall.mass;
            turbine.rate = turbineSmall.rate;
            turbine.thrustModifier = turbineSmall.thrustModifier;
        }
    }

    public void setPump(string selectedPump, Pump pump)
    {
        if(selectedPump == "PumpRaptor")
        {
            PumpRaptor pumpRaptor = new PumpRaptor();

            pump.pumpName = pumpRaptor.pumpName;
            pump.mass = pumpRaptor.mass;
            pump.thrust = pumpRaptor.thrust;
        }

        if(selectedPump == "PumpSmall")
        {
            PumpSmall pumpSmall = new PumpSmall();

            pump.pumpName = pumpSmall.pumpName;
            pump.mass = pumpSmall.mass;
            pump.thrust = pumpSmall.thrust;
        }
    }

    public void setTVC(string selectedTVC, TVC tvc)
    {
        if(selectedTVC == "TVCRaptor")
        {
            TVCRaptor tvcRaptor = new TVCRaptor();

            tvc.TVCName = tvcRaptor.TVCName;
            tvc.mass = tvcRaptor.mass;
            tvc.maxAngle = tvcRaptor.maxAngle;
            tvc.speed = tvcRaptor.speed; 
        }

        if(selectedTVC == "TVCSmall")
        {
            TVCSmall tvcSmall = new TVCSmall();

            tvc.TVCName = tvcSmall.TVCName;
            tvc.mass = tvcSmall.mass;
            tvc.maxAngle = tvcSmall.maxAngle;
            tvc.speed = tvcSmall.speed; 
        }
    }

    public void setValues(TVC tvc, Nozzle nozzle, Turbine turbine, Pump pump)
    {
        TVCName = tvc.TVCName;
        nozzleName = nozzle.nozzleName;
        pumpName = pump.pumpName;
        turbineName = turbine.turbineName;

        //Order is important
        rate = turbine.rate;
        thrust = pump.thrust;

        float rateChangeNozzle = rate * (nozzle.rateModifier-1);
        float thrustChangeNozzle = thrust * (nozzle.thrustModifier-1);
        float thrustChangeTurbine = thrust * (turbine.thrustModifier-1);

        rate += rateChangeNozzle;
        thrust += thrustChangeNozzle + thrustChangeTurbine;

        maxAngle = tvc.maxAngle;
        speed = tvc.speed;

        mass = turbine.mass + tvc.mass + nozzle.mass + pump.mass;

        updateCreatorData(mass.ToString(), rate.ToString(), thrust, 0.05f);
    }

    public void save(string selectedTVC, string selectedNozzle, string selectedPump, string selectedTurbine)
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder);
        }

        saveName = "/" + savePath.text;

        if(!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json"))
        {
            saveEngine saveObject = new saveEngine();
            saveObject.path = savePathRef.engineFolder;
            saveObject.engineName = saveName;
            saveObject.thrust_s = thrust;
            saveObject.mass_s = mass;
            saveObject.rate_s = rate;
            saveObject.tvcSpeed_s = speed;
            saveObject.tvcMaxAngle_s = maxAngle;

            saveObject.tvcName_s = selectedTVC;
            saveObject.nozzleName_s = selectedNozzle;
            saveObject.turbineName_s = selectedTurbine;
            saveObject.pumpName_s = selectedPump;

            saveObject.reliability = 0.05f;
            saveObject.maxTime = 1f;


            var jsonString = JsonConvert.SerializeObject(saveObject);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json", jsonString);
        }else if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName+ savePathRef.engineFolder + saveName + ".json"))
        {
            saveEngine saveEngine = new saveEngine();
            var jsonString2 = JsonConvert.SerializeObject(saveEngine);
            jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json");
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString2);

            if(loadedEngine.usedNum == 0)
            {
                File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json");
                save(selectedTVC, selectedNozzle, selectedPump, selectedTurbine);
                return;
            }
        }
    }

    public void initializeEngineInFolder()
    {
        List<string> options = new List<string>();
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder);
            return;
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder);
        var fileInfo = info.GetFiles();
        if(fileInfo.Length == 0)
        {
            return;
        }
        foreach (var file in fileInfo)
        {
            options.Add(Path.GetFileName(file.ToString()));
        }
        engineDropdown.AddOptions(options);
    }

    public void loadData()
    {
        saveEngine saveEngine = new saveEngine();
        var jsonString = JsonConvert.SerializeObject(saveEngine);
        jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + "/" + engineDropdown.options[engineDropdown.value].text.ToString());
        saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);
        engineName.text = loadedEngine.engineName;
        engineExpectedThrust.text = (loadedEngine.reliability * loadedEngine.thrust_s).ToString() + "-" + ((2-loadedEngine.reliability)*loadedEngine.thrust_s).ToString();
        engineMaximumRunTime.text = loadedEngine.maxTime.ToString();
        engineEstimatedReliability.text = loadedEngine.reliability.ToString();
        engineMassFlowRate.text = loadedEngine.rate_s.ToString();
        engineMass.text = loadedEngine.mass_s.ToString();
    }

    public void updateCreatorData(string mass, string flowRate, float thrust, float reliability)
    {
        engineThurst_C.text = (thrust*reliability).ToString() + "-" + ((2-reliability)*thrust).ToString();
        engineMass_C.text = mass;
        engineFlowRate_C.text = flowRate;
        engineReliability_C.text = reliability.ToString();
    }

}
