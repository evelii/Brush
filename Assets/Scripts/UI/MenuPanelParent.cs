using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelParent : MonoBehaviour
{
    public bool sketchButton;
    public bool pathButton;
    public bool motionButton;
    public bool soundButton;

    public bool resultAOn;
    public bool resultBOn;

    public ControllerMode controllerMode;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (resultAOn || resultBOn || sketchButton || pathButton || motionButton || soundButton)
        {
            controllerMode.SelectionMode();
            if(OVRInput.GetDown(OVRInput.Button.One))
            {
                resultAOn = false;
                resultBOn = false;
                sketchButton = false;
                pathButton = false;
                motionButton = false;
                soundButton = false;
            }
            
        }
        else
        {
            controllerMode.BrushMode();
        }
    }
}
