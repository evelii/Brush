using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandler : MonoBehaviour
{
    public Transform cam;
    public Button sketchBtn;
    public Button pathBtn;
    public Button motionBtn;
    public Button soundBtn;
    public string curBrush;
    private Color highlightColor;
    private Color normalColor;

    void Start()
    {
        sketchBtn.Select();
        curBrush = "sketch";
        sketchBtn.onClick.AddListener(SketchTaskOnClick);
        pathBtn.onClick.AddListener(PathTaskOnClick);
        motionBtn.onClick.AddListener(MotionTaskOnClick);
        soundBtn.onClick.AddListener(SoundTaskOnClick);

        float r = 150;  // red component
        float g = 208;  // green component
        float b = 243;  // blue component
        highlightColor = new Color(r, g, b);
        normalColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (curBrush == "sketch")
        {
            sketchBtn.Select();  // highlight the button
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

    void PathTaskOnClick()
    {
        sketchBtn.Select();
        curBrush = "path";
    }

    void MotionTaskOnClick()
    {
        sketchBtn.Select();
        curBrush = "motion";
    }

    void SoundTaskOnClick()
    {
        sketchBtn.Select();
        curBrush = "sound";
    }
}
