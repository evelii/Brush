using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class MyMenuButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed;
    public TextMeshProUGUI text;
    public CanvasHandler canvas;
    public Button button;

    public GameObject rightHand;
    public MenuPanelParent panel;
    public string myTag;
    private Color initColor;
    private Color pressedColor;

    public void Start()
    {
        button = GetComponent<Button>();
        initColor = button.colors.normalColor;
        myTag = gameObject.name;
        pressedColor = new Color32(63, 139, 226, 255);
    }

    public void Update()
    {
        float dis = Vector3.Distance(gameObject.transform.position, rightHand.transform.position);
        if (dis <= 0.1f)
        {
            var colors = button.colors;
            colors.normalColor = pressedColor;
            button.colors = colors;
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
        else if(canvas.curBrush == myTag)
        {
            var colors = button.colors;
            colors.normalColor = pressedColor;
            button.colors = colors;
        }
        else
        {
            var colors = GetComponent<Button>().colors;
            colors.normalColor = initColor;
            GetComponent<Button>().colors = colors;
            if (myTag == "SketchButton") panel.sketchButton = false;
            else if (myTag == "PathButton") panel.pathButton = false;
            else if (myTag == "MotionButton") panel.motionButton = false;
            else if (myTag == "SoundButton") panel.soundButton = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
        if(text.text == "Sketch")
        {
            canvas.SketchTaskOnClick();
        } else if(text.text == "Select")
        {
            canvas.SelectTaskOnClick();
        } else if(text.text == "Path Brush")
        {
            canvas.PathTaskOnClick();
        } else if(text.text == "Motion Brush")
        {
            canvas.MotionTaskOnClick();
        } else if(text.text == "Sound Brush")
        {
            canvas.SoundTaskOnClick();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }
}
