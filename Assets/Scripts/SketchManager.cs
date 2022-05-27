using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchManager : MonoBehaviour
{
    public List<GameObject> sketchObjects; // all the sketches created by the user

    public static SketchManager manager;

    public static GameObject _parentObject;
    public static SketchEntity curEditingObject;
    public static SketchEntity curSelected;
    public static int labelCounter = 0;

    void Awake()
    {
        if (manager != null)
            GameObject.Destroy(manager);
        else
            manager = this;

        DontDestroyOnLoad(this);
    }
}
