using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeStroke : MonoBehaviour
{
    public SplineMaker splineMaker;
    private List<Vector3> _strokePoints;

    public int StrokesCount => _strokePoints.Count;
    public Vector3 LastStrokePoint => _strokePoints[_strokePoints.Count - 1];
    public Vector3[] StrokePoints => _strokePoints.ToArray();

    private bool _init = false;

    void Start()
    {
        if (!_init) init();
    }

    private void init()
    {
        TubeRenderer tube = GetComponent<TubeRenderer>();

        splineMaker = gameObject.AddComponent<SplineMaker>();
        splineMaker.pointsPerSegment = 16;
        splineMaker.onUpdated.AddListener((points) => tube.points = points);
        _init = true;
    }

    void Update()
    {
        
    }

    public void AddPoint(Vector3 p)
    {
        if (_strokePoints == null)
        {
            _strokePoints = new List<Vector3>();
        }
        _strokePoints.Add(p);
    }

    public List<Vector3> getStrokePoints() {
        return _strokePoints;
    }

    internal void UpdateAnchorPoints()
    {
        if (!_init) init();
        splineMaker.anchorPoints = StrokePoints;
    }

    internal bool CanUpdate()
    {
        if (!_init) init();
        return splineMaker.CanUpdate;
    }
}
