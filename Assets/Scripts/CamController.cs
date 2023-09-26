using UnityEngine;

public class CamController : MonoBehaviour
{
    Vector3 lastMousePosition;

    public enum State
    {
        Free,
        Focus,
    }

    [SerializeField]
    State currentState;

    [SerializeField]
    GameObject centerObject;

    Vector3 focusModeOffset;

    void Start()
    {
        ToFreeMode();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Free:
                FreeModeLogic();
                break;
            case State.Focus:
                FocusModeLogic();
                break;
            default:
                Debug.LogError("Unknown camera state: " + currentState.ToString());
                break;
        }
    }

    void FreeModeLogic()
    {
        if (Simulator.instance != null && Simulator.instance.CurrentState == Simulator.State.Freeze)
        {
            return;
        }
        // Use WASD to move the camera around.
        float speed = Globals.CamMovSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed = Globals.CamSprintSpeed;
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
            Debug.Log(Globals.CamRotateSpeed);
            Debug.Log(delta);
            transform.Rotate(
                -delta.y * Globals.CamRotateSpeed,
                delta.x * Globals.CamRotateSpeed,
                0
            );
            Vector3 rotationInEulerAngles = transform.rotation.eulerAngles;
            rotationInEulerAngles.z = 0; // set z rotation to zero
            transform.rotation = Quaternion.Euler(rotationInEulerAngles); // convert back to Quaternion
            lastMousePosition = Input.mousePosition;
        }
    }

    void FocusModeLogic()
    {
        transform.position = centerObject.transform.position + focusModeOffset;
        // Check for key presses to switch back to free mode
        if (
            Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.LeftControl)
            || Input.GetKey(KeyCode.Space)
        )
        {
            ToFreeMode();
            return;
        }
        // Assuming centerObject is the object you want to rotate around.
        Vector3 rotateAroundPoint = centerObject.transform.position;

        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            // Rotate around the up axis of the world based on horizontal mouse movement
            transform.RotateAround(rotateAroundPoint, Vector3.up, delta.x * Globals.CamRotateSpeed);
            // Rotate around the right axis of the camera based on vertical mouse movement
            transform.RotateAround(
                rotateAroundPoint,
                transform.right,
                -delta.y * Globals.CamRotateSpeed
            );
            lastMousePosition = Input.mousePosition;
        }
        // get the distance
        float distance = Vector3.Distance(transform.position, rotateAroundPoint);
        // apply zoom smoother
        float smoothZoomSpeed =
            (
                -1
                * Globals.CamZoomSpeed
                * distance
                * distance
                / (Globals.camMaxZoomDistance * Globals.camMaxZoomDistance)
            ) + (2 * Globals.CamZoomSpeed * distance / Globals.camMaxZoomDistance);
        // Zoom in/out based on the mouse wheel movement
        float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * smoothZoomSpeed;
        transform.Translate(0, 0, zoomAmount, Space.Self);

        // check if the distance exceeds the limit
        distance = Vector3.Distance(transform.position, rotateAroundPoint);
        RestrictCam(distance, rotateAroundPoint);
        focusModeOffset = transform.position - centerObject.transform.position;
        transform.LookAt(rotateAroundPoint);
    }

    public void ToFreeMode()
    {
        currentState = State.Free;
    }

    public void ToFocusMode(GameObject focusObject)
    {
        currentState = State.Focus;
        centerObject = focusObject;
        // reset camera position
        Vector3 rotateAroundPoint = focusObject.transform.position;
        float distance = Vector3.Distance(transform.position, rotateAroundPoint);
        if (focusModeOffset == Vector3.zero)
        {
            RestrictCam(distance, rotateAroundPoint);
            focusModeOffset = transform.position - focusObject.transform.position;
        }
        transform.position = centerObject.transform.position + focusModeOffset;
    }

    void RestrictCam(float distance, Vector3 rotateAroundPoint)
    {
        if (distance > Globals.camMaxZoomDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                rotateAroundPoint,
                distance - Globals.camMaxZoomDistance
            );
        }
        else if (distance < Globals.camMinZoomDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                rotateAroundPoint,
                distance - Globals.camMinZoomDistance
            );
        }
    }
}
