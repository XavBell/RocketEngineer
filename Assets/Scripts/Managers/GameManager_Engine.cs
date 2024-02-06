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
    public GameObject Engine;
    //Specs Float
    private float mass;
    private float thrust;
    private float rate;
    private float maxAngle;
    private float speed;
    private float cost;
    [SerializeField]
    private TMP_InputField savePath;
    private string saveName;

    //For data visualizer
    [SerializeField]
    private TMP_Dropdown engineDropdown;

    private savePath savePathRef = new savePath();

    private MasterManager MasterManager = new MasterManager();
    [SerializeField]
    private GameObject MainPanel;
    [SerializeField]
    private GameObject CreatorPanel;
    [SerializeField]
    private GameObject DataPanel;

    //Text field for Data Viewer
    [SerializeField]
    private TMP_Text engineName;
    [SerializeField]
    private TMP_Text engineExpectedThrust;
    [SerializeField]
    private TMP_Text engineMaximumRunTime;
    [SerializeField]
    private TMP_Text engineEstimatedReliability;
    [SerializeField]
    private TMP_Text engineMassFlowRate;
    [SerializeField]
    private TMP_Text engineMass;

    //Text field for Creator
    [SerializeField]
    private TMP_Text engineMass_C;
    [SerializeField]
    private TMP_Text engineThurst_C;
    [SerializeField]
    private TMP_Text engineFlowRate_C;
    [SerializeField]
    private TMP_Text engineReliability_C;
    [SerializeField]
    private TMP_Text engineCost;
    [SerializeField]
    private GameObject[] panels;

    [SerializeField]
    private Turbine selectedTurbine;
    [SerializeField]
    private Pump selectedPump;
    [SerializeField]
    private Nozzle selectedNozzle;
    [SerializeField]
    private TVC selectedTVC;


    //Buttons for tech tree and parts
    public List<GameObject> TurbineBtn;
    public List<GameObject> NozzleBtn;
    public List<GameObject> PumpBtn;
    public List<GameObject> TVCBtn;


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
        setValues(selectedTVC, selectedNozzle, selectedTurbine, selectedPump);
    }

    public void Create()
    {
        setValues(selectedTVC, selectedNozzle, selectedTurbine, selectedPump);

        if (savePath.text != null)
        {
            save();
        }
    }

    public void setNozzle(Nozzle nozzle)
    {
        selectedNozzle = nozzle;

        Engine.GetComponentInChildren<autoSpritePositionner>().nozzle.GetComponent<SpriteRenderer>().sprite = nozzle.sprite;
    }

    public void setTurbine(Turbine turbine)
    {
        selectedTurbine = turbine;
        Engine.GetComponentInChildren<autoSpritePositionner>().turbine.GetComponent<SpriteRenderer>().sprite = turbine.sprite;
    }

    public void setPump(Pump pump)
    {
        selectedPump = pump;
        Engine.GetComponentInChildren<autoSpritePositionner>().pump.GetComponent<SpriteRenderer>().sprite = pump.sprite;
    }

    public void setTVC(TVC tvc)
    {
        selectedTVC = tvc;
    }

    public void setValues(TVC tvc, Nozzle nozzle, Turbine turbine, Pump pump)
    {
        if (tvc != null && nozzle != null && turbine != null && pump != null)
        {
            //Order is important
            rate = turbine.rate;
            thrust = pump.thrust;

            float rateChangeNozzle = rate * (nozzle.rateModifier - 1);
            float thrustChangeNozzle = thrust * (nozzle.thrustModifier - 1);
            float thrustChangeTurbine = thrust * (turbine.thrustModifier - 1);

            rate += rateChangeNozzle;
            thrust += thrustChangeNozzle + thrustChangeTurbine;

            maxAngle = tvc.maxAngle;
            speed = tvc.speed;

            mass = turbine.mass + tvc.mass + nozzle.mass + pump.mass;
            cost = nozzle.cost + tvc.cost + nozzle.cost + pump.cost;

            updateCreatorData(mass.ToString(), rate.ToString(), thrust, 0.05f, cost);
        }

    }

    public void save()
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder);
        }

        saveName = "/" + savePath.text;

        if (!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json"))
        {
            saveEngine saveObject = new saveEngine();
            saveObject.path = savePathRef.engineFolder;
            saveObject.engineName = saveName;
            saveObject.thrust_s = thrust;
            saveObject.mass_s = mass;
            saveObject.rate_s = rate;
            saveObject.tvcSpeed_s = speed;
            saveObject.tvcMaxAngle_s = maxAngle;
            saveObject.cost = Convert.ToSingle(engineCost.text);

            saveObject.tvcName_s = selectedTVC.TVCName;
            saveObject.nozzleName_s = selectedNozzle.nozzleName;
            saveObject.turbineName_s = selectedTurbine.turbineName;
            saveObject.pumpName_s = selectedPump.pumpName;

            saveObject.reliability = 0.05f;
            saveObject.maxTime = 1f;


            var jsonString = JsonConvert.SerializeObject(saveObject);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json", jsonString);
        }
        else if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json"))
        {
            saveEngine saveEngine = new saveEngine();
            var jsonString2 = JsonConvert.SerializeObject(saveEngine);
            jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json");
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString2);

            if (loadedEngine.usedNum == 0)
            {
                File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json");
                save();
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
        if (fileInfo.Length == 0)
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
        engineExpectedThrust.text = (loadedEngine.reliability * loadedEngine.thrust_s).ToString() + "-" + ((2 - loadedEngine.reliability) * loadedEngine.thrust_s).ToString();
        engineMaximumRunTime.text = loadedEngine.maxTime.ToString();
        engineEstimatedReliability.text = loadedEngine.reliability.ToString();
        engineMassFlowRate.text = loadedEngine.rate_s.ToString();
        engineMass.text = loadedEngine.mass_s.ToString();
    }

    public void updateCreatorData(string mass, string flowRate, float thrust, float reliability, float cost)
    {
        engineThurst_C.text = (thrust * reliability).ToString() + "-" + ((2 - reliability) * thrust).ToString();
        engineMass_C.text = mass;
        engineFlowRate_C.text = flowRate;
        engineReliability_C.text = reliability.ToString();
        engineCost.text = cost.ToString();
    }

    public void activateDeactivate(GameObject button)
    {
        hidePanels(button);
        if (button.activeSelf == true)
        {
            button.SetActive(false);
            return;
        }

        if (button.activeSelf == false)
        {
            button.SetActive(true);
            return;
        }
    }

    public void hidePanels(GameObject excludedPanel)
    {
        foreach (GameObject panel in panels)
        {
            if (panel != excludedPanel)
            {
                panel.SetActive(false);
            }
        }
    }
}
