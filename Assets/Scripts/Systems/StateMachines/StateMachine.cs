using System;
using System.Collections.Generic;
using UnityEngine;

namespace Summoner.Systems.StateMachines
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private StateBehaviour[] stateBehaviours;

        private StateBehaviour currentStateBehaviour;

        private readonly Dictionary<Type, StateBehaviour> stateInstanceTypeToStateBehaviour = new();
        public Dictionary<Type, StateBehaviour> StateInstanceTypeToStateBehaviour
        {
            get
            {
                Initialize();
                return stateInstanceTypeToStateBehaviour;
            }
        }

        private bool initialized;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            
            foreach (StateBehaviour stateBehaviour in stateBehaviours)
            {
                stateInstanceTypeToStateBehaviour.Add(stateBehaviour.StateInstanceType, stateBehaviour);
            }
        }

        public void Queue(StateInstance stateInstance)
        {
            if (!StateInstanceTypeToStateBehaviour.TryGetValue(stateInstance.GetType(), out StateBehaviour stateBehaviour))
            {
                return;
            }
        }
    }
}