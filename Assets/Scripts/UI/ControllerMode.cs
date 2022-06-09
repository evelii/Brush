using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerMode : MonoBehaviour
{
    public GameObject laserPointer;
    public GameObject laserTip; // Sphere
    public GameObject brushTip;
    public Cursor cursorScript;
    public DrawTubes drawTubes;
    public CanvasHandler canvas;
    public MotionBrush motionBrush;
    public OVRRaycaster raycaster;

    public bool readyForSketch = true;

    // Start is called before the first frame update
    void Start()
    {
        laserPointer.GetComponent<LineRenderer>().enabled = false;
        laserPointer.SetActive(false);
        laserTip.SetActive(false);
        brushTip.SetActive(true);
        cursorScript.canDraw = true;
        motionBrush.ready = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.GetDown(OVRInput.Button.One))
        //{
        //    if (!readyForSketch)
        //    {
        //        laserPointer.GetComponent<LineRenderer>().enabled = false;
        //        laserPointer.SetActive(false);
        //        laserTip.SetActive(false);
        //        brushTip.SetActive(true);
        //        readyForSketch = true;
        //    }

        //    else if (readyForSketch && canvas.curBrush == "sketch")
        //    {
        //        cursorScript.canDraw = true;
        //    }

        //    else if (readyForSketch && canvas.curBrush == "motion") motionBrush.ready = true;
            
        //}
        //else if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        //{
        //    laserPointer.SetActive(true);
        //    laserPointer.GetComponent<LineRenderer>().enabled = true;
        //    laserTip.SetActive(true);
        //    brushTip.SetActive(false);
        //    drawTubes.state = StrokeState.WAITING;
        //    cursorScript.canDraw = false;
        //    readyForSketch = false;
        //    motionBrush.ready = false;
        //}

        if (OVRRaycaster.interactHit)
        {
            Debug.LogWarning("+++++++++++++++++++");
            laserPointer.SetActive(true);
            laserPointer.GetComponent<LineRenderer>().enabled = true;
            laserTip.SetActive(true);
            brushTip.SetActive(false);
            drawTubes.state = StrokeState.WAITING;
            cursorScript.canDraw = false;
            readyForSketch = false;
            motionBrush.ready = false;
        } else
        {
            if (!readyForSketch)
            {
                laserPointer.GetComponent<LineRenderer>().enabled = false;
                laserPointer.SetActive(false);
                laserTip.SetActive(false);
                brushTip.SetActive(true);
                readyForSketch = true;
            }

            else if (readyForSketch && canvas.curBrush == "sketch")
            {
                cursorScript.canDraw = true;
            }

            else if (readyForSketch && canvas.curBrush == "motion") motionBrush.ready = true;
        }
    }
}
