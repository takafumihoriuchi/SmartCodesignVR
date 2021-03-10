using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// こいつが"Smart Object"な訳だから、
// 他のクラスから受動的に変えられるより、能動的にこいつがセンシングして欲しい。そういう実装にする。
public class SmartObject : MonoBehaviour
{
    Component[] rigidBodyComponents;
    public GameObject[] rigidBodyGameObjects;

    Component[] meshRendComponents;
    public GameObject[] meshRendGameObjects;
    public Material[] defaultMaterials;


    private void Start()
    {
        rigidBodyComponents = GetComponentsInChildren<Rigidbody>(true);
        rigidBodyGameObjects = ConvertComponentArrayToGameObjectArray(rigidBodyComponents);

        meshRendComponents = GetComponentsInChildren<MeshRenderer>(true);
        meshRendGameObjects = ConvertComponentArrayToGameObjectArray(meshRendComponents);
        defaultMaterials = GetMaterialArray(meshRendGameObjects);
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// common methods
    /// </summary>
    

    /// <summary>
    /// LightUp-related methods
    /// </summary>

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

    public void ApplyMaterials(Material[] matArr)
    {
        int nParts = meshRendGameObjects.Length;
        for (int i = 0; i < nParts; i++)
            meshRendGameObjects[i].GetComponent<Renderer>().material = matArr[i];
    }

}
