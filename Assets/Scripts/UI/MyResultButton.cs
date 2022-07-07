using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

public class MyResultButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed;
    public RecognitionResult recognitionResult;
    public TextMeshProUGUI text;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
        recognitionResult.userChoice(text.text);
        OVRRaycaster.intersectHit = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }
}