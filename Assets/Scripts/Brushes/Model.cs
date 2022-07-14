using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public GameObject rightHand;
    public MyOutline outline;

    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.AddComponent<MyOutline>();

        outline.OutlineMode = MyOutline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 15f;
        outline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (rightHand.transform.position == Vector3.zero) return;

        float dis = Vector3.Distance(transform.parent.gameObject.transform.position, rightHand.transform.position);
        
        if (dis <= 0.1f)
        {
            Debug.LogWarning(dis);
            outline.enabled = true;
            if (DrawTubes.buttonOneIsDown)
            {
                
            }
        }
       
        else
        {
            outline.enabled = false;
        }
    }
}
