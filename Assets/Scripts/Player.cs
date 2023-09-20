using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")] [Min(0.1f)] public float horizontalSpeedAcceleration;
    [Min(5)] public float horizontalSpeedWalking;
    [Min(5)] public float horizontalSpeedJumping;

    [Header("Jump")] [Min(10)] public float gravityAccelerationUp;
    [Min(10)] public float gravityAccelerationDown;
    [Min(5)] public float jumpForce;
    [Min(1)] public float jumpAccelerationDecrease;
    public float jumpBufferTime;
    public float minGravityAcceleration;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float verticalVelocity;
    [SerializeField] private bool active = true;

    private float _horizontalInput = 0;

    private Animator _animator;
    private CapsuleCollider2D _selfCollider;
    private BoxCollider2D _groundingCollider;
    private Rigidbody2D _body;
    private SpriteRenderer _sprite;
    private float _requestedJumpTime;
    private Vector3 _startPosition;

    public event Action<Collision2D> OnGrounded;

    private T RequiredGetComponent<T>()
    {
        return GetComponent<T>() ?? throw new ArgumentNullException(nameof(T));
    }

    private void Start()
    {
        _animator = RequiredGetComponent<Animator>();
        _selfCollider = RequiredGetComponent<CapsuleCollider2D>();
        _body = RequiredGetComponent<Rigidbody2D>();
        _sprite = RequiredGetComponent<SpriteRenderer>();
        _requestedJumpTime = float.MinValue;
        _startPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateIsGrounded(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateIsGrounded(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private void UpdateIsGrounded(Collision2D collision)
    {
        if (isGrounded) return;
        for (int i = 0; i < collision.contactCount; i++)
        {
            var normal = collision.GetContact(i).normal;
            if (normal.y >= .9f)
            {
                isGrounded = true;
                if (OnGrounded != null) OnGrounded(collision);
            }

            if (normal.y <= -.9f && verticalVelocity > 0)
            {
                verticalVelocity /= 2f;
            }
        }
    }

    private void UpdateAnimator()
    {
        _animator.SetFloat("horizontalSpeed", Mathf.Abs(_horizontalInput));
        _animator.SetFloat("verticalSpeed", verticalVelocity);
        _animator.SetBool("isGrounded", isGrounded);
    }

    private void Update()
    {
        if (!active) return;
        if (Input.GetButtonDown("Jump"))
        {
            _requestedJumpTime = Time.time;
        }

        if (Input.GetButtonUp("Jump") && verticalVelocity > 0)
        {
            verticalVelocity /= jumpAccelerationDecrease;
        }

        if (isGrounded && Time.time - _requestedJumpTime < jumpBufferTime)
        {
            verticalVelocity = jumpForce;
        }

        _horizontalInput = Input.GetAxis("Horizontal");
        if (_horizontalInput != 0)
        {
            _sprite.flipX = _horizontalInput < 0;
        }

        if (_horizontalInput != 0 && _body.velocity.x / _horizontalInput < 0)
        {
            var velocity = _body.velocity;
            velocity.x = 0;
            _body.velocity = velocity;
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!active) return;
        var gravityAcceleration = verticalVelocity > 0 ? gravityAccelerationUp : gravityAccelerationDown;
        verticalVelocity = isGrounded && verticalVelocity < 0
            ? -10f
            : Mathf.Max(minGravityAcceleration, verticalVelocity - gravityAcceleration * Time.deltaTime);
        var horizontalVelocityTarget =
            _horizontalInput * (isGrounded ? horizontalSpeedWalking : horizontalSpeedJumping);
        var horizontalVelocity =
            Mathf.MoveTowards(_body.velocity.x, horizontalVelocityTarget, horizontalSpeedAcceleration);
        _body.velocity = new Vector2(horizontalVelocity, verticalVelocity);
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        active = true;
        transform.position = _startPosition;
    }

    public void Die()
    {
        _animator.SetTrigger("deadTrigger");
        _body.velocity = Vector2.zero;
        active = false;
        StartCoroutine(RespawnRoutine());
    }
}