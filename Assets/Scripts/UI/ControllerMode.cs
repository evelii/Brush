using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerMode : MonoBehaviour
{
    public GameObject laserPointer;
    public GameObject laserTip; // Sphere
    public GameObject brushTip;
    public ControllerCursor cursorScript;
    public DrawTubes drawTubes;
    public CanvasHandler canvas;
    public MotionBrush motionBrush;
    public PathFollower pathBrush;
    public SoundBrush soundBrush;
    public OVRRaycaster raycaster;

    public bool readyForSketch = true;

    // Start is called before the first frame update
    void Start()
    {
        //laserPointer.GetComponent<LineRenderer>().enabled = false;
        //laserPointer.SetActive(false);
        //laserTip.SetActive(false);
        brushTip.SetActive(true);
        cursorScript.canDraw = true;
        motionBrush.ready = false;
        pathBrush.ready = false;
        soundBrush.ready = false;
    }

    // Update is called once per frame
    void Update()
    {
       if (!readyForSketch)
        {
            //laserPointer.GetComponent<LineRenderer>().enabled = false;
            //laserPointer.SetActive(false);
            //laserTip.SetActive(false);
            brushTip.SetActive(true);
            readyForSketch = true;
        }

        else if (readyForSketch && canvas.curBrush == "SketchButton")
        {
            cursorScript.canDraw = true;
        }

        else if (readyForSketch && canvas.curBrush == "MotionButton") motionBrush.ready = true;

        else if (readyForSketch && canvas.curBrush == "PathButton") pathBrush.ready = true;

        else if (readyForSketch && canvas.curBrush == "SoundButton") soundBrush.ready = true;
    }

    public void SelectionMode()
    {
        brushTip.SetActive(false);
        drawTubes.state = StrokeState.WAITING;
        cursorScript.canDraw = false;
        readyForSketch = false;
        motionBrush.ready = false;
        pathBrush.ready = false;
        soundBrush.ready = false;
    }

    public void BrushMode()
    {
        if (!readyForSketch)
        {
            brushTip.SetActive(true);
            readyForSketch = true;
        }

        else if (readyForSketch && canvas.curBrush == "SketchButton")
        {
            cursorScript.canDraw = true;
        }

        else if (readyForSketch && canvas.curBrush == "MotionButton") motionBrush.ready = true;

        else if (readyForSketch && canvas.curBrush == "PathButton") pathBrush.ready = true;

        else if (readyForSketch && canvas.curBrush == "SoundButton") soundBrush.ready = true;
    }
}
