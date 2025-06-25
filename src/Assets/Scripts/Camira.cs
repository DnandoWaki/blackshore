using UnityEngine;

public class Camira : MonoBehaviour
{
    float rotaX;
    float rotaY;
    public Transform orientacao;
    public float senY;
    public float senX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Camera();
    }

    private void Camera()
    {
        //pegar o input
        float mouseX = Input.GetAxisRaw("Mouse X") * senX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * senY * Time.deltaTime;

        rotaX -= mouseY;
        rotaX = Mathf.Clamp(rotaX, -90f, 90f);
        rotaY += mouseX;

        //rotacionar camera e orientacao
        transform.rotation = Quaternion.Euler(rotaX, rotaY, 0);
        orientacao.rotation = Quaternion.Euler(0, rotaY, 0);
    }
}
