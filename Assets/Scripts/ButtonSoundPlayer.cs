using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public AudioSource clickSound;
    public AudioSource hoverSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playClick()
    {
        clickSound.time = 0.12f;
        clickSound.Play();
    }

    public void playHover()
    {
        hoverSound.time = 0.52f;
        hoverSound.Play();
    }
}
