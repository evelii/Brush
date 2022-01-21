using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{

    public GameObject ColorPickedPrefab;
    private ColorPickerTriangle CP;
    private GameObject go;
    public static ColorManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            go = (GameObject)Instantiate(ColorPickedPrefab, transform.position + Vector3.left * 3.0f + Vector3.up * 0.1f + Vector3.forward * 1.2f, Quaternion.Euler(new Vector3(0, 165, 0)));
            
            go.transform.rotation = Quaternion.Euler(new Vector3(0, 165, 0));
            go.transform.localScale = Vector3.one * 1.5f;
            //go.transform.LookAt(Camera.main.transform);
            CP = go.GetComponent<ColorPickerTriangle>();
            CP.TheColor = Color.black;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    //public void Update()
    //{
    //    Debug.Log("Color is " + GetColor());
    //}

    public Color GetColor()
    {
        return CP.TheColor;
    }
}
