using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public abstract class Card
{
    protected GameObject environmentObject;
    protected GameObject cardNameText;
    protected TextMeshProUGUI cardNameTMP;
    protected GameObject statementBox;
    protected TextMeshProUGUI statementTMP;
    protected GameObject propObjects;

    protected bool isConfirmed;

    protected abstract void BehaviourDuringPrototyping();

    protected abstract string GetCardName();
    protected abstract string InitDescriptionText();
    protected abstract void InitPropFields();

    public void CardSetup(ref GameObject environmentObject,
        ref GameObject cardNameText, GameObject statementBox, GameObject propObjects)
    {
        this.environmentObject = environmentObject;
        this.environmentObject.SetActive(true);
        // envObj is passed by reference -> no copy is made

        this.cardNameText = cardNameText;
        cardNameTMP = this.cardNameText.GetComponent<TextMeshProUGUI>();
        cardNameTMP.SetText(GetCardName());
        this.cardNameText.SetActive(true);

        this.statementBox = statementBox;
        statementTMP = this.statementBox.transform.Find("DescriptionText")
            .gameObject.GetComponent<TextMeshProUGUI>();
        statementTMP.SetText(InitDescriptionText());
        this.statementBox.SetActive(true);
        // need to adjust transform.position when PrototypingSceneCore.instIdx >= 1

        this.propObjects = propObjects;
        InitPropFields();
        this.propObjects.SetActive(true);
        // inProps is passed by value -> a copy is already made
        // todo SetActivate(true)がないとHiddenのままか；これでコピーが作られているかどうかを確認する
        // コピーは作られていないような感じ。要詳細調査。
    }

}


public abstract class InputCard : Card
{
    [HideInInspector] public bool inputCondition = false;

    protected delegate bool InputConditionDelegate();

    protected InputConditionDelegate InputConditionDefinition;

    protected abstract InputConditionDelegate DetermineInputEvaluationDelegate();

    public void ConfirmInputCondition() {
        isConfirmed = true;
        InputConditionDefinition = DetermineInputEvaluationDelegate();
    }

    public void UpdateInputCondition() {
        UpdatesForInputConditionEvaluation();
        if (isConfirmed) {
            inputCondition = InputConditionDefinition();
        }
        else BehaviourDuringPrototyping();
    }

    protected abstract void UpdatesForInputConditionEvaluation();
}


public abstract class OutputCard : Card
{
    public abstract void OutputBehaviour();
    public abstract void UpdateOutputBehaviour();
    public abstract void ConfirmOutputBehaviour();
}
