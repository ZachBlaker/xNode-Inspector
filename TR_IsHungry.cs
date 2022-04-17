//#define debug
using System;

namespace State_Machine
{
    /** @ingroup group_transition */
    [CustomDebug("Random Destination", "navy")]
    public class TR_IsHungry : SM_Transition
    {
        public TR_IsHungry()
        {
            name = "Check Hunger";
            description = "Checks if the creature is hungry";
            category = BehaviourCategory.Checks;
            requiredCreatureComponents = new Type[] { typeof(SC_Hunger) };
        }

        [Results]
        public enum ResultTypes
        {
            True,
            False
        }

        SC_Hunger hungerComponent;

        protected override void Setup()
        {
            hungerComponent = GetStateComponent<SC_Hunger>();
        }


        public override Enum GetTransitionResult()
        {
            if (hungerComponent.isHungry)
                return ResultTypes.True;
            else
                return ResultTypes.False;
        }
    }
}
