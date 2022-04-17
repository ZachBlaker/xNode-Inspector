//#define debug
using System;
using Game.Navigation;
using Game.Core;
using UnityEngine;
using static CustomDebugLogging;

namespace State_Machine
{
    /// @ingroup group_state
    public class ST_MoveToTarget : SM_State
    {
        public ST_MoveToTarget()
        {
            name = "Move To Target";
            description = "Moves the creature towards the coordinates of the StateHandlers current target MapObject";
            category = BehaviourCategory.Movement;
            requiredCreatureComponents = new Type[] { typeof(SC_Movement) };
        }

        [Results]
        public enum ResultTypes    
        {
            DestinationReached,
            PathBlocked,
            TargetLost
        }

        SC_Movement movement;     

        protected override void Setup()
        {
            movement = GetStateComponent<SC_Movement>();
        }

        Vector2Int lastTargetPosition;
        MapObject target;
        bool targetLost = false;

        public override void EnterState()
        {
            Debug("Started moving to position");
            target = handler.currentTarget;
            targetLost = false;

            lastTargetPosition = target.position;

            if(!movement.SetDestinationAndCheckPath(lastTargetPosition))
                StateComplete(ResultTypes.PathBlocked);

            else
            {
                Debug($"Setting destination to target coordinates of {target.position}");
                movement.StartMovingToDestination();
                ListenForEvent(SC_Movement.Events.PathBlocked, PathBlocked);
                isUpdating = true;
            }
        }

        private void PathBlocked()
        {
            StateComplete(ResultTypes.PathBlocked);
        }
        private void UpdateTargetDestination()
        {
            if (handler.currentTarget == null && !targetLost)
            {
                targetLost = true;
                movement.SetDestinationAsNextTileInPath();
                Debug("Current target is null");
            }

            else if (handler.currentTarget != target)
                throw new Exception("Target changed during state somehow");

            else if (lastTargetPosition != handler.currentTarget.position)
            {
                Debug("Current target has moved locations");
                lastTargetPosition = handler.currentTarget.position;
                movement.SetDestination(lastTargetPosition);
            }
        }

        public override void Update()
        {
            if(targetLost)
            {
                if(!movement.currentlyMoving)
                    StateComplete(ResultTypes.TargetLost);
            }
            else if (movement.IsDestinationReached())
                StateComplete(ResultTypes.DestinationReached);
            else
                UpdateTargetDestination();
        }

        protected override void OnForceBeginExiting()
        {
            movement.SetDestinationAsNextTileInPath();
        }

        protected override void OnForceImmediateExit()
        {
            movement.ForceFinishMovingBetweenTiles();
        }

        protected override void OnExit()
        {
            lastTargetPosition = new Vector2Int(-1, -1);
            RemoveListener(SC_Movement.Events.PathBlocked, PathBlocked);
        }
    }
}
