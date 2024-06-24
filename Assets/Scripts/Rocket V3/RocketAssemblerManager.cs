using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RocketAssemblerManager : MonoBehaviour
{
    MasterManager masterManager;
    public StageEditor stageEditor;
    public DesignerCursor designerCursor;
    public GameObject activePart;
    public GameObject originalPart;
    public RocketController rocketController;
    public rocketSaveManager RocketSaveManager = new rocketSaveManager();
    public bool partPlaced = false;

    //UI References
    public GameObject MainPanel;
    public GameObject CreatorPanel;
    public GameObject DataPanel;
    public GameObject propellantPanel;
    public GameObject scrollEngine;
    public GameObject scrollTank;
    public GameObject engineButtonPrefab;
    public GameObject tankButtonPrefab;
    public TMP_Dropdown lineDropdown;
    public TMP_InputField lineName;
    public TMP_InputField rocketName;


    //For rocket wide variables
    public List<string> lineNames = new List<string>();
    public List<Guid> lineGuids = new List<Guid>();

    //For editor
    public savePath savePathRef = new savePath();
    public GameObject[] panels;
    public string path;
    public string partType;



    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        //savePathRef = new savePath();
        UpdateSaveFolder();
        retrieveEngineSaved();
        retrieveTankSaved();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateRocketName();
    }

    public void selectPart(GameObject part)
    {
        if(part != null)
        {
            GameObject newPart = Instantiate(part, designerCursor.transform);
            designerCursor.selectedPart = newPart;
            initializePartFromType(newPart, newPart.GetComponent<PhysicsPart>().type);
            activePart = newPart;
            Cursor.visible = false;
        }
    }

    public void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            placePart();
        }

        if(Input.GetMouseButtonDown(1))
        {
            OpenLineSetter();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            Rotate();
        }
    }

    void UpdateSaveFolder()
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.rocketFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.rocketFolder);
        }
    }

    public void UpdateRocketName()
    {
        rocketController.rocketName = rocketName.text;
    }

    public void Rotate()
    {
        if(activePart != null)
        {
            activePart.transform.Rotate(0, 0, 90);
        }
    }

    public void placePart()
    {
        //Check if root part is present
        if(partPlaced == false)
        {
            if(activePart != null)
            {
                rocketController.transform.position = activePart.transform.position;
                activePart.transform.parent = rocketController.transform;
                originalPart = activePart;
                activePart.GetComponent<PhysicsPart>().guid = Guid.NewGuid();
                initializePartFromType(activePart, activePart.GetComponent<PhysicsPart>().type);
                ClearPart();
                partPlaced = true;
            }
        }

        //If root is present snap to closest part
        if(partPlaced == true)
        {
            if(activePart != null)
            {
                //Find closest part with PhysicsPart component on other placed parts
                AttachPoint[] attachs = FindObjectsOfType<AttachPoint>();
                AttachPoint closestAttach = null;
                float closestDistance = Mathf.Infinity;
                foreach(AttachPoint attach in attachs)
                {
                    float distance = Vector2.Distance(attach.transform.position, activePart.transform.position);
                    if(distance < closestDistance && attach.transform.parent != activePart.transform && attach.isConnected == false)
                    {
                        closestAttach = attach;
                        closestDistance = distance;
                    }
                }

                //Snap to the PhysicsPart Collider if attach is found
                if (closestAttach != null)
                {
                    if (closestAttach.GetComponentInParent<PhysicsPart>().CanHaveChildren == false)
                    {
                        DestroyImmediate(activePart);
                        partPlaced = true;
                        ClearPart();
                    }
                    else if(closestAttach.GetComponentInParent<PhysicsPart>().CanHaveChildren == true)
                    {
                        //Find attach point on part that is closest to the attach point
                        AttachPoint closestAttachPoint = FindClosestAttachPoint(closestAttach);

                        closestAttach.isConnected = true;
                        closestAttachPoint.isConnected = true;

                        //Move the part to the attach point
                        Vector2 offset = closestAttach.transform.position - closestAttachPoint.transform.position;
                        activePart.GetComponent<PhysicsPart>().guid = Guid.NewGuid();
                        initializePartFromType(activePart, activePart.GetComponent<PhysicsPart>().type);
                        activePart.transform.position = new Vector2(activePart.transform.position.x + offset.x, activePart.transform.position.y + offset.y);
                        activePart.transform.parent = closestAttach.transform.parent.transform;
                        ClearPart();
                    }
                }

            }
            
        }
    }

    public void AddLine()
    {
        rocketController.lineNames.Add(lineName.text);
        rocketController.lineGuids.Add(Guid.NewGuid());
        lineDropdown.AddOptions(new List<string> { lineName.text });
    }

    public void RemoveLine()
    {
        int index = rocketController.lineNames.IndexOf(lineDropdown.options[lineDropdown.value].text);
        rocketController.lineNames.RemoveAt(index);
        rocketController.lineGuids.RemoveAt(index);
        lineDropdown.ClearOptions();
        lineDropdown.AddOptions(rocketController.lineNames);
    }

    public void OpenLineSetter()
    {
        RaycastHit2D raycastHit;
        Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 ray = cameraPos;
        raycastHit = Physics2D.Raycast(ray, new Vector2(0, 1000));
        if(raycastHit.transform != null)
        {
            if(raycastHit.transform.gameObject.GetComponent<TankComponent>())
            {
                propellantPanel.SetActive(true);
                propellantPanel.GetComponent<dropDownManager>().tank = raycastHit.transform.gameObject.GetComponent<TankComponent>();
            }
        }
    }

    private AttachPoint FindClosestAttachPoint(AttachPoint closestAttach)
    {
        AttachPoint[] attachPoints = activePart.transform.GetComponentsInChildren<AttachPoint>();
        AttachPoint closestAttachPoint = null;
        float closestAttachPointDistance = Mathf.Infinity;
        foreach (AttachPoint attachPoint in attachPoints)
        {
            float distance = Vector2.Distance(attachPoint.transform.position, closestAttach.transform.position);
            if (distance < closestAttachPointDistance && attachPoint.transform.parent != closestAttach.transform.parent.transform)
            {
                closestAttachPoint = attachPoint;
                closestAttachPointDistance = distance;
            }
        }
        return closestAttachPoint;
    }

    private void ClearPart()
    {
        stageEditor.UpdateButtons();
        activePart = null;
        designerCursor.selectedPart = null;
        Cursor.visible = true;
    }

    public void initializePartFromType(GameObject part, string type)
    {
        if(type == "decoupler")
        {
            InitializeDecoupler(part);
        }

        if(type == "tank")
        {
            initializeTank(part, path);
        }

        if(type == "engine")
        {
            initializeEngine(part, path);
        }
    }

    public void initializeTank(GameObject tank, string path)
    {
        tank.GetComponent<PhysicsPart>().path = path;
        var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.tankFolder + path);
        saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);
        tank.transform.localScale = new Vector3(loadedTank.tankSizeX, loadedTank.tankSizeY, tank.transform.localScale.z);
    }

    public void initializeEngine(GameObject engine, string path)
    {
        engine.GetComponent<PhysicsPart>().path = path;
        var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.engineFolder + path);
        saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);
        engine.GetComponent<EngineComponent>()._nozzleName = loadedEngine.nozzleName_s;
        engine.GetComponent<EngineComponent>()._pumpName = loadedEngine.pumpName_s;
        engine.GetComponent<EngineComponent>()._turbineName = loadedEngine.turbineName_s;
        engine.GetComponent<EngineComponent>().InitializeSprite();
    }

    private static void InitializeDecoupler(GameObject part)
    {
        AttachPoint[] attachPoints = part.transform.GetComponentsInChildren<AttachPoint>();
        bool topConnected = false;
        bool bottomConnected = false;
        foreach (AttachPoint attachPoint in attachPoints)
        {
            //Determine if the decoupler is connected to the top or bottom to know how it will detach
            if (attachPoint.relativeOrientation == "top")
            {
                if (attachPoint.isConnected == true)
                {
                    topConnected = true;
                }
            }
            if (attachPoint.relativeOrientation == "bottom")
            {
                if (attachPoint.isConnected == true)
                {
                    bottomConnected = true;
                }
            }
        }

        if (topConnected == true)
        {
            part.GetComponent<DecouplerComponent>().detachFromParent = true;
        }

        if (bottomConnected == true)
        {
            part.GetComponent<DecouplerComponent>().detachFromParent = false;
        }
    }

    public void save()
    {
        RocketSaveManager.saveRocket(rocketController, rocketController.rocketName);
    }

    public void load()
    {
        RocketSaveManager.loadRocket(rocketController, rocketController.rocketName);
        partPlaced = true;
    }

    public void retrieveEngineSaved()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("engineButton");
        foreach (GameObject but in buttons)
        {
            Destroy(but);
        }

        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.engineFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.engineFolder);
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.engineFolder);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            GameObject engine = Instantiate(engineButtonPrefab) as GameObject;
            GameObject child = engine.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scrollEngine.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
            child.GetComponentInChildren<OnClick>().filePath = savePathRef.engineFolder;
        }
    }

    public void retrieveTankSaved()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("tankButton");
        foreach (GameObject but in buttons)
        {
            Destroy(but);
        }

        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.tankFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.tankFolder);
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.tankFolder);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            GameObject tank = Instantiate(tankButtonPrefab) as GameObject;
            GameObject child = tank.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scrollTank.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
            child.GetComponentInChildren<OnClick>().filePath = savePathRef.tankFolder;
        }
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

    public void activateDeactivate(GameObject button)
    {
        hidePanels(button);
        if (button.activeSelf == true)
        {
            button.SetActive(false);
            return;
        }

        if (button.activeSelf == false)
        {
            button.SetActive(true);
            return;
        }
    }

    public void hidePanels(GameObject excludedPanel)
    {
        foreach (GameObject panel in panels)
        {
            if (panel != excludedPanel)
            {
                panel.SetActive(false);
            }
        }
    }

    public void ExitCreator()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
}
