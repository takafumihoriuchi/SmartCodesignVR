using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public abstract class Card : MonoBehaviour
{
    protected GameObject environmentObject;
    protected TextMeshPro cardNameTMP;
    protected bool isConfirmed;

    protected GameObject cardNameText;
    protected GameObject statementBox;
    protected TextMeshPro statementTMP;
    protected GameObject propObjects;

    protected abstract void BehaviourDuringPrototyping();
    protected abstract string GetCardName();
    protected abstract string InitDescriptionText();
    protected abstract void InitPropFields();
}



public abstract class InputCard : Card
{
    
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

        cardNameText = inCardText;
        cardNameTMP = cardNameText.GetComponent<TextMeshPro>();
        cardNameTMP.SetText(GetCardName());
        cardNameText.SetActive(true);

        statementBox = inCondBox;
        statementTMP = statementBox.transform.Find("DescriptionText").gameObject.GetComponent<TextMeshPro>();
        statementTMP.SetText(InitDescriptionText());
        statementBox.SetActive(true);
        // need to adjust transform.position when PrototypingSceneCore.instIdx >= 1

        propObjects = inProps; // inProps is passed by value -> a copy is already made
        InitPropFields();
        propObjects.SetActive(true);
        // todo SetActivate(true)がないとHiddenのままか；これでコピーが作られているかどうかを確認する
    }

    protected abstract InputConditionDelegate DetermineInputEvaluationDelegate();
    protected abstract void UpdatesForInputConditionEvaluation();
}



public abstract class OutputCard : Card
{

    public void SetOutputBehaviour(ref GameObject envObj,
        ref GameObject outCardText, GameObject outBehavBox, GameObject outProps)
    {
        environmentObject = envObj;
        environmentObject.SetActive(true);

        cardNameText = outCardText;
        cardNameTMP = cardNameText.GetComponent<TextMeshPro>();
        cardNameTMP.SetText(GetCardName());
        cardNameText.SetActive(true);

        statementBox = outBehavBox;
        statementTMP = statementBox.transform.Find("DescriptionText").gameObject.GetComponent<TextMeshPro>();
        statementTMP.SetText(InitDescriptionText());
        statementBox.SetActive(true);

        propObjects = outProps;
        InitPropFields();
        propObjects.SetActive(true);
    }


    public abstract void UpdateOutputBehaviour();
    public abstract void ConfirmOutputBehaviour();
    public abstract void OutputBehaviour();

}