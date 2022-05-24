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

    private MeshRenderer[] meshRenderers = null;
    bool isSelected = false;

    private void Awake()
    {
        meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = enterColor;
        print("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = normalColor;
        print("Exit");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = downColor;
        print("Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = enterColor;
        print("Up");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = true;
        onClick.Invoke();
        print("Click");
    }
}
