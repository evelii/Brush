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

    // Start is called before the first frame update
    void Start()
    {
        laserPointer.GetComponent<LineRenderer>().enabled = false;
        laserPointer.SetActive(false);
        laserTip.SetActive(false);
        brushTip.SetActive(true);
        cursorScript.canDraw = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            laserPointer.GetComponent<LineRenderer>().enabled = false;
            laserPointer.SetActive(false);
            laserTip.SetActive(false);
            brushTip.SetActive(true);
            cursorScript.canDraw = true;
        }
        else if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            laserPointer.SetActive(true);
            laserPointer.GetComponent<LineRenderer>().enabled = true;
            laserTip.SetActive(true);
            brushTip.SetActive(false);
            drawTubes.state = StrokeState.WAITING;
            cursorScript.canDraw = false;
        }
    }
}
