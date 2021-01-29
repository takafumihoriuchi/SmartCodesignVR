using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CardSelectionMediator
{
    // parameters are set in CardSelectionScene
    public static Dictionary<string, string> selectionDict
        = new Dictionary<string, string>() {
        {"environment", null},
        {"input", null},
        {"output", null}
    };
}


public class PrototypingSceneCore : MonoBehaviour
{
    // Environment Objects

    [SerializeField] private GameObject envObjTrashBin = null;
    [SerializeField] private GameObject envObjTree = null;
    [SerializeField] private GameObject envObjStreetLight = null;
    [SerializeField] private GameObject envObjStreetSign = null;
    [SerializeField] private GameObject envObjBridge = null;

    [SerializeField] private GameObject inPropsButton = null;
    [SerializeField] private GameObject inPropsSound = null;
    [SerializeField] private GameObject inPropsFire = null;
    [SerializeField] private GameObject inPropsSpeed = null;
    [SerializeField] private GameObject inPropsWeather = null;

    [SerializeField] private GameObject outPropsLightUp = null;
    [SerializeField] private GameObject outPropsMakeSound = null;
    [SerializeField] private GameObject outPropsVibrate = null;
    [SerializeField] private GameObject outPropsMove = null;
    [SerializeField] private GameObject outPropsSend = null;


    // IO Canvas Fileds

    [SerializeField] private GameObject inputCardNameField = null;
    [SerializeField] private GameObject outputCardNameField = null;
    [SerializeField] private GameObject inputDescriptionField = null;
    [SerializeField] private GameObject outputDescriptionField = null;

    [SerializeField] private GameObject inputStatementFieldGroup = null; // type:Button
    [SerializeField] private GameObject outputStatementFieldGroup = null; // type:Button

    [SerializeField] private Button addInstanceButton = null;
    [SerializeField] private Button removeInstanceButton = null;


    // Confirmation Canvas Fields

    [SerializeField] private GameObject confirmationMessageField = null; // todo assign in inspector
    // todo こういうtext-fieldは、ここで直接 TextMeshProUGUI として読み込んだ方が良いのでは?
    [SerializeField] private Button confirmationButton = null;
    [SerializeField] private Button backToEditButton = null; // todo make new and put instance in inspector
    [SerializeField] private Button finalizationButton = null;
    // todo make new and add "finalize smart object" button in inspector
    bool confirmed = false; // after-confirmed か before-confirmed かを知るため（back-to-edit の操作で可逆性あり）


    [SerializeField] private GameObject menuCanvas = null;
    [SerializeField] private Button backToSceneButton = null;
    [SerializeField] private Button closeMenuButton = null;
    private bool menuIsOpened = false;

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;
    private List<InputCard> inputInstanceList = new List<InputCard>();
    private List<OutputCard> outputInstanceList = new List<OutputCard>();


    // for developmental use only
    private void DevelopmentPurposeAssign()
    {
        CardSelectionMediator.selectionDict["environment"] = "TrashBin";
        CardSelectionMediator.selectionDict["input"] = "Fire";
        CardSelectionMediator.selectionDict["output"] = "LightUp";

        Debug.Log("[env, in, out] = ["
            + CardSelectionMediator.selectionDict["environment"] + ", "
            + CardSelectionMediator.selectionDict["input"] + ", "
            + CardSelectionMediator.selectionDict["output"] + "]");
    }


