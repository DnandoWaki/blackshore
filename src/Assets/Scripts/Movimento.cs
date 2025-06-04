using UnityEngine;

public class Movimento : MonoBehaviour
{
    private CharacterController controller;
    public float Corrida;
    private bool grounded;
    private Vector3 movimento;
    [SerializeField ]private Transform peDoPersonagem;
    [SerializeField] private LayerMask colisaoLayer;
    private float forcaY;
    private float resistenciaar = 0.65f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movimento = new Vector3(horizontal, 0, vertical);
        Vector3 gravity = new Vector3 (horizontal, 0 ,vertical);
        movimento = transform.right * horizontal + transform.forward * vertical;
        Vector3 jump_spd = Vector3.zero;
        Vector3 dft_spd = movimento * Time.deltaTime * 5;
        Vector3 run_spd = movimento * Time.deltaTime * 5 * Corrida;
        transform.Translate(movimento * Time.deltaTime);
        controller.Move(new Vector3(0, forcaY, 0) * Time.deltaTime);

        controller.Move(dft_spd);
        if (Input.GetKey(KeyCode.LeftShift) && grounded){
            controller.Move(run_spd);
        }

        grounded = Physics.CheckSphere(peDoPersonagem.position, 1.58f, colisaoLayer);

        
        if (Input.GetKey(KeyCode.Space) && grounded) { 
            forcaY = 5f;
            controller.Move(jump_spd = dft_spd * Time.deltaTime * resistenciaar);
            if(Input.GetKey(KeyCode.LeftShift)) {
                controller.Move(jump_spd = run_spd * Time.deltaTime * resistenciaar);
            }
        }

        if (forcaY > -12f)
        {
            forcaY += -12f * Time.deltaTime;
        }
    }


    //Vector3 Calc_jump_spd(Vector3 spd)
    //{
        //return spd * Time.deltaTime * resistenciaar;
    //}

}
