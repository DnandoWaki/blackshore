using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float andaSpeed;
    public float correSpeed;
    public float slideSpeed;

    private float moveSpeedDesejada;
    private float lastMoveSpeedDesejada;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Pulo")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Tamanho")]
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Tá no chão?")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Grudar na Ladeira")]
    public float inclinaturaMax;
    private RaycastHit slopeHit;
    private bool saiSlope;
    

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        sliding,
        air
    }

    public bool sliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // checagem chão
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // quando pular
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        // Modo - Deslizando
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
                moveSpeedDesejada = slideSpeed;

            else
                moveSpeedDesejada = correSpeed;
        }

        // Modo - Correndo
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeedDesejada = correSpeed;
        }

        // Modo - Andando
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeedDesejada = andaSpeed;
        }

        // Mode - No ar
        else
        {
            state = MovementState.air;
        }

        // checar se a speed desejada mudou drasticamente
        if(Mathf.Abs(moveSpeedDesejada - lastMoveSpeedDesejada) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = moveSpeedDesejada;
        }

        lastMoveSpeedDesejada = moveSpeedDesejada;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // diminuição de speed não brusca até o desejado
        float time = 0;
        float diferencia = Mathf.Abs(moveSpeedDesejada - moveSpeed);
        float valorComeco = moveSpeed;

        while (time < diferencia)
        {
            moveSpeed = Mathf.Lerp(valorComeco, moveSpeedDesejada, time / diferencia);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = moveSpeedDesejada;
    }

    private void MovePlayer()
    {
        // calculo da direção
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // na ladeira
        if (OnSlope() && !saiSlope)
        {
            rb.AddForce(PegaSlopeMoveDirecao(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // no chão
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // no ar
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // desligar gravidade na ladeira
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limitando velocidade na ladeira
        if (OnSlope() && !saiSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        // limitando velocidade no chão ou ar
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limite de velocidade extra
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        saiSlope = true;

        // reset da velocidade Y
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        saiSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < inclinaturaMax && angle != 0;
        }

        return false;
    }

    public Vector3 PegaSlopeMoveDirecao(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}