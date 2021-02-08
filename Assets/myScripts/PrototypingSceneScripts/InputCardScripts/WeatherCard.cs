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
        bool tmp = isXxxCurrentFrame[SUNNY_IDX];
        isXxxCurrentFrame[SUNNY_IDX] = false;
        return tmp;
    }
    public bool CloudyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = isXxxCurrentFrame[CLOUDY_IDX];
        isXxxCurrentFrame[CLOUDY_IDX] = false;
        return tmp;
    }
    public bool RainyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = isXxxCurrentFrame[RAINY_IDX];
        isXxxCurrentFrame[RAINY_IDX] = false;
        return tmp;
    }
    public bool StormyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = isXxxCurrentFrame[STORMY_IDX];
        isXxxCurrentFrame[STORMY_IDX] = false;
        return tmp;
    }
    public bool SnowyForecast()
    {
        SetGrabbedWeatherRayAll();
        bool tmp = isXxxCurrentFrame[SNOWY_IDX];
        isXxxCurrentFrame[SNOWY_IDX] = false;
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
            isXxxCurrentFrame[SUNNY_IDX] = true;
    }
    void cloudyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            isXxxCurrentFrame[CLOUDY_IDX] = true;
    }
    void rainyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            isXxxCurrentFrame[RAINY_IDX] = true;
    }
    void stormyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            isXxxCurrentFrame[STORMY_IDX] = true;
    }
    void snowyTriggerStay(Collider other) {
        if (IsEnvObj(other.transform.gameObject))
            isXxxCurrentFrame[SNOWY_IDX] = true;
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