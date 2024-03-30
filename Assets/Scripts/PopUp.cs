using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    void Update()
    {
        Vector3 lookDirection = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }
}
