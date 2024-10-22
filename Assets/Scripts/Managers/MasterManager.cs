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

    public GameObject bigTextCanvas; 

    public string worldPath;

    public string gameState = "Building";

    public GameObject currentBuildingBody;

    public GameObject ActiveRocket;

    public GameObject MainCanvas;
    public GameObject loadCanvas;
    public GameObject newWorldCanvas;
    public GameObject optionCanvas;
    public TMP_Dropdown resolutionDropdown;
    public List<GameObject> buttons = new List<GameObject>();

    public Toggle postProcessingToggle;
    public TMP_InputField scrollMultiplier;

    public float MenuThrust = 200;
    public GameObject MenuRocket;
    public bool rocketActive = false;

    public List<string> rockets = new List<string>();
    public List<string> tanks = new List<string>();
    public List<string> engines = new List<string>();

    public List<Turbine> turbineUnlocked = new List<Turbine>();
    public List<Pump> pumpUnlocked = new List<Pump>();
    public List<Nozzle> nozzleUnlocked = new List<Nozzle>();
    public List<TVC> tvcUnlocked = new List<TVC>();

    public List<string> tankMaterialUnlocked = new List<string>();

    public float maxTankBuildSizeX = 1;
    public float maxTankBuildSizeY = 1;

    public float maxRocketBuildSizeX = 1;
    public float maxRocketBuildSizeY = 1;

    public List<string> nodeUnlocked = new List<string>();

    public Toggle fullScreen;
    public Toggle stars;

    public bool postProcess = true;
    public bool showStars = true;
    public float scrollMultiplierValue = 1;

    GameObject btCreate;
    GameObject btOptions;
    GameObject btLoad;

    [SerializeField] GameObject bgMenu;

    // Start is called before the first frame update
    void Start()
    {
        //Check if current resolution is in 16:9, if not set it
        Screen.SetResolution(1920, 1080, true);
        

        if (File.Exists(Application.persistentDataPath + "/saveUser.json"))
        {
            saveUser saveUserRef = new saveUser();
            string currentVersion = saveUserRef.version;
            string saveUserPath = Application.persistentDataPath + "/saveUser.json";
            string saveUserJson = File.ReadAllText(saveUserPath);
            saveUser saveUser = JsonConvert.DeserializeObject<saveUser>(saveUserJson);
            if(saveUser.version == currentVersion)
            {
                // Load saveUser values here<
                postProcess = saveUser.postProcess;
                scrollMultiplierValue = saveUser.scrollMultiplier;
                stars.isOn = saveUser.showStars;
                Screen.SetResolution((int)saveUser.xRes, (int)saveUser.yRes, fullScreen.isOn);
            }else{
                createSaveUser();
            }
            
        }
        else
        {
            createSaveUser();
        }
        updateButtons();
        DontDestroyOnLoad(this.gameObject);

        btCreate = bigTextCanvas.transform.GetChild(0).gameObject;
        btOptions = bigTextCanvas.transform.GetChild(1).gameObject;
        btLoad = bigTextCanvas.transform.GetChild(2).gameObject;
    }

    private void createSaveUser()
    {
        // Create a new saveUser object with default values
        saveUser saveUser = new saveUser();
        saveUser.postProcess = postProcess;
        saveUser.scrollMultiplier = scrollMultiplierValue;
        saveUser.xRes = Screen.width;
        saveUser.yRes = Screen.height;
        saveUser.showStars = showStars;

        // Serialize the saveUser object to JSON
        string saveUserJson = JsonConvert.SerializeObject(saveUser);

        // Write the JSON string to the saveUser.json file
        File.WriteAllText(Application.persistentDataPath + "/saveUser.json", saveUserJson);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBuildingBody == null)
        {
            GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
            foreach (GameObject go in planets)
            {
                if (go.GetComponent<TypeScript>().type == "earth")
                {
                    currentBuildingBody = go;
                }
            }
        }
    }

    void updateButtons()
    {
        foreach (GameObject child in buttons)
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

        initializeResolutionDropdown();
        initializeGraphics();
        initializeGameSettings();
    }

    public void newGame()
    {
        string folder = savePath.text.ToString();
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder);
            FolderName = folder;
            saveWorld saveWorld = new saveWorld();
            var jsonString = JsonConvert.SerializeObject(saveWorld);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder + "/" + FolderName + ".json", jsonString);
            worldPath = Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder + "/" + FolderName + ".json";
            SceneManager.LoadScene("SampleScene");
        }
        else if (Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder))
        {
            StartCoroutine(Text());
        }
    }

    public void load()
    {
        if (FolderName != null)
        {
            worldPath = Application.persistentDataPath + savePathRef.worldsFolder + "/" + FolderName + "/" + FolderName + ".json";
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void delete()
    {
        string folder = savePath.text.ToString();
        if (FolderName != null)
        {
            var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder);
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
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

        btCreate.SetActive(false);
        btOptions.SetActive(false);
        btLoad.SetActive(true);
    }

    public void newWorld()
    {
        rocketActive = true;
        MainCanvas.SetActive(false);
        newWorldCanvas.SetActive(true);

        btCreate.SetActive(true);
        btOptions.SetActive(false);
        btLoad.SetActive(false);
    }

    public void options()
    {
        rocketActive = true;
        MainCanvas.SetActive(false);
        optionCanvas.SetActive(true);

        btCreate.SetActive(false);
        btOptions.SetActive(true);
        btLoad.SetActive(false);
    }

    public void back()
    {
        MainCanvas.SetActive(true);
        newWorldCanvas.SetActive(false);
        loadCanvas.SetActive(false);
        optionCanvas.SetActive(false);

        btCreate.SetActive(false);
        btOptions.SetActive(false);
        btLoad.SetActive(false);
    }

    IEnumerator Text()
    {
        AlertText.SetActive(true);
        yield return new WaitForSeconds(1);
        AlertText.SetActive(false);
    }

    public void toggleFullscreen()
    {
        Screen.fullScreen = fullScreen.isOn;
    }

    public void toggleStars()
    {
        showStars = stars.isOn;
        bgMenu.SetActive(showStars);
        // Update the saveUser file with the new postProcess value
        if (File.Exists(Application.persistentDataPath + "/saveUser.json"))
        {
            string saveUserPath = Application.persistentDataPath + "/saveUser.json";
            string saveUserJson = File.ReadAllText(saveUserPath);
            saveUser saveUser = JsonConvert.DeserializeObject<saveUser>(saveUserJson);
            saveUser.showStars = showStars;
            saveUserJson = JsonConvert.SerializeObject(saveUser);
            File.WriteAllText(saveUserPath, saveUserJson);
        }
    }


    void initializeResolutionDropdown()
    {
        //Set resolution dropdown option to current resolution
        if (Screen.width == 1280 && Screen.height == 720)
        {
            resolutionDropdown.value = 0;
        }

        if (Screen.width == 1920 && Screen.height == 1080)
        {
            resolutionDropdown.value = 1;
        }

        if (Screen.width == 2560 && Screen.height == 1440)
        {
            resolutionDropdown.value = 2;
        }

        if (Screen.width == 3840 && Screen.height == 2160)
        {
            resolutionDropdown.value = 3;
        }

        //Update full screen toggle state
        fullScreen.isOn = Screen.fullScreen;
    }

    public void initializeGraphics()
    {
        //postProcessingToggle.isOn = postProcess;
    }

    public void SetPostProcessing()
    {
        postProcess = postProcessingToggle.isOn;


        // Update the saveUser file with the new postProcess value
        if (File.Exists(Application.persistentDataPath + "/saveUser.json"))
        {
            string saveUserPath = Application.persistentDataPath + "/saveUser.json";
            string saveUserJson = File.ReadAllText(saveUserPath);
            saveUser saveUser = JsonConvert.DeserializeObject<saveUser>(saveUserJson);
            saveUser.postProcess = postProcess;
            saveUserJson = JsonConvert.SerializeObject(saveUser);
            File.WriteAllText(saveUserPath, saveUserJson);
        }

    }

    public void initializeGameSettings()
    {
        scrollMultiplier.text = scrollMultiplierValue.ToString();
    }

    public void SetScrollMultiplier()
    {
        scrollMultiplierValue = float.Parse(scrollMultiplier.text);

        // Update the saveUser file with the new postProcess value
        if (File.Exists(Application.persistentDataPath + "/saveUser.json"))
        {
            string saveUserPath = Application.persistentDataPath + "/saveUser.json";
            string saveUserJson = File.ReadAllText(saveUserPath);
            saveUser saveUser = JsonConvert.DeserializeObject<saveUser>(saveUserJson);
            saveUser.scrollMultiplier = scrollMultiplierValue;
            saveUserJson = JsonConvert.SerializeObject(saveUser);
            File.WriteAllText(saveUserPath, saveUserJson);
        }
    }

    public void SetResolution()
    {
        //0 is 1280x720
        //1 is 1920x1080
        //2 is 2560x1440
        //3 is 3840x2160

        //Set resolution based on dropdown value
        if (resolutionDropdown.value == 0)
        {
            Screen.SetResolution(1280, 720, Screen.fullScreen);
        }

        if (resolutionDropdown.value == 1)
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }

        if (resolutionDropdown.value == 2)
        {
            Screen.SetResolution(2560, 1440, Screen.fullScreen);
        }

        if (resolutionDropdown.value == 3)
        {
            Screen.SetResolution(3840, 2160, Screen.fullScreen);
        }

        // Update the saveUser file with the new resolution values
        if (File.Exists(Application.persistentDataPath + "/saveUser.json"))
        {
            string saveUserPath = Application.persistentDataPath + "/saveUser.json";
            string saveUserJson = File.ReadAllText(saveUserPath);
            saveUser saveUser = JsonConvert.DeserializeObject<saveUser>(saveUserJson);
            saveUser.xRes = Screen.width;
            saveUser.yRes = Screen.height;
            saveUserJson = JsonConvert.SerializeObject(saveUser);
            File.WriteAllText(saveUserPath, saveUserJson);
        }
    }
}
