using System;
using UnityEngine;

public class AddAnimation : MonoBehaviour
{
    public GameObject _currentObject;
    public GameObject _parentObject;

    public bool bounce;
    private bool movementPrepare;
    public bool movement;
    public bool insertKeyframe = false;

    private Vector3[] pos;

    public GameObject animatedObject; // the object which moves along the path
    public GameObject keyframeObject; // object which is inserted as a key frame

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
    }

    // Update is called once per frame
    public void Update()
    {
        if (bounce)
        {
            // Check if there is user defined path
            pos = path.getPathPoints();
            // 1. There is a customized movement path, just follow the path
            if (pos.Length > 1)
            {

            }

            else addBouncingEffect();
        }

        if (movement)
        {
            // Check if there is user defined path
            pos = path.getPathPoints();
            if (insertKeyframe)
            {
                resetPath();
                insertKeyframe = false;
            }
            // 1. There is a customized movement path, just follow the path
            if(pos.Length > 1)
            {
                movementInit();
                followMovementPath();
            }

            // 2. There is no customized path, use the default behaviour
            else
            {
                defaultMovement();
            }
        }

        // press left/right arrow to move along the movement path
        if (Input.GetKey("right"))
        {
            insertKeyframe = true;
            moveAlong("right");
        } else if (Input.GetKey("left"))
        {
            insertKeyframe = true;
            moveAlong("left");
        }
    }

    public void FixedUpdate()
    {
        
    }

    private void addBouncingEffect()
    {
        SquashAndStretchKit.SquashAndStretch tem = _parentObject.AddComponent<SquashAndStretchKit.SquashAndStretch>();
        tem.enableSquash = true;
        tem.enableStretch = true;
        tem.maxSpeedThreshold = 20;
        tem.minSpeedThreshold = 1;
        tem.maxSquash = 1.6f;
        tem.maxStretch = 1.5f;
        _parentObject.AddComponent<Rigidbody>();
        _parentObject.AddComponent<BoxCollider>();

        Collider collider = _currentObject.AddComponent<BoxCollider>();
        collider.material.bounciness = 1.0f;
        FitColliderToChildren(_parentObject);
        bounce = false;  // necessary components have been added, so turned off the bool
        motionBrush.resetBrush();
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

    void movementInit()
    {
        if (movementPrepare) return;

        pos = path.getPathPoints();

        checkPos();
        movementPrepare = true;
    }

    void checkPos()
    {
        if (curIdx >= 0 && curIdx < pos.Length)
        {
            currentPosHolder = pos[curIdx];
        }
        else resetPath();
    }

    float percentsPerSecond = 0.15f; // %15 of the path moved per second
    float currentPathPercent = 0.0f; //min 0, max 1

    private void followMovementPath()
    {
        if (currentPathPercent >= 1)
        {
            // reset
            movement = false;
            currentPathPercent = 0;
        }

        currentPathPercent += percentsPerSecond * Time.deltaTime;
        Vector3 tarPos = Interp(pos, currentPathPercent);
        float distance = Vector3.Distance(currentPosHolder, animatedObject.transform.position);
        tarPos = currentPosHolder;  // TODO: tarpos and change moveSpeed*Time.deltaTime to moveSpeed

        animatedObject.transform.right = Vector3.RotateTowards(animatedObject.transform.right, tarPos - animatedObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
        animatedObject.transform.position = Vector3.MoveTowards(animatedObject.transform.position, tarPos, moveSpeed*Time.deltaTime);

        if (distance <= 0.3f)
        {
            curIdx += 1;
            checkPos();
        }
    }

    // use key to move along the movement path: left or right
    private void moveAlong(string direction)
    {
        movementInit();

        float distance = Vector3.Distance(currentPosHolder, animatedObject.transform.position);
        Vector3 tarPos = currentPosHolder;

        if (direction == "right")
        {
            animatedObject.transform.right = Vector3.RotateTowards(animatedObject.transform.right, tarPos - animatedObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
            animatedObject.transform.position = Vector3.MoveTowards(animatedObject.transform.position, tarPos, moveSpeed * Time.deltaTime);

            if (distance <= 0.3f)
            {
                curIdx += 1;
                checkPos();
            }
        }
        else if(direction == "left")
        {
            animatedObject.transform.right = Vector3.RotateTowards(animatedObject.transform.right, - tarPos + animatedObject.transform.position, rotationSpeed * Time.deltaTime, 0.0f);
            animatedObject.transform.position = Vector3.MoveTowards(animatedObject.transform.position, tarPos, moveSpeed * Time.deltaTime);

            if (distance <= 0.3f)
            {
                curIdx -= 1;
                checkPos();
            }
        }
    }

    void defaultMovement()
    {
        if (motionBrush.moveDirection == "right")
        {
            if (animatedObject.transform.position.x < 7)
                animatedObject.transform.Translate(Vector3.right * 9 * Time.deltaTime);
            else
            {
                motionBrush.resetBrush();
                movement = false;
            }
        }

        else if (motionBrush.moveDirection == "left")
        {
            if (animatedObject.transform.position.x > -7)
                animatedObject.transform.Translate(Vector3.left * 9 * Time.deltaTime);
            else
            {
                motionBrush.resetBrush();
                movement = false;
            }
        }

        else {
            Debug.LogError("No motion direction is given by speed lines!");
        }
    }

    void resetPath()
    {
        curIdx = 0;
        currentPathPercent = 0;
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
