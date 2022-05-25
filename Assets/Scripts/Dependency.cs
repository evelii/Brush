using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dependency
{
    public SketchEntity depSketch;
    public Vector3 selfPos;
    public bool visible = false;
    public GameObject guardian;
    public Vector3 showupPos;

    public Dependency(SketchEntity sketch, Vector3 pos, GameObject guard)
    {
        depSketch = sketch;
        selfPos = pos;
        guardian = guard;
        //Physics.IgnoreCollision(guardian.GetComponent<Collider>(), GetComponent<Collider>());
    }
}
