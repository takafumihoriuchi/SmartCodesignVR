﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public abstract class Card
{
    protected GameObject environmentObject;
    protected GameObject propObjects;
    protected GameObject statementFieldGroup;
    public GameObject StatementFieldGroup {
        set { } get { return statementFieldGroup; }}
    protected TextMeshProUGUI cardNameField;
    protected TextMeshProUGUI descriptionField;
    protected TextMeshProUGUI indexTextTMP;
    protected TextMeshProUGUI contentTextTMP;
    protected TextMeshProUGUI variableTextTMP;

    protected string cardName;
    protected string descriptionText;
    protected string contentText;
    private int instanceID = -1;
    public int InstanceID {
        set { } get { return instanceID; } }

    protected bool isConfirmed = false;
    public bool IsConfirmed {
        set { isConfirmed = value;
            if (isConfirmed) OnConfirm();
            else OnBackToEdit(); }
        get { return isConfirmed; }}

    protected bool isFocused;
    public bool IsFocused {
        set { isFocused = value;
            if (isFocused) OnFocusGranted();
            else OnFocusDeprived(); }
        get { return isFocused; }}

    protected bool canBeConfirmed = false;
    public bool CanBeConfirmed {
        set { } get { return canBeConfirmed; } }

    protected abstract void InitPropFields();
    protected abstract void BehaviourDuringPrototyping();

    protected abstract void OnConfirm();
    protected abstract void OnBackToEdit();
    protected abstract void OnFocusGranted();
    protected abstract void OnFocusDeprived();

    protected abstract string GetIndexSubText();
    // index text string is used for determining
    // which statement-field-button was selected
    private string GetIndexText() {
        return "<u>" + GetIndexSubText() + " #"
            + (instanceID + 1).ToString() + "</u>";}

    // only necessary for the first card instance
    public void CardDescriptionSetup(
        ref TextMeshProUGUI cardNameField,
        ref TextMeshProUGUI descriptionField) {
        this.cardNameField = cardNameField;
        this.descriptionField = descriptionField;
        cardNameField.SetText(cardName);
        descriptionField.SetText(descriptionText);
    }

    public void CardStatementSetup(
        ref GameObject environmentObject,
        ref GameObject propObjects,
        ref GameObject statementFieldGroup,
        int instanceID) {
        this.instanceID = instanceID; // assign before setting indexText
        this.environmentObject = environmentObject;
        this.environmentObject.SetActive(true);
        this.propObjects = propObjects;
        InitPropFields();
        this.propObjects.SetActive(true);
        this.statementFieldGroup = Object.Instantiate(
            statementFieldGroup, statementFieldGroup.transform.parent);
        indexTextTMP = this.statementFieldGroup.transform.
            Find("IndexText").gameObject.GetComponent<TextMeshProUGUI>();
        indexTextTMP.SetText(GetIndexText());
        contentTextTMP = this.statementFieldGroup.transform.
            Find("ContentText").gameObject.GetComponent<TextMeshProUGUI>();
        contentTextTMP.SetText(contentText);
        variableTextTMP = this.statementFieldGroup.transform.
            Find("Variable/VariableText").gameObject.GetComponent<TextMeshProUGUI>();
        variableTextTMP.SetText(string.Empty);
        this.statementFieldGroup.SetActive(true);
    }

}



public abstract class InputCard : Card
{
    protected override string GetIndexSubText() { return "input"; }

    protected int maxInstanceNum;
    public int MaxInstanceNum { set { } get { return maxInstanceNum; } }

    public delegate bool InputEvaluationDelegate();
    public Dictionary<string, InputEvaluationDelegate> inputEvalDeleDict;
    protected string conditionKeyword = string.Empty;
    public string ConditionKeyword {
        set { conditionKeyword = value; variableTextTMP.SetText(conditionKeyword); }
        get { return conditionKeyword; } }
    public readonly string ALREADY_EXISTS = "already exists";



    private bool f2TOnThisFrame = false; // inCondPosTriggerFlg - inCondNegTriggerFlg
    public bool F2TOnThisFrame
    {
        set {}
        get {
            if (f2TOnThisFrame)
            {
                f2TOnThisFrame = false;
                return true;
            }
            else return false;
        }
    }
    private bool t2FOnThisFrame = false;
    public bool T2FOnThisFrame // trueの状態でgetされるとすぐにtrueを失ってしまうような変数
    {
        set {} // readonlyなプロパティにできる
        get {
            if (t2FOnThisFrame) {
                t2FOnThisFrame = false;
                return true;
            }
            else return false;
        }
    }
    private bool inputCondition;
    public bool InputCondition {
        set { // only set from UpdateInputCondition()
            if (!inputCondition && value)
                f2TOnThisFrame = true;
            else if (inputCondition && !value)
                t2FOnThisFrame = true;
            inputCondition = value;
        }
        get {
            return inputCondition; // not used
        }}

    public void UpdateInputCondition()
    {
        if (isConfirmed) {
            // to check continuously during test
            InputCondition = inputEvalDeleDict[conditionKeyword]();
        } else {
            BehaviourDuringPrototyping();
            canBeConfirmed = !(string.IsNullOrEmpty(conditionKeyword)
                || conditionKeyword == ALREADY_EXISTS);
        }
    }

}



public abstract class OutputCard : Card
{
    public void UpdateOutputBehaviour()
    {
        if (isConfirmed) {
            return;
        } else {
            BehaviourDuringPrototyping();
            canBeConfirmed = !string.IsNullOrEmpty(variableTextTMP.text);
        }
    }

    public abstract void OutputBehaviour();
    public abstract void OutputBehaviourNegative();
    protected override string GetIndexSubText() { return "output"; }
}