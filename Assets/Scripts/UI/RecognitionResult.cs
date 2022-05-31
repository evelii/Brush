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
    public GameObject resultA;
    public GameObject resultB;

    public TextMeshProUGUI textA;
    public TextMeshProUGUI textB;

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
        textA = resultA.GetComponent<TextMeshProUGUI>();
        textB = resultB.GetComponent<TextMeshProUGUI>();
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
}
