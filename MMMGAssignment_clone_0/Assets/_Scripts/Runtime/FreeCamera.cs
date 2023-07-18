using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [SerializeField] private Camera _freecamera;
    [SerializeField] private Rigidbody _cameraRig;

    [SerializeField] private float _xSensitivity = 5f;
    [SerializeField] private float _ySensitivity = 5f;
    [SerializeField] private float _verticalMaxAngle = 80f;

    [SerializeField] private float _horizontalSpeed = 5f;
    [SerializeField] private float _verticalSpeed = 15f;

    private float _xRotation;
    private float _yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * _xSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _ySensitivity;

        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_verticalMaxAngle, _verticalMaxAngle);
        _freecamera.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        transform.rotation = Quaternion.Euler(0f, _yRotation, 0f);

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        float y = Input.mouseScrollDelta.y;
        if (y < 0)
        {
            y = -1f;
        }
        else if (y > 0)
        {
            y = 1f;
        }
        else
            y = 0f;

        Vector3 forceToAdd = transform.forward * z * _horizontalSpeed + transform.right * x * _horizontalSpeed + transform.up * y * _verticalSpeed;

        _cameraRig.AddForce(forceToAdd);
    }
}
