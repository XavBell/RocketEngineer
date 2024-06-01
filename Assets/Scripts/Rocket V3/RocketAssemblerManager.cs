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
        onClick();   
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

    public void onClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            placePart();
        }
    }

    public void placePart()
    {
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

        if(partPlaced == true)
        {
            if(activePart != null)
            {
                //Find closest part with PhysicsPart component
                AttachPoint[] attachs = FindObjectsOfType<AttachPoint>();
                AttachPoint closestAttach = null;
                float closestDistance = Mathf.Infinity;
                foreach(AttachPoint attach in attachs)
                {
                    float distance = Vector2.Distance(attach.transform.position, activePart.transform.position);
                    if(distance < closestDistance && attach.transform.parent != activePart.transform)
                    {
                        closestAttach = attach;
                        closestDistance = distance;
                    }
                }

                //Snap to the PhysicsPart Collider
                if (closestAttach != null)
                {
                    if (closestAttach.GetComponentInParent<PhysicsPart>().CanHaveChildren == false)
                    {
                        print("Placing part");
                        DestroyImmediate(activePart);
                        activePart = null;
                        designerCursor.selectedPart = null;
                        Cursor.visible = true;
                        partPlaced = true;
                    }
                    else if(closestAttach.GetComponentInParent<PhysicsPart>().CanHaveChildren == true)
                    {
                        print("Attaching part");
                        //Move the part to the attach point
                        //Find attach point on part that is closest to the attach point
                        AttachPoint[] attachPoints = activePart.transform.GetComponentsInChildren<AttachPoint>();
                        AttachPoint closestAttachPoint = null;
                        float closestAttachPointDistance = Mathf.Infinity;
                        foreach(AttachPoint attachPoint in attachPoints)
                        {
                            float distance = Vector2.Distance(attachPoint.transform.position, closestAttach.transform.position);
                            if(distance < closestAttachPointDistance && attachPoint.transform.parent != closestAttach.transform.parent.transform)
                            {
                                closestAttachPoint = attachPoint;
                                closestAttachPointDistance = distance;
                            }
                        }

                        //Move the part to the attach point
                        Vector2 offset = closestAttach.transform.position - closestAttachPoint.transform.position;
                        activePart.transform.position = new Vector2(activePart.transform.position.x + offset.x, activePart.transform.position.y + offset.y);


                        activePart.transform.parent = closestAttach.transform.parent.transform;
                        activePart = null;
                        designerCursor.selectedPart = null;
                        Cursor.visible = true;
                    }
                }

            }
            print(originalPart.transform.childCount);
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
        originalPart.transform.parent = null;
        LoadChildren(rocketData.rootPart, originalPart);
    }

    public void LoadChildren(PartData parent, GameObject parentObject)
    {
        foreach(PartData child in parent.children)
        {
            GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + child.partType));
            newPart.transform.parent = parentObject.transform;
            newPart.transform.position = new Vector2(child.x_pos, child.y_pos);
            LoadChildren(child, newPart);
        }
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
            parent.children.Add(newPart);
            AddChildren(newPart, child.gameObject);
        }
    }
}
