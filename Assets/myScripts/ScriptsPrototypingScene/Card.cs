using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public abstract class Card : MonoBehaviour
{
    protected GameObject environmentObject;
    protected TextMeshPro cardNameTMP;
    protected bool isConfirmed;
    protected abstract void BehaviourDuringPrototyping();
}


public abstract class InputCard : Card
{
    protected GameObject inputSelectionText;
    protected GameObject inputConditionBox;
    protected TextMeshPro inputConditionTMP;
    protected GameObject inputProps;

    [HideInInspector] public bool inputCondition;
    protected delegate bool InputConditionDelegate();
    protected InputConditionDelegate InputConditionDefintion;

    public void ConfirmInputCondition() {
        isConfirmed = true;
        InputConditionDefintion = DetermineInputEvaluationDelegate();
    }

    public void UpdateInputCondition() {
        UpdatesForInputConditionEvaluation();
        if (isConfirmed) inputCondition = InputConditionDefintion();
        else BehaviourDuringPrototyping();
    }

    public void SetInputCondition(ref GameObject envObj,
        ref GameObject inCardText, GameObject inCondBox, GameObject inProps)
    {
        environmentObject = envObj; // envObj is passed by reference -> no copy is made
        environmentObject.SetActive(true);

        inputSelectionText = inCardText;
        cardNameTMP = inputSelectionText.GetComponent<TextMeshPro>();
        cardNameTMP.SetText(GetCardName());
        inputSelectionText.SetActive(true);

        inputConditionBox = inCondBox;
        inputConditionTMP = inputConditionBox.transform.Find("DescriptionText").gameObject.GetComponent<TextMeshPro>();
        inputConditionTMP.SetText(InitDescriptionText());
        inputConditionBox.SetActive(true);
        // need to adjust transform.position when PrototypingSceneCore.instIdx >= 1

        inputProps = inProps; // inProps is passed by value -> a copy is already made
        InitPropFields();
        inputProps.SetActive(true);
        // todo SetActivate(true)がないとHiddenのままか；これでコピーが作られているかどうかを確認する
    }

    protected abstract string GetCardName();
    protected abstract string InitDescriptionText();
    protected abstract void InitPropFields();
    protected abstract InputConditionDelegate DetermineInputEvaluationDelegate();
    protected abstract void UpdatesForInputConditionEvaluation();
}


public abstract class OutputCard : Card
{
    protected GameObject outputSelectionText;
    protected GameObject outputBehaviourBox;
    protected TextMeshPro outputBehaviourTMP;
    protected GameObject outputProps;

    public abstract void SetOutputBehaviour(ref GameObject envObj,
        ref GameObject outCardText, GameObject outBehavBox, GameObject outProps);
    public abstract void UpdateOutputBehaviour();
    public abstract void ConfirmOutputBehaviour();
    public abstract void OutputBehaviour();
    //なんらかの形で鍵をかける必要があるか(e.g. 録音中に再生が始まらないように)


}