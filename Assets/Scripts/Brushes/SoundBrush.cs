using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBrush : MonoBehaviour
{
    public PathSetState state;
    public Transform soundCursor; // a path cursor user used to defince the movement path
    private LineRenderer _currLine; // path for the main object
    private LineRenderer _currKeyframeLine;
    private Vector3 lastPos, curPos;
    public int numClicks = 0;
    public bool canDraw = true;

    public DrawTubes drawTubes; // to retrieve stroke lists
    public CanvasHandler canvas;
    public ControllerMode controllerMode;
    public GameObject cursor;
    public bool ready = false;

    private bool showSketchDone = false;

    // Start is called before the first frame update
    void Start()
    {
        state = PathSetState.WAITING;
    }

    // Update is called once per frame
    void Update()
    {
        if (!showSketchDone && controllerMode.readyForSketch && canvas.curBrush == "SoundButton" && OVRInput.GetDown(OVRInput.Button.One))
        {

        }

        else if (ready && canvas.curBrush == "SoundButton" && OVRInput.GetDown(OVRInput.Button.One))
        {
            _createNewPath();
            state = PathSetState.DRAW;
        }

        else if (canvas.curBrush == "SoundButton" && OVRInput.GetUp(OVRInput.Button.One))
        {
            state = PathSetState.WAITING;
        }
    }

    public bool isSketchShown()
    {
        return showSketchDone;
    }

    public void sketchSwitch(bool val)
    {
        showSketchDone = val;
    }

    private void _createNewPath()
    {
        lastPos = transform.position;
        GameObject newLine = new GameObject("Sound Line");
        if (SketchManager.curSelected != null) SketchManager.curSelected.AddSoundMark(newLine);
        else SketchManager.curEditingObject.AddSoundMark(newLine);
        _currLine = newLine.AddComponent<LineRenderer>();
        _currLine.startWidth = .01f;
        _currLine.endWidth = .01f;
        _currLine.material.color = Color.red;

        numClicks = 0;
    }

    public void FixedUpdate()
    {
        if (state == PathSetState.DRAW)
        {
            curPos = soundCursor.transform.position;

            if (curPos != lastPos)
            {  // when the controller is held
                 _currLine.positionCount = numClicks + 1;
                 _currLine.SetPosition(numClicks, curPos);

                numClicks++;
                lastPos = curPos;
            }
        }
    }

    public Vector3[] GetPathPoints()
    {
        if (_currLine == null) return null;  // no path is drawn

        Vector3[] pos;
        pos = new Vector3[_currLine.positionCount];
        _currLine.GetPositions(pos);

        return pos;
    }

    public Vector3[] GetPathKeyframe()
    {
        Vector3[] pos = new Vector3[_currKeyframeLine.positionCount];
        _currKeyframeLine.GetPositions(pos);
        return pos;
    }
}
