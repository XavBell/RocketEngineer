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
    public string saveName;

    public GameObject Tank;
    public GameObject tankRef;
    public SpriteRenderer tankSP;

    public AttachPointScript attachTopRef;
    public GameObject attachTopObj;

    public AttachPointScript attachBottomRef;
    public GameObject attachBottomObj;

    public float tankHeightFloat;
    public float tankDiameterFloat;

    public float fuel;
    public float mass;

    public savePath savePathRef = new savePath();
    public float currentD;
    public float currentH;

    public GameObject panel;
    public GameObject popUpPart;

    public float elapsedFrames = 0;


    public Vector3 startingScaleD;
    public Vector3 startingScaleH;

    public MasterManager MasterManager = new MasterManager();
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "TankDesign")
        {
            tankRef = Tank.GetComponent<Part>().tank;
            tankSP = tankRef.GetComponent<SpriteRenderer>();

            attachTopRef = Tank.GetComponent<Part>().attachTop;
            attachTopObj = GameObject.Find(attachTopRef.name);

            attachBottomRef = Tank.GetComponent<Part>().attachBottom;
            attachBottomObj = GameObject.Find(attachBottomRef.name);

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
                tankSP.size = Vector2.Lerp(startingScaleD, new Vector2(tankDiameterFloat, tankSP.size.y), currentD * 5);
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
                tankSP.size = Vector2.Lerp(startingScaleH, new Vector2(tankSP.size.x, tankHeightFloat), currentH*5);
                currentH += Time.deltaTime;
            }
            
        }

    }

    void calculate()
    {
        fuel = tankDiameterFloat * tankHeightFloat * 100;
        mass = tankHeightFloat * tankDiameterFloat * 100;

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
        saveObject.name = saveName;
        saveObject.tankSizeX = tankSP.size.x;
        saveObject.tankSizeY = tankSP.size.y;
        saveObject.attachTopPos = attachTopObj.transform.position.y - tankSP.bounds.center.y;
        saveObject.attachBottomPos = tankSP.bounds.center.y - attachTopObj.transform.position.y;
        Debug.Log(saveObject.attachTopPos);
        saveObject.maxFuel = fuel;
        saveObject.mass = mass;

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
            Vector2 position = new Vector2(x, y);
            Instantiate(popUpPart, position, Quaternion.identity);
            panel.active = false;
        }
    }

    public void updateAttachPosition()
    {
        attachTopObj.transform.position = (new Vector2(attachTopObj.transform.position.x, tankSP.bounds.max.y));
        attachBottomObj.transform.position = (new Vector2(attachBottomObj.transform.position.x, tankSP.bounds.min.y));
    }

    public void backToBuild()
    {
        SceneManager.LoadScene("Building");
    }

}
