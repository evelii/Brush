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

        if (ready && canvas.curBrush == "MotionButton" && OVRInput.GetDown(OVRInput.Button.One))
        {
            _createNewPath();
            state = PathSetState.DRAW;
        }

        else if (canvas.curBrush == "MotionButton" && OVRInput.GetUp(OVRInput.Button.One) && _currLine != null)
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
                Vector3 dir = _currLine.GetPosition(_currLine.positionCount - 1) - _currLine.GetPosition(0);
                Vector3 dir1 = dir / dir.magnitude;

                Vector3 dir2 = (SketchManager.curSelected.gameObject.transform.position - _currLine.GetPosition(0));
                Vector3 dirBetweenStrokeAndObject = dir2 / dir2.magnitude;

                // draw from left to right
                if (Vector3.Angle(dir1, dir2) < 90) SketchManager.curSelected.strokeDirection = dir1;
                // draw from right to left
                else SketchManager.curSelected.strokeDirection = -dir1;

                SketchManager.curSelected.aniStart = true;
                SketchManager.curSelected.moveSpeed *= (avgLen / baseline);
            }
            else
            {
                Vector3 dir = _currLine.GetPosition(_currLine.positionCount - 1) - _currLine.GetPosition(0);
                Vector3 dir1 = dir / dir.magnitude;
                
                Vector3 dir2 = (SketchManager.curEditingObject.gameObject.transform.position - _currLine.GetPosition(0));
                Vector3 dirBetweenStrokeAndObject = dir2 / dir2.magnitude;

                // draw from left to right
                if(Vector3.Angle(dir1, dir2) < 90) SketchManager.curEditingObject.strokeDirection = dir1;
                // draw from right to left
                else SketchManager.curEditingObject.strokeDirection = -dir1;


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
