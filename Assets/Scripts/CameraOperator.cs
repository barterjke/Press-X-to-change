using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOperator : MonoBehaviour
{
    public GameObject target;
    public float smoothness;

    void Update()
    {
        Vector3 targetPosition = Vector2.Lerp(transform.position, target.transform.position, Time.deltaTime / smoothness);
        targetPosition.z = transform.position.z;
        transform.position = targetPosition;
    }
}