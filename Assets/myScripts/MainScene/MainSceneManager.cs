using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainSceneManager : MonoBehaviour
{
    /// <summary>
    /// card selection related fields
    /// </summary>

    CardSelectionDetector EnvSelectionDetector;
    [SerializeField] private GameObject envBoxObj = null;
    [SerializeField] private GameObject[] envObjArr = null;

    CardSelectionDetector InSelectionDetector;
    [SerializeField] private GameObject inBoxObj = null;
    [SerializeField] private GameObject[] inObjArr = null;

    CardSelectionDetector OutSelectionDetector;
    [SerializeField] private GameObject outBoxObj = null;
    [SerializeField] private GameObject[] outObjArr = null;

    /// <summary>
    /// prototyping related fields
    /// </summary>

    [SerializeField] private GameObject envObjTrashBin = null;                  // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjTree = null;                      // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjStreetLight = null;               // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjStreetSign = null;                // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject envObjBridge = null;                    // put tag "DeactivateOnLoad"

    [SerializeField] private GameObject inPropsButton = null;                   // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsSound = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsFire = null;                     // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsSpeed = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropsWeather = null;                  // put tag "DeactivateOnLoad"

    [SerializeField] private GameObject outPropsLightUp = null;                 // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsMakeSound = null;               // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsVibrate = null;                 // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsMove = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropsSend = null;                    // put tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI inputCardNameField = null;
    [SerializeField] private TextMeshProUGUI inputDescriptionField = null;
    [SerializeField] private GameObject inputStatementFieldGroup = null;        // put tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI outputCardNameField = null;
    [SerializeField] private TextMeshProUGUI outputDescriptionField = null;
    [SerializeField] private GameObject outputStatementFieldGroup = null;       // put tag "DeactivateOnLoad"

    [SerializeField] private Button addInstanceButton = null;
    [SerializeField] private Button removeInstanceButton = null;

    [SerializeField] private GameObject ioArrowObject = null;                   // put tag "DeactivateOnLoad"

    [SerializeField] private TextMeshProUGUI confirmationMessageField = null;
    [SerializeField] private Button confirmationButton = null;
    [SerializeField] private Button backToEditButton = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private Button finalizationButton = null;                  // put tag "DeactivateOnLoad"
    readonly string beforeConfirmMessage = "Ready to test the Smart Object?";
    readonly string afterConfirmMessage = "Do you want to keep editing or finalize?";
    // todo change behavior after preview mode ("confirm")

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;

    // IO-Instance list: mapping between index and vertical positioning is in descending order
    // Arrow list: mapping between index and vertical positioning is in ascending order
    private List<InputCard> inputInstanceList = new List<InputCard>();
    private List<OutputCard> outputInstanceList = new List<OutputCard>();
    private List<GameObject> ioArrowList = new List<GameObject>();

    bool previewMode = false;

    const float VSHAMT = 0.5f;                                                  // vertical shift amount of statement fields
    readonly Color BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 1.0f);          // when statement-box is focused
    readonly Color LIGHT_BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 0.6f);    // when statement-box is unfocused
    readonly Color POP_BEIGE = new Color(1.0f, 0.9210f, 0.7405f, 1.0f);         // when statement-box is in effect
    readonly Color WHITE = new Color(1.0f, 1.0f, 1.0f, 1.0f);                   // when arrow is focused
    readonly Color LIGHT_WHITE = new Color(1.0f, 1.0f, 1.0f, 0.4f);             // when arrow is unfocused

    [SerializeField] private GameObject editorEventSystem = null;               // for developmental use only
    [SerializeField] private GameObject HMDEventSystem = null;                  // for developmental use only

    private AudioSource[] clickSound;

    /// <summary>
    /// member methods
    /// </summary>

    void Start()
    {
        DevConfigSetting();
        SetActiveStateByTag("DeactivateOnLoad", false);
        SetActiveStateByTag("ActivateOnLoad", true);

        // todo continue refactoring from here
        // change to "dynamically reading what was selected"
        /*
        environmentObject = GetEnvObjByName(SmartObject.cardSelectionDict["environment"]);
        inputProps = GetInPropsByName(SmartObject.cardSelectionDict["input"]);
        outputProps = GetOutPropsByName(SmartObject.cardSelectionDict["output"]);
        */


        EnvSelectionDetector
            = new CardSelectionDetector(envBoxObj, envObjArr);
        InSelectionDetector
            = new CardSelectionDetector(inBoxObj, inObjArr);
        OutSelectionDetector
            = new CardSelectionDetector(outBoxObj, outObjArr);
    }


    void Update()
    {
        if (EnvSelectionDetector.TriggerFlag)
        {
            // triggered once when box content is changed
            // use "EnvSelectionDetector.SelectedCardObj" to get selected card obj
        }
        if (InSelectionDetector.TriggerFlag)
        {

        }
        if (OutSelectionDetector.TriggerFlag)
        {

        }

        EnvSelectionDetector.CenterGravityMotion();
        InSelectionDetector.CenterGravityMotion();
        OutSelectionDetector.CenterGravityMotion();
    }



    private void SetActiveStateByTag(string tag, bool state)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in taggedObjects) obj.SetActive(state);
    }

    // decide between button-clicks (editor use) or laser-pointer (HMD use)
    private void DevConfigSetting()
    {
        bool isQuest2 =
            OVRManager.systemHeadsetType
            == OVRManager.SystemHeadsetType.Oculus_Quest_2;
        HMDEventSystem.SetActive(isQuest2);
        editorEventSystem.SetActive(!isQuest2);
        GameObject[] objectsWithRaycaster = GameObject.FindGameObjectsWithTag("ObjectWithRaycaster");
        foreach (GameObject obj in objectsWithRaycaster)
        {
            obj.GetComponent<OVRRaycaster>().enabled = isQuest2;
            obj.GetComponent<GraphicRaycaster>().enabled = !isQuest2;
        }
    }

}

/*

まずは、選択された側から即座に対象がPrototypingエリアに反映されるようにする
その選択が箱から解除された時には、その設定だけがリセットされるようにする
 
 */