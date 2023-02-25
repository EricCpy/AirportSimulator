using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private float movementSpeed = 10f, maxZoom = 50f, minZoom = 100f, zoomSpeed = 10f;
    public bool moveWithMouse { get; set; } = true;
    private Camera _camera;
    private Vector3 dragOrigin;
    private void Awake() {
        _camera = GetComponent<Camera>();
    }
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
        ;
        if(moveWithMouse == true) {
            if(Input.GetMouseButtonDown(0)) {
                dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if(Input.GetMouseButton(0)) {
                Vector3 dir = dragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.position += dir;
            }
        }
        
        if(Input.GetAxis("Mouse ScrollWheel") != 0f) {
            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    public void Zoom(float intensity) {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - (intensity * zoomSpeed), minZoom, maxZoom);
    }
}
