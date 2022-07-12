using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color enterColor = Color.white;
    [SerializeField] private Color downColor = Color.white;
    [SerializeField] private UnityEvent onClick = new UnityEvent();

    public CanvasHandler canvas;
    private MeshRenderer[] meshRenderers = null;
    bool isSelected = false;
    public GameObject rightHand;
    public DrawTubes tubes;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasHandler>();
        meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        rightHand = GameObject.Find("rightHand");
        tubes = GameObject.Find("Tubes").GetComponent<DrawTubes>();
    }

    public void Update()
    {
        float dis = Vector3.Distance(gameObject.transform.position, rightHand.transform.position);
        if(dis <= 0.2f)
        {
            foreach (MeshRenderer render in meshRenderers)
                render.material.color = enterColor;
        } else
        {
            foreach (MeshRenderer render in meshRenderers)
                render.material.color = tubes.strokeColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canvas.curBrush != "select" || isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = enterColor;
        print("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canvas.curBrush != "select" || isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = normalColor;
        print("Exit");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvas.curBrush != "select") return; 
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = downColor;
        print("Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canvas.curBrush != "select" || isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = enterColor;
        print("Up");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canvas.curBrush != "select") return;
        isSelected = true;
        onClick.Invoke();
        print("Click");
        SketchManager.curSelected = gameObject.GetComponent<SketchEntity>();
    }
}
