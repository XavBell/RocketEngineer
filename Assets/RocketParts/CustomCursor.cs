using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomCursor : MonoBehaviour
{
    private SpriteRenderer sp;
    private BoxCollider2D box;
    public GameObject earth;
    public bool constructionAllowed = true;
    void Start()
    {
        sp = this.GetComponent<SpriteRenderer>();
        box = this.GetComponent<BoxCollider2D>();
        sp.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;

        if(sp != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            Vector2 position = this.transform.position;
            Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
            float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            position = (v.normalized*-(50f + sp.size.y/2));
            this.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
            this.transform.position = position;

        }

        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(box != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            if(other.tag == "building")
            {
                sp.color = Color.red;
                constructionAllowed = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(box != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            if(other.tag == "building")
            {
                sp.color = Color.green;
                constructionAllowed = true;
            }
        }
    }
}
