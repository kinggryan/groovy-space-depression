using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    /** Type Definitions */ 
    // TODO: If this stuff gets complicated, we can do an actual state machine
    // Enum is fine for now
    [System.Serializable]
    enum State
    {
        Default,
        LookAtPlayer
    }

    // Holds information about how each state behaves
    // This could just be an actual state machine but w/e
    [System.Serializable]
    struct StateInfo
    {
        // Because unity can't serialize a dictionary, we are putting the relevant state in the StateInfo itself
        public State state;
        public FMODUnity.StudioEventEmitter enterSoundEmitter;
        public FMODUnity.StudioEventEmitter exitSoundEmitter;
        public FMODUnity.StudioEventEmitter loopingSoundEmitter;
    }

    /** Public Vars */

    /** Private Vars */

    [SerializeField]
    private List<StateInfo> stateInfos = new List<StateInfo>();

    [SerializeField]
    private float lookAtPlayerLerpSpeed = 1;
    
    private State state = State.Default;

    // Start is called before the first frame update
    public void LookAtPlayer()
    {
        SetState(State.LookAtPlayer);
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

    void SetState(State state)
    {
        DidExitState(state);
        this.state = state;
        DidEnterState(state);
    }

    void DidEnterState(State state)
    {
        var info = GetStateInfoForState(state);
        if (info.enterSoundEmitter != null)
        {
            info.enterSoundEmitter.Play();
            info.enterSoundEmitter.SetParameter("TurnOff", 1);
        }
        if (info.loopingSoundEmitter != null)
        {
            // TODO: Play the looping sound emitter
        }
    }

    void DidExitState(State state)
    {
        var info = GetStateInfoForState(state);
        if (info.exitSoundEmitter != null)
        {
            info.exitSoundEmitter.Play();
            info.exitSoundEmitter.SetParameter("TurnOff", 1);
        }
        if (info.loopingSoundEmitter != null)
        {
            // TODO: Stop the looping sound emitter
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

    StateInfo GetStateInfoForState(State state)
    {
        foreach(var stateInfo in stateInfos)
        {
            if(stateInfo.state == state)
            {
                return stateInfo;
            }
        }

        // Return empty info if there is none
        Debug.LogWarning("No state info specified for state " + state + " on object " + this);
        return new StateInfo();
    }
}
