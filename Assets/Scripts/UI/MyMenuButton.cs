using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

public class MyMenuButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed;
    public TextMeshProUGUI text;
    public CanvasHandler canvas;

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
