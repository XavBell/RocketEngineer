using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class stageContainer : MonoBehaviour
{
    public StageEditor stageEditor;
    public GameObject container;
    // Start is called before the first frame update
    void Start()
    {
        stageEditor = FindObjectOfType<StageEditor>();
    }

    // Update is called once per frame
    void Update()
    {
        updateStageName();   
    }

    void updateStageName()
    {
        this.GetComponentInChildren<TMP_Text>().text = "Stage " + this.transform.GetSiblingIndex().ToString();
    }

    public void removeStage()
    {
        stageEditor.stageContainers.Remove(this);
        stageEditor.UpdateButtons();
        Destroy(this.gameObject);
    }

    public void addStage()
    {
        GameObject StageContainer = Instantiate(stageEditor.stageContainer, stageEditor.container.transform);
        stageEditor.stageContainers.Add(StageContainer.GetComponent<stageContainer>());
    }
}
