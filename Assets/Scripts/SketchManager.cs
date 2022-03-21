using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SketchManager : MonoBehaviour
{
    public List<GameObject> sketchObjects; // all the sketches created by the user

    public static SketchManager manager;

    void Awake()
    {
        if (manager != null)
            GameObject.Destroy(manager);
        else
            manager = this;

        DontDestroyOnLoad(this);
    }
}