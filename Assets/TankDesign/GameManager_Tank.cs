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


    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "TankDesign")
        {
            tankRef = Tank.GetComponent<Part>().tank;
            tankSP = tankRef.GetComponent<SpriteRenderer>();

            attachTopRef = Tank.GetComponent<Part>().attachTop;
            attachTopObj = GameObject.Find(attachTopRef.name);
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
            tankSP.transform.localScale = new Vector3 (float.Parse(tankDiameter.text), tankSP.transform.localScale.y, tankSP.transform.localScale.z);
            tankDiameterFloat = float.Parse(tankDiameter.text);
        }

        if (float.TryParse(tankHeight.text, out number))
        {
            tankSP.transform.localScale = new Vector3 (tankSP.transform.localScale.x, float.Parse(tankHeight.text), tankSP.transform.localScale.z);
            tankHeightFloat = float.Parse(tankHeight.text);
        }

    }

    void calculate()
    {
        fuel = tankDiameterFloat * tankHeightFloat * 100;
        mass = tankHeightFloat * tankDiameterFloat * 100;

    }


    public void save()
    {
        saveTank saveObject = new saveTank();
        saveName = "/" + savePath.text;

        
        saveObject.path = savePathRef.tankFolder;
        saveObject.name = saveName;
        saveObject.tankSizeX = tankSP.transform.localScale.x;
        saveObject.tankSizeX = tankSP.transform.localScale.y;

        saveObject.fuel = fuel;
        saveObject.mass = mass;

        var jsonString = JsonConvert.SerializeObject(saveObject);
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.tankFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.tankFolder);
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.tankFolder + saveName + ".json", jsonString);
    }

    public void updateAttachPosition()
    {
        attachBottomRef = Tank.GetComponent<Part>().attachBottom;
        attachBottomObj = GameObject.Find(attachBottomRef.name);
    }

    public void backToBuild()
    {
        SceneManager.LoadScene("Building");
    }

}
