using UnityEngine;

public class Camira : MonoBehaviour
{
    private float rotaX;
    public Transform selfspin;
    public float rotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Camera();
    }

    private void Camera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * rotation * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * rotation * Time.deltaTime;

        rotaX -= mouseY;
        transform.localRotation = Quaternion.Euler(rotaX, 0f, 0f);
        selfspin.Rotate(Vector3.up * mouseX);
    }
}
