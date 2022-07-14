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
    public ControllerMode controllerMode;
    public MyOutline outline;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasHandler>();
        meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        rightHand = GameObject.Find("rightHand");
        tubes = GameObject.Find("Tubes").GetComponent<DrawTubes>();
        controllerMode = rightHand.GetComponent<ControllerMode>();

        outline = gameObject.AddComponent<MyOutline>();
        outline.OutlineMode = MyOutline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 15f;
        outline.enabled = false;
    }

    public void Update()
    {
        float dis = Vector3.Distance(gameObject.transform.position, rightHand.transform.position);
        if(dis <= 0.2f)
        {
            outline.enabled = true;
            controllerMode.SelectionMode();
        } else
        {
            outline.enabled = false;
            controllerMode.BrushMode();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canvas.curBrush != "SelectButton" || isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = enterColor;
        print("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canvas.curBrush != "SelectButton" || isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = normalColor;
        print("Exit");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvas.curBrush != "SelectButton") return; 
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = downColor;
        print("Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canvas.curBrush != "SelectButton" || isSelected) return;
        foreach (MeshRenderer render in meshRenderers)
            render.material.color = enterColor;
        print("Up");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canvas.curBrush != "SelectButton") return;
        isSelected = true;
        onClick.Invoke();
        print("Click");
        SketchManager.curSelected = gameObject.GetComponent<SketchEntity>();
    }
}
