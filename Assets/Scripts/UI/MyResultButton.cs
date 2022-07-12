using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class MyResultButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool buttonPressed;
    public RecognitionResult recognitionResult;
    public TextMeshProUGUI text;
    public GameObject rightHand;
    private Color initColor;

    public void Start()
    {
        rightHand = GameObject.Find("rightHand");
        initColor = GetComponent<Button>().colors.normalColor;
    }

    public void Update()
    {
        float dis = Vector3.Distance(gameObject.transform.position, rightHand.gameObject.transform.position);
        if (dis <= 0.3f)
        {
            var colors = GetComponent<Button>().colors;
            colors.normalColor = new Color32(243, 188, 127, 255);
            GetComponent<Button>().colors = colors;
            
        } else
        {
            var colors = GetComponent<Button>().colors;
            colors.normalColor = initColor;
            GetComponent<Button>().colors = colors;
        }
    }

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