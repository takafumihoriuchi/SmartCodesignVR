using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBridge;

// task in each input-cards:
// - define inputEvalDeleDict
// - assign ConditionKeyword (e.g. ConditionKeyword = RAINY;)
public class WeatherCard : InputCard
{
    private GameObject[] weatherObjArr;
    private GameObject[] weatherRayArr;

    readonly string SUNNY = "be sunny";
    readonly string CLOUDY = "be cloudy";
    readonly string RAINY = "rain";
    readonly string THUNDERSTORMY = "be a storm";
    readonly string SNOWY = "snow";

    public WeatherCard()
    {
        maxInstanceNum = 5; // sunny, cloudy, rainy, snowy, thunderstormy

        cardName = "Weather";

        descriptionText = "<i>I can get information about the weather forecast.</i>\n"
            + "<b>Steps:</b>\n"
            + "<b>1.</b> <indent=10%>Grab the weather representative by holding the controller trigger.</indent>\n"
            + "<b>2.</b> <indent=10%>Bring the weather close to the object so that the object enters the ray.</indent>\n"
            + "Maximum number of instances: 5";

        contentText = "When it is about to";

        inputEvalDeleDict = new Dictionary<string, InputEvaluationDelegate>
        {
            {SUNNY, hoge0},
            {CLOUDY, hoge1},
            {RAINY, hoge2},
            {THUNDERSTORMY, hoge3},
            {SNOWY, hoge4}
        };
    }

    public bool hoge0() // どうしてこれはprivateなのか
    {
        
    }
    public bool hoge1()
    {

    }
    public bool hoge2()
    {

    }
    public bool hoge3()
    {

    }
    public bool hoge4()
    {

    }

    protected override void InitPropFields()
    {

    }

    public override void BehaviourDuringPrototyping()
    {

    }

    protected override void OnFocusGranted()
    {

    }

    protected override void OnFocusDeprived()
    {

    }

    protected override void OnConfirm()
    {
        // 使っていないpropはSetActive(false)
    }

    protected override void OnBackToEdit()
    {
        // 全ての（または隠した）propsをSetActive(true)
    }

}

/*
 * 3D models
 * - sun: https://poly.google.com/view/8atTd-V-X7i
 * - raindrop: https://poly.google.com/view/fMyBNkWqMC
 * - cloud: https://poly.google.com/view/fRmb17kTurm
 * - lightning: https://poly.google.com/view/7I1IhiE7O8s
 * - snowman: http://blog.sunday-model.net/article/178647073.html
 * 
 * 
 */