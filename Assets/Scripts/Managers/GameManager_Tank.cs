using System.Numerics;
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
using UnityEngine.Rendering;

public class GameManager_Tank : MonoBehaviour
{
    public TMP_InputField tankHeight;
    public TMP_InputField tankDiameter;

    public TMP_InputField savePath;
    public TMP_Dropdown materialDropdown;
    public TMP_Dropdown tankDropdown;
    public string saveName;

    public GameObject Tank;
    public SpriteRenderer tankSP;

    public GameObject attachTopObj;
    public GameObject attachBottomObj;
    public GameObject attachRightObj;
    public GameObject attachLeftObj;

    public float tankHeightFloat;
    public float tankDiameterFloat;
    public float costValue;
    public string propellantCategory;

    //Volume in m3
    public float volume;
    public float mass;
    public float thermalConductivity;
    public float maxPressure;

    public savePath savePathRef = new savePath();
    public float currentD;
    public float currentH;

    public GameObject panel;
    public GameObject popUpPart;

    public float elapsedFrames = 0;

    public UnityEngine.Vector3 startingScaleD;
    public UnityEngine.Vector3 startingScaleH;
    public string tankMaterial;
    public MasterManager MasterManager = new MasterManager();

    public GameObject MainPanel;
    public GameObject CreatorPanel;
    public GameObject DataPanel;

    public TMP_Text mass_c;
    public TMP_Text maxPressure_c;
    public TMP_Text maxVolume_c;
    public TMP_Text thermalConductivity_c;
    public TMP_Text cost;

    public TMP_Text tankName;
    public TMP_Text massViz;
    public TMP_Text maxRecPressure;
    public TMP_Text thermalConductivityViz;
    public TMP_Text volumeViz;
    public GameObject maxMassShower;
    public TMP_Dropdown substanceDropdown;
    public TMP_Text maxMass;
    public bool tested = false;

