using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventBridge;
using System;

// task in each input-cards:
// - define inputEvalDeleDict
// - assign ConditionKeyword (e.g. ConditionKeyword = RAINY;)
public class WeatherCard : InputCard
{
    Component[] envPartsComponent;
    GameObject[] envPartsGameObject;

    const int N = 5;
    const int SUNNY_IDX = 0;
    const int CLOUDY_IDX = 1;
    const int RAINY_IDX = 2;
    const int THUNDERSTORMY_IDX = 3;
    const int SNOWY_IDX = 4;

    IComponentEventHandler sunnyEventHandler;
    IComponentEventHandler cloudyEventHandler;
    IComponentEventHandler rainyEventHandler;
    IComponentEventHandler thunderstormyEventHandler;
    IComponentEventHandler snowyEventHandler;

    private GameObject[] modelArr = new GameObject[N];
    private GameObject[] rayArr = new GameObject[N];

    private bool isSunnyNow;
    private bool isCloudyNow;
    private bool isRainyNow;
    private bool isThunderstormyNow;
    private bool isSnowyNow;

    private bool wasSunnyPrevFrame;
    private bool wasCloudyPrevFrame;
    private bool wasRainyPrevFrame;
    private bool wasThunderstormyPrevFrame;
    private bool wasSnowyPrevFrame;

