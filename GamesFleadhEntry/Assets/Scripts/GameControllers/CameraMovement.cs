using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;
 
    void Start()
    {
        offset = new Vector3(0, 0, -20); 
    }

    void Update()
    {
        transform.position = player.transform.position + offset;
    }
}
