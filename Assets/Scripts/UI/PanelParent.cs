using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelParent : MonoBehaviour
{
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
        if(resultAOn || resultBOn)
        {
            controllerMode.SelectionMode();
        } else
        {
            controllerMode.BrushMode();
        }
    }
}
