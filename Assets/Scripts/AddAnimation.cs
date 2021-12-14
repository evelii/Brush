using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAnimation : MonoBehaviour
{
    public GameObject _currentObject;
    private Transform _squashParent;
    
    private Vector3 _originalScale;

    public bool bounce;
    private bool ss; // Stretch and Squash
    private bool movementPrepare;
    public bool movement;

    private Vector3[] pos;

    public GameObject animatedObject; // the object which moves along the path
    public float moveSpeed; // the speed when moving along the path
    float timer; // default time
    int curIdx;
    Vector3 startPos;
    static Vector3 currentPosHolder;
    public PathFollower path;

    // Start is called before the first frame update
    void Start()
    {
        ss = false;
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
        if (ss)
        {
            _currentObject.transform.parent = transform;
            _currentObject.transform.localPosition = Vector3.zero;
            _currentObject.transform.localScale = _originalScale;
            _currentObject.transform.localRotation = Quaternion.identity;

            transform.localScale = Vector3.one;

            Rigidbody _rigidbody = _currentObject.GetComponent<Rigidbody>();
            var velocity = _rigidbody.velocity; // falling down: speed value is negative

            if (velocity.sqrMagnitude > 0.01f)
            {
                _squashParent.rotation = Quaternion.FromToRotation(Vector3.right, velocity);
            }

            //Debug.LogError(velocity.magnitude);

            var scaleX = 1.0f + (velocity.magnitude * 0.1f);
            var scaleY = 1.0f / scaleX;

            transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
            //_currentObject.transform.parent = _squashParent;
            //transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
        }
    }

    private void addBouncingEffect()
    {
        _currentObject.AddComponent<Rigidbody>();
        Collider collider = _currentObject.AddComponent<SphereCollider>();
        collider.material.bounciness = 1.0f;
        bounce = false;  // necessary components have been added, so turned off the bool
        //ss = true;
    }

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
        timer = 0;
        if (curIdx < pos.Length) currentPosHolder = pos[curIdx];
        startPos = animatedObject.transform.position;
    }

    private void followMovementPath()
    {
        timer += Time.deltaTime * moveSpeed;
        if (animatedObject.transform.position != currentPosHolder)
        {
            animatedObject.transform.position = Vector3.Lerp(startPos, currentPosHolder, timer);
        } else
        {
            curIdx++;
            checkPos();
        }
    }
}
