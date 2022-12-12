using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jinwoo.FirstPersonController;
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float groundDrag;

    [Range(1, 10)]
    public float maxSpeed = 5;

    [Range(0.1f, 100f)]
    public float acceleration = 50, deAcceleration = 50;

    protected float _currentVelocity = 3;
    protected Vector3 _movementDirection;

    protected Rigidbody _rigid;

    public float CurrentSpeed =>
        new Vector3(_rigid.velocity.x, _rigid.velocity.z).magnitude;


    protected Vector3 moveInput;

    public CharacterInput _input;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        MoveAgent(new Vector3(_input.GetHorizontalMovementInput(), 0, _input.GetVerticalMovementInput()));
    }

    public void MoveAgent(Vector3 movementInput)
    {
        if (movementInput.sqrMagnitude > 0) // ¿Œ«≤ ¥©∏£∏È
        {
            if (Vector3.Dot(movementInput, _movementDirection) < 0) //
            {
                _currentVelocity = 0;
            }
            _movementDirection = movementInput.normalized;
        }
        _currentVelocity = CalculateSpeed(movementInput);

        //var percent = _currentVelocity / _movementSO.maxSpeed;
        //animator.MouseDir(_movementDirection, percent);
    }

    private float CalculateSpeed(Vector3 movementInput)
    {
        if (movementInput.sqrMagnitude > 0)
        {
            _currentVelocity += acceleration * Time.deltaTime;
        }
        else
        {
            _currentVelocity -= deAcceleration * Time.deltaTime;
        }

        return Mathf.Clamp(_currentVelocity, 0, maxSpeed);
    }
    protected virtual void FixedUpdate()
    {
         _rigid.velocity = _movementDirection * _currentVelocity;
            //var percent = currentSpeed / _movementSO.maxSpeed;
            //animator.PlayMoveAnimation(moveInput, percent);


    }
}
