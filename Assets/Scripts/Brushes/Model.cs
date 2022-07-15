using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public GameObject rightHand;
    public MyOutline outline;

    public CanvasHandler canvas;
    public MenuPanelParent panel;
    public string myTag;

    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.AddComponent<MyOutline>();
        outline.OutlineMode = MyOutline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 15f;
        outline.enabled = false;

        myTag = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (rightHand.transform.position == Vector3.zero) return;

        float dis = Vector3.Distance(transform.parent.gameObject.transform.position, rightHand.transform.position);
        
        if (dis <= 0.1f)
        {
            //Debug.LogWarning(dis);
            outline.enabled = true;
            if (myTag == "SketchButton") panel.sketchButton = true;
            else if (myTag == "PathButton") panel.pathButton = true;
            else if (myTag == "MotionButton") panel.motionButton = true;
            else if (myTag == "SoundButton") panel.soundButton = true;
            if (DrawTubes.buttonOneIsDown)
            {
                if (myTag == "SketchButton")
                {
                    canvas.SketchTaskOnClick();
                }
                else if (myTag == "PathButton")
                {
                    canvas.PathTaskOnClick();
                }
                else if (myTag == "MotionButton")
                {
                    canvas.MotionTaskOnClick();
                }
                else if (myTag == "SoundButton")
                {
                    canvas.SoundTaskOnClick();
                }
            }
        }
        else if (canvas.curBrush == myTag)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
            if (myTag == "SketchButton") panel.sketchButton = false;
            else if (myTag == "PathButton") panel.pathButton = false;
            else if (myTag == "MotionButton") panel.motionButton = false;
            else if (myTag == "SoundButton") panel.soundButton = false;
        }
    }
}
