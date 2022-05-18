using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iTweenScript : MonoBehaviour
{
    public bool start;
    public PathFollower path;
    public float speed = 12.0f;

    public Vector3[] waypointArray;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            waypointArray = path.GetPathPoints();
            iTween.MoveTo(gameObject, iTween.Hash("path", waypointArray,
     "orienttopath", true, "looktime", 0.2f, "speed", speed, "easetype", iTween.EaseType.linear));
            start = false;
        }
    }
}
