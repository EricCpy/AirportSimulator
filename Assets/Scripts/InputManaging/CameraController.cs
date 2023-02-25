using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private float movementSpeed = 10f;
    public bool moveWithMouse { get; set; } = true;
    void Update()
    {
        if(Input.GetKey(KeyCode.UpArrow)) {
            transform.position += transform.up * movementSpeed * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.DownArrow)) {
            transform.position += -transform.up * movementSpeed * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.LeftArrow)) {
            transform.position += -transform.right * movementSpeed * Time.deltaTime;
        } else if(Input.GetKey(KeyCode.RightArrow)) {
            transform.position += transform.right * movementSpeed * Time.deltaTime;
        }
    }
}
