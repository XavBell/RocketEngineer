using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageButtonDrag : MonoBehaviour
{
    Transform parentAfterDrag;    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag()
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag()
    {
        stageContainer[] stageContainers = FindObjectsOfType<stageContainer>();
        float closest = float.MaxValue;
        stageContainer closestContainer = null;
        foreach(stageContainer container in stageContainers)
        {
            float distance = Vector2.Distance(transform.position, container.transform.position);
            if(distance < closest)
            {
                closest = distance;
                closestContainer = container;
            }
        }
        transform.SetParent(closestContainer.container.transform);
    }
}
