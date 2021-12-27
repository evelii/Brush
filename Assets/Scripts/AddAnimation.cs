using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAnimation : MonoBehaviour
{
    public GameObject _currentObject;
    public GameObject _parentObject;

    public bool bounce;
    private bool movementPrepare;
    public bool movement;

    private Vector3[] pos;

    public GameObject animatedObject; // the object which moves along the path
    public float moveSpeed; // the speed when moving along the path
    public float rotationSpeed;
    int curIdx;
    Vector3 startPos;
    static Vector3 currentPosHolder;
    public PathFollower path;

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
            addBouncingEffect();
        }

        if (movement)
        {
            movementInit();
            followMovementPath();
        }
    }

    public void FixedUpdate()
    {
        
    }

    private void addBouncingEffect()
    {
        _parentObject.AddComponent<Rigidbody>();
        Collider collider = _currentObject.AddComponent<BoxCollider>();
        collider.material.bounciness = 1.0f;
        FitColliderToChildren(_parentObject);
        bounce = false;  // necessary components have been added, so turned off the bool
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
        if (pos.Length == 0)
        {
            Debug.LogError("Movement path hasn't been specified yet!");
            throw new Exception();
        }
        checkPos();
        movementPrepare = true;
    }

    void checkPos()
    {
        if (curIdx < pos.Length) currentPosHolder = pos[curIdx];
        //startPos = animatedObject.transform.position;
    }

    /**
     * record timestamps and corresponding positions for both the movement path and the speed
     * 
     */
    private void followMovementPath()
    {
        float distance = Vector3.Distance(currentPosHolder, animatedObject.transform.position);
        animatedObject.transform.position = Vector3.MoveTowards(animatedObject.transform.position, currentPosHolder, Time.deltaTime * moveSpeed);

        var rotation = Quaternion.LookRotation(currentPosHolder - animatedObject.transform.position);
        animatedObject.transform.rotation = Quaternion.Slerp(animatedObject.transform.rotation, rotation, Time.deltaTime * rotationSpeed);

        if (distance <= 1.0f)
        {
            curIdx+=5;
            checkPos();
        }
    }
}
