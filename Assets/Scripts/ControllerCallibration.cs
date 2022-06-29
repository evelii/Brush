using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCallibration : MonoBehaviour
{
    public Transform ZedCameraEyes;
    public Transform virtualController;
    public Transform virtualBrush;
    public Vector3 initialOffset;


    private bool calibrating = false;
    private bool calibrated = false;
    private Vector3 delta_p = Vector3.zero;

    void Start()
    {
        virtualController.GetComponent<Renderer>().enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (calibrating)
                EndCalibration();
            else
                StartCalibration();

        }
    }

    void StartCalibration()
    {
        calibrated = false;
        virtualController.GetComponent<Renderer>().enabled = true;
        //virtualBrush.GetComponent<Renderer>().enabled = false;
        virtualController.parent = ZedCameraEyes;
        virtualController.localPosition = initialOffset;
        virtualController.localRotation = Quaternion.identity;
        virtualController.parent = null;
        calibrating = true;
        Debug.Log("Calibration Started");
    }

    void EndCalibration()
    {
        if (calibrating)
        {
            delta_p = virtualController.transform.position - transform.position;
            virtualController.GetComponent<Renderer>().enabled = false;
            //virtualBrush.GetComponent<Renderer>().enabled = true;
            calibrating = false;
            calibrated = true;
            Debug.Log("Calibration Ended");
        }
    }

    void LateUpdate()
    {
        if (calibrated)
        {
            transform.position += delta_p;
        }
    }
}
