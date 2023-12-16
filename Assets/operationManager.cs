using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class operationManager : MonoBehaviour
{
    private MasterManager MasterManager;
    private savePath savePathRef = new savePath();
    [SerializeField] GameObject selectButton;
    [SerializeField]private TMP_Dropdown vehicleLaunchDropdown;
    [SerializeField]private TMP_Dropdown vehicleWDRDropdown;
    [SerializeField]private TMP_Dropdown vehicleStaticFireDropdown;
    [SerializeField]private TMP_Dropdown engineDropdown;
    [SerializeField]private TMP_Dropdown tankDropdown;
    [SerializeField]private TMP_Dropdown operationDropdown;
    [SerializeField]private StageViewer stageViewer;
    [SerializeField]private StaticFireViewer staticFireViewer;
    public GameObject selectedLaunchPad;
    public OnClick onclick;
    public TMP_Text launchpadName;
    public TMP_Text staticFireStandName;
    public TMP_Text standName;
    public TMP_Text launchPadWDR;
    public TMP_Text launchPadSF;
    public GameObject launchPanel;
    public GameObject staticFireEnginePanel;
    public GameObject staticFireRocketPanel;
    public GameObject tankPressurePanel;
    public GameObject WDRPanel;
    // Start is called before the first frame update
    void Start()
    {
        if(MasterManager == null)
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
        }
        retrieveRocketSaved();
        retrieveEngineSaved();
        retrieveTankSaved();
    }

    // Update is called once per frame
    void Update()
    {
        if(MasterManager == null)
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
        }
    }

    public void updatePanel()
    {
        if(operationDropdown.options[operationDropdown.value].text == "Launch")
        {
            launchPanel.SetActive(true);
            staticFireEnginePanel.SetActive(false);
            staticFireRocketPanel.SetActive(false);
            tankPressurePanel.SetActive(false);
            WDRPanel.SetActive(false);
        }

        if(operationDropdown.options[operationDropdown.value].text == "Static Fire (Engine)")
        {
            launchPanel.SetActive(false);
            staticFireEnginePanel.SetActive(true);
            staticFireRocketPanel.SetActive(false);
            tankPressurePanel.SetActive(false);
            WDRPanel.SetActive(false);
        }

        if(operationDropdown.options[operationDropdown.value].text == "Static Fire (Rocket)")
        {
            launchPanel.SetActive(false);
            staticFireEnginePanel.SetActive(false);
            staticFireRocketPanel.SetActive(true);
            tankPressurePanel.SetActive(false);
            WDRPanel.SetActive(false);
        }

        if(operationDropdown.options[operationDropdown.value].text == "Pressure Test (Tank)")
        {
            launchPanel.SetActive(false);
            staticFireEnginePanel.SetActive(false);
            staticFireRocketPanel.SetActive(false);
            tankPressurePanel.SetActive(true);
            WDRPanel.SetActive(false);
        }

        if(operationDropdown.options[operationDropdown.value].text == "Wet Dress Rehearsal")
        {
            launchPanel.SetActive(false);
            staticFireEnginePanel.SetActive(false);
            staticFireRocketPanel.SetActive(false);
            tankPressurePanel.SetActive(false);
            WDRPanel.SetActive(true);
        }
    }

    public void retrieveRocketSaved()
    {
        List<string> options = new List<string>();
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
            return;
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
        var fileInfo = info.GetFiles();
        if(fileInfo.Length == 0)
        {
            return;
        }
        foreach (var file in fileInfo)
        {
            options.Add(Path.GetFileName(file.ToString()));
        }
        vehicleLaunchDropdown.AddOptions(options);
        vehicleStaticFireDropdown.AddOptions(options);
        vehicleWDRDropdown.AddOptions(options);
    }

    public void retrieveEngineSaved()
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

    public void retrieveTankSaved()
    {
        List<string> options = new List<string>();
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder);
            return;
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder);
        var fileInfo = info.GetFiles();
        if(fileInfo.Length == 0)
        {
            return;
        }
        foreach (var file in fileInfo)
        {
            options.Add(Path.GetFileName(file.ToString()));
        }
        tankDropdown.AddOptions(options);
    }

    public void selectLaunchpad()
    {
        launchPadManager[] launchPads = FindObjectsOfType<launchPadManager>();
        foreach(launchPadManager launchPad in launchPads)
        {
            launchPad.button.SetActive(true);
            launchPad.button.GetComponent<OnClick>().op = this;
            launchPad.button.GetComponent<OnClick>().savedLaunchpad = launchPad.gameObject;
        }
    }

    public void selectStaticFireStand()
    {
        staticFireStandManager[] stands = FindObjectsOfType<staticFireStandManager>();
        foreach(staticFireStandManager stand in stands)
        {
            stand.button.SetActive(true);
            stand.button.GetComponent<OnClick>().op = this;
            stand.button.GetComponent<OnClick>().savedLaunchpad = stand.gameObject;
        }
    }

    public void selectPressureStand()
    {
        standManager[] stands = FindObjectsOfType<standManager>();
        foreach(standManager stand in stands)
        {
            stand.button.SetActive(true);
            stand.button.GetComponent<OnClick>().op = this;
            stand.button.GetComponent<OnClick>().savedLaunchpad = stand.gameObject;
        }
    }

    public void hidePadButtons()
    {
        launchPadManager[] launchPads = FindObjectsOfType<launchPadManager>();
        foreach(launchPadManager launchPad in launchPads)
        {
            launchPad.button.SetActive(false);
            //launchPad.button.GetComponent<OnClick>().op = null;
        }

        staticFireStandManager[] stands = FindObjectsOfType<staticFireStandManager>();
        foreach(staticFireStandManager stand in stands)
        {
            stand.button.SetActive(false);
            //stand.button.GetComponent<OnClick>().op = null;
        }

        standManager[] stands1 = FindObjectsOfType<standManager>();
        foreach(standManager stand in stands1)
        {
            stand.button.SetActive(false);
            //stand.button.GetComponent<OnClick>().op = null;
        }

        if(selectedLaunchPad != null)
        {
            if(selectedLaunchPad.GetComponent<launchPadManager>())
            {
                launchpadName.text = "launchpad";
                launchPadSF.text = "launchpad";
                launchPadWDR.text = "launchpad";
            }

            if(selectedLaunchPad.GetComponent<staticFireStandManager>())
            {
                staticFireStandName.text = "staticFireStand";
            }

            if(selectedLaunchPad.GetComponent<standManager>())
            {
                standName.text = "pressureStand";
            }
        }
    }

    public void run()
    {
        if(operationDropdown.value == 0) //Launch
        {
            int value = vehicleLaunchDropdown.value;
            onclick.path = "/"+vehicleLaunchDropdown.options[value].text.ToString();
            onclick.launchPad = selectedLaunchPad;
            onclick.load("/rockets");
            stageViewer.gameObject.SetActive(true);
            stageViewer.rocket = onclick.spawnedRocket;
            stageViewer.updateStagesView(false);
            stageViewer.updateInfoPerStage(false);
            selectedLaunchPad.GetComponent<launchPadManager>().ConnectedRocket = onclick.spawnedRocket;
        }

        if(operationDropdown.value == 1) //Engine static fire
        {
            int value = engineDropdown.value;
            onclick.path = "/"+engineDropdown.options[value].text.ToString();
            onclick.launchPad = selectedLaunchPad;
            onclick.load("/engines");
            staticFireViewer.gameObject.SetActive(true);
            staticFireViewer.staticFireStand = selectedLaunchPad;
            selectedLaunchPad.GetComponent<staticFireStandManager>().ConnectedEngine = onclick.spawnedRocket;
        }

        if(operationDropdown.value == 2) //Tank test
        {
            int value = tankDropdown.value;
            onclick.path = "/"+tankDropdown.options[value].text.ToString();
            onclick.launchPad = selectedLaunchPad;
            onclick.load("/tanks");
            selectedLaunchPad.GetComponent<standManager>().ConnectedTank = onclick.spawnedRocket;
        }

        if(operationDropdown.value == 3) //WDR test
        {
            int value = vehicleWDRDropdown.value;
            onclick.path = "/"+vehicleWDRDropdown.options[value].text.ToString();
            onclick.launchPad = selectedLaunchPad;
            onclick.load("/rockets");
            selectedLaunchPad.GetComponent<launchPadManager>().ConnectedRocket = onclick.spawnedRocket;
            selectedLaunchPad.GetComponent<launchPadManager>().operation = "WDR";
        }

        if(operationDropdown.value == 4) //Rocket SF test
        {
            int value = vehicleStaticFireDropdown.value;
            onclick.path = "/"+vehicleStaticFireDropdown.options[value].text.ToString();
            onclick.launchPad = selectedLaunchPad;
            onclick.load("/rockets");
            selectedLaunchPad.GetComponent<launchPadManager>().ConnectedRocket = onclick.spawnedRocket;
            selectedLaunchPad.GetComponent<launchPadManager>().operation = "staticFire";
        }
    }

    void OnEnable()
    {
        if(MasterManager == null)
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
        }
    }


    
}
