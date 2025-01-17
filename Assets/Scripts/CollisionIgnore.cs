using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionIgnore : MonoBehaviour
{
    public string label;
    public GameObject guardian;
    public List<GameObject> others = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AvoidCollision()
    {
        Physics.IgnoreCollision(guardian.GetComponent<Collider>(), GetComponent<Collider>());
        others.Add(guardian);
        Transform[] ts = guardian.GetComponentsInChildren<Transform>();
        foreach (Transform child in ts)
        {
            Physics.IgnoreCollision(child.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            others.Add(child.gameObject);
        }
        Debug.LogWarning(label);
    }
}
