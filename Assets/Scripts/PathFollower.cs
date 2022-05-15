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
    private LineRenderer _currKeyframeLine; 
    private Vector3 lastPos, curPos;
    public int numClicks = 0;
    public bool canDraw = true;

    public AddAnimation addAnimation;
    public DrawTubes drawTubes; // to retrieve stroke lists

    // Start is called before the first frame update
    void Start()
    {
        state = PathSetState.WAITING;
    }

    // Update is called once per frame
    void Update()
    {

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            _createNewPath();
            state = PathSetState.DRAW;
        }

        else if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
            state = PathSetState.WAITING;
        }

    }

    private void _createNewPath()
    {
        lastPos = transform.position;
        if(!addAnimation.insertKeyframe)
        {
            GameObject newPath = new GameObject("New Path");
            _currLine = newPath.AddComponent<LineRenderer>();
            _currLine.startWidth = .05f;
            _currLine.endWidth = .05f;
            _currLine.material.color = Color.white;
        } else
        {
            GameObject newPath = new GameObject("New Keyframe Path");
            _currKeyframeLine = newPath.AddComponent<LineRenderer>();
            _currKeyframeLine.startWidth = .05f;
            _currKeyframeLine.endWidth = .05f;
        }
        
        numClicks = 0;
    }

    public void FixedUpdate()
    {
        if (state == PathSetState.DRAW)
        {
            curPos = pathSetter.transform.position;

            if (curPos != lastPos)
            {  // when the controller is held
                if(!addAnimation.insertKeyframe)
                {
                    _currLine.positionCount = numClicks + 1;
                    _currLine.SetPosition(numClicks, curPos);
                } else
                {
                    _currKeyframeLine.positionCount = numClicks + 1;
                    _currKeyframeLine.SetPosition(numClicks, curPos);
                }
                
                numClicks++;
                lastPos = curPos;
            }
        }
    }

    public Vector3[] getPathPoints()
    {
        if (_currLine == null) return null;  // no path is drawn

        Vector3[] pos;
        if (!addAnimation.insertKeyframe || _currKeyframeLine == null)
        {
             pos = new Vector3[_currLine.positionCount];
            _currLine.GetPositions(pos);
        } else
        {
            pos = new Vector3[_currKeyframeLine.positionCount];
            _currKeyframeLine.GetPositions(pos);
        }
        
        return pos;
    }

    public Vector3[] getPathKeyframe()
    {
        Vector3[] pos = new Vector3[_currKeyframeLine.positionCount];
        _currKeyframeLine.GetPositions(pos);
        return pos;
    }
}
