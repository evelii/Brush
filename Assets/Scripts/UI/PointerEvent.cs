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

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasHandler>();
        meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
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
        SketchManager._parentObject = gameObject;
        SketchManager.curEditingObject = gameObject.GetComponent<SketchEntity>();
    }
}
