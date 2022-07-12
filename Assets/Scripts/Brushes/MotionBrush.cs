using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBrush : MonoBehaviour
{
    public PathSetState state;
    public Transform motionCursor; // a path cursor user used to defince the movement path
    private List<GameObject> motionLines;
    private LineRenderer _currLine; // path for the main object
    private Vector3 lastPos, curPos;
    public int numClicks = 0;
    public bool canDraw = true;
    public int count;
    private float totalLen = 0;
    private float baseline;

    public DrawTubes drawTubes; // to retrieve stroke lists
    public CanvasHandler canvas;
    public ControllerMode controllerMode;

    public bool ready = false;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        baseline = 0.1f;
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

        else if (canvas.curBrush == "motion" && OVRInput.GetUp(OVRInput.Button.One) && _currLine != null)
        {

            Vector3 pos = _currLine.GetPosition(0);
            for (int i = 1; i < _currLine.positionCount; i++)
            {
                totalLen += (_currLine.GetPosition(i) - pos).magnitude;
                pos = _currLine.GetPosition(i);
            }
            
            count++;
            state = PathSetState.WAITING;
        }

        if (count == 3)
        {
            // calculate the length of the speed lines
            float avgLen = totalLen / count;
            Debug.LogWarning("average length is " + avgLen);

            if (SketchManager.curSelected != null)
            {
                if (_currLine.GetPosition(0).x < SketchManager.curSelected.gameObject.transform.position.x) SketchManager.curSelected.defaultDirection = "right";
                else SketchManager.curSelected.defaultDirection = "left";
                SketchManager.curSelected.aniStart = true;
                SketchManager.curSelected.moveSpeed *= (avgLen / baseline);
            }
            else
            {
                if (_currLine.GetPosition(0).x < SketchManager.curEditingObject.gameObject.transform.position.x) SketchManager.curEditingObject.defaultDirection = "right";
                else SketchManager.curEditingObject.defaultDirection = "left";
                SketchManager.curEditingObject.aniStart = true;
                SketchManager.curEditingObject.moveSpeed *= (avgLen / baseline);
            }

            // hide the motion lines from the display
            foreach (GameObject l in motionLines)
            {
                l.SetActive(false);
            }

            motionLines.Clear();
            count = -1;
            totalLen = 0;
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
}
