using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBrush : MonoBehaviour
{
    public PathSetState state;
    public Transform motionCursor; // a path cursor user used to defince the movement path
    private List<GameObject> motionLines;
    private LineRenderer _currLine; // path for the main object
    private LineRenderer _currKeyframeLine;
    private Vector3 lastPos, curPos;
    public int numClicks = 0;
    public bool canDraw = true;
    private int count = 0;

    public DrawTubes drawTubes; // to retrieve stroke lists
    public CanvasHandler canvas;
    public ControllerMode controllerMode;

    public bool ready = false;

    // Start is called before the first frame update
    void Start()
    {
        state = PathSetState.WAITING;
        motionLines = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

        if (ready && canvas.curBrush == "motion" && OVRInput.GetDown(OVRInput.Button.One))
        {
            _createNewPath();
            state = PathSetState.DRAW;
        }

        else if (canvas.curBrush == "motion" && OVRInput.GetUp(OVRInput.Button.One))
        {
            count++;
            state = PathSetState.WAITING;
        }

        if (count == 4)
        {
            if (SketchManager.curSelected != null) SketchManager.curSelected.aniStart = true;
            else SketchManager.curEditingObject.aniStart = true;

            // hide the motion lines from the display
            foreach (GameObject l in motionLines)
            {
                l.SetActive(false);
            }

            motionLines.Clear();
            count = 0;
        }
    }

    private void _createNewPath()
    {
        lastPos = transform.position;
        GameObject newLine = new GameObject("Motion Line");
        motionLines.Add(newLine);
        _currLine = newLine.AddComponent<LineRenderer>();
        _currLine.startWidth = .01f;
        _currLine.endWidth = .01f;
        _currLine.material.color = Color.green;
        numClicks = 0;
    }

    public void FixedUpdate()
    {
        if (state == PathSetState.DRAW)
        {
            curPos = motionCursor.transform.position;

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
