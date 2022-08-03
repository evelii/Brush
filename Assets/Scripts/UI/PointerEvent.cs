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

    bool outlineAdded = false;

    string oldBrush;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<CanvasHandler>();
        meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        rightHand = GameObject.Find("rightHand");
        tubes = GameObject.Find("Tubes").GetComponent<DrawTubes>();
        controllerMode = rightHand.GetComponent<ControllerMode>();
    }

    public void Update()
    {
        if(canvas.curBrush != "SelectButton") oldBrush = canvas.curBrush;

        if (!outlineAdded)
        {
            outline = gameObject.AddComponent<MyOutline>();
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 15f;
            outline.enabled = false;
            outline.OutlineMode = MyOutline.Mode.OutlineAll;
            outlineAdded = true;
        }


        float dis = Vector3.Distance(gameObject.transform.position, rightHand.transform.position);
        if (dis <= 0.1f)
        {
            outline.enabled = true;
            controllerMode.SelectionMode();
            canvas.curBrush = "SelectButton";
            if(DrawTubes.buttonOneIsDown)
            {
                SketchManager.curSelected = gameObject.GetComponent<SketchEntity>();
            }
        }
        else
        {
            outline.enabled = false;
            controllerMode.BrushMode();
            canvas.curBrush = oldBrush;
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
