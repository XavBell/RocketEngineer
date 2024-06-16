using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.Rendering;
using System;

public class VABManager : MonoBehaviour
{
    public GameObject tankText;
    public GameObject engineText;
    public TMP_Dropdown type;
    public TMP_Dropdown partName;
    public TMP_Text builtTxt;
    public TMP_Text engineBuiltTxt;
    public TMP_Text tankBuiltTxt;
    public TMP_Text costTxt;
    public savePath savePathRef = new savePath();
    public MasterManager MasterManager;
    public BuildingManager buildingManager;

    public List<string> tanks = new List<string>();
    public List<string> engines = new List<string>();
    public bool needUpdate = false;
    public int tempBuilt = 0;

    // Start is called before the first frame update
    void Start()
    {
        MasterManager = FindObjectOfType<MasterManager>();
        onValueChanged();
    }

    public void onValueChanged()
    {
        if (type.value == 0)
        {
            retrieveRocketSaved();
            retrieveInfo();
        }

        if (type.value == 1)
        {
            retrieveEngineSaved();
            retrieveInfo();
        }

        if (type.value == 2)
        {
            retrieveTankSaved();
            retrieveInfo();
        }
    }

    public void retrieveRocketSaved()
    {
        partName.ClearOptions();
        List<string> options = new List<string>();
        if(MasterManager == null)
        {
            MasterManager = FindObjectOfType<MasterManager>();
        }

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
        partName.AddOptions(options);
    }

    public void retrieveEngineSaved()
    {
        partName.ClearOptions();
        if(MasterManager == null)
        {
            MasterManager = FindObjectOfType<MasterManager>();
        }
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
        partName.AddOptions(options);
    }

    public void retrieveTankSaved()
    {
        partName.ClearOptions();
        if(MasterManager == null)
        {
            MasterManager = FindObjectOfType<MasterManager>();
        }
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
        partName.AddOptions(options);
    }

    public void retrieveInfo()
    {
        if (type.value == 0)
        {
            OnRocketSelected();
        }

        if (type.value == 1)
        {
            OnEngineSelected();
        }

        if (type.value == 2)
        {
            OnTankSelected();
        }
    }

    public void OnRocketSelected()
    {
        if (partName.options.Count > 0)
        {
            
        }

    }

    void OnTankSelected()
    {

        if (partName.options.Count > 0)
        {
            saveTank saveObject = new saveTank();
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + "/" + partName.options[partName.value].text);
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);
            int built = 0;
            foreach(string tank in MasterManager.tanks)
            {
                if(tank == partName.options[partName.value].text)
                {
                    built += 1;
                }
            }

            tempBuilt = built;

            builtTxt.text = built.ToString();
            costTxt.text = loadedTank.cost.ToString();
            engineBuiltTxt.gameObject.SetActive(false);
            tankBuiltTxt.gameObject.SetActive(false);
            tankText.SetActive(false);
            engineText.SetActive(false);
        }
        else
        {
            tankText.SetActive(false);
            engineText.SetActive(false);
        }

    }

    void OnEngineSelected()
    {
        if (partName.options.Count > 0)
        {
            saveEngine saveObject = new saveEngine();
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + "/" + partName.options[partName.value].text);
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);

            //Count number of engines built
            int built = 0;
            foreach(string engine in MasterManager.engines)
            {
                if(engine == partName.options[partName.value].text)
                {
                    built += 1;
                }
            }

            tempBuilt = built;
            
            builtTxt.text = built.ToString();
            costTxt.text = loadedEngine.cost.ToString();
            engineBuiltTxt.gameObject.SetActive(false);
            tankBuiltTxt.gameObject.SetActive(false);
            tankText.gameObject.SetActive(false);
            engineText.gameObject.SetActive(false);
        }
        else
        {
            tankText.gameObject.SetActive(false);
            engineText.gameObject.SetActive(false);
        }

    }

    public void build()
    {
        //0 is Rocket
        //1 is Engine
        //2 is Tank
        if (Convert.ToSingle(costTxt.text) < MasterManager.gameObject.GetComponent<pointManager>().nPoints)
        {
            if(type.value == 0)
            {
                //Build rocket
                MasterManager.rockets.Add(partName.options[partName.value].text);
            }

            if(type.value == 1)
            {
                MasterManager.engines.Add(partName.options[partName.value].text);
            }
            
            if(type.value == 2)
            {
                MasterManager.tanks.Add(partName.options[partName.value].text);
            }

            //Adjust new points
            MasterManager.gameObject.GetComponent<pointManager>().nPoints -= Convert.ToSingle(costTxt.text);
        }
        retrieveInfo();
    }

}