    // todo weather-indexを使った配列でリファクタできる。

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
            {SUNNY, SunnyForecast},
            {CLOUDY, CloudyForecast},
            {RAINY, RainyForecast},
            {THUNDERSTORMY, ThunderstormyForecast},
            {SNOWY, SnowyForecast}
        };
    }

    // xxxTriggerStayを更新するときと、ConditionKeywordを更新するときとでは、マルチインスタンスの扱いが異なる
    // TODO trueに変化させるのは簡単。falseに戻すのに一工夫必要。
    public bool SunnyForecast()
    {
        // constantly updates current weather with one-frame delay
        SetRay(modelArr[SUNNY_IDX], rayArr[SUNNY_IDX]);
        bool tmp = wasSunnyPrevFrame;
        wasSunnyPrevFrame = isSunnyNow;
        isSunnyNow = false;
        Debug.Log("isSunnyNow: " + isSunnyNow + ", wasSunnyPrevFrame:" + wasSunnyPrevFrame);
        return tmp;
    }
    public bool CloudyForecast()
    {
        SetRay(modelArr[CLOUDY_IDX], rayArr[CLOUDY_IDX]);
        bool tmp = wasCloudyPrevFrame;
        wasCloudyPrevFrame = isCloudyNow;
        isCloudyNow = false;
        return tmp;
    }
    public bool RainyForecast()
    {
        SetRay(modelArr[RAINY_IDX], rayArr[RAINY_IDX]);
        bool tmp = wasRainyPrevFrame;
        wasRainyPrevFrame = isRainyNow;
        isRainyNow = false;
        return tmp;
    }
    public bool ThunderstormyForecast()
    {
        SetRay(modelArr[THUNDERSTORMY_IDX], rayArr[THUNDERSTORMY_IDX]);
        bool tmp = wasThunderstormyPrevFrame;
        wasThunderstormyPrevFrame = isThunderstormyNow;
        isThunderstormyNow = false;
        return tmp;
    }
    public bool SnowyForecast()
    {
        SetRay(modelArr[SNOWY_IDX], rayArr[SNOWY_IDX]);
        bool tmp = wasSnowyPrevFrame;
        wasSnowyPrevFrame = isSnowyNow;
        isSnowyNow = false;
        return tmp;
    }

    protected override void InitPropFields()
    {
        envPartsComponent = environmentObject.GetComponentsInChildren<Rigidbody>(true);
        envPartsGameObject = ConvertComponentArrayToGameObjectArray(envPartsComponent);

        modelArr[SUNNY_IDX] = propObjects.transform.Find("sunny").gameObject;
        modelArr[CLOUDY_IDX] = propObjects.transform.Find("cloudy").gameObject;
        modelArr[RAINY_IDX] = propObjects.transform.Find("rainy").gameObject;
        modelArr[THUNDERSTORMY_IDX] = propObjects.transform.Find("thunderstormy").gameObject;
        modelArr[SNOWY_IDX] = propObjects.transform.Find("snowy").gameObject;

        rayArr[SUNNY_IDX] = propObjects.transform.Find("sunny/ray").gameObject;
        rayArr[CLOUDY_IDX] = propObjects.transform.Find("cloudy/ray").gameObject;
        rayArr[RAINY_IDX] = propObjects.transform.Find("rainy/ray").gameObject;
        rayArr[THUNDERSTORMY_IDX] = propObjects.transform.Find("thunderstormy/ray").gameObject;
        rayArr[SNOWY_IDX] = propObjects.transform.Find("snowy/ray").gameObject;

        // each ray has its own trigger detector
        sunnyEventHandler = rayArr[SUNNY_IDX].RequestEventHandlers();
        cloudyEventHandler = rayArr[CLOUDY_IDX].RequestEventHandlers();
        rainyEventHandler = rayArr[RAINY_IDX].RequestEventHandlers();
        thunderstormyEventHandler = rayArr[THUNDERSTORMY_IDX].RequestEventHandlers();
        snowyEventHandler = rayArr[SNOWY_IDX].RequestEventHandlers();

        for (int i = 0; i < N; i++) {
            // deactivate ray on start
            SetRayMeshRenderer(rayArr[i], false);
            SetRayMeshCollider(rayArr[i], false);
        }
    }

    private void SetRayMeshRenderer(GameObject ray, bool state) {
        if (ray.GetComponent<MeshRenderer>().enabled != state)
            ray.GetComponent<MeshRenderer>().enabled = state;
    }

    private void SetRayMeshCollider(GameObject ray, bool state) {
        if (ray.GetComponent<MeshCollider>().enabled != state)
            ray.GetComponent<MeshCollider>().enabled = state;
    }

    // activate ray if grabbed
    private void SetRay(GameObject model, GameObject ray)
    {
        bool isGrabbed = model.GetComponent<OVRGrabbable>().isGrabbed;
        SetRayMeshRenderer(ray, isGrabbed);
        SetRayMeshCollider(ray, isGrabbed);
    }

    public override void BehaviourDuringPrototyping()
    {
        for (int i = 0; i < N; i++) { SetRay(modelArr[i], rayArr[i]); }
    }

    private void sunnyTriggerEnter(Collider other) {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            ConditionKeyword = SUNNY;
        }
    }
    private void cloudyTriggerEnter(Collider other) {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            ConditionKeyword = CLOUDY;
        }
    }
    private void rainyTriggerEnter(Collider other) {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            ConditionKeyword = RAINY;
        }
    }
    private void thunderstormyTriggerEnter(Collider other) {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            ConditionKeyword = THUNDERSTORMY;
        }
    }
    private void snowyTriggerEnter(Collider other) {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            ConditionKeyword = SNOWY;
        }
    }

    //

    private void sunnyTriggerStay(Collider other)
    {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            wasSunnyPrevFrame = isSunnyNow;
            isSunnyNow = true;
        }
        Debug.Log("isSunnyNow: " + isSunnyNow + ", wasSunnyPrevFrame:" + wasSunnyPrevFrame);
    }
    private void cloudyTriggerStay(Collider other)
    {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            wasCloudyPrevFrame = isCloudyNow;
            isCloudyNow = true;
        }
    }
    private void rainyTriggerStay(Collider other)
    {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            wasRainyPrevFrame = isRainyNow;
            isRainyNow = true;
        }
    }
    private void thunderstormyTriggerStay(Collider other)
    {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            wasThunderstormyPrevFrame = isThunderstormyNow;
            isThunderstormyNow = true;
        }
    }
    private void snowyTriggerStay(Collider other)
    {
        if (Array.IndexOf(envPartsGameObject, other.transform.gameObject) != -1)
        {
            wasSnowyPrevFrame = isSnowyNow;
            isSnowyNow = true;
        }
    }


    protected override void OnFocusGranted()
    {
        sunnyEventHandler.TriggerEnter += sunnyTriggerEnter;
        cloudyEventHandler.TriggerEnter += cloudyTriggerEnter;
        rainyEventHandler.TriggerEnter += rainyTriggerEnter;
        thunderstormyEventHandler.TriggerEnter += thunderstormyTriggerEnter;
        snowyEventHandler.TriggerEnter += snowyTriggerEnter;
    }

    protected override void OnFocusDeprived()
    {
        sunnyEventHandler.TriggerEnter -= sunnyTriggerEnter;
        cloudyEventHandler.TriggerEnter -= cloudyTriggerEnter;
        rainyEventHandler.TriggerEnter -= rainyTriggerEnter;
        thunderstormyEventHandler.TriggerEnter -= thunderstormyTriggerEnter;
        snowyEventHandler.TriggerEnter -= snowyTriggerEnter;
    }

    protected override void OnConfirm() {
        sunnyEventHandler.TriggerStay += sunnyTriggerStay;
        cloudyEventHandler.TriggerStay += cloudyTriggerStay;
        rainyEventHandler.TriggerStay += rainyTriggerStay;
        thunderstormyEventHandler.TriggerStay += thunderstormyTriggerStay;
        snowyEventHandler.TriggerStay += snowyTriggerStay;
    }
    protected override void OnBackToEdit() {
        sunnyEventHandler.TriggerStay -= sunnyTriggerStay;
        cloudyEventHandler.TriggerStay -= cloudyTriggerStay;
        rainyEventHandler.TriggerStay -= rainyTriggerStay;
        thunderstormyEventHandler.TriggerStay -= thunderstormyTriggerStay;
        snowyEventHandler.TriggerStay -= snowyTriggerStay;
    }


    GameObject[] ConvertComponentArrayToGameObjectArray(Component[] compArr)
    {
        GameObject[] objArr = new GameObject[compArr.Length];
        int len = compArr.Length;
        for (int i = 0; i < len; i++)
            objArr[i] = compArr[i].gameObject;
        return objArr;
    }

}


/* 
 * 3D models
 * - sun: https://poly.google.com/view/8atTd-V-X7i
 * - raindrop: https://poly.google.com/view/fMyBNkWqMC-
 * - cloud: https://poly.google.com/view/fRmb17kTurm
 * - lightning: https://poly.google.com/view/7I1IhiE7O8s
 * - snowman: http://blog.sunday-model.net/article/178647073.html
 */