using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFloor : MonoBehaviour
{

    public Transform floor;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            floor.position = transform.position;
            floor.rotation = transform.rotation;
            floor.forward = -transform.forward;
            Vector3 fEuler = floor.rotation.eulerAngles;
            floor.rotation = Quaternion.Euler(0f, fEuler.y, fEuler.z);

            //floor.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
