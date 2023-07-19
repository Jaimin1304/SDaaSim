using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField]
    float speed;

    [SerializeField]
    float mouseSpeed;

    Vector3 lastMousePosition;

    void Start()
    {
        speed = Globals.camMovSpeed;
        mouseSpeed = Globals.camRotateSpeed;
    }

    void Update()
    {
        // Use WASD to move the camera around.
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Use space and left control to move the camera up and down.
        if (Input.GetKey(KeyCode.LeftControl))
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Space))
            transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Use the mouse to rotate the camera.
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            transform.Rotate(-delta.y * mouseSpeed, delta.x * mouseSpeed, 0);
            Vector3 rotationInEulerAngles = transform.rotation.eulerAngles;
            rotationInEulerAngles.z = 0; // set z rotation to zero
            transform.rotation = Quaternion.Euler(rotationInEulerAngles); // convert back to Quaternion
            lastMousePosition = Input.mousePosition;
        }
    }
}
