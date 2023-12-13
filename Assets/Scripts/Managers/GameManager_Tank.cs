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

public class GameManager_Tank : MonoBehaviour
{
    public TMP_InputField tankHeight;
    public TMP_InputField tankDiameter;

    public TMP_InputField savePath;
    public TMP_Dropdown materialDropdown;
    public string saveName;

    public GameObject Tank;
    public SpriteRenderer tankSP;

    public GameObject attachTopObj;
    public GameObject attachBottomObj;
    public GameObject attachRightObj;
    public GameObject attachLeftObj;

    public float tankHeightFloat;
    public float tankDiameterFloat;
    public string propellantCategory;

    public float volume;
    public float mass;

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
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "TankDesign")
        {
            tankSP = Tank.GetComponent<SpriteRenderer>();

            attachTopObj = Tank.GetComponent<Tank>()._attachTop;

            attachBottomObj = Tank.GetComponent<Tank>()._attachBottom;

            attachRightObj = Tank.GetComponent<Tank>()._attachRight;

            attachLeftObj = Tank.GetComponent<Tank>()._attachLeft;

            startingScaleD = tankSP.transform.localScale;
            startingScaleH = tankSP.transform.localScale;

            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();
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
        }

    }

    void updateSize()
    {
        float number;
        if(float.TryParse(tankDiameter.text, out number))
        {
            tankDiameterFloat = float.Parse(tankDiameter.text);

            if(tankDiameterFloat == tankSP.size.x)
            {
                startingScaleD = tankSP.size;
                currentD = 0;
            }

            if(tankSP.size.x != tankDiameterFloat)
            {
                tankSP.size = UnityEngine.Vector2.Lerp(startingScaleD, new UnityEngine.Vector2(tankDiameterFloat, tankSP.size.y), currentD * 5);
                currentD += Time.deltaTime;
            }
            
        }

        if (float.TryParse(tankHeight.text, out number))
        {
            tankHeightFloat = float.Parse(tankHeight.text);
            if(tankSP.size.y == tankHeightFloat)
            {
                startingScaleH = tankSP.size;
                currentH = 0;
            }

            if(tankSP.size.y != tankHeightFloat)
            {
                tankSP.size = UnityEngine.Vector2.Lerp(startingScaleH, new UnityEngine.Vector2(tankSP.size.x, tankHeightFloat), currentH*5);
                currentH += Time.deltaTime;
            }
            
        }

    }

    void calculate()
    {
        float massDensity = 1750f; //Assuming aluminium
        float wallThickness = 0.0005f; //Assuming 0.1 cm thickness
        //float fuelDensity = 460.0f; //Assuming methane
        volume = Mathf.PI * Mathf.Pow(tankDiameterFloat/2, 2) * tankHeightFloat;
        mass = (volume -  Mathf.PI * Mathf.Pow(tankDiameterFloat/2 - wallThickness, 2)*tankHeightFloat) * massDensity;
        tankMaterial = materialDropdown.options[materialDropdown.value].text.ToString();
    }


    public void save()
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder);
        }

        saveName = "/" + savePath.text;

        if(!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json"))
        {
            saveTank saveObject = new saveTank();
            saveObject.path = savePathRef.tankFolder;
            saveObject.tankName = saveName;
            saveObject.tankSizeX = tankSP.size.x;
            saveObject.tankSizeY = tankSP.size.y;
            saveObject.attachTopPos = attachTopObj.transform.position.y - tankSP.bounds.center.y;
            saveObject.attachBottomPos = tankSP.bounds.center.y - attachTopObj.transform.position.y;
            saveObject.attachRightPos = attachRightObj.transform.position.x - tankSP.bounds.center.x;
            saveObject.attachLeftPos = attachLeftObj.transform.position.x - tankSP.bounds.center.x;
            saveObject.volume = volume;
            saveObject.mass = mass;
            saveObject.tankMaterial = tankMaterial;

            var jsonString = JsonConvert.SerializeObject(saveObject);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json", jsonString);
        }else if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName+ savePathRef.engineFolder + saveName + ".json"))
        {
            saveTank saveTank = new saveTank();
            var jsonString2 = JsonConvert.SerializeObject(saveTank);
            jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder + saveName + ".json");
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString2);

            if(loadedTank.usedNum == 0)
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

}
