using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{
    public Button sketchBtn;
    public Button selectBtn;
    public Button pathBtn;
    public Button motionBtn;
    public Button soundBtn;
    public string curBrush;

    void Start()
    {
        sketchBtn.Select();
        curBrush = "SketchButton";
        sketchBtn.onClick.AddListener(SketchTaskOnClick);
        selectBtn.onClick.AddListener(SelectTaskOnClick);
        pathBtn.onClick.AddListener(PathTaskOnClick);
        motionBtn.onClick.AddListener(MotionTaskOnClick);
        soundBtn.onClick.AddListener(SoundTaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (curBrush == "SketchButton")
        {
            sketchBtn.Select();  // highlight the button
        }
        else if (curBrush == "SelectButton")
        {
            selectBtn.Select();
        }
        else if (curBrush == "PathButton")
        {
            pathBtn.Select();  // highlight the button
        }
        else if (curBrush == "MotionButton")
        {
            motionBtn.Select();  // highlight the button
        }
        else if (curBrush == "SoundButton")
        {
            soundBtn.Select();  // highlight the button
        }
    }

    public void SketchTaskOnClick()
    {
        sketchBtn.Select();
        curBrush = "SketchButton";
    }

    public void SelectTaskOnClick()
    {
        selectBtn.Select();
        curBrush = "SelectButton";
    }

    public void PathTaskOnClick()
    {
        pathBtn.Select();
        curBrush = "PathButton";
    }

    public void MotionTaskOnClick()
    {
        motionBtn.Select();
        curBrush = "MotionButton";
    }

    public void SoundTaskOnClick()
    {
        soundBtn.Select();
        curBrush = "SoundButton";
    }
}