    [SerializeField]private Substance kerosene;
    [SerializeField]private Substance LOX;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "TankDesign")
        {
            tankSP = Tank.GetComponent<SpriteRenderer>();

            startingScaleD = tankSP.transform.localScale;
            startingScaleH = tankSP.transform.localScale;

            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
            initializeTanksnFolder();
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name.ToString() == "TankDesign")
        {
            updateSize();
            updateAttachPosition();
            calculate();
            updateValues();
        }

    }

    void updateSize()
    {
        float number;
        if (float.TryParse(tankDiameter.text, out number))
        {

            tankDiameterFloat = float.Parse(tankDiameter.text);

                if (tankDiameterFloat == tankSP.size.x)
                {
                    startingScaleD = tankSP.size;
                    currentD = 0;
                }

                if (tankSP.size.x != tankDiameterFloat)
                {
                    tankSP.size = UnityEngine.Vector2.Lerp(startingScaleD, new UnityEngine.Vector2(tankDiameterFloat, tankSP.size.y), currentD * 5);
                    currentD += Time.deltaTime;
                }
            


        }

        if (float.TryParse(tankHeight.text, out number))
        {
            tankHeightFloat = float.Parse(tankHeight.text);

                if (tankSP.size.y == tankHeightFloat)
                {
                    startingScaleH = tankSP.size;
                    currentH = 0;
                }

                if (tankSP.size.y != tankHeightFloat)
                {
                    tankSP.size = UnityEngine.Vector2.Lerp(startingScaleH, new UnityEngine.Vector2(tankSP.size.x, tankHeightFloat), currentH * 5);
                    currentH += Time.deltaTime;
                }
            


        }

    }

    void calculate()
    {
        float massDensity = 1750f; //Assuming aluminium
        float wallThickness = 0.0005f; //Assuming 0.1 cm thickness
        volume = Mathf.PI * Mathf.Pow(tankDiameterFloat / 2, 2) * tankHeightFloat;
        mass = (volume - Mathf.PI * Mathf.Pow(tankDiameterFloat / 2 - wallThickness, 2) * tankHeightFloat) * massDensity;
        tankMaterial = materialDropdown.options[materialDropdown.value].text.ToString();
        if (tankMaterial == "StainlessSteel")
        {
            maxPressure = 200000f;
            thermalConductivity = 0.09f;
            costValue = mass * 1.2f;
        }
    }

    void updateValues()
    {
        mass_c.text = mass.ToString();
        maxVolume_c.text = volume.ToString();
        thermalConductivity_c.text = thermalConductivity.ToString();
        maxPressure_c.text = maxPressure.ToString();
        cost.text = costValue.ToString();
    }


    public void save()
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder);
        }

        saveName = "/" + savePath.text;

        if (!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json"))
        {
            saveTank saveObject = new saveTank();
            saveObject.path = savePathRef.tankFolder;
            saveObject.tankName = saveName;
            saveObject.tankSizeX = tankSP.size.x;
            saveObject.tankSizeY = tankSP.size.y;
            saveObject.attachTopPos = attachTopObj.transform.localPosition.y;
            saveObject.attachBottomPos = attachBottomObj.transform.localPosition.y;
            saveObject.attachRightPos = attachRightObj.transform.localPosition.x;
            saveObject.attachLeftPos = attachLeftObj.transform.localPosition.x;
            saveObject.volume = volume;
            saveObject.mass = mass;
            saveObject.thermalConductivity = thermalConductivity;
            saveObject.maxPressure = maxPressure;
            saveObject.tankMaterial = tankMaterial;
            saveObject.cost = Convert.ToSingle(cost.text);

            var jsonString = JsonConvert.SerializeObject(saveObject);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json", jsonString);
        }
        else if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json"))
        {
            saveTank saveTank = new saveTank();
            var jsonString2 = JsonConvert.SerializeObject(saveTank);
            jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json");
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString2);

            if (loadedTank.usedNum == 0)
            {
                File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json");
                save();
                return;
            }

            int x = Screen.width / 2;
            int y = Screen.height / 2;
            UnityEngine.Vector2 position = new UnityEngine.Vector2(x, y);
            Instantiate(popUpPart, position, UnityEngine.Quaternion.identity);
            panel.SetActive(false);
        }
    }

    public void updateAttachPosition()
    {
        attachTopObj.transform.position = new UnityEngine.Vector2(attachTopObj.transform.position.x, tankSP.bounds.max.y);
        attachBottomObj.transform.position = new UnityEngine.Vector2(attachBottomObj.transform.position.x, tankSP.bounds.min.y);
        attachRightObj.transform.position = new UnityEngine.Vector2(tankSP.bounds.max.x, attachRightObj.transform.position.y);
        attachLeftObj.transform.position = new UnityEngine.Vector2(tankSP.bounds.min.x, attachLeftObj.transform.position.y);
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

    public void initializeTanksnFolder()
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

    public void loadData()
    {
        saveTank saveTank = new saveTank();
        var jsonString = JsonConvert.SerializeObject(saveTank);
        jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + "/" + tankDropdown.options[tankDropdown.value].text.ToString());
        saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);
        tankName.text = loadedTank.tankName;
        massViz.text = loadedTank.mass.ToString();
        volumeViz.text = loadedTank.volume.ToString();
        thermalConductivityViz.text = loadedTank.thermalConductivity.ToString();
        maxRecPressure.text = loadedTank.maxRecPressure.ToString();
        tested = loadedTank.tested;
        if(tested == false)
        {
            maxMassShower.SetActive(false);
        }

        if(tested == true)
        {
            maxMassShower.SetActive(true);
            updateMaxMass();
        }
    }

    public void updateMaxMass()
    {
        if(substanceDropdown.options[substanceDropdown.value].text.ToString() == "Kerosene")
        {
            maxMass.text = (float.Parse(volumeViz.text) * kerosene.Density).ToString();
        }

        if (substanceDropdown.options[substanceDropdown.value].text.ToString() == "LOX")
        {
            maxMass.text = (float.Parse(volumeViz.text) * LOX.Density).ToString();
        }
        
    }

}
