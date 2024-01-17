using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class operationManager : MonoBehaviour
{
    private MasterManager MasterManager;
    private savePath savePathRef = new savePath();
    [SerializeField] GameObject selectButton;
    [SerializeField] private TMP_Dropdown vehicleLaunchDropdown;
    [SerializeField] private TMP_Dropdown vehicleWDRDropdown;
    [SerializeField] private TMP_Dropdown vehicleStaticFireDropdown;
    [SerializeField] private TMP_Dropdown engineDropdown;
    [SerializeField] private TMP_Dropdown tankDropdown;
    [SerializeField] private TMP_Dropdown operationDropdown;
    [SerializeField] private StageViewer stageViewer;
    [SerializeField] private StaticFireViewer staticFireViewer;
    [SerializeField] private PressureTestViewer pressureTestViewer;
    [SerializeField] private WDRTestViewer WDRTestViewer;
    [SerializeField] private RocketStaticFireViewer RocketStaticFireViewer;
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
    public GameObject popUpBuilt;
    // Start is called before the first frame update
    void Start()
    {
        if (MasterManager == null)
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
        if (MasterManager == null)
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
        }
    }

    public void updatePanel()
    {
        if (operationDropdown.options[operationDropdown.value].text == "Launch")
        {
            launchPanel.SetActive(true);
            staticFireEnginePanel.SetActive(false);
            staticFireRocketPanel.SetActive(false);
            tankPressurePanel.SetActive(false);
            WDRPanel.SetActive(false);
        }

        if (operationDropdown.options[operationDropdown.value].text == "Static Fire (Engine)")
        {
            launchPanel.SetActive(false);
            staticFireEnginePanel.SetActive(true);
            staticFireRocketPanel.SetActive(false);
            tankPressurePanel.SetActive(false);
            WDRPanel.SetActive(false);
        }

        if (operationDropdown.options[operationDropdown.value].text == "Static Fire (Rocket)")
        {
            launchPanel.SetActive(false);
            staticFireEnginePanel.SetActive(false);
            staticFireRocketPanel.SetActive(true);
            tankPressurePanel.SetActive(false);
            WDRPanel.SetActive(false);
        }

        if (operationDropdown.options[operationDropdown.value].text == "Pressure Test (Tank)")
        {
            launchPanel.SetActive(false);
            staticFireEnginePanel.SetActive(false);
            staticFireRocketPanel.SetActive(false);
            tankPressurePanel.SetActive(true);
            WDRPanel.SetActive(false);
        }

        if (operationDropdown.options[operationDropdown.value].text == "Wet Dress Rehearsal")
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
        if (fileInfo.Length == 0)
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
        if (fileInfo.Length == 0)
        {
            return;
        }
        foreach (var file in fileInfo)
        {
            options.Add(Path.GetFileName(file.ToString()));
        }
        tankDropdown.AddOptions(options);
    }

    public void PanelFadeIn(GameObject panel)
    {
        panel.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        panel.GetComponent<RectTransform>().transform.DOScale(1, 0.1f);
    }

    public void PanelFadeOut(GameObject panel)
    {
        panel.transform.DOScale(0, 0.1f);
        panel.transform.localScale = new Vector3(1, 1, 1);
    }

    private IEnumerator ActiveDeactive(float waitTime, GameObject panel, bool activated)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(activated);
    }


    public void selectLaunchpad()
    {
        launchPadManager[] launchPads = FindObjectsOfType<launchPadManager>();
        foreach (launchPadManager launchPad in launchPads)
        {
            launchPad.button.SetActive(true);
            PanelFadeIn(launchPad.button);
            launchPad.button.GetComponent<OnClick>().op = this;
            launchPad.button.GetComponent<OnClick>().savedLaunchpad = launchPad.gameObject;
        }
    }

    public void selectStaticFireStand()
    {
        staticFireStandManager[] stands = FindObjectsOfType<staticFireStandManager>();
        foreach (staticFireStandManager stand in stands)
        {
            stand.button.SetActive(true);
            PanelFadeIn(stand.button);
            stand.button.GetComponent<OnClick>().op = this;
            stand.button.GetComponent<OnClick>().savedLaunchpad = stand.gameObject;
        }
    }

    public void selectPressureStand()
    {
        standManager[] stands = FindObjectsOfType<standManager>();
        foreach (standManager stand in stands)
        {
            stand.button.SetActive(true);
            PanelFadeIn(stand.button);
            stand.button.GetComponent<OnClick>().op = this;
            stand.button.GetComponent<OnClick>().savedLaunchpad = stand.gameObject;
        }
    }

    public void selectDestination()
    {
        if (operationDropdown.value == 0)
        {
            selectLaunchpad();
        }

        if (operationDropdown.value == 1)
        {
            selectStaticFireStand();
        }

        if (operationDropdown.value == 2)
        {
            selectPressureStand();
        }

        if (operationDropdown.value == 3)
        {
            selectLaunchpad();
        }

        if (operationDropdown.value == 4)
        {
            selectLaunchpad();
        }
    }

    public void hidePadButtons()
    {
        launchPadManager[] launchPads = FindObjectsOfType<launchPadManager>();
        foreach (launchPadManager launchPad in launchPads)
        {
            PanelFadeOut(launchPad.button);
            StartCoroutine(ActiveDeactive(1, launchPad.button, false));
        }

        staticFireStandManager[] stands = FindObjectsOfType<staticFireStandManager>();
        foreach (staticFireStandManager stand in stands)
        {
            PanelFadeOut(stand.button);
            StartCoroutine(ActiveDeactive(1, stand.button, false));
        }

        standManager[] stands1 = FindObjectsOfType<standManager>();
        foreach (standManager stand in stands1)
        {
            PanelFadeOut(stand.button);
            StartCoroutine(ActiveDeactive(1, stand.button, false));
        }

        if (selectedLaunchPad != null)
        {
            if (selectedLaunchPad.GetComponent<launchPadManager>())
            {
                //launchpadName.text = "launchpad";
                //launchPadSF.text = "launchpad";
                //launchPadWDR.text = "launchpad";
            }

            if (selectedLaunchPad.GetComponent<staticFireStandManager>())
            {
                //staticFireStandName.text = "staticFireStand";
            }

            if (selectedLaunchPad.GetComponent<standManager>())
            {
                //standName.text = "pressureStand";
            }
        }
    }

    public void run()
    {
        if (operationDropdown.value == 0) //Launch
        {

            int value = vehicleLaunchDropdown.value;
            if (MasterManager.partType[MasterManager.partName.IndexOf(vehicleLaunchDropdown.options[value].text.ToString())] != null)
            {
                if (MasterManager.partType[MasterManager.partName.IndexOf(vehicleLaunchDropdown.options[value].text.ToString())] == "Rocket" && MasterManager.count[MasterManager.partName.IndexOf(vehicleLaunchDropdown.options[value].text.ToString())] > 0)
                {
                    MasterManager.count[MasterManager.partName.IndexOf(vehicleLaunchDropdown.options[value].text.ToString())] -= 1;
                    onclick.path = "/" + vehicleLaunchDropdown.options[value].text.ToString();
                    onclick.launchPad = selectedLaunchPad;
                    onclick.load("/rockets");
                    stageViewer.gameObject.SetActive(true);
                    stageViewer.rocket = onclick.spawnedRocket;
                    stageViewer.updateStagesView(false);
                    stageViewer.updateInfoPerStage(false);
                    selectedLaunchPad.GetComponent<launchPadManager>().ConnectedRocket = onclick.spawnedRocket;
                }
                else
                {
                    popUpBuilt.SetActive(true);
                    PanelFadeIn(popUpBuilt);
                    StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
                }
            }
            else
            {
                popUpBuilt.SetActive(true);
                PanelFadeIn(popUpBuilt);
                StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
            }
        }

        if (operationDropdown.value == 1) //Engine static fire
        {
            int value = engineDropdown.value;
            if (MasterManager.partType[MasterManager.partName.IndexOf(engineDropdown.options[value].text.ToString())] != null)
            {
                if (MasterManager.partType[MasterManager.partName.IndexOf(engineDropdown.options[value].text.ToString())] == "Engine" && MasterManager.count[MasterManager.partName.IndexOf(engineDropdown.options[value].text.ToString())] > 0)
                {
                    MasterManager.count[MasterManager.partName.IndexOf(engineDropdown.options[value].text.ToString())] -= 1;
                    onclick.path = "/" + engineDropdown.options[value].text.ToString();
                    onclick.launchPad = selectedLaunchPad;
                    onclick.load("/engines");
                    staticFireViewer.gameObject.SetActive(true);
                    staticFireViewer.staticFireStand = selectedLaunchPad;
                    selectedLaunchPad.GetComponent<staticFireStandManager>().ConnectedEngine = onclick.spawnedRocket;
                }
                else
                {
                    popUpBuilt.SetActive(true);
                    PanelFadeIn(popUpBuilt);
                    StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
                }
            }
            else
            {
                popUpBuilt.SetActive(true);
                PanelFadeIn(popUpBuilt);
                StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
            }

        }

        if (operationDropdown.value == 2) //Tank test
        {
            int value = tankDropdown.value;
            if (MasterManager.partType[MasterManager.partName.IndexOf(tankDropdown.options[value].text.ToString())] != null)
            {
                if (MasterManager.partType[MasterManager.partName.IndexOf(tankDropdown.options[value].text.ToString())] == "Tank" && MasterManager.count[MasterManager.partName.IndexOf(tankDropdown.options[value].text.ToString())] > 0)
                {
                    MasterManager.count[MasterManager.partName.IndexOf(tankDropdown.options[value].text.ToString())] -= 1;
                    onclick.path = "/" + tankDropdown.options[value].text.ToString();
                    onclick.launchPad = selectedLaunchPad;
                    onclick.load("/tanks");
                    pressureTestViewer.gameObject.SetActive(true);
                    pressureTestViewer.Stand = selectedLaunchPad;
                    selectedLaunchPad.GetComponent<standManager>().ConnectedTank = onclick.spawnedRocket;
                }
                else
                {
                    popUpBuilt.SetActive(true);
                    PanelFadeIn(popUpBuilt);
                    StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
                }
            }
            else
            {
                popUpBuilt.SetActive(true);
                PanelFadeIn(popUpBuilt);
                StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
            }

        }

        if (operationDropdown.value == 3) //WDR test
        {
            int value = vehicleWDRDropdown.value;
            if (MasterManager.partType[MasterManager.partName.IndexOf(vehicleWDRDropdown.options[value].text.ToString())] != null)
            {
                if (MasterManager.partType[MasterManager.partName.IndexOf(vehicleWDRDropdown.options[value].text.ToString())] == "Rocket" && MasterManager.count[MasterManager.partName.IndexOf(vehicleWDRDropdown.options[value].text.ToString())] > 0)
                {
                    MasterManager.count[MasterManager.partName.IndexOf(vehicleWDRDropdown.options[value].text.ToString())] -= 1;
                    onclick.path = "/" + vehicleWDRDropdown.options[value].text.ToString();
                    onclick.launchPad = selectedLaunchPad;
                    onclick.load("/rockets");
                    WDRTestViewer.gameObject.SetActive(true);
                    WDRTestViewer.launchPad = selectedLaunchPad;
                    selectedLaunchPad.GetComponent<launchPadManager>().ConnectedRocket = onclick.spawnedRocket;
                    selectedLaunchPad.GetComponent<launchPadManager>().operation = "WDR";
                }
                else
                {
                    popUpBuilt.SetActive(true);
                    PanelFadeIn(popUpBuilt);
                    StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
                }
            }
            else
            {
                popUpBuilt.SetActive(true);
                PanelFadeIn(popUpBuilt);
                StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
            }

        }

        if (operationDropdown.value == 4) //Rocket SF test
        {
            int value = vehicleStaticFireDropdown.value;
            if (MasterManager.partType[MasterManager.partName.IndexOf(vehicleStaticFireDropdown.options[value].text.ToString())] != null)
            {
                if (MasterManager.partType[MasterManager.partName.IndexOf(vehicleStaticFireDropdown.options[value].text.ToString())] == "Rocket" && MasterManager.count[MasterManager.partName.IndexOf(vehicleStaticFireDropdown.options[value].text.ToString())] > 0)
                {
                    MasterManager.count[MasterManager.partName.IndexOf(vehicleStaticFireDropdown.options[value].text.ToString())] -= 1;
                    onclick.path = "/" + vehicleStaticFireDropdown.options[value].text.ToString();
                    onclick.launchPad = selectedLaunchPad;
                    onclick.load("/rockets");
                    RocketStaticFireViewer.gameObject.SetActive(true);
                    RocketStaticFireViewer.launchpad = selectedLaunchPad;
                    selectedLaunchPad.GetComponent<launchPadManager>().ConnectedRocket = onclick.spawnedRocket;
                    selectedLaunchPad.GetComponent<launchPadManager>().operation = "staticFire";
                }
                else
                {
                    popUpBuilt.SetActive(true);
                    PanelFadeIn(popUpBuilt);
                    StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
                }
            }
            else
            {
                popUpBuilt.SetActive(true);
                PanelFadeIn(popUpBuilt);
                StartCoroutine(ActiveDeactive(2, popUpBuilt, false));
            }

        }
    }

    void OnEnable()
    {
        if (MasterManager == null)
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
        }
    }
}
