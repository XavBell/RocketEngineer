using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DestroyPopUpManager : MonoBehaviour
{
    public GameObject destroyPopUpText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Print(string text)
    {
        destroyPopUpText.SetActive(true);
        destroyPopUpText.GetComponentInChildren<TextMeshProUGUI>().text = text;
        yield return new WaitForSeconds(2);
        destroyPopUpText.SetActive(false);
    }

    public void ShowDestroyPopUp(string text)
    {
        StartCoroutine(Print(text));
    }
}
