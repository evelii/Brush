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
    public GameObject resultAText;
    public GameObject resultBText;

    public TextMeshProUGUI textA;
    public TextMeshProUGUI textB;

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
        textB.SetText(B);
        Vector3 offset = new Vector3(0, 0.09f, 0);
        canvas.transform.position = client.curObjectForRecognition.gameObject.transform.position + offset;
        canvas.SetActive(true);
    }

    void ButtonAOnClick()
    {
        Debug.LogWarning("A is selected");
    }

    void ButtonBOnClick()
    {
        Debug.LogWarning("B is selected");
    }
}
