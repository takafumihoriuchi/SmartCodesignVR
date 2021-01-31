using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


public class CardSelectionMediator
{
    // parameters are set in CardSelectionScene
    public static Dictionary<string, string> selectionDict
        = new Dictionary<string, string>() {
        {"environment", null},
        {"input", null},
        {"output", null}
    };

    // todo 最後に受け渡すデータ

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

    [SerializeField] private GameObject inputCardNameField = null; // todo これとかは初めからTextMeshProUGUIで読み込んでしまった方が少し効率が良い
    [SerializeField] private GameObject outputCardNameField = null;
    [SerializeField] private GameObject inputDescriptionField = null;
    [SerializeField] private GameObject outputDescriptionField = null;

    [SerializeField] private GameObject inputStatementFieldGroup = null; // type:Button
    [SerializeField] private GameObject outputStatementFieldGroup = null; // type:Button

    [SerializeField] private Button addInstanceButton = null;
    [SerializeField] private Button removeInstanceButton = null;

    [SerializeField] private GameObject ioArrowObject = null; // arrow image object
    private List<GameObject> ioArrowList = new List<GameObject>();

    // Confirmation Canvas Fields

    // todo こういうtext-fieldは、最初からそのコンポーネントしか使わないことがわかっていれば、GameObject以外を使用しても良い。
    [SerializeField] private TextMeshProUGUI confirmationMessageField = null; // todo assign in inspector
    [SerializeField] private Button confirmationButton = null;
    [SerializeField] private Button backToEditButton = null; // todo make new and put instance in inspector
    [SerializeField] private Button finalizationButton = null;
    // todo make new and add "finalize smart object" button in inspector
    bool isConfirmed = false; // after-confirmed か before-confirmed かを知るため（back-to-edit の操作で可逆性あり）
    readonly string beforeConfirmMessage = "Ready to test the Smart Object?";
    readonly string afterConfirmMessage = "Do you want to keep editing or finalize?";

    [SerializeField] private GameObject menuCanvas = null;
    [SerializeField] private Button backToSceneButton = null;
    [SerializeField] private Button closeMenuButton = null;
    private bool menuIsOpened = false;

    private GameObject environmentObject;
    private GameObject inputProps;
    private GameObject outputProps;
    private List<InputCard> inputInstanceList = new List<InputCard>();
    private List<OutputCard> outputInstanceList = new List<OutputCard>();

    readonly Color BEIGE = new Color(0.9803f, 0.9568f, 0.9019f, 1.0f); // statement focused
    readonly Color SHADE = new Color(0.7803f, 0.7568f, 0.7019f, 1.0f); // statement shaded

    const float VSHAMT = 0.5f; // vertical shift amount

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

    // Startの中は、なるべくメソッドで処理の塊をバッチ化する。（ボタンのAddListenerの塊とか）
    private void Start()
    {
        DevelopmentPurposeAssign(); // make sure to delete after development

        DeactivateAllCardRepresentations();

        environmentObject = GetEnvObjByName(CardSelectionMediator.selectionDict["environment"]);

        int idx = 0;
        int minInstanceID = AvailableMinInstanceID();

        inputProps = GetInPropsByName(CardSelectionMediator.selectionDict["input"]);
        inputInstanceList.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        inputInstanceList[idx].CardDescriptionSetup(
            ref inputCardNameField, ref inputDescriptionField); // todo 参照型だから多分大丈夫だと思うけど、最初に生成したものが後に削除された場合、これらの要素が残るかを確認する
        inputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref inputProps, ref inputStatementFieldGroup, minInstanceID);

