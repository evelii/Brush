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
    public PanelParent panel;
    public string myTag;
    private Color initColor;

    public void Start()
    {
        rightHand = GameObject.Find("rightHand");
        panel = GameObject.Find("ResultPanel").GetComponent<PanelParent>();
        initColor = GetComponent<Button>().colors.normalColor;
        if (gameObject.name == "ResultA") myTag = "ResultA";
        else myTag = "ResultB";
    }

    public void Update()
    {
        float dis = Vector3.Distance(gameObject.transform.position, rightHand.gameObject.transform.position);
        if (dis <= 0.3f)
        {
            var colors = GetComponent<Button>().colors;
            colors.normalColor = new Color32(243, 188, 127, 255);
            GetComponent<Button>().colors = colors;
            if (myTag == "ResultA") panel.resultAOn = true;
            else panel.resultBOn = true;
            if (DrawTubes.buttonOneIsDown) recognitionResult.userChoice(text.text);

        } else
        {
            Debug.LogWarning("back!");
            var colors = GetComponent<Button>().colors;
            colors.normalColor = initColor;
            GetComponent<Button>().colors = colors;
            if (myTag == "ResultA") panel.resultAOn = false;
            else panel.resultBOn = false;

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