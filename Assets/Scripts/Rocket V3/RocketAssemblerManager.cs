using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class RocketAssemblerManager : MonoBehaviour
{
    public DesignerCursor designerCursor;
    public GameObject activePart;
    public GameObject originalPart;
    public bool partPlaced = false;

    //Prefab list
    public GameObject tank;
    public GameObject engine;

    // Start is called before the first frame update
    void Start()
    {
        
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
                activePart.transform.parent = null;
                originalPart = activePart;
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
                        initializePartFromType(activePart, activePart.GetComponent<PhysicsPart>().type);
                        ClearPart();
                    }
                }

            }
            print(originalPart.transform.childCount);
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

    public void saveRocket()
    {
        RocketData rocketData = new RocketData();
        rocketData.rocketName = "Rocket";
        rocketData.rootPart = new PartData();
        rocketData.rootPart.partType = originalPart.GetComponent<PhysicsPart>().type;
        rocketData.rootPart.x_pos = originalPart.transform.position.x;
        rocketData.rootPart.y_pos = originalPart.transform.position.y;
        rocketData.rootPart.z_rot = originalPart.transform.eulerAngles.z;
        savePartFromType(rocketData.rootPart.partType, originalPart, rocketData.rootPart);

        //Get all children
        PartData rootPart = rocketData.rootPart;
        GameObject rootPartObject = originalPart;
        AddChildren(rootPart, rootPartObject);
        

        //Write file
        string saveUserPath = Application.persistentDataPath + "/rocket.json";
        string rocketData1 = JsonConvert.SerializeObject(rocketData);
        File.WriteAllText(saveUserPath, rocketData1);

    }

    public void loadRocket()
    {
        string saveUserPath = Application.persistentDataPath + "/rocket.json";
        string rocketData1 = File.ReadAllText(saveUserPath);
        RocketData rocketData = JsonConvert.DeserializeObject<RocketData>(rocketData1);

        //Load rocket
        GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + rocketData.rootPart.partType));
        originalPart = newPart;
        originalPart.transform.position = new Vector2(rocketData.rootPart.x_pos, rocketData.rootPart.y_pos);
        originalPart.transform.rotation = Quaternion.Euler(0, 0, rocketData.rootPart.z_rot);
        partPlaced = true;
        originalPart.transform.parent = null;
        loadPartFromType(rocketData.rootPart.partType, originalPart, rocketData.rootPart);
        LoadChildren(rocketData.rootPart, originalPart);
    }

    public void LoadChildren(PartData parent, GameObject parentObject)
    {
        foreach(PartData child in parent.children)
        {
            GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + child.partType));
            //Important to change rotation before assigning parent
            newPart.transform.rotation = Quaternion.Euler(0, 0, child.z_rot);
            newPart.transform.position = new Vector2(child.x_pos, child.y_pos);
            newPart.transform.parent = parentObject.transform;
            loadPartFromType(child.partType, newPart, child);
            LoadChildren(child, newPart);
        }
    }

    public void loadPartFromType(string type, GameObject part, PartData partData)
    {
        if(type == "decoupler")
        {
            loadDecoupler(part, partData);
        }
    }

    public void loadDecoupler(GameObject part, PartData partData)
    {
        part.GetComponent<DecouplerComponent>().detachFromParent = partData.detachFromParent;
    }

    public void AddChildren(PartData parent, GameObject parentObject)
    {
        foreach(Transform child in parentObject.transform)
        {
            if(child.GetComponent<PhysicsPart>() == null)
            {
                continue;
            }
            PartData newPart = new PartData();
            newPart.partType = child.GetComponent<PhysicsPart>().type;
            newPart.x_pos = child.position.x;
            newPart.y_pos = child.position.y;
            newPart.z_rot = child.eulerAngles.z;
            savePartFromType(newPart.partType, child.gameObject, newPart);
            parent.children.Add(newPart);
            AddChildren(newPart, child.gameObject);
        }
    }

    public void savePartFromType(string type, GameObject part, PartData partData)
    {
        if(type == "decoupler")
        {
            saveDecoupler(part, partData);
        }
    }

    public void saveDecoupler(GameObject part, PartData partData)
    {
        partData.detachFromParent = part.GetComponent<DecouplerComponent>().detachFromParent;
    }
}
