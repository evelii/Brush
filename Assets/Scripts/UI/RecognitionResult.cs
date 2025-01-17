using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecognitionResult : MonoBehaviour
{
    public TCPClient client;
    public Transform cam;
    public GameObject canvas;
    public GameObject panel;
    public Button resultABtn;
    public Button resultBBtn;
    public Image imageA;
    public Image imageB;
    public GameObject resultAText;
    public GameObject resultBText;

    public TextMeshProUGUI textA;
    public TextMeshProUGUI textB;

    public Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
        textA = resultAText.GetComponent<TextMeshProUGUI>();
        textB = resultBText.GetComponent<TextMeshProUGUI>();
        resultABtn.onClick.AddListener(ButtonAOnClick);
        resultBBtn.onClick.AddListener(ButtonBOnClick);        
    }

    // Update is called once per frame
    void Update()
    {
        canvas.transform.LookAt(canvas.transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
    }

    public void ShowPredictionResults(string A, string B)
    {
        textA.SetText(A);
        string path = "Images/" + A + "_img"; // filename.png should be stored in your Assets/Resources folder
        sprite = Resources.Load<Sprite>(path);
        imageA.sprite = sprite;

        textB.SetText(B);
        path = "Images/" + B + "_img"; // filename.png should be stored in your Assets/Resources folder
        sprite = Resources.Load<Sprite>(path);
        imageB.sprite = sprite;

        //Vector3 offset = new Vector3(0.60f, 0.10f, 0.0f);
        Vector3 offset = new Vector3(0.10f, 0.10f, 0.5f);
        canvas.transform.position = SketchManager.curEditingObject.gameObject.transform.position;
        //canvas.transform.RotateAround(canvas.transform.position, Vector3.up, 180);
        canvas.SetActive(true);
    }

    void ButtonAOnClick()
    {
        client.userChoice = textA.text;
        canvas.SetActive(false);
    }

    void ButtonBOnClick()
    {
        client.userChoice = textB.text;
        canvas.SetActive(false);
    }

    public void UserChoice(string choice)
    {
        client.userChoice = choice;
        canvas.SetActive(false);
    }
}
