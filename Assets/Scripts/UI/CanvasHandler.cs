using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{
    public Transform cam;
    public Button sketchBtn;
    public Button selectBtn;
    public Button pathBtn;
    public Button motionBtn;
    public Button soundBtn;
    public string curBrush;

    void Start()
    {
        sketchBtn.Select();
        curBrush = "sketch";
        sketchBtn.onClick.AddListener(SketchTaskOnClick);
        selectBtn.onClick.AddListener(SelectTaskOnClick);
        pathBtn.onClick.AddListener(PathTaskOnClick);
        motionBtn.onClick.AddListener(MotionTaskOnClick);
        soundBtn.onClick.AddListener(SoundTaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (curBrush == "sketch")
        {
            sketchBtn.Select();  // highlight the button
        }
        else if (curBrush == "select")
        {
            selectBtn.Select();
        }
        else if (curBrush == "path")
        {
            pathBtn.Select();  // highlight the button
        }
        else if (curBrush == "motion")
        {
            motionBtn.Select();  // highlight the button
        }
        else if (curBrush == "sound")
        {
            soundBtn.Select();  // highlight the button
        }
    }

    void SketchTaskOnClick()
    {
        sketchBtn.Select();
        curBrush = "sketch";
    }

    void SelectTaskOnClick()
    {
        selectBtn.Select();
        curBrush = "select";
    }

    void PathTaskOnClick()
    {
        pathBtn.Select();
        curBrush = "path";
    }

    void MotionTaskOnClick()
    {
        motionBtn.Select();
        curBrush = "motion";
    }

    void SoundTaskOnClick()
    {
        soundBtn.Select();
        curBrush = "sound";
    }
}