        outputProps = GetOutPropsByName(CardSelectionMediator.selectionDict["output"]);
        outputInstanceList.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        outputInstanceList[idx].CardDescriptionSetup(
            ref outputCardNameField, ref outputDescriptionField);
        outputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref outputProps, ref outputStatementFieldGroup, minInstanceID);

        GrantFocusToTargetInstances(idx);

        // ボタンとしてのstatementBoxの追加と、それが押下された時にIsFocusedがtrueになる処理
        //inputInstanceList[idx].StatementFieldGroup.GetComponent<Button>().onClick.AddListener(StatementBoxOnClick);
        //outputInstanceList[idx].StatementFieldGroup.GetComponent<Button>().onClick.AddListener(StatementBoxOnClick);
        inputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(inputInstanceList[idx].InstanceID); }
            );
        outputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(outputInstanceList[idx].InstanceID); }
            );
        // todo この方法で良いのかの確認

        ioArrowList.Add(ioArrowObject);

        // UI button settings

        addInstanceButton.onClick.AddListener(AddInstanceToList);
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.onClick.AddListener(RemoveInstanceFromList);
        removeInstanceButton.interactable = false;

        confirmationMessageField.SetText(beforeConfirmMessage);
        confirmationButton.onClick.AddListener(ConfirmSmartObject);
        confirmationButton.interactable = false; // set to true when every-instances.canBeConfirmed is true
        confirmationButton.gameObject.SetActive(true);
        backToEditButton.onClick.AddListener(GoBackToEditMode);
        backToEditButton.interactable = false;
        backToEditButton.gameObject.SetActive(false);
        finalizationButton.onClick.AddListener(FinalizeSmartObject);
        finalizationButton.interactable = false;
        finalizationButton.gameObject.SetActive(false);


        backToSceneButton.onClick.AddListener(LoadCardSelectionScene);
        closeMenuButton.onClick.AddListener(CloseMenu);

    }

    private bool CheckInstanceListCapacity()
    {
        return inputInstanceList.Count < inputInstanceList[0].MaxInstanceNum;
    }

    // todo Startの中の処理と合わせて、リファクタの余地あり; ボタン登録も関数化可能(indexを引数に渡す)
    private void AddInstanceToList()
    {
        inputInstanceList.Add(GetInputInstanceByName(CardSelectionMediator.selectionDict["input"]));
        outputInstanceList.Add(GetOutputInstanceByName(CardSelectionMediator.selectionDict["output"]));
        int idx = inputInstanceList.Count - 1; // tail of updated list

        // note that we are skipping CardDescriptionSetup(); this is only necessary in the first instance generated in Start()
        int minInstanceID = AvailableMinInstanceID();

        inputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref inputProps, ref inputStatementFieldGroup, minInstanceID);
        outputInstanceList[idx].CardStatementSetup(
            ref environmentObject, ref outputProps, ref outputStatementFieldGroup, minInstanceID);

        //GrantFocusToTargetInstances(idx);
        //DepriveFocusFromOtherInstances(idx);
        DepriveFocusFromOtherAndGrantToTargetInstances(idx);

        // ボタンの登録　（一番初めのinstanceのボタン登録はStart()で個別にやる）
        inputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(inputInstanceList[idx].InstanceID); }
            );
        outputInstanceList[idx].StatementFieldGroup.
            GetComponent<Button>().onClick.AddListener(
            () => { StatementFieldOnClick(outputInstanceList[idx].InstanceID); }
            );
        // どのインスタンスのボタンが押されたのかの情報が欲しい => TODO この方法(lambda expression)で実現できているのかの動作確認
        // inputInstanceList[idx].InstanceID は最初にセットされた後は、書き換えられない想定だからユニークなインスタンスに紐ずく

        ShiftStatementFields(); // 他人を押し上げる

        // 矢印 => インスタンスに付随する必要はない。instance.Countに応じて、個数と位置を調整する
        AddIOArrow();

        // check for allawability of adding and removing instances
        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputInstanceList.Count <= 1);
    }


    // TODO 本当にこれで良いのかがわからない。つまり、idxの渡し方が
    // todo IsFocusedなものとそうでないものとで色の濃淡を変化させる
    // todo on click でfocusが移り変わる機能の実装
    // recieves the instance-ID of the clicked statement-box
    private void StatementFieldOnClick(int instanceID)
    {
        int idx = GetInstanceListIndexFromInstanceID(instanceID);
        //GrantFocusToTargetInstances(idx);
        //DepriveFocusFromOtherInstances(idx);
        DepriveFocusFromOtherAndGrantToTargetInstances(idx);
    }

    private void ShiftStatementFields()
    {
        Vector3 inputBasePosition = inputStatementFieldGroup.transform.position;
        Vector3 outputBasePosition = outputStatementFieldGroup.transform.position;
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 verticalShiftAmount = new Vector3(0, VSHAMT * (instanceCount-1-i), 0);
            inputInstanceList[i].StatementFieldGroup.transform.position = inputBasePosition + verticalShiftAmount;
            outputInstanceList[i].StatementFieldGroup.transform.position = outputBasePosition + verticalShiftAmount;
        }
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
        GrantFocusToTargetInstances(inputInstanceList.Count - 1); // focus on tail instance

        ShiftStatementFields();
        ReduceIOArrow();

        if (inputInstanceList.Count <= 1)
            removeInstanceButton.interactable = false;
    }

    // ずらし方がIOinstanceとは異なる。ここでは、新しいのが上に積み上げられる
    private void AddIOArrow()
    {
        int n = ioArrowList.Count;
        Vector3 uppermostArrowPosition = ioArrowList[n-1].transform.position;
        Vector3 newArrowPosition = uppermostArrowPosition + new Vector3(0, VSHAMT, 0);
        GameObject newIOArrow = Instantiate(ioArrowList[0],
            newArrowPosition, Quaternion.identity, ioArrowList[0].transform.parent);
        ioArrowList.Add(newIOArrow);
    }

    // reduceしてはいけない時には、すでに呼び出せないようになっている
    private void ReduceIOArrow()
    {
        ioArrowList.RemoveAt(ioArrowList.Count-1);
    }


    // 呼び出す側で xputInstanceList.IndexOf(xxx)（特にbuttonがpressされた時）
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
                // statement (is button) の色を変える
                inputInstanceList[focusedIdx].StatementFieldGroup.GetComponent<Image>().color = SHADE;
                outputInstanceList[focusedIdx].StatementFieldGroup.GetComponent<Image>().color = SHADE;
            }
        }
    }

    private void GrantFocusToTargetInstances(int targetIdx)
    {
        inputInstanceList[targetIdx].IsFocused = true;
        outputInstanceList[targetIdx].IsFocused = true;
        // statement (is button) の色を変える
        inputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
        outputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
    }

    private void DepriveFocusFromOtherAndGrantToTargetInstances(int targetIdx)
    {
        // Note: focus transition order must be "deprivae from other THEN focus on target" for some specific card types
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            inputInstanceList[i].IsFocused = false;
            outputInstanceList[i].IsFocused = false;
            inputInstanceList[i].StatementFieldGroup.GetComponent<Image>().color = SHADE;
            outputInstanceList[i].StatementFieldGroup.GetComponent<Image>().color = SHADE;
        }
        inputInstanceList[targetIdx].IsFocused = true;
        outputInstanceList[targetIdx].IsFocused = true;
        inputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
        outputInstanceList[targetIdx].StatementFieldGroup.GetComponent<Image>().color = BEIGE;
    }


    private void Update()
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++) {
            if (!inputInstanceList[i].IsFocused) continue;
            inputInstanceList[i].UpdateInputCondition();
            outputInstanceList[i].UpdateOutputBehaviour();
        }

        // check if all instances has been set a value
        bool isConfirmable = CheckConfirmability(); // CanBeConfirmed プロパティを各カードクラスで更新する
        if (isConfirmable) confirmationButton.interactable = true;
        else confirmationButton.interactable = false;
        // todo Update()の中ではなく、どこかの発火から処理を行うのが理想的

        if (isConfirmed)
        {
            for (int i = 0; i < instanceCount; i++)
            {
                if (inputInstanceList[i].inputCondition)
                {
                    outputInstanceList[i].OutputBehaviour();
                }
                else
                {
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
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            bflag &= inputInstanceList[i].CanBeConfirmed;
            bflag &= outputInstanceList[i].CanBeConfirmed;
        }
        return bflag;
    }


    // Confirmation Button OnClick
    private void ConfirmSmartObject()
    {
        isConfirmed = true;

        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++) {
            inputInstanceList[i].IsConfirmed = true;
            outputInstanceList[i].IsConfirmed = true;
        }

        confirmationButton.interactable = false;
        confirmationButton.gameObject.SetActive(false);
        backToEditButton.interactable = true;
        backToEditButton.gameObject.SetActive(true);
        finalizationButton.interactable = true;
        finalizationButton.gameObject.SetActive(true);
        confirmationMessageField.SetText(afterConfirmMessage);

        addInstanceButton.interactable = false;
        removeInstanceButton.interactable = false;

        ChangeStatementFieldButtonInteractability(false);
        ChangeUnfocusedStatementFieldColors(BEIGE);
    }

    // Back-To-Edit Button OnClick
    private void GoBackToEditMode()
    {
        isConfirmed = false;

        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            inputInstanceList[i].IsConfirmed = false;
            outputInstanceList[i].IsConfirmed = false;
        }

        confirmationButton.interactable = true;
        confirmationButton.gameObject.SetActive(true);
        backToEditButton.interactable = false;
        backToEditButton.gameObject.SetActive(false);
        finalizationButton.interactable = false;
        finalizationButton.gameObject.SetActive(false);
        confirmationMessageField.SetText(beforeConfirmMessage);

        addInstanceButton.interactable = CheckInstanceListCapacity();
        removeInstanceButton.interactable = !(inputInstanceList.Count <= 1);

        ChangeStatementFieldButtonInteractability(true);
        ChangeUnfocusedStatementFieldColors(SHADE);
    }

    private void ChangeStatementFieldButtonInteractability(bool tf)
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            inputInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
            outputInstanceList[i].StatementFieldGroup.
                GetComponent<Button>().interactable = tf;
        }
    }

    private void ChangeUnfocusedStatementFieldColors(Color color)
    {
        int instanceCount = inputInstanceList.Count;
        for (int i = 0; i < instanceCount; i++)
        {
            if (inputInstanceList[i].IsFocused) continue;
            else
            {
                inputInstanceList[i].StatementFieldGroup.
                    GetComponent<Image>().color = color;
                outputInstanceList[i].StatementFieldGroup.
                    GetComponent<Image>().color = color;
            }
        }
    }

    // Finalization Button OnClick
    private void FinalizeSmartObject()
    {
        // todo pack smart object information data and pass to next scene
        // => CardSelectionMediator class に新しいフィールドを用意する
        // Smart Object を記述するのに何が必要かを考える（environment object, input-delegate, output-behaviour, props, ...）

        Debug.Log("Moving to InteractionScene");
        // SceneManager.LoadScene(3); // InteractionScene
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
            if (inputInstanceList[i].InstanceID == -1) continue; // skip itself (set to initial value)
            if (idCandidate == inputInstanceList[i].InstanceID) idCandidate++;
        }
        return idCandidate;
    }

    // returns index in Instance List 
    // returns -1 when not found in Instance List
    private int GetInstanceListIndexFromInstanceID(int id)
    {
        for (int idx = 0; idx < inputInstanceList.Count; idx++)
        {
            if (inputInstanceList[idx].InstanceID == id) return idx;
        }
        return -1; // todo may want to make it throw error
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