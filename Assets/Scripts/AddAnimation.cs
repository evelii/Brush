using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAnimation : MonoBehaviour
{
    public SketchEntity sketch;
    public GameObject _childObject;
    public GameObject _animatedObject; // the object which moves along the path

    public bool movement;
    public bool insertKeyframe = false;

    private Vector3[] pos; // the movement path for the main object
    private Vector3[] posKeyframe; // the movement path for the keyframe object

    public Vector3 keyframePos; // the position of the animatedObject where key frame should be added
    public GameObject keyframeObject; // object which is inserted as a key frame
    public bool passedKeyframe = false;

    static Vector3 currentPosHolder;
    public PathFollower path;

    // Motion Brush
    public MotionBrush motionBrush;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        _animatedObject = SketchManager._parentObject;
        if (_animatedObject)
        {
            // press left/right arrow to move along the movement path
            if (Input.GetKey("right"))
            {
                insertKeyframe = true;
                sketch.TurnOnEditingMode();
                //MoveAlong("right");
                sketch.HideSoundMarks();
            }
            else if (Input.GetKey("left"))
            {
                insertKeyframe = true;
                sketch.TurnOnEditingMode();
                //MoveAlong("left");
                sketch.HideSoundMarks();
            }
        }
            
    }

    public void FixedUpdate()
    {
        
    }

    int curIdxKeyframe = 0;
    Vector3 currentPosHolderKeyframe;

    void CheckPos2()
    {
        if (curIdxKeyframe >= 0 && curIdxKeyframe < posKeyframe.Length)
        {
            currentPosHolderKeyframe = posKeyframe[curIdxKeyframe];
        }
    }

    void HandleKeyframe()
    {
        if(!insertKeyframe && keyframeObject) // if we are now out of the mode of insertingKeyframe and there's a sketch to be inserted
        {
            float dis = Vector3.Distance(_animatedObject.transform.position, keyframePos);
            if (dis <= 0.1f)
            {
                keyframeObject.SetActive(true);
                passedKeyframe = true;
            } else if (!passedKeyframe)
            {
                keyframeObject.SetActive(false); // hide the sketch
                curIdxKeyframe = 0;
                if (posKeyframe != null)
                {
                    keyframeObject.transform.position = posKeyframe[0];
                    currentPosHolderKeyframe = posKeyframe[0];
                }
            }
        }

        if(movement && posKeyframe==null && keyframeObject != null)
        {
            posKeyframe = path.GetPathKeyframe();
        }

        if(posKeyframe!=null && passedKeyframe)
        {
            float distance = Vector3.Distance(currentPosHolderKeyframe, keyframeObject.transform.position);

            keyframeObject.transform.right = Vector3.RotateTowards(keyframeObject.transform.right, currentPosHolderKeyframe - keyframeObject.transform.position, sketch.rotationSpeed * Time.deltaTime, 0.0f);
            keyframeObject.transform.position = Vector3.MoveTowards(keyframeObject.transform.position, currentPosHolderKeyframe, sketch.moveSpeed * 1.6f * Time.deltaTime);

            if (distance <= 0.3f)
            {
                curIdxKeyframe += 1;
                CheckPos2();
            }
        }
    } 

    private Vector3 Interp(Vector3[] pts, float t)
    {
        int numSections = pts.Length-3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }
}
