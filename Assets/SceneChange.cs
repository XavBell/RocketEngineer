using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public GameObject capsule;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        capsule = GameObject.FindGameObjectWithTag("capsule");
    }

    public void ChangeScene()
    {
        Destroy(capsule);
        SceneManager.LoadScene("Building");
        
    }
}
