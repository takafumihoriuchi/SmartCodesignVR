using System;
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

    [SerializeField] private GameObject inPropButton = null;                   // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropSound = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropFire = null;                     // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropSpeed = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject inPropWeather = null;                  // put tag "DeactivateOnLoad"

    [SerializeField] private GameObject outPropLightUp = null;                 // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropMakeSound = null;               // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropVibrate = null;                 // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropMove = null;                    // put tag "DeactivateOnLoad"
    [SerializeField] private GameObject outPropSend = null;                    // put tag "DeactivateOnLoad"

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

    private GameObject inputSelection;
    private GameObject outputSelection;

    private GameObject envObj;
    private SmartObject smartObj;
    private GameObject inputProp;
    private GameObject outputProp;

    // IO-Instance list: mapping between index and vertical positioning is in descending order
    // Arrow list: mapping between index and vertical positioning is in ascending order
    private List<InputCard> inputCardInstanceList = new List<InputCard>();
    private List<OutputCard> outputCardInstanceList = new List<OutputCard>();
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

        EnvSelectionDetector
            = new CardSelectionDetector(envBoxObj, envObjArr);
        InSelectionDetector
            = new CardSelectionDetector(inBoxObj, inObjArr);
        OutSelectionDetector
            = new CardSelectionDetector(outBoxObj, outObjArr);
    }


    void Update()
    {
        EnvSelectionDetector.CenterDragMotion();
        InSelectionDetector.CenterDragMotion();
        OutSelectionDetector.CenterDragMotion();

        // triggered once when box content is changed
        if (EnvSelectionDetector.TriggerFlag)
        {
            GameObject envSelection = EnvSelectionDetector.SelectedCardObj;
            if (envSelection == null)
            {
                Destroy(envObj);
            }
            else
            {
                envObj = InstantiateEnvObjByName(envSelection.name);
                envObj.SetActive(true);
                smartObj = envObj.AddComponent<SmartObject>();
            }
        }


        if (InSelectionDetector.TriggerFlag)
        {
            inputSelection = InSelectionDetector.SelectedCardObj;
            if (inputSelection != null)
            {
                inputProp = InstantiateInputPropByName(inputSelection.name);
                inputProp.SetActive(true);
                AddInputCardInstanceToList();
            }
            else
            {
                Destroy(inputProp);
                // todo ここにinputCardInstanceList を消滅させる処理を挿入
            }
        }

        //if (OutSelectionDetector.TriggerFlag)
        //{
        //    GameObject outSelected = OutSelectionDetector.SelectedCardObj;
        //    if (outSelected == null)
        //    {
        //        outputProps = null;
        //    }
        //    else
        //    {
        //        outputProps = GetOutPropsByName(outSelected.name);
        //    }
        //}
        // todo outputのstatementFieldGroupに関しては、outputSelectionBoxに何かが後から
        // 入れられた時に、その時点でinputStatementFieldGroupが複数 設定されていたら、
        // 一気にその数だけ出現するようにする必要がある
        // また、初めにoutputがboxに入れられた場合には、statementFieldをひとつだけ出現させる

    }

    // todo addInstanceButton OnClick での処理を考える

    private void AddInputCardInstanceToList()
    {
        inputCardInstanceList.Add(GetInputCardInstanceByName(inputSelection.name));
        int idx = inputCardInstanceList.Count - 1; // get tail of updated list; call after adding new instance
        int minInstanceID = AvailableMinInstanceID(inputCardInstanceList);
        inputCardInstanceList[idx].CardStatementSetup(
            minInstanceID, ref smartObj, ref inputProp, ref inputStatementFieldGroup);
        ShiftFocusToTargetInstance(inputCardInstanceList, idx);
        ShiftStatementFieldPositions(inputCardInstanceList);
        inputCardInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(StatementFieldOnClick);

        // check for allawability of adding and removing instances
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputCardInstanceList.Count <= 1);

        ioArrowList.Add(CreateIOArrow());
        ShiftFocusToTargetIOArrow(idx);
    }


    //private void AddCardInstanceToList()
    //{
    //    if (inputSelection != null)
    //    {
    //        inputCardInstanceList.Add(GetInputCardInstanceByName(inputSelection.name));
    //    }
    //    // 両方に追加する場合

    //    // inputの方だけに追加する場合
    //}


    private void StatementFieldOnClick()
    {
        int instanceIdx = GetCurrentSelectedCardInstanceIndex();
        if (inputCardInstanceList.Count > 0)
            ShiftFocusToTargetInstance(inputCardInstanceList, instanceIdx);
        if (outputCardInstanceList.Count > 0)
            ShiftFocusToTargetInstance(outputCardInstanceList, instanceIdx);
        if (ioArrowList.Count > 0)
            ShiftFocusToTargetIOArrow(instanceIdx);
    }






    /// <summary>
    /// helper methods
    /// </summary>

    private bool CheckInstanceListCapacity()
    {
        return inputCardInstanceList.Count < inputCardInstanceList[0].MaxInstanceNum;
    }

    private int GetCurrentSelectedCardInstanceIndex()
    {
        // get "instance-index" from "statement-field-button-label"
        string statementIdText = EventSystem.current.currentSelectedGameObject.
            transform.Find("IndexText").gameObject.GetComponent<TMP_Text>().text; // todo modify to method without "find()"
        string extrudedIdText = Regex.Replace(statementIdText, @"[^0-9]", "");
        int extrudedId = int.Parse(extrudedIdText);
        int instanceID = extrudedId - 1;
        int instanceIdx = GetInstanceListIndexFromInstanceID(instanceID);
        return instanceIdx;
    }

    // returns index in Instance List 
    // returns -1 when not found in Instance List
    private int GetInstanceListIndexFromInstanceID(int id)
    {
        int cnt = inputCardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (inputCardInstanceList[i].InstanceID == id) return i;
        }
        return -1;
    }

    private void ShiftFocusToTargetInstance<Type>(List<Type> cardInstanceList, int idx)
        where Type : Card
    {
        int cnt = cardInstanceList.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (i == idx)
            {
                cardInstanceList[idx].IsFocused = true;
                cardInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
            }
            else
            {
                cardInstanceList[idx].IsFocused = false;
                cardInstanceList[idx].StatementFieldGroup.GetComponent<Image>().color = LIGHT_BEIGE;
            }
        }
    }

    private void ShiftStatementFieldPositions<Type>(List<Type> cardInstanceList)
        where Type : Card
    {
        int cnt = cardInstanceList.Count;
        int tail = cnt - 1;
        Vector3 basePosition = cardInstanceList[tail].StatementFieldGroup.transform.localPosition;
        for (int i = 0; i < cnt; i++)
        {
            Vector3 verticalShiftAmount = new Vector3(0, VSHAMT * (cnt - 1 - i), 0);
            cardInstanceList[i].StatementFieldGroup.transform.localPosition = basePosition + verticalShiftAmount;
        }
    }

    // parameter int:idx is passed assuming the use with IOInstance index order
    // set optional parameter to 'true' if calling with real index
    // todo make the IOArrow order match with IOStatements
    private void ShiftFocusToTargetIOArrow(int idx, bool realIndex = false)
    {
        if (!realIndex)
            idx = RevIdx(idx);
        for (int i = 0; i < ioArrowList.Count; i++)
        {
            if (i == idx) ioArrowList[i].GetComponent<Image>().color = WHITE;
            else ioArrowList[i].GetComponent<Image>().color = LIGHT_WHITE;
        }
    }

    private int RevIdx(int idx)
    {
        return ioArrowList.Count - 1 - idx;
    }


    private GameObject InstantiateEnvObjByName(string cardName)
    {
        switch (cardName)
        {
            case "TrashBin": return Instantiate(envObjTrashBin);
            case "Tree": return Instantiate(envObjTree);
            case "StreetLight": return Instantiate(envObjStreetLight);
            case "StreetSign": return Instantiate(envObjStreetSign);
            case "Bridge": return Instantiate(envObjBridge);
            default: return null;
        }
    }


    private GameObject InstantiateInputPropByName(string cardName)
    {
        switch (cardName)
        {
            case "Button": return Instantiate(inPropButton);
            case "Sound": return Instantiate(inPropSound);
            case "Fire": return Instantiate(inPropFire);
            case "Speed": return Instantiate(inPropSpeed);
            case "Weather": return Instantiate(inPropWeather);
            default: return null;
        }
    }

    private InputCard GetInputCardInstanceByName(string cardName)
    {
        switch (cardName)
        {
            case "Button": return new ButtonCard();
            case "Sound": return new SoundCard();
            case "Fire": return new FireCard();
            case "Speed": return new SpeedCard();
            case "Weather": return new WeatherCard();
            default: return null;
        }
    }


    //private GameObject GetOutPropsByName(string cardName)
    //{
    //    switch (cardName)
    //    {
    //        case "LightUp": return outPropsLightUp;
    //        case "MakeSound": return outPropsMakeSound;
    //        case "Vibrate": return outPropsVibrate;
    //        case "Move": return outPropsMove;
    //        case "Send": return outPropsSend;
    //        default: return null;
    //    }
    //}

    //private OutputCard GetOutputInstanceByName(string cardName)
    //{
    //    switch (cardName)
    //    {
    //        case "LightUp": return new LightUpCard();
    //        case "MakeSound": return new MakeSoundCard();
    //        case "Vibrate": return new VibrateCard();
    //        case "Move": return new MoveCard();
    //        case "Send": return new SendCard();
    //        default: return null;
    //    }
    //}

    // returns position adjusted io-arrow gameObject
    private GameObject CreateIOArrow()
    {
        GameObject newArrow = Instantiate(ioArrowObject, ioArrowObject.transform.parent);
        newArrow.transform.localPosition += new Vector3(0, VSHAMT * (ioArrowList.Count), 0);
        newArrow.SetActive(true);
        return newArrow;
    }

    private int AvailableMinInstanceID<Type>(List<Type> cardInstanceList)
        where Type : Card
    {
        int cnt = cardInstanceList.Count;
        int idCandidate = 0;
        for (int i = 0; i < cnt; i++)
        {
            if (cardInstanceList[i].InstanceID == -1)
                continue; // skip itself (set to initial value)
            if (cardInstanceList[i].InstanceID == idCandidate)
                idCandidate++;
        }
        return idCandidate;
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