extends Node

export var nozzleExitSize: Resource
export var nozzleEndSize: Resource
export var turbopumpSize: Resource
export var nozzleLenght: Resource
export var massFlowRate: Resource
export var thrustInput: Resource
export var savePath: Resource
export var saveName: String
export var Engine: NodePath
export var nozzleExitRef: NodePath
export var nozzleEndRef: NodePath
export var turbopumpRef: NodePath
export var attachBottomObj: NodePath
export var nozzleExitSizeFloat: float
export var nozzleEndSizeFloat: float
export var turbopumpSizeFloat: float
export var nozzleLenghtFloat: float
export var mass: float
export var thrust: float
export var rate: float
export var currentE: float
export var currentEn: float
export var currentT: float
export var currentEy: float
export var startingScaleE: Vector3
export var startingScaleEn: Vector3
export var startingScaleT: Vector3
export var startingScaleEy: Vector3
export var initialScaleY: float
export var panel: NodePath
export var popUpPart: NodePath
export var thrustT: Resource
export var massT: Resource
export var rateT: Resource
export var MasterManager: Resource

# using System.Collections;
# using System.Collections.Generic;
# using UnityEngine;
# using UnityEngine.UI;
# using UnityEngine.SceneManagement;
# using TMPro;
# using System.Runtime.Serialization.Formatters.Binary;
# using System.IO;
# using System;
# using System.Runtime.Serialization;
# using System.Xml.Linq;
# using System.Text;
# using Newtonsoft.Json;
# 
# public class GameManager_Engine : MonoBehaviour
# {
#     //Visual
#     public TMP_InputField nozzleExitSize;
#     public TMP_InputField nozzleEndSize;
#     public TMP_InputField turbopumpSize;
#     public TMP_InputField nozzleLenght;
# 
#     //Specs
#     public TMP_InputField massFlowRate;
#     public TMP_InputField thrustInput;
# 
# 
# 
#     public TMP_InputField savePath;
#     public string saveName;
# 
#     public GameObject Engine;
#     public GameObject nozzleExitRef;
#     public GameObject nozzleEndRef;
#     public GameObject turbopumpRef;
# 
#     public GameObject attachBottomObj;
# 
#     //Visual float
#     public float nozzleExitSizeFloat;
#     public float nozzleEndSizeFloat;
#     public float turbopumpSizeFloat;
#     public float nozzleLenghtFloat;
# 
#     
#     //Specs Float
#     public float mass;
#     public float thrust;
#     public float rate;
# 
#     public float currentE = 0;
#     public float currentEn = 0;
#     public float currentT = 0;
#     public float currentEy = 0;
# 
#     public Vector3 startingScaleE;
#     public Vector3 startingScaleEn;
#     public Vector3 startingScaleT;
#     public Vector3 startingScaleEy;
# 
#     public float initialScaleY;
# 
#     public savePath savePathRef = new savePath();
#     public GameObject panel;
#     public GameObject popUpPart;
# 
#     //TEMPORARY!!!
#     public TextMeshProUGUI thrustT;
#     public TextMeshProUGUI massT;
#     public TextMeshProUGUI rateT;
# 
#     public MasterManager MasterManager = new MasterManager();
#     // Start is called before the first frame update
#     void Start()
#     {
#         if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
#         {
#             nozzleExitRef = Engine.GetComponent<Engine>()._nozzleEnd;
#             nozzleEndRef = Engine.GetComponent<Engine>()._nozzleStart;
#             turbopumpRef = Engine.GetComponent<Engine>()._turbopump;
# 
#             startingScaleE = nozzleExitRef.transform.localScale;
#             startingScaleEy = nozzleExitRef.transform.localScale;
#             initialScaleY = nozzleExitRef.transform.localScale.y;
#             startingScaleEn = nozzleEndRef.transform.localScale;
#             startingScaleT = turbopumpRef.transform.localScale;
# 
#             attachBottomObj = Engine.GetComponent<Engine>()._attachBottom;
# 
#             GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
#             MasterManager = GMM.GetComponent<MasterManager>();
#         }
#         
#     }
# 
#     // Update is called once per frame
#     void Update()
#     {
# 
#         if (SceneManager.GetActiveScene().name.ToString() == "EngineDesign")
#         {
#             updateSize();
#             updateAttachPosition();
#         }
# 
#     }
# 
#     void updateSize()
#     {
#         float number;
#         if(float.TryParse(nozzleExitSize.text, out number))
#         {
#             nozzleExitSizeFloat = float.Parse(nozzleExitSize.text);
# 
#             if(nozzleExitRef.transform.localScale.x == nozzleExitSizeFloat)
#             {
#                 startingScaleE = nozzleExitRef.transform.localScale;
#                 currentE = 0;
#             }
# 
#             if(nozzleExitRef.transform.localScale.x != nozzleExitSizeFloat)
#             { 
#                 nozzleExitRef.transform.localScale = Vector3.Lerp(startingScaleE, new Vector3(nozzleExitSizeFloat, nozzleExitRef.transform.localScale.y, 0), currentE*5);
#                 currentE += Time.deltaTime;
#             }
#         }
# 
#         if (float.TryParse(nozzleEndSize.text, out number))
#         {
#             nozzleEndSizeFloat = float.Parse(nozzleEndSize.text);
# 
#             if(nozzleEndRef.transform.localScale.x == nozzleEndSizeFloat)
#             {
#                 startingScaleEn = nozzleEndRef.transform.localScale;
#                 currentEn = 0;
#             }
# 
#             if(nozzleEndRef.transform.localScale.x != nozzleEndSizeFloat)
#             {
#                 nozzleEndRef.transform.localScale = Vector3.Lerp(startingScaleEn, new Vector3(nozzleEndSizeFloat, nozzleEndRef.transform.localScale.y, 0), currentEn*5);
#                 currentEn += Time.deltaTime;
#             }
#         }
# 
#         if (float.TryParse(turbopumpSize.text, out number))
#         {
#             turbopumpSizeFloat = float.Parse(turbopumpSize.text);
# 
#             if(turbopumpRef.transform.localScale.x == turbopumpSizeFloat)
#             {
#                 startingScaleT = turbopumpRef.transform.localScale;
#                 currentT = 0;
#             }
# 
#             if(turbopumpRef.transform.localScale.x != turbopumpSizeFloat)
#             {
#                 turbopumpRef.transform.localScale = Vector3.Lerp(startingScaleT, new Vector3(turbopumpSizeFloat, turbopumpRef.transform.localScale.y, 0), currentT*5);
#                 currentT += Time.deltaTime;
#             }
#         }
# 
# 
#         if (float.TryParse(nozzleLenght.text, out number))
#         {
#             nozzleLenghtFloat = float.Parse(nozzleLenght.text);
# 
#             if(nozzleExitRef.transform.localScale.y == nozzleLenghtFloat)
#             {
#                 startingScaleEy = nozzleExitRef.transform.localScale;
#                 currentEy = 0;
#             }
# 
#             if(nozzleExitRef.transform.localScale.y != nozzleLenghtFloat)
#             { 
#                 nozzleExitRef.transform.localScale = Vector3.Lerp(startingScaleEy, new Vector3(nozzleExitRef.transform.localScale.x, nozzleLenghtFloat, 0), currentEy*5);
#                 currentEy += Time.deltaTime;
#                 float changeY = initialScaleY - nozzleExitRef.transform.localScale.y;
#                 nozzleExitRef.transform.position += new Vector3(0, changeY/2, 0);
#                 initialScaleY = nozzleExitRef.transform.localScale.y;  
#             }
# 
# 
#         }
# 
#         if (float.TryParse(massFlowRate.text, out number))
#         {
#             rate = float.Parse(massFlowRate.text);
#         }
# 
#         if (float.TryParse(thrustInput.text, out number))
#         {
#             thrust = float.Parse(thrustInput.text);
#         }
# 
#         mass = 1600f; //Assuming Raptor Engine
#     }
# 
# 
#     public void updateAttachPosition()
#     {
#         attachBottomObj.transform.position = (new Vector2(attachBottomObj.transform.position.x, nozzleExitRef.GetComponent<BoxCollider2D>().bounds.min.y));
#     }
# 
# 
#     public void save()
#     {
#         if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder))
#         {
#             Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder);
#         }
#         
#         saveName = "/"+ savePath.text;
# 
#         if(!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json"))
#         {
#             saveEngine saveObject = new saveEngine();
#             List<float> sizes = new List<float>();
# 
#             saveObject.path = savePathRef.engineFolder;
#             saveObject.engineName = saveName;
#             saveObject.nozzleExitSize_s = nozzleExitRef.GetComponent<SpriteRenderer>().transform.localScale.x;
#             sizes.Add(saveObject.nozzleExitSize_s);
#             saveObject.nozzleEndSize_s = nozzleEndRef.GetComponent<SpriteRenderer>().transform.localScale.x;
#             sizes.Add(saveObject.nozzleEndSize_s);
#             saveObject.turbopumpSize_s = turbopumpRef.GetComponent<SpriteRenderer>().transform.localScale.x;
#             sizes.Add(saveObject.turbopumpSize_s);
# 
#             float bestSize = 0;
#             foreach(float size in sizes)
#             {
#                 if(size > bestSize)
#                 {
#                     bestSize = size;
#                 }
#             }
# 
#             saveObject.verticalSize_s = nozzleExitRef.GetComponent<BoxCollider2D>().transform.localScale.y;
#             saveObject.attachBottomPos = attachBottomObj.transform.localPosition.y;
#             saveObject.verticalPos = nozzleExitRef.transform.localPosition.y;
#             saveObject.horizontalBestSize_s = bestSize;
#             saveObject.thrust_s = mass;
#             saveObject.thrust_s = thrust;
#             saveObject.rate_s = rate;
# 
#             var jsonString = JsonConvert.SerializeObject(saveObject);
#             System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json", jsonString);
#             Debug.Log("saved");
# 
#         }else if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json"))
#         {
#             saveEngine saveEngine = new saveEngine();
#             var jsonString2 = JsonConvert.SerializeObject(saveEngine);
#             jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json");
#             saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString2);
# 
#             if(loadedEngine.usedNum == 0)
#             {
#                 File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder + saveName + ".json");
#                 save();
#                 return;
#             }
# 
#             int x = Screen.width / 2;
#             int y = Screen.height / 2;
#             Vector2 position = new Vector2(x, y);
#             Instantiate(popUpPart, position, Quaternion.identity);
#             panel.SetActive(false);
#         }
#     }
# 
#     public void backToBuild()
#     {
#         SceneManager.LoadScene("SampleScene");
#     }
# 
# }
