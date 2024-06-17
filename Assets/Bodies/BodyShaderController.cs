using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//rtificr's amazing awesome beautiful impeccable planet script
[ExecuteInEditMode]
public class BodyShaderController : MonoBehaviour
{
    [SerializeField] Transform lightPos;
    Material mat;
    Material atmoMat;
    [SerializeField] Vector3 scaledLightPos;
    [SerializeField] Vector3 roundedLightPos;

    [Header("Planet Settings")]
    [SerializeField] Texture2D GroundTex;
    [Range(0f, 1f)][SerializeField] float AmbientLight = 0.2f;
    [Range(0f, 1f)][SerializeField] float MaximumLight = 1f;

    [Header("Atmosphere Settings")]
    [SerializeField] Texture2D gradient;
    [SerializeField] float size;
    [SerializeField] Vector2 cutoff = new Vector2(0.3f, 1f);
    [Range(0f, 5f)][SerializeField] float falloffPower = 2;
    [Range(0f, 1f)][SerializeField] float densityDesaturation = 0.25f;

    GameObject child;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -transform.localScale.x/50);

        lightPos = GameObject.Find("Sun").GetComponent<Transform>();
        
        mat = GetComponent<Renderer>().sharedMaterial;
        child = transform.GetChild(0).gameObject;

        UpdateMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        scaledLightPos = RoundVector3(lightPos.position / 100000, 4); 
        roundedLightPos = RoundVector3(scaledLightPos, 4);

        mat.SetVector("_LightPos", -roundedLightPos);

        if (child != null && atmoMat != null)
            atmoMat.SetVector("_LightPos", -roundedLightPos);
        else Debug.Log("Atmosphere material not found. Ignoring...");

        
    }

    Vector3 RoundVector3(Vector3 v, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        v.x = Mathf.Round(v.x * multiplier) / multiplier;
        v.y = Mathf.Round(v.y * multiplier) / multiplier;
        v.z = Mathf.Round(v.z * multiplier) / multiplier;
        return v;
    }

    void UpdateMaterials()
    {
        if (mat != null)
        {
            mat.SetTexture("_Texture", GroundTex);
            mat.SetVector("_MinMaxLight", new Vector2(AmbientLight, MaximumLight));
        }

        if (child != null)
        {
            atmoMat = child.GetComponent<Renderer>().sharedMaterial;
            float atmoSize = 0;
            if(GetComponent<EarthScript>() != null)
            {
                atmoSize = (float)(GetComponent<EarthScript>().SolarSystemManager.earthRadius * 2 * 1.5 * 0.1);
            }
            
            child.transform.localScale = new Vector3(atmoSize, atmoSize, atmoSize);

            if (atmoMat != null)
            {
                atmoMat.SetTexture("_Gradient", gradient);
                atmoMat.SetFloat("_Size", size);
                atmoMat.SetVector("_Atmosphere_Cutoff", cutoff);
                atmoMat.SetFloat("_Falloff_Power", falloffPower);
                atmoMat.SetFloat("_Density_Desaturation", densityDesaturation);
            }
            else Debug.Log("Atmosphere material not found. Ignoring...");
        }
        else Debug.Log("Atmosphere not found. Ignoring...");
    }

    private void OnValidate()
    {
        UpdateMaterials();
    }
}
