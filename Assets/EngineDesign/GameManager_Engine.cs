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

public class GameManager_Engine : MonoBehaviour
{
    public TMP_InputField nozzleExitSize;
    public TMP_InputField nozzleEndSize;
    public TMP_InputField turbopumpSize;
    public TMP_InputField nozzleLenght;
    public TMP_InputField turbopumpRate;

    public TMP_InputField savePath;
    public string saveName;

    public GameObject Engine;
    public GameObject nozzleExitRef;
    public GameObject nozzleEndRef;
    public GameObject turbopumpRef;

    public float nozzleLenghtFloat;
    public float turbopumpRateFloat;

    public float mass;
    public float thrust;
    public float rate;




    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
        {
            nozzleExitRef = Engine.GetComponent<Part>().nozzleExit;
            nozzleEndRef = Engine.GetComponent<Part>().nozzleEnd;
            turbopumpRef = Engine.GetComponent<Part>().turbopump;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
        {
            updateSize();
            calculate();
        }

    }

    void updateSize()
    {
        if(nozzleExitSize.text != "")
        {
            nozzleExitRef.transform.localScale = new Vector3 (float.Parse(nozzleExitSize.text), nozzleExitRef.transform.localScale.y, nozzleExitRef.transform.localScale.z);
        }

        if (nozzleEndSize.text != "")
        {
            nozzleEndRef.transform.localScale = new Vector3(float.Parse(nozzleEndSize.text), nozzleEndRef.transform.localScale.y, nozzleEndRef.transform.localScale.z);
        }

        if (turbopumpSize.text != "")
        {
            turbopumpRef.transform.localScale = new Vector3(float.Parse(turbopumpSize.text), turbopumpRef.transform.localScale.y, turbopumpRef.transform.localScale.z);
        }


        if (nozzleLenght.text != "")
        {
            nozzleLenghtFloat = float.Parse(nozzleLenght.text);
        }

        if (turbopumpRate.text != "")
        {
            turbopumpRateFloat = float.Parse(turbopumpRate.text);
        }
    }

    void calculate()
    {
        float nozzleExit_nozzleEndRatio = nozzleExitRef.transform.localScale.x/nozzleEndRef.transform.localScale.x;
        float turboRate_nozzleLengthRatio = turbopumpRateFloat / nozzleLenghtFloat;
        float turboRate_turboSizeRatio = turbopumpRateFloat / turbopumpRef.transform.localScale.x;

        //Debug.Log(turboRate_turboSizeRatio);

        if(nozzleExit_nozzleEndRatio < 1)
        {
            thrust = 0;
            rate = 0;
            return;
        }

        if (nozzleExit_nozzleEndRatio > 20)
        {
            thrust = 0;
            rate = 0;
            return;
        }

        if (turboRate_turboSizeRatio > 20)
        {
            thrust = 0;
            rate = 0;
            return;
        }

        thrust = (1/turboRate_turboSizeRatio) * turboRate_nozzleLengthRatio * (nozzleExit_nozzleEndRatio / 2) * (turbopumpRateFloat/2) * (1/ turbopumpRef.transform.localScale.x);
        rate = (turboRate_turboSizeRatio)/turbopumpRef.transform.localScale.x;
        mass = 500 * turbopumpRef.transform.localScale.x;

    }


    public void save()
    {
        saveEngine saveObject = new saveEngine();
        saveName = savePath.text;
        saveObject.path = "/engines/";
        saveObject.name = "/" + saveName;
        saveObject.nozzleExitSize_s = nozzleExitRef.transform.localScale.x;
        saveObject.nozzleEndSize_s = nozzleEndRef.transform.localScale.x;
        saveObject.turbopumpSize_s = turbopumpRef.transform.localScale.x;

        saveObject.thrust_s = mass;
        saveObject.thrust_s = thrust;
        saveObject.rate_s = rate;

        var jsonString = JsonConvert.SerializeObject(saveObject);
        if (!Directory.Exists(Application.persistentDataPath + "/engines"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/engines");
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + "/engines/" + saveName + ".json", jsonString);
    }

    public void backToBuild()
    {
        SceneManager.LoadScene("Building");
    }

}
