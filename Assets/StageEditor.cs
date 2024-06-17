using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEditor : MonoBehaviour
{
    public GameObject engineBtn;
    public GameObject verticalBox;
    public int previousChildCount = 0;
    public RocketController rocketController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public void clearButtons()
    {
        foreach (Transform child in verticalBox.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void UpdateButtons()
    {
        clearButtons();
        print("Updating buttons");
        Transform[] transforms = rocketController.gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform part in transforms)
        {
            if(part.GetComponent<EngineComponent>())
            {
                print("Updated");
                GameObject EngineButton = Instantiate(engineBtn, verticalBox.transform);
            }
        }
    }

}
