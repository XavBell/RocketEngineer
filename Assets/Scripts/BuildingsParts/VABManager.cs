using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.Rendering;
using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;

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


    // Start is called before the first frame update
    void Start()
    {
        MasterManager = FindObjectOfType<MasterManager>();


        if (MasterManager != null)
        {
            retrieveRocketSaved();
            if (partName.options.Count > 0)
            {
                retrieveInfo();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

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
            savecraft saveObject = new savecraft();
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + "/" + partName.options[partName.value].text);
            savecraft loadedRocket = JsonConvert.DeserializeObject<savecraft>(jsonString);
            GameObject temp = new GameObject();
            //engines.Clear();
            //tanks.Clear();
            int totalEngineParts = 0;
            int totalTankParts = 0;
            float cost = 0;
            int engineID = 0;
            foreach (string usedPart in loadedRocket.engineName)
            {
                engines.Add(usedPart + ".json");
                totalEngineParts++;
                cost += loadedRocket.engineCost[engineID];
                engineID++;
            }

            int tankID = 0;
            foreach (string usedPart in loadedRocket.tankName)
            {
                tanks.Add(usedPart + ".json");
                totalTankParts++;
                cost += loadedRocket.tankCost[tankID];
                tankID++;
            }
            engines.Distinct().ToList();
            tanks.Distinct().ToList();

            List<float> costsEngine = new List<float>();
            foreach (string engine in engines)
            {
                saveEngine saveEngine = new saveEngine();
                var jsonStringEngine = JsonConvert.SerializeObject(saveEngine);
                jsonStringEngine = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + engine);
                saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonStringEngine);
                costsEngine.Add(loadedEngine.cost);
            }

            List<float> costsTank = new List<float>();
            foreach (string tank in tanks)
            {
                saveTank saveTank = new saveTank();
                var jsonStringTank = JsonConvert.SerializeObject(saveTank);
                jsonStringTank = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + tank);
                saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonStringTank);
                costsTank.Add(loadedTank.cost);
            }

            int engineBuilt = 0;
            int tankBuilt = 0;
            //Reduce price of rocket based on built parts
            List<string> tempEngines = new List<string>();
            foreach(string engine in MasterManager.engines)
            {
                tempEngines.Add(engine);
            }
            int i = 0;
            foreach(string engine in loadedRocket.engineName)
            {
                if(tempEngines.Contains(engine.Replace("/", "") + ".json"))
                {
                    tempEngines.RemoveAt(tempEngines.IndexOf(engine.Replace("/", "")+ ".json"));
                    engineBuilt++;
                    cost -= loadedRocket.engineCost[i];
                }
                i++;
            }

            List<string> tempTanks = new List<string>();
            foreach(string tank in MasterManager.tanks)
            {
                tempTanks.Add(tank);
            }
            i = 0;
            foreach(string tank in loadedRocket.tankName)
            {
                //print(MasterManager.tanks[0]);
                //print(tank);
                if(tempTanks.Contains(tank.Replace("/", "") + ".json"))
                {
                    tempTanks.RemoveAt(tempTanks.IndexOf(tank.Replace("/", "") + ".json"));
                    tankBuilt++;
                    cost -= loadedRocket.tankCost[i];
                }
                i++;
            }

            //Count number of rocket built
            int built = 0;
            foreach(string rocket in MasterManager.rockets)
            {
                if(rocket == partName.options[partName.value].text)
                {
                    built += 1;
                }
            }

            engineBuiltTxt.gameObject.SetActive(true);
            tankBuiltTxt.gameObject.SetActive(true);
            tankText.SetActive(true);
            engineText.SetActive(true);
            builtTxt.text = built.ToString();

            costTxt.text = cost.ToString();
            engineBuiltTxt.text = engineBuilt.ToString() + "/" + totalEngineParts.ToString();
            tankBuiltTxt.text = tankBuilt.ToString() + "/" + totalTankParts.ToString();
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
        print("OH NO");
        if (Convert.ToSingle(costTxt.text) < MasterManager.gameObject.GetComponent<pointManager>().nPoints)
        {
            if(type.value == 0)
            {
                savecraft saveObject = new savecraft();
                var jsonString = JsonConvert.SerializeObject(saveObject);
                jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + "/" + partName.options[partName.value].text);
                savecraft loadedRocket = JsonConvert.DeserializeObject<savecraft>(jsonString);
                List<string> tanks = new List<string>();
                List<int> countTank = new List<int>();

                List<string> tanktoBeRemoved = new List<string>();
                foreach (string tank in loadedRocket.tankName)
                {
                    if (!tanks.Contains(tank))
                    {
                        tanks.Add(tank);
                        countTank.Add(1);
                    }
                    else if (tanks.Contains(tank))
                    {
                        countTank[tanks.IndexOf(tank)]++;
                    }
                    tanktoBeRemoved.Add(tank);
                }

                List<string> engines = new List<string>();
                List<int> countEngine = new List<int>();
                List<string> enginetoBeRemoved = new List<string>();
                foreach (string engine in loadedRocket.engineName)
                {
                    if (!engines.Contains(engine))
                    {
                        engines.Add(engine);
                        countEngine.Add(1);
                    }
                    else if (engines.Contains(engine))
                    {
                        countEngine[engines.IndexOf(engine)]++;
                    }
                    enginetoBeRemoved.Add(engine);
                }


                //Remove parts
                if(MasterManager.engines.Count > 0)
                {
                    List<string> tempEngine = MasterManager.engines;
                    foreach (string engine in enginetoBeRemoved)
                    {
                        if(tempEngine.Contains(engine.Replace("/", "") + ".json"))
                        {
                            tempEngine.RemoveAt(tempEngine.IndexOf(engine.Replace("/", "") + ".json"));
                        }
                    }
                    MasterManager.engines = tempEngine;
                }

                if(MasterManager.tanks.Count > 0)
                {
                    List<string> tempTank = MasterManager.tanks;
                    foreach (string tank in tanktoBeRemoved)
                    {
                        if(tempTank.Contains(tank.Replace("/", "") + ".json"))
                        {
                            tempTank.RemoveAt(tempTank.IndexOf(tank.Replace("/", "") + ".json"));
                        }
                    }
                    MasterManager.tanks = tempTank;
                }

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
        onValueChanged();
    }

}
