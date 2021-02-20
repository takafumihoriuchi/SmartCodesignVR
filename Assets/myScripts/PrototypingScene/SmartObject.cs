using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SmartObject
{
    // parameters are set in CardSelectionScene
    public static Dictionary<string, string> cardSelectionDict
        = new Dictionary<string, string>() {
        {"environment", null},
        {"input", null},
        {"output", null}
    };

    public static GameObject environmentObject;

    public static GameObject propObjects;

    // public static List<System.Delegate> inputEvaluationList;

    // public static List<System.Delegate> outputBehaviourList;

}
