using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Card : MonoBehaviour
{
    protected GameObject environmentObject;

    protected TextMeshPro cardNameTMP;

}


public abstract class InputCard : Card
{
    protected GameObject inputProps;
    protected GameObject inputSelectionText;
    protected GameObject inputConditionBox;

    protected TextMeshPro inputConditionTMP;

    [HideInInspector] public bool inputCondition;
    protected bool isConfirmed; // movable to Card-Class??
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

    public abstract void SetInputCondition(ref GameObject envObj,
        ref GameObject inCardText, GameObject inCondBox, GameObject inProps);
    protected abstract InputConditionDelegate DetermineInputEvaluationDelegate();
    protected abstract void UpdatesForInputConditionEvaluation();
    protected abstract void BehaviourDuringPrototyping();

}


public abstract class OutputCard : Card
{
    protected GameObject outputProps;
    protected GameObject outputSelectionText;
    protected GameObject outputBehaviourBox;

    protected TextMeshPro outputBehaviourTMP;

    public abstract void SetOutputBehaviour(ref GameObject envObj,
        ref GameObject outCardText, GameObject outBehavBox, GameObject outProps);
    public abstract void UpdateOutputBehaviour();
    public abstract void ConfirmOutputBehaviour();
    public abstract void OutputBehaviour();
    //なんらかの形で鍵をかける必要があるか(e.g. 録音中に再生が始まらないように)

}