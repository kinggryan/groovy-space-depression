using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.MouseLook m_MouseLook;
    [SerializeField] private float m_Acceleration;
    [SerializeField] private float m_MaxSpeed;

    private Camera m_Camera;
    private CharacterController m_CharacterController;

    private Vector3 m_CurrentVelocity;

    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = GetComponentInChildren<Camera>();
        m_MouseLook.Init(transform, m_Camera.transform);
    }

    private void Update()
    {
        RotateView();
        Move();
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

    private void Move()
    {
        var localDesiredMovement = m_MaxSpeed * transform.TransformDirection(GetInputMoveDirection());
        m_CurrentVelocity = Vector3.MoveTowards(m_CurrentVelocity, localDesiredMovement, m_Acceleration * Time.deltaTime);
        m_CharacterController.Move(m_CurrentVelocity * Time.deltaTime);
    }

    private Vector3 GetInputMoveDirection()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
    }
}
