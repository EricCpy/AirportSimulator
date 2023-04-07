using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private float movementSpeed = 10f, maxZoom = 50f, minZoom = 100f, zoomSpeed = 10f;
    public bool moveWithMouse { get; set; } = true;
    private Camera _camera;
    private Vector3 dragOrigin;
    private float CellSize { get => BuildingSystem.Instance.grid.GetCellSize(); }
    private float width, height;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }
    private void Start()
    {
        width = BuildingSystem.Instance.grid.GetWidth() * CellSize;
        height = BuildingSystem.Instance.grid.GetHeight() * CellSize;
        transform.position = new Vector3(width / 2, height / 2, -10);
    }
    void Update()
    {
        if (GameManager.Instance.uiOpen) return;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += transform.up * movementSpeed * Time.unscaledDeltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += -transform.up * movementSpeed * Time.unscaledDeltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += -transform.right * movementSpeed * Time.unscaledDeltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += transform.right * movementSpeed * Time.unscaledDeltaTime;
        }

        if (moveWithMouse)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 dir = dragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.position += dir;
            }
        }
        transform.position = new Vector3(Mathf.Min(Mathf.Max(transform.position.x, 0), width),
                                         Mathf.Min(Mathf.Max(transform.position.y, 0), height),
                                         transform.position.z);

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    public void Zoom(float intensity)
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - (intensity * zoomSpeed), minZoom, maxZoom);
    }
}
