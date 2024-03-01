using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class researchButton : MonoBehaviour
{
    public Node node;
    public List<GameObject> dependencies;
    public GameObject linePrefab = null;
    MasterManager masterManager;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        setVisibility();
        connect();
    }

    // Update is called once per frame
    public void Update()
    {

        
    }

    public void connect()
    {
        foreach(GameObject dependency in dependencies)
        {
            GameObject LinePrefab = Instantiate(linePrefab);
            LinePrefab.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
            LinePrefab.GetComponent<LineRenderer>().SetPosition(1, dependency.transform.position);
        }
    }

    public void setVisibility()
    {
        if(masterManager.nodeUnlocked.Contains(node.NodeName))
        {
            this.GetComponent<Button>().interactable = false;
            this.GetComponent<Image>().color = Color.green;
            return;
        }
        else
        {
            foreach(GameObject dependency in dependencies)
            {
                if(masterManager.nodeUnlocked.Contains(dependency.GetComponent<researchButton>().node.NodeName))
                {
                    this.GetComponent<Button>().interactable = true;
                    return;
                }
                else
                {
                    this.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
}
