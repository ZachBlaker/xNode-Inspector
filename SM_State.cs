//#define debug
using System;
using UnityEngine.Events;
using static CustomDebugLogging;
namespace State_Machine
{
    /// <summary> Base class for all States</summary>
    ///  @ingroup group_stateMachine
    [Serializable]
    [CustomDebug("State","navy", "*")]
    public abstract class SM_State : SM_NodeBehaviour
    {

        /// <summary>  Toggles calling of Update()</summary>
        public bool isUpdating;

        /// <summary>Called by this GameObjects StateHandler when an Event triggers exiting of the state</summary>
        /// <param name="instant"> Forces the exit to occur instantly on this frame</param>
        public void ForceExit(bool instant = false)
        {
            if (instant == false)
                OnForceBeginExiting();
            else
            {
                OnForceImmediateExit();
                StateComplete(null);
            }
        }

        /// <summary> Call to end a state with an Enum representing the result</summary>
        /// <param name="result"> Result of the state as enum, must be an enum tagged with [Results]</param>
        protected void StateComplete(Enum result)
        {
            Debug($"State complete {result}");
            isUpdating = false;
            OnExit();
            handler.StateComplete(result);
        }

        /// <summary> Starts the State</summary>
        public abstract void EnterState();

        /// <summary> Called every MonoBehaviour.Update when isUpdating is true</summary>
        public abstract void Update();

        /// <summary> Used to setup the State to exit as soon as possible </summary>
        protected abstract void OnForceBeginExiting();

        /// <summary> Used to force the State to exit instantly</summary>
        protected abstract void OnForceImmediateExit();

        /// <summary> Used for cleanup of State, called by StateComplete()</summary>
        protected virtual void OnExit()
        {

        }

        /// <summary> Listen for an Event invoked by a StateComponent</summary>
        protected void ListenForEvent(Enum eventEnum, UnityAction listener)
        {
            handler.eventHandler.ListenForEvent(eventEnum, listener);
        }

        /// <summary> Stop listening for an Event invoked by a StateComponent</summary>
        protected void RemoveListener(Enum eventEnum, UnityAction listener)
        {
            handler.eventHandler.RemoveListener(eventEnum, listener);
        }
    }
}