    private void Start()
    {
        DevelopmentPurposeAssign(); // make sure to delete after development

        DeactivateAllCardRepresentations();

        environmentObject = GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]);

        int idx = 0;

        inputProps = GetInPropsByName(CardSelectionMediator.selectionDict["input"]);
        inputInstanceList.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        inputInstanceList[idx].CardDescriptionSetup(
            ref inputCardNameField, ref inputDescriptionField); // todo 参照型だから多分大丈夫だと思うけど、最初に生成したものが後に削除された場合、これらの要素が残るかを確認する
        inputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref inputProps, inputStatementFieldGroup, AvailableMinInstanceID());
        // todo "inputStatementFieldGroup"は値渡しになっている？参照になっているような気がしている

        outputProps = GetOutPropsByName(CardSelectionMediator.selectionDict["output"]);
        outputInstanceList.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        outputInstanceList[idx].CardDescriptionSetup(
            ref outputCardNameField, ref outputDescriptionField);
        outputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref outputProps, outputStatementFieldGroup, AvailableMinInstanceID());

        GrantFocusToInstances(idx);

        // UI button settings

        // TODO ボタンとしてのstatementBoxの追加と、それが押下された時にIsFocusedがtrueになる処理
        // statementboxはすでにある。buttonコンポーネントにアクセスする
        // todo IsFocusedなものとそうでないものとで色の濃淡を変化させる
        // todo on click でfocusが移り変わる機能の実装

        addInstanceButton.onClick.AddListener(AddInstanceToList);
        removeInstanceButton.onClick.AddListener(RemoveInstanceFromList);
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = false;

        confirmationButton.onClick.AddListener(ConfirmSmartObject);
        confirmationButton.interactable = false; // set to true when every-instances.canBeConfirmed is true
        backToEditButton.onClick.AddListener(GoBackToEditMode);
        backToEditButton.interactable = false;
        finalizationButton.onClick.AddListener(FinalizeSmartObject);
        finalizationButton.interactable = false;

        backToSceneButton.onClick.AddListener(LoadCardSelectionScene);
        closeMenuButton.onClick.AddListener(CloseMenu);

    }

    private bool CheckInstanceListCapacity()
    {
        return (inputInstanceList.Count < inputInstanceList[0].MaxInstanceNum);
    }

    // todo isFocused; 生まれた時には注目されている。生まれたてのものに注目が集まる。他は関心が薄れる。
    private void AddInstanceToList()
    {
        inputInstanceList.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        outputInstanceList.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        int idx = inputInstanceList.Count - 1;
        inputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref inputProps, inputStatementFieldGroup, AvailableMinInstanceID());
        outputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref outputProps, outputStatementFieldGroup, AvailableMinInstanceID());

        GrantFocusToInstances(idx);
        DepriveFocusFromOtherInstances(idx);

        // todo 表示位置の調整（todo 矢印のパネルも追加する）

        if (!CheckInstanceListCapacity()) addInstanceButton.interactable = false;
    }

    // AddInstanceとは異なり、focuseされている要素をremoveする
    private void RemoveInstanceFromList() // removeボタンが押せるということは、removeしていい、ということ
    {
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].IsFocused) // <- 消す対象
            {
                inputInstanceList.RemoveAt(i);
                outputInstanceList.RemoveAt(i);
                break;
            }
        }
        GrantFocusToInstances(inputInstanceList.Count - 1);

        if (inputInstanceList.Count <= 1) removeInstanceButton.interactable = false;
    }
    // isFocused；末尾にあるものに注目するようにする。

    // 汎用的なメソッドにする；これに関しては、focusされたものが末尾にあるとは限らない
    // indexを受け取るのが最も冗長性が少ないと思われる。
    // 呼び出す側で xputInstanceList.IndexOf(xxx)
    private void DepriveFocusFromOtherInstances(int focusedIdx)
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            if (i == focusedIdx) continue;
            else
            {
                inputInstanceList[i].IsFocused = false;
                outputInstanceList[i].IsFocused = false;
            }
        }
    }

    private void GrantFocusToInstances(int targetIdx)
    {
        inputInstanceList[targetIdx].IsFocused = true;
        outputInstanceList[targetIdx].IsFocused = true;
    }


    private void Update()
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++) {
            inputInstanceList[i].UpdateInputCondition();
            outputInstanceList[i].UpdateOutputBehaviour();
        }

        bool isConfirmable = CheckConfirmability(); // todo CanBeConfirmed プロパティを各カードクラスで更新する
        if (isConfirmable) confirmationButton.interactable = true;
        else confirmationButton.interactable = false;

        if (confirmed) {
            for (int i = 0; i < instanceCount; i++) {
                if (inputInstanceList[i].inputCondition) {
                    outputInstanceList[i].OutputBehaviour();
                } else {
                    outputInstanceList[i].OutputBehaviourNegative();
                }
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.Start)) {
            if (!menuIsOpened) OpenMenu();
            else CloseMenu();
        }
    }

    private bool CheckConfirmability()
    {
        bool bflag = true;
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            bflag &= inputInstanceList[i].CanBeConfirmed;
            bflag &= outputInstanceList[i].CanBeConfirmed;
        }
        return bflag;
    }


    // confirmationBtn is interactable only when all instances are confirmable (done)
    private void ConfirmSmartObject()
    {
        confirmed = true; // todo what to do with IsConfirmed in each instance??
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            inputInstanceList[i].ConfirmInputCondition();
            outputInstanceList[i].ConfirmOutputBehaviour();
        }
        backToEditButton.interactable = true;
        finalizationButton.interactable = true;
    }

    private void GoBackToEditMode()
    {
        // todo 内容を確認する
        confirmed = false;
        backToEditButton.interactable = false;
        finalizationButton.interactable = false;
    }

    private void FinalizeSmartObject()
    {
        // todo pack smart object information data and pass to next scene
        // CardSelectionMediator class に新しいフィールドを用意する
        SceneManager.LoadScene(3); // InteractionScene
    }


    private GameObject GetEnvObjByName(string cardName)
    {
        switch (cardName)
        {
            case "TrashBin": return envObjTrashBin;
            case "Tree": return envObjTree;
            case "StreetLight": return envObjStreetLight;
            case "StreetSign": return envObjStreetSign;
            case "Bridge": return envObjBridge;
            default: return null;
        }
    }

    private GameObject GetInPropsByName(string cardName)
    {
        switch (cardName)
        {
            case "Button": return inPropsButton;
            case "Sound": return inPropsSound;
            case "Fire": return inPropsFire;
            case "Speed": return inPropsSpeed;
            case "Weather": return inPropsWeather;
            default: return null;
        }
    }

    private GameObject GetOutPropsByName(string cardName)
    {
        switch (cardName)
        {
            case "LightUp": return outPropsLightUp;
            case "MakeSound": return outPropsMakeSound;
            case "Vibrate": return outPropsVibrate;
            case "Move": return outPropsMove;
            case "Send": return outPropsSend;
            default: return null;
        }
    }

    private InputCard GetInputInstanceByName(string cardName)
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

    private OutputCard GetOutputInstanceByName(string cardName)
    {
        switch (cardName)
        {
            case "LightUp": return new LightUpCard();
            case "MakeSound": return new MakeSoundCard();
            case "Vibrate": return new VibrateCard();
            case "Move": return new MoveCard();
            case "Send": return new SendCard();
            default: return null;
        }
    }

    // for making sure that all card representations are initially deactivated
    private void DeactivateAllCardRepresentations()
    {
        GameObject[] cardRepresentations;
        cardRepresentations = GameObject.FindGameObjectsWithTag("DeactivateOnLoad");
        foreach (GameObject prop in cardRepresentations)
        {
            prop.SetActive(false);
        }
    }

    private int AvailableMinInstanceID()
    {
        int idCandidate = 0;
        for (int i = 0; i < inputInstanceList.Count; i++)
        {
            if (inputInstanceList[i].InstanceID == -1) continue;
            if (idCandidate == inputInstanceList[i].InstanceID) idCandidate++;
        }
        return idCandidate;
    }


    private void OpenMenu()
    {
        menuIsOpened = true;
        menuCanvas.SetActive(true);
    }

    private void CloseMenu()
    {
        menuIsOpened = false;
        menuCanvas.SetActive(false);
    }

    private void LoadCardSelectionScene()
    {
        SceneManager.LoadScene(1); // CardSelectionScene
    }

}

/*

private void DepriveFocusFromOthers(InputCard inInstance, OutputCard outInstance)
{
    int focusedIdx = inputInstanceList.IndexOf(inInstance);
    // index should be identical for input and output instance list
    if (focusedIdx != outputInstanceList.IndexOf(outInstance))
    {
        Debug.Log("Warning: Indexes of Input and Output instance list are not aligned.");
        // some error handling here
    }
    int instanceCount = inputInstanceList.Count;
    for (int i = 0; i < instanceCount; i++)
    {
        if (i == focusedIdx) continue;
        else
        {
            inputInstanceList[i].IsFocused = false;
            outputInstanceList[i].IsFocused = false;
        }
    }
}

*/