using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketAssemblerManager : MonoBehaviour
{
    public DesignerCursor designerCursor;
    public GameObject activePart;
    public GameObject originalPart;
    public bool partPlaced = false;

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
                print("Placing part");
                //Find closest part with PhysicsPart component
                PhysicsPart[] parts = FindObjectsOfType<PhysicsPart>();
                PhysicsPart closestPart = null;
                float closestDistance = Mathf.Infinity;
                foreach(PhysicsPart part in parts)
                {
                    float distance = Vector2.Distance(part.transform.position, activePart.transform.position);
                    if(distance < closestDistance && part.gameObject != activePart)
                    {
                        closestPart = part;
                        closestDistance = distance;
                    }
                }

                //Snap to the PhysicsPart Collider
                if (closestPart != null)
                {
                    if (closestPart.CanHaveChildren == false)
                    {
                        DestroyImmediate(activePart);
                        originalPart = activePart;
                        activePart = null;
                        designerCursor.selectedPart = null;
                        Cursor.visible = true;
                        partPlaced = true;
                    }
                    else
                    {

                        activePart.transform.parent = closestPart.transform;
                        activePart = null;
                        designerCursor.selectedPart = null;
                        Cursor.visible = true;
                    }
                }

            }
            print(originalPart.transform.childCount);
        }
    }
}
