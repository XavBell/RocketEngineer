using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.UI;
using TMPro;

public class RocketAssemblerManager : MonoBehaviour
{
    MasterManager masterManager;
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


    //For rocket wide variables
    public List<string> lineNames = new List<string>();
    public List<Guid> lineGuids = new List<Guid>();

    //For editor
    public string lineName;
    public savePath savePathRef = new savePath();



    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        //savePathRef = new savePath();
        retrieveEngineSaved();
        retrieveTankSaved();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    public void selectPart(GameObject part)
    {
        if(part != null)
        {
            GameObject newPart = Instantiate(part, designerCursor.transform);
            designerCursor.selectedPart = newPart;
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

        if(Input.GetKeyDown(KeyCode.R))
        {
            Rotate();
        }
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
                activePart = null;
                designerCursor.selectedPart = null;
                Cursor.visible = true;
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
                        activePart.transform.position = new Vector2(activePart.transform.position.x + offset.x, activePart.transform.position.y + offset.y);
                        activePart.transform.parent = closestAttach.transform.parent.transform;
                        activePart.GetComponent<PhysicsPart>().guid = Guid.NewGuid();
                        initializePartFromType(activePart, activePart.GetComponent<PhysicsPart>().type);
                        ClearPart();
                    }
                }

            }
            print(originalPart.transform.childCount);
        }
    }

    public void AddLine()
    {
        rocketController.lineNames.Add(lineName);
        rocketController.lineGuids.Add(Guid.NewGuid());
    }

    //Purely for editor
    public void SetLine(string line, TankComponent tankComponent)
    {
        tankComponent.lineName = line;
        tankComponent.lineGuid = rocketController.lineGuids[rocketController.lineNames.IndexOf(line)];
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
        RocketSaveManager.saveRocket(rocketController, true);
    }

    public void load()
    {
        RocketSaveManager.loadRocket(rocketController, true);
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
    
}
