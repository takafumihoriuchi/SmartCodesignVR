using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmartObject : MonoBehaviour
{
    Component[] partComponents;
    GameObject[] partGameObjects;
    Material[] defaultMaterials;


    private void Start()
    {
        partComponents = GetComponentsInChildren<MeshRenderer>(true);
        partGameObjects = ConvertComponentArrayToGameObjectArray(partComponents);
        defaultMaterials = GetMaterialArray(partGameObjects);
    }

    private void Update()
    {
        
    }


    // todo use this in LightUpCard class
    // => smartObject.ApplyMaterial(matArr);
    public void ApplyMaterial(Material[] matArr)
    {
        int nParts = partGameObjects.Length;
        for (int i = 0; i < nParts; i++)
            partGameObjects[i].GetComponent<Renderer>().material = matArr[i];
    }


    private GameObject[] ConvertComponentArrayToGameObjectArray(Component[] cmpArr)
    {
        int len = cmpArr.Length;
        GameObject[] objArr = new GameObject[len];
        for (int i = 0; i < len; i++)
            objArr[i] = cmpArr[i].gameObject;
        return objArr;
    }


    private Material[] GetMaterialArray(GameObject[] objArr)
    {
        int len = objArr.Length;
        Material[] matArr = new Material[len];
        for (int i = 0; i < len; i++)
            matArr[i] = objArr[i].GetComponent<Renderer>().material;
        return matArr;
    }

    // このクラスに、ApplyMaterial() などのメソッドも集約する；このクラスにアクセスさせる

}
