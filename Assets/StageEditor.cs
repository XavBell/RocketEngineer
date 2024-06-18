using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditor : MonoBehaviour
{
    public GameObject engineBtn;
    public GameObject decouplerBtn;
    public GameObject stageContainer;
    public List<GameObject> buttons;
    public List<stageContainer> stageContainers;
    public GameObject container;
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
        foreach(GameObject button in buttons)
        {
            if(button != null)
            {
                Destroy(button);
            }
        }

        buttons.Clear();
    }

    public void UpdateButtons()
    {
        clearButtons();
        if(stageContainers.Count == 0)
        {
            GameObject StageContainer = Instantiate(stageContainer, container.transform);
            stageContainers.Add(StageContainer.GetComponent<stageContainer>());
        }

        Transform[] transforms = rocketController.gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform part in transforms)
        {
            if(part.GetComponent<EngineComponent>())
            {
                GameObject EngineButton = Instantiate(engineBtn);
                buttons.Add(EngineButton.GetComponentInChildren<Button>().gameObject);
                EngineButton.GetComponentInChildren<Button>().gameObject.transform.SetParent(stageContainers[stageContainers.Count - 1].container.gameObject.transform);
                DestroyImmediate(EngineButton);
            }

            if(part.GetComponent<DecouplerComponent>())
            {
                GameObject DecouplerButton = Instantiate(decouplerBtn);
                buttons.Add(DecouplerButton.GetComponentInChildren<Button>().gameObject);
                DecouplerButton.GetComponentInChildren<Button>().gameObject.transform.SetParent(stageContainers[stageContainers.Count - 1].container.gameObject.transform);
                DestroyImmediate(DecouplerButton);
            }
        }
    }

}
