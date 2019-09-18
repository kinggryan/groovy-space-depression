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
    [SerializeField] private float m_MaxInteractRange;

    private Camera m_Camera;
    private CharacterController m_CharacterController;

    private Vector3 m_CurrentVelocity;
    private bool m_InStoryMode;

    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = GetComponentInChildren<Camera>();
        m_MouseLook.Init(transform, m_Camera.transform);
    }

    private void Update()
    {
        if (m_InStoryMode)
        {
            // Do some story stuff? I mean this might be controlled by a diff script
        }
        else
        {
            RotateView();
            Move();
            InteractWithNPCs();
        }
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

    void InteractWithNPCs()
    {
        // do a raycast against NPC layer with given talking range
        // if hit detected and 'interact' used
        // start story with the npc
        RaycastHit hitInfo;
        if(Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hitInfo, m_MaxInteractRange))
        {
            if (Input.GetButtonDown("Interact"))
            {
                var inkController = hitInfo.collider.GetComponent<CharacterInkController>();
                if (inkController != null)
                {
                    inkController.StartStory();
                    // TODO: Look at the correct place etc
                    EnterStoryMode();
                }
            }
        }
    }

    void EnterStoryMode()
    {
        m_InStoryMode = true;
        // TODO: When we have actual dialogue UI this will be different
        m_MouseLook.SetCursorLock(false);
    }
    
    public void ExitStoryMode()
    {
        m_InStoryMode = false;
        m_MouseLook.SetCursorLock(true);
    }
}
