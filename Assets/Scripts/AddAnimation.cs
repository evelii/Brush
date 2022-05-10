using System;
using UnityEngine;

public class AddAnimation : MonoBehaviour
{
    public SketchedObject sketch;
    public GameObject _childObject;
    public GameObject _animatedObject; // the object which moves along the path

    public bool bounce;
    private bool movementPrepare;
    public bool movement;
    public bool addCollider = true;
    public bool insertKeyframe = false;

    private Vector3[] pos; // the movement path for the main object
    private Vector3[] posKeyframe; // the movement path for the keyframe object

    public Vector3 keyframePos; // the position of the animatedObject where key frame should be added
    public GameObject keyframeObject; // object which is inserted as a key frame
    public bool passedKeyframe = false;

    public float moveSpeed; // the speed when moving along the path
    public float rotationSpeed;
    int curIdx;
    static Vector3 currentPosHolder;
    public PathFollower path;

    // Motion Brush
    public MotionBrush motionBrush;

    // Start is called before the first frame update
    void Start()
    {
        movementPrepare = false;
        addCollider = true;
    }

    // Update is called once per frame
    public void Update()
    {
        _animatedObject = SketchManager._parentObject;
        _childObject = SketchManager._curEditingObject;
        if (_animatedObject) sketch = SketchManager._parentObject.GetComponent<SketchedObject>();

        if ((bounce || movement) && addCollider)
        {
            _animatedObject.AddComponent<Rigidbody>();
            //_childObject.AddComponent<BoxCollider>();
            //_animatedObject.GetComponent<SketchedObject>().AddColliders();
            //FitColliderToChildren(_animatedObject);
            addCollider = false;
        }

        if (bounce)
        {
            _animatedObject.GetComponent<SketchedObject>().aniStart = true;
            // Check if there is user defined path
            pos = path.getPathPoints();
            // 1. There is a customized movement path, just follow the path
            if (pos.Length > 1)
            {

            }

            else AddBouncingEffect();
        }

        if (movement)
        {
            _animatedObject.GetComponent<SketchedObject>().aniStart = true;
            // Check if there is user defined path
            if (pos == null) pos = path.getPathPoints();
            if (insertKeyframe)
            {
                ResetPath();
                insertKeyframe = false;
                sketch.TurnOffEditingMode();
            }
            // 1. There is a customized movement path, just follow the path
            if(pos.Length > 1)
            {
                _animatedObject.GetComponent<Rigidbody>().useGravity = false;
                MovementInit();
                FollowMovementPath();
            }

            // 2. There is no customized path, use the default behaviour
            else
            {
                DefaultMovement();
            }
        }

        // press left/right arrow to move along the movement path
        if (Input.GetKey("right"))
        {
            insertKeyframe = true;
            sketch.TurnOnEditingMode();
            MoveAlong("right");
            sketch.HideSoundMarks();
        } else if (Input.GetKey("left"))
        {
            insertKeyframe = true;
            sketch.TurnOnEditingMode();
            MoveAlong("left");
            sketch.HideSoundMarks();
        }
    }

    public void FixedUpdate()
    {
        
    }

    private void AddBouncingEffect()
    {
        SquashAndStretchKit.SquashAndStretch tem = _animatedObject.AddComponent<SquashAndStretchKit.SquashAndStretch>();
        tem.enableSquash = true;
        tem.enableStretch = true;
        tem.maxSpeedThreshold = 20;
        tem.minSpeedThreshold = 1;
        tem.maxSquash = 1.6f;
        tem.maxStretch = 1.5f;
        //_parentObject.AddComponent<Rigidbody>();
        //_parentObject.AddComponent<BoxCollider>();

        //Collider collider = _childObject.GetComponent<BoxCollider>();
        //collider.material.bounciness = 1.0f;
        //FitColliderToChildren(_parentObject);
        bounce = false;  // necessary components have been added, so turned off the bool
        motionBrush.ResetBrush();
    }

