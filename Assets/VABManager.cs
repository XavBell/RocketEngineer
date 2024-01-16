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
        buildingManager = FindObjectOfType<BuildingManager>();
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
        savecraft saveObject = new savecraft();
        var jsonString = JsonConvert.SerializeObject(saveObject);
        jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + "/" + partName.options[partName.value].text);
        savecraft loadedRocket = JsonConvert.DeserializeObject<savecraft>(jsonString);
        GameObject temp = new GameObject();
        //Rocket rocket = temp.AddComponent<Rocket>();
        //List<string> engines = new List<string>();
        //List<string> tanks = new List<string>();
        engines.Clear();
        tanks.Clear();
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

        int i = 0;
        int engineBuilt = 0;
        int tankBuilt = 0;
        foreach (string name in MasterManager.partName)
        {
            if (engines.Contains("/" + name))
            {
                engineBuilt += MasterManager.count[i];
                int index = engines.IndexOf("/" + name);
                if(cost - costsEngine[index]*MasterManager.count[i] >= 0)
                {
                    cost -= costsEngine[index]*MasterManager.count[i];
                }
            }

            if (tanks.Contains("/" + name))
            {
                tankBuilt += MasterManager.count[i];
                int index = tanks.IndexOf("/" + name);
                if(cost - costsTank[index]*MasterManager.count[i] >= 0)
                {
                    cost -= costsTank[index]*MasterManager.count[i];
                }
            }

            i++;
        }
        costTxt.text = cost.ToString();
        engineBuiltTxt.text = engineBuilt.ToString() + "/" + totalEngineParts.ToString();
        tankBuiltTxt.text = tankBuilt.ToString() + "/" + totalTankParts.ToString();
    }

    void OnTankSelected()
    {
        saveTank saveObject = new saveTank();
        var jsonString = JsonConvert.SerializeObject(saveObject);
        jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + "/" + partName.options[partName.value].text);
        saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);
        List<int> typeCorresponding = new List<int>();
        int i = 0;
        foreach (string partType in MasterManager.partType)
        {
            if (partType == type.options[type.value].text)
            {
                typeCorresponding.Add(i);
            }
            i++;
        }

        int indexFinal = -1;
        foreach (int index in typeCorresponding)
        {
            if (MasterManager.partName[index] == partName.options[partName.value].text)
            {
                indexFinal = index;
            }
        }

        int built = 0;
        if (indexFinal != -1)
        {
            built = MasterManager.count[indexFinal];
        }else{
            built = 0;
        }
        builtTxt.text = built.ToString();
        costTxt.text = loadedTank.cost.ToString();
    }

    void OnEngineSelected()
    {
        saveEngine saveObject = new saveEngine();
        var jsonString = JsonConvert.SerializeObject(saveObject);
        jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + "/" + partName.options[partName.value].text);
        saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);
        List<int> typeCorresponding = new List<int>();
        int i = 0;
        foreach (string partType in MasterManager.partType)
        {
            if (partType == type.options[type.value].text)
            {
                typeCorresponding.Add(i);
            }
            i++;
        }

        int indexFinal = -1;
        foreach (int index in typeCorresponding)
        {
            if (MasterManager.partName[index] == partName.options[partName.value].text)
            {
                indexFinal = index;
            }
        }

        int built = 0;
        if (indexFinal != -1)
        {
            built = MasterManager.count[indexFinal];
        }else{
            built = 0;
        }
        builtTxt.text = built.ToString();
        costTxt.text = loadedEngine.cost.ToString();
    }

    public void build()
    {
        if (Convert.ToSingle(costTxt.text) < MasterManager.gameObject.GetComponent<pointManager>().nPoints && (type.value == 1 || type.value == 2))
        {
            List<int> typeCorresponding = new List<int>();
            int i = 0;
            foreach (string partType in MasterManager.partType)
            {
                if (partType == type.options[type.value].text)
                {
                    typeCorresponding.Add(i);
                }
                i++;
            }

            int indexFinal = -1;
            foreach (int index in typeCorresponding)
            {
                if (MasterManager.partName[index] == partName.options[partName.value].text)
                {
                    indexFinal = index;
                }
            }

            if (indexFinal == -1)
            {
                MasterManager.partType.Add(type.options[type.value].text);
                MasterManager.partName.Add(partName.options[partName.value].text);
                MasterManager.count.Add(1);
            }
            else
            {
                MasterManager.count[indexFinal]++;
            }

            MasterManager.gameObject.GetComponent<pointManager>().nPoints -= Convert.ToSingle(costTxt.text);
        }
        else
        {

            if (Convert.ToSingle(costTxt.text) < MasterManager.gameObject.GetComponent<pointManager>().nPoints && type.value == 0)
            {
                savecraft saveObject = new savecraft();
                var jsonString = JsonConvert.SerializeObject(saveObject);
                jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + "/" + partName.options[partName.value].text);
                savecraft loadedRocket = JsonConvert.DeserializeObject<savecraft>(jsonString);
                List<string> tanks = new List<string>();
                List<int> countTank = new List<int>();

                List<int> indexTanks = new List<int>();
                List<int> indexEngines = new List<int>();
                foreach(string tank in loadedRocket.tankName)
                {
                    //Debug.Log("tank");
                    if (!tanks.Contains(tank))
                    {
                        tanks.Add(tank);
                        countTank.Add(1);
                    }
                    else if (tanks.Contains(tank))
                    {
                        countTank[tanks.IndexOf(tank)]++;
                    }
                }

                foreach (string tank in tanks)
                {
                    List<int> typeCorresponding = new List<int>();
                    int id = 0;
                    foreach (string partType in MasterManager.partType)
                    {
                        if (partType == "Tank")
                        {
                            typeCorresponding.Add(id);
                        }
                        id++;
                    }

                    int indexFinal = -1;
                    foreach (int index in typeCorresponding)
                    {
                        if ("/"+MasterManager.partName[index] == tank+".json")
                        {
                            indexFinal = index;
                        }
                    }
                    if(indexFinal != -1)
                    {
                        indexTanks.Add(indexFinal);
                    }
                    
                }



                List<string> engines = new List<string>();
                List<int> countEngine = new List<int>();
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
                }

                foreach (string engine in engines)
                {
                    List<int> typeCorresponding = new List<int>();
                    int id = 0;
                    foreach (string partType in MasterManager.partType)
                    {
                        if (partType == "Engine")
                        {
                            typeCorresponding.Add(id);
                        }
                        id++;
                    }

                    int indexFinal = -1;
                    foreach (int index in typeCorresponding)
                    {
                        Debug.Log("/"+MasterManager.partName[index]);
                        Debug.Log(engine);
                        if ("/"+MasterManager.partName[index] == engine+".json")
                        {
                            indexFinal = index;
                        }
                    }
                    if(indexFinal != -1)
                    {
                        indexEngines.Add(indexFinal);
                    }
                }

                //Remove parts
                int i = 0;
                Debug.Log(indexEngines.Count);
                foreach (int index in indexEngines)
                {
                    Debug.Log(index);
                    if (countEngine[i] <= MasterManager.count[index])
                    {
                        MasterManager.count[index] -= countEngine[i];
                    }
                    else if (countEngine[i] >= MasterManager.count[index])
                    {
                        MasterManager.count[index] = 0;
                    }
                    i++;
                }


                int i2 = 0;
                if (indexTanks.Count > 0)
                {
                    foreach (int index in indexTanks)
                    {
                        if (countTank[i2] <= MasterManager.count[index])
                        {
                            MasterManager.count[index] -= countTank[i2];
                        }
                        else if (countTank[i2] >= MasterManager.count[index])
                        {
                            MasterManager.count[index] = 0;
                        }
                        i2++;
                    }
                }

                //Build rocket
                List<int> typeCorrespondinFinal = new List<int>();
                int i3 = 0;
                foreach (string partType in MasterManager.partType)
                {
                    if (partType == type.options[type.value].text)
                    {
                        typeCorrespondinFinal.Add(i3);
                    }
                    i++;
                }

                int indexFinalRocket = -1;
                foreach (int index in typeCorrespondinFinal)
                {
                    if (MasterManager.partName[index] == partName.options[partName.value].text)
                    {
                        indexFinalRocket = index;
                    }
                }

                if (indexFinalRocket == -1)
                {
                    MasterManager.partType.Add(type.options[type.value].text);
                    MasterManager.partName.Add(partName.options[partName.value].text);
                    MasterManager.count.Add(1);
                }
                else
                {
                    MasterManager.count[indexFinalRocket]++;
                }

                MasterManager.gameObject.GetComponent<pointManager>().nPoints -= Convert.ToSingle(costTxt.text);
                retrieveInfo();

            }
        }
    }
}
