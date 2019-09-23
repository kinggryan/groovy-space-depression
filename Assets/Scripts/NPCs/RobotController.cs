using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    // TODO: If this stuff gets complicated, we can do an actual state machine
    // Enum is fine for now
    enum State
    {
        Default,
        LookAtPlayer
    }

    [SerializeField]
    private float lookAtPlayerLerpSpeed = 1;
    
    private State state = State.Default;

    // Start is called before the first frame update
    public void LookAtPlayer()
    {
        state = State.LookAtPlayer;
    }

    void Update()
    {
        switch(state)
        {
            case State.Default:
                // do nothing
                break;
            case State.LookAtPlayer:
                // Rotate towards player on x-z plane
                var targetLookPos = GetPlayerPosition();
                targetLookPos.y = transform.position.y;
                var targetLookDirection = (targetLookPos - transform.position).normalized;
                var newForward = Vector3.Slerp(transform.forward, targetLookDirection, lookAtPlayerLerpSpeed*Time.deltaTime);
                transform.LookAt(transform.position + newForward, Vector3.up);
                break;
        }
    }

    Vector3 GetPlayerPosition()
    {
        var player = Object.FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player not found when trying to find player position.");
            return Vector3.zero;
        }

        return player.transform.position;
    }
}