    private void FitColliderToChildren(GameObject parentObj)
    {
        BoxCollider bc = parentObj.GetComponent<BoxCollider>();
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero); // center, size
        bool hasBounds = false;
        Renderer[] renderers = parentObj.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers)
        {
            if (hasBounds)
            {
                bounds.Encapsulate(render.bounds);
            }
            else
            {
                bounds = render.bounds;
                hasBounds = true;
            }
        }
        if (hasBounds)
        {
            bc.center = bounds.center - parentObj.transform.position;
            bc.size = bounds.size;
        }
        else
        {
            bc.size = bc.center = Vector3.zero;
            bc.size = Vector3.zero;
        }
    }

    //private void addBouncingEffect()
    //{
    //    _currentObject.AddComponent<Rigidbody>();
    //    Collider collider = _currentObject.AddComponent<SphereCollider>();
    //    collider.material.bounciness = 1.0f;
    //    SquashAndStretchKit.SquashAndStretch tem = _currentObject.AddComponent<SquashAndStretchKit.SquashAndStretch>();
    //    tem.enableSquash = true;
    //    tem.enableStretch = true;
    //    tem.maxSpeedThreshold = 20;
    //    tem.minSpeedThreshold = 1;
    //    tem.maxSquash = 1.6f;
    //    tem.maxStretch = 1.5f;

    //    bounce = false;  // necessary components have been added, so turned off the bool
    //}

    void MovementInit()
    {
        if (movementPrepare) return;

        if (pos == null) pos = path.getPathPoints();

        CheckPos();
        movementPrepare = true;
    }

    void CheckPos()
    {
        if (curIdx >= 0 && curIdx < pos.Length)
        {
            currentPosHolder = pos[curIdx];
        }
        else
        {
            ResetPath();
            //movement = false;
            passedKeyframe = false;
        }
    }

    float percentsPerSecond = 0.15f; // %15 of the path moved per second
    float currentPathPercent = 0.0f; //min 0, max 1

    private void FollowMovementPath()
    {
        if (currentPathPercent >= 1)
        {
            // reset
            movement = false;
            currentPathPercent = 0;
        }

        currentPathPercent += percentsPerSecond * Time.deltaTime;
        Vector3 tarPos = Interp(pos, currentPathPercent);
        float distance = Vector3.Distance(currentPosHolder, _animatedObject.transform.position);
        tarPos = currentPosHolder;  // TODO: tarpos and change moveSpeed*Time.deltaTime to moveSpeed

        _animatedObject.transform.right = Vector3.RotateTowards(_animatedObject.transform.right, tarPos - _animatedObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
        _animatedObject.transform.position = Vector3.MoveTowards(_animatedObject.transform.position, tarPos, moveSpeed*Time.deltaTime);

        if (distance <= 0.3f)
        {
            curIdx += 1;
            CheckPos();
        }

        HandleKeyframe();
    }

    // use key to move along the movement path: left or right
    private void MoveAlong(string direction)
    {
        MovementInit();

        float distance = Vector3.Distance(currentPosHolder, _animatedObject.transform.position);
        Vector3 tarPos = currentPosHolder;

        if (direction == "right")
        {
            _animatedObject.transform.right = Vector3.RotateTowards(_animatedObject.transform.right, tarPos - _animatedObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
            _animatedObject.transform.position = Vector3.MoveTowards(_animatedObject.transform.position, tarPos, moveSpeed * Time.deltaTime);

            if (distance <= 0.3f)
            {
                curIdx += 1;
                CheckPos();
            }
        }
        else if(direction == "left")
        {
            _animatedObject.transform.right = Vector3.RotateTowards(_animatedObject.transform.right, - tarPos + _animatedObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
            _animatedObject.transform.position = Vector3.MoveTowards(_animatedObject.transform.position, tarPos, moveSpeed * Time.deltaTime);

            if (distance <= 0.3f)
            {
                curIdx -= 1;
                CheckPos();
            }
        }
    }

    void DefaultMovement()
    {
        if (motionBrush.moveDirection == "right")
        {
            if (_animatedObject.transform.position.x < 7)
                _animatedObject.transform.Translate(Vector3.right * 9 * Time.deltaTime);
            else
            {
                motionBrush.ResetBrush();
                movement = false;
            }
        }

        else if (motionBrush.moveDirection == "left")
        {
            if (_animatedObject.transform.position.x > -7)
                _animatedObject.transform.Translate(Vector3.left * 9 * Time.deltaTime);
            else
            {
                motionBrush.ResetBrush();
                movement = false;
            }
        }

        else {
            Debug.LogError("No motion direction is given by speed lines!");
        }
    }

    void ResetPath()
    {
        curIdx = 0;
        currentPathPercent = 0;
        if (pos != null)
        {
            _animatedObject.transform.position = pos[0];
            currentPosHolder = pos[0];
        }
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
            posKeyframe = path.getPathKeyframe();
        }

        if(posKeyframe!=null && passedKeyframe)
        {
            float distance = Vector3.Distance(currentPosHolderKeyframe, keyframeObject.transform.position);

            keyframeObject.transform.right = Vector3.RotateTowards(keyframeObject.transform.right, currentPosHolderKeyframe - keyframeObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
            keyframeObject.transform.position = Vector3.MoveTowards(keyframeObject.transform.position, currentPosHolderKeyframe, moveSpeed * 1.6f * Time.deltaTime);

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

    public void AddColliderToSketch()
    {
        _animatedObject.GetComponent<SketchedObject>().AddColliders();
        FitColliderToChildren(_animatedObject);
    }
}
