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
    const int STORMY_IDX = 3;
    const int SNOWY_IDX = 4;

    readonly string SUNNY_STR = "be sunny";
    readonly string CLOUDY_STR = "be cloudy";
    readonly string RAINY_STR = "rain";
    readonly string STORMY_STR = "be a storm";
    readonly string SNOWY_STR = "snow";

    IComponentEventHandler[] weatherEventHandler = new IComponentEventHandler[N];
    GameObject[] weatherObjGrp = new GameObject[N];
    GameObject[] weatherRay = new GameObject[N];
    bool[] isXxxCurrentFrame = new bool[N];
    bool[] wasXxxPrevFrame = new bool[N];
    AudioSource[] weatherSound = new AudioSource[N];


    public WeatherCard()
    {
        maxInstanceNum = N;

        cardName = "Weather";

        descriptionText = "<i>I can get information about the weather forecast.</i>\n"
            + "<b>Steps:</b>\n"
            + "<b>1.</b> <indent=10%>Grab the weather representative by holding the controller trigger.</indent>\n"
            + "<b>2.</b> <indent=10%>Project the ray to the object.</indent>\n"
            + "Maximum number of instances: 5";

        contentText = "When it is about to";

        inputEvalDeleDict = new Dictionary<string, InputEvaluationDelegate>
        {
            {SUNNY_STR, SunnyForecast},
            {CLOUDY_STR, CloudyForecast},
            {RAINY_STR, RainyForecast},
            {STORMY_STR, StormyForecast},
            {SNOWY_STR, SnowyForecast}
        };
    }

    // constantly updates current weather with one-frame delay
    public bool SunnyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = wasXxxPrevFrame[SUNNY_IDX];
        UpdateWeatherWithFalse(SUNNY_IDX);
        return tmp;
    }
    public bool CloudyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = wasXxxPrevFrame[CLOUDY_IDX];
        UpdateWeatherWithFalse(CLOUDY_IDX);
        return tmp;
    }
    public bool RainyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = wasXxxPrevFrame[RAINY_IDX];
        UpdateWeatherWithFalse(RAINY_IDX);
        return tmp;
    }
    public bool StormyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = wasXxxPrevFrame[STORMY_IDX];
        UpdateWeatherWithFalse(STORMY_IDX);
        return tmp;
    }
    public bool SnowyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = wasXxxPrevFrame[SNOWY_IDX];
        UpdateWeatherWithFalse(SNOWY_IDX);
        return tmp;
    }


    protected override void InitPropFields()
    {
        envPartsComponent = environmentObject.GetComponentsInChildren<Rigidbody>(true);
        envPartsGameObject = ConvertComponentArrayToGameObjectArray(envPartsComponent);

        string[] objGrpPath = new string[N] { "sunny", "cloudy", "rainy", "stormy", "snowy" };
        string[] rayPath = new string[N] { "sunny/ray", "cloudy/ray", "rainy/ray", "stormy/ray", "snowy/ray" };

        for (int i = 0; i < N; i++)
        {
            weatherObjGrp[i] = propObjects.transform.Find(objGrpPath[i]).gameObject;
            weatherRay[i] = propObjects.transform.Find(rayPath[i]).gameObject;
            weatherEventHandler[i] = weatherRay[i].RequestEventHandlers();
            weatherSound[i] = weatherObjGrp[i].GetComponent<AudioSource>();
            weatherRay[i].SetActive(false);
        }
    }


    public override void BehaviourDuringPrototyping()
    {
        SetGrabbedWeatherRayAll();
    }


    protected override void OnFocusGranted()
    {
        weatherEventHandler[SUNNY_IDX].TriggerEnter += sunnyTriggerEnter;
        weatherEventHandler[CLOUDY_IDX].TriggerEnter += cloudyTriggerEnter;
        weatherEventHandler[RAINY_IDX].TriggerEnter += rainyTriggerEnter;
        weatherEventHandler[STORMY_IDX].TriggerEnter += stormyTriggerEnter;
        weatherEventHandler[SNOWY_IDX].TriggerEnter += snowyTriggerEnter;
    }

    protected override void OnFocusDeprived()
    {
        weatherEventHandler[SUNNY_IDX].TriggerEnter -= sunnyTriggerEnter;
        weatherEventHandler[CLOUDY_IDX].TriggerEnter -= cloudyTriggerEnter;
        weatherEventHandler[RAINY_IDX].TriggerEnter -= rainyTriggerEnter;
        weatherEventHandler[STORMY_IDX].TriggerEnter -= stormyTriggerEnter;
        weatherEventHandler[SNOWY_IDX].TriggerEnter -= snowyTriggerEnter;
    }

    protected override void OnConfirm()
    {
        if (isFocused)
        {
            weatherEventHandler[SUNNY_IDX].TriggerEnter -= sunnyTriggerEnter;
            weatherEventHandler[CLOUDY_IDX].TriggerEnter -= cloudyTriggerEnter;
            weatherEventHandler[RAINY_IDX].TriggerEnter -= rainyTriggerEnter;
            weatherEventHandler[STORMY_IDX].TriggerEnter -= stormyTriggerEnter;
            weatherEventHandler[SNOWY_IDX].TriggerEnter -= snowyTriggerEnter;
        }
        weatherEventHandler[SUNNY_IDX].TriggerStay += sunnyTriggerStay;
        weatherEventHandler[CLOUDY_IDX].TriggerStay += cloudyTriggerStay;
        weatherEventHandler[RAINY_IDX].TriggerStay += rainyTriggerStay;
        weatherEventHandler[STORMY_IDX].TriggerStay += stormyTriggerStay;
        weatherEventHandler[SNOWY_IDX].TriggerStay += snowyTriggerStay;
    }
    protected override void OnBackToEdit()
    {
        if (isFocused)
        {
            weatherEventHandler[SUNNY_IDX].TriggerEnter += sunnyTriggerEnter;
            weatherEventHandler[CLOUDY_IDX].TriggerEnter += cloudyTriggerEnter;
            weatherEventHandler[RAINY_IDX].TriggerEnter += rainyTriggerEnter;
            weatherEventHandler[STORMY_IDX].TriggerEnter += stormyTriggerEnter;
            weatherEventHandler[SNOWY_IDX].TriggerEnter += snowyTriggerEnter;
        }
        weatherEventHandler[SUNNY_IDX].TriggerStay -= sunnyTriggerStay;
        weatherEventHandler[CLOUDY_IDX].TriggerStay -= cloudyTriggerStay;
        weatherEventHandler[RAINY_IDX].TriggerStay -= rainyTriggerStay;
        weatherEventHandler[STORMY_IDX].TriggerStay -= stormyTriggerStay;
        weatherEventHandler[SNOWY_IDX].TriggerStay -= snowyTriggerStay;
    }


    void sunnyTriggerEnter(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            ConditionKeyword = SUNNY_STR;
    }
    void cloudyTriggerEnter(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            ConditionKeyword = CLOUDY_STR;
    }
    void rainyTriggerEnter(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            ConditionKeyword = RAINY_STR;
    }
    void stormyTriggerEnter(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            ConditionKeyword = STORMY_STR;
    }
    void snowyTriggerEnter(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            ConditionKeyword = SNOWY_STR;
    }

    void sunnyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            UpdateWeatherWithTrue(SUNNY_IDX);
    }
    void cloudyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            UpdateWeatherWithTrue(CLOUDY_IDX);
    }
    void rainyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            UpdateWeatherWithTrue(RAINY_IDX);
    }
    void stormyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            UpdateWeatherWithTrue(STORMY_IDX);
    }
    void snowyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            UpdateWeatherWithTrue(SNOWY_IDX);
    }

    void UpdateWeatherWithTrue(int weatherIdx)
    {
        wasXxxPrevFrame[weatherIdx] = isXxxCurrentFrame[weatherIdx];
        isXxxCurrentFrame[weatherIdx] = true;
    }

    void UpdateWeatherWithFalse(int weatherIdx)
    {
        wasXxxPrevFrame[weatherIdx] = isXxxCurrentFrame[weatherIdx];
        isXxxCurrentFrame[weatherIdx] = false;
    }

    void SetGrabbedWeatherRayAll()
    {
        for (int i = 0; i < N; i++)
        {
            bool isGrabbed = weatherObjGrp[i].GetComponent<OVRGrabbable>().isGrabbed;
            weatherRay[i].SetActive(isGrabbed);
            if (isGrabbed && !weatherSound[i].isPlaying) weatherSound[i].Play();
            else if (!isGrabbed && weatherSound[i].isPlaying) weatherSound[i].Stop();
        }
    }

    GameObject[] ConvertComponentArrayToGameObjectArray(Component[] compArr)
    {
        GameObject[] objArr = new GameObject[compArr.Length];
        int len = compArr.Length;
        for (int i = 0; i < len; i++)
            objArr[i] = compArr[i].gameObject;
        return objArr;
    }

    bool IsEnvObj(GameObject obj)
    {
        return (Array.IndexOf(envPartsGameObject, obj) != -1);
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