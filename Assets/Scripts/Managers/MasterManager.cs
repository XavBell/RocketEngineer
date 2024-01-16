using System.Net.Mime;
using System.Diagnostics;
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

public class MasterManager : MonoBehaviour
{
    public savePath savePathRef = new savePath();
    public string FolderName;

    public TMP_InputField savePath;

    public GameObject AlertText;

    public GameObject scrollBox;

    public GameObject buttonPrefab;

    public GameObject loadButton;

    public string worldPath;

    public string gameState = "Building";

    public GameObject currentBuildingBody;

    public GameObject ActiveRocket;

    public GameObject MainCanvas;
    public GameObject loadCanvas;
    public GameObject newWorldCanvas;
    public GameObject optionCanvas;
    public List<GameObject> buttons = new List<GameObject>();

    public float MenuThrust = 200;
    public GameObject MenuRocket;
    public bool rocketActive = false;

    public List<string> partType = new List<string>();
    public List<string> partName = new List<string>();
    public List<int> count = new List<int>();

    public List<string> turbineUnlocked = new List<string>();
    public List<string> pumpUnlocked = new List<string>();
    public List<string> nozzleUnlocked = new List<string>();
    public List<string> tvcUnlocked = new List<string>();

    public List<string> tankMaterialUnlocked = new List<string>();

    public float maxTankBuildSizeX = 1;
    public float maxTankBuildSizeY = 1;

    public float maxRocketBuildSizeX = 1;
    public float maxRocketBuildSizeY = 1;

    public List<string> nodeUnlocked = new List<string>();

    public Toggle fullScreen;
    
   

    // Start is called before the first frame update
    void Start()
    {
        updateButtons();
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBuildingBody == null)
        {
            GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
            foreach(GameObject go in planets)
            {
                if(go.GetComponent<TypeScript>().type == "earth")
                {
                    currentBuildingBody = go;
                }
            }
        }

        if(SceneManager.GetActiveScene().name == "Menu")
        {
            thrust();
        }
    }

    void updateButtons()
    {
        foreach(GameObject child in buttons)
        {
            DestroyImmediate(child);
        }

        buttons = new List<GameObject>();
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder);
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder);
        var fileInfo = info.GetDirectories();
        foreach (var file in fileInfo)
        {
            GameObject button = Instantiate(buttonPrefab);
            GameObject child = button.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scrollBox.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
            buttons.Add(button);
            buttons.Add(child);
        } 
    }

    public void newGame()
    {
        string folder = savePath.text.ToString();
        if(!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder);
            FolderName = folder;
            saveWorld saveWorld = new saveWorld();
            var jsonString = JsonConvert.SerializeObject(saveWorld);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder + "/" + FolderName + ".json", jsonString);
            worldPath = Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder + "/" + FolderName + ".json";
            SceneManager.LoadScene("SampleScene");
        }
        else if(Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder))
        {
            StartCoroutine(Text());
        }
    }

    public void load()
    {
        if(FolderName != null)
        {
            worldPath = Application.persistentDataPath + savePathRef.worldsFolder + "/" + FolderName + "/" + FolderName + ".json";
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void delete()
    {
        string folder = savePath.text.ToString();
        if(FolderName != null)
        {
            var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder);
            var fileInfo = info.GetFiles();
            foreach(var file in fileInfo)
            {
                file.Delete();
            }
            Directory.Delete(Application.persistentDataPath + savePathRef.worldsFolder + "/" + FolderName, true);
        }

        updateButtons();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void loadWorld()
    {
        rocketActive = true;
        MainCanvas.SetActive(false);
        loadCanvas.SetActive(true);
    }

    public void newWorld()
    {
        rocketActive = true;
        MainCanvas.SetActive(false);
        newWorldCanvas.SetActive(true);
    }

    public void options()
    {
        rocketActive = true;
        MainCanvas.SetActive(false);
        optionCanvas.SetActive(true);
    }

    public void back()
    {
        MainCanvas.SetActive(true);
        newWorldCanvas.SetActive(false);
        loadCanvas.SetActive(false);
        optionCanvas.SetActive(false);
    }

    void thrust()
    {
        if(rocketActive == true)
        {
            Vector3 Thrust = MenuRocket.transform.up * MenuThrust;
            MenuRocket.GetComponent<Rigidbody2D>().AddForce(Thrust);
        }
    }

    IEnumerator Text() 
    {
	    AlertText.SetActive(true);
        yield return new WaitForSeconds(1);
        AlertText.SetActive(false);
    }

    public void Toggle()
    {
        if(fullScreen.isOn == true)
        {
            Screen.fullScreen = true;
        }

        if(fullScreen.isOn == false)
        {
            Screen.fullScreen = false;
        }
    }
}
