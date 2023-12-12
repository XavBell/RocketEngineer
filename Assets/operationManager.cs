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
    [SerializeField]private TMP_Dropdown vehicleDropdown;
    [SerializeField]private TMP_Dropdown operationDropdown;
    [SerializeField]private StageViewer stageViewer;
    public GameObject selectedLaunchPad;
    public OnClick onclick;
    public TMP_Text launchpadName;
    // Start is called before the first frame update
    void Start()
    {
        
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
        vehicleDropdown.AddOptions(options);
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

    public void hidePadButtons()
    {
        launchPadManager[] launchPads = FindObjectsOfType<launchPadManager>();
        foreach(launchPadManager launchPad in launchPads)
        {
            launchPad.button.SetActive(false);
            launchPad.button.GetComponent<OnClick>().op = null;
            launchPad.button.GetComponent<OnClick>().savedLaunchpad = null;
        }

        if(selectedLaunchPad != null)
        {
            launchpadName.text = "launchpad";
        }
    }

    public void run()
    {
        if(operationDropdown.value == 0)
        {
            int value = vehicleDropdown.value;
            onclick.path = "/"+vehicleDropdown.options[value].text.ToString();
            onclick.launchPad = selectedLaunchPad;
            onclick.load("/rockets");
            stageViewer.gameObject.SetActive(true);
            stageViewer.rocket = onclick.spawnedRocket;
            stageViewer.updateStagesView(false);
        }
    }

    void OnEnable()
    {
        if(MasterManager == null)
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
        }
        retrieveRocketSaved();
    }

    
}
