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

    public float nozzleExitSizeFloat;
    public float nozzleEndSizeFloat;
    public float turbopumpSizeFloat;
    public float nozzleLenghtFloat;
    public float turbopumpRateFloat;

    public float mass;
    public float thrust;
    public float rate;

    public float currentE = 0;
    public float currentEn = 0;
    public float currentT = 0;

    public Vector3 startingScaleE;
    public Vector3 startingScaleEn;
    public Vector3 startingScaleT;

    public savePath savePathRef = new savePath();


    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
        {
            nozzleExitRef = Engine.GetComponent<Part>().nozzleExit;
            nozzleEndRef = Engine.GetComponent<Part>().nozzleEnd;
            turbopumpRef = Engine.GetComponent<Part>().turbopump;

            startingScaleE = nozzleExitRef.transform.localScale;
            startingScaleEn = nozzleEndRef.transform.localScale;
            startingScaleT = turbopumpRef.transform.localScale;
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
        float number;
        if(float.TryParse(nozzleExitSize.text, out number))
        {
            nozzleExitSizeFloat = float.Parse(nozzleExitSize.text);

            if(nozzleExitRef.transform.localScale.x == nozzleExitSizeFloat)
            {
                startingScaleE = nozzleExitRef.transform.localScale;
                currentE = 0;
            }

            if(nozzleExitRef.transform.localScale.x != nozzleExitSizeFloat)
            { 
                nozzleExitRef.transform.localScale = Vector3.Lerp(startingScaleE, new Vector3(nozzleExitSizeFloat, nozzleExitRef.transform.localScale.y, 0), currentE*5);
                currentE += Time.deltaTime;
            }
        }

        if (float.TryParse(nozzleEndSize.text, out number))
        {
            nozzleEndSizeFloat = float.Parse(nozzleEndSize.text);

            if(nozzleEndRef.transform.localScale.x == nozzleEndSizeFloat)
            {
                startingScaleEn = nozzleEndRef.transform.localScale;
                currentEn = 0;
            }

            if(nozzleEndRef.transform.localScale.x != nozzleEndSizeFloat)
            {
                nozzleEndRef.transform.localScale = Vector3.Lerp(startingScaleEn, new Vector3(nozzleEndSizeFloat, nozzleEndRef.transform.localScale.y, 0), currentEn*5);
                currentEn += Time.deltaTime;
            }
        }

        if (float.TryParse(turbopumpSize.text, out number))
        {
            turbopumpSizeFloat = float.Parse(turbopumpSize.text);

            if(turbopumpRef.transform.localScale.x == turbopumpSizeFloat)
            {
                startingScaleT = turbopumpRef.transform.localScale;
                currentT = 0;
            }

            if(turbopumpRef.transform.localScale.x != turbopumpSizeFloat)
            {
                turbopumpRef.transform.localScale = Vector3.Lerp(startingScaleT, new Vector3(turbopumpSizeFloat, turbopumpRef.transform.localScale.y, 0), currentT*5);
                currentT += Time.deltaTime;
            }
        }


        if (float.TryParse(nozzleLenght.text, out number))
        {
            nozzleLenghtFloat = float.Parse(nozzleLenght.text);
        }

        if (float.TryParse(turbopumpRate.text, out number))
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
        if(nozzleEndSizeFloat == 0 || nozzleExitSizeFloat == 0 || turbopumpSizeFloat == 0 || nozzleLenghtFloat == 0 || turbopumpRateFloat == 0)
        {
            thrust = 0;
            rate = 0;
            return;
        }

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
        mass = 300 * turbopumpRef.transform.localScale.x;

    }


    public void save()
    {
        saveEngine saveObject = new saveEngine();
        List<float> sizes = new List<float>();
        saveName = "/"+ savePath.text;

        saveObject.path = savePathRef.engineFolder;
        saveObject.name = saveName;
        saveObject.nozzleExitSize_s = nozzleExitRef.GetComponent<SpriteRenderer>().transform.localScale.x;
        sizes.Add(saveObject.nozzleExitSize_s);
        saveObject.nozzleEndSize_s = nozzleEndRef.GetComponent<SpriteRenderer>().transform.localScale.x;
        sizes.Add(saveObject.nozzleEndSize_s);
        saveObject.turbopumpSize_s = turbopumpRef.GetComponent<SpriteRenderer>().transform.localScale.x;
        sizes.Add(saveObject.turbopumpSize_s);

        float bestSize = 0;
        foreach(float size in sizes)
        {
            if(size > bestSize)
            {
                bestSize = size;
            }
        }

        saveObject.verticalSize_s = Engine.GetComponent<SpriteRenderer>().transform.localScale.y;
        saveObject.horizontalBestSize_s = bestSize;
        saveObject.thrust_s = mass;
        saveObject.thrust_s = thrust;
        saveObject.rate_s = rate;

        var jsonString = JsonConvert.SerializeObject(saveObject);
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.engineFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.engineFolder);
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.engineFolder + saveName + ".json", jsonString);
    }

    public void backToBuild()
    {
        SceneManager.LoadScene("Building");
    }

}
