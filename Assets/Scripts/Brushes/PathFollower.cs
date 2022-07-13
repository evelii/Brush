using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathSetState
{
    WAITING,
    START_SETTING,
    DRAW
}

public class PathFollower : MonoBehaviour
{
    public PathSetState state;
    public Transform pathSetter; // a path cursor user used to defince the movement path
    private LineRenderer _currLine; // path for the main object
    private Vector3 lastPos, curPos;
    public int numClicks = 0;
    public bool canDraw = true;

    public DrawTubes drawTubes; // to retrieve stroke lists
    public CanvasHandler canvas;
    public ControllerMode controllerMode;

    // Start is called before the first frame update
    void Start()
    {
        state = PathSetState.WAITING;
    }

    // Update is called once per frame
    void Update()
    {

        if (controllerMode.readyForSketch && canvas.curBrush == "PathButton" && OVRInput.GetDown(OVRInput.Button.One))
        {
            _createNewPath();
            state = PathSetState.DRAW;
        }

        else if (canvas.curBrush == "PathButton" && OVRInput.GetUp(OVRInput.Button.One))
        {
            state = PathSetState.WAITING;
        }

    }

    private void _createNewPath()
    {
        lastPos = transform.position;
         GameObject newPath = new GameObject("New Path");
        _currLine = newPath.AddComponent<LineRenderer>();
        _currLine.startWidth = .05f;
        _currLine.endWidth = .05f;
        _currLine.material.color = Color.white;
        
        numClicks = 0;
    }

    public void FixedUpdate()
    {
        if (state == PathSetState.DRAW)
        {
            curPos = pathSetter.transform.position;

            if (curPos != lastPos && _currLine != null)
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

        _currLine = null;
        return pos;
    }
}
