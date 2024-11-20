using System;
using System.Collections.Generic;
using System.Linq;
using Summoner.Utilities;
using Summoner.Utilities.Enums;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UpdateFunction = UnityEngine.LowLevel.PlayerLoopSystem.UpdateFunction;

namespace Summoner.Systems.PlayerLoop
{
    public static class PlayerLoopUtility
    {
        private static bool initialized;
        private static HashSet<PlayerLoopSetup> subscribedSetups;
        
        private static PlayerLoopSetup<EarlyUpdate> earlyUpdateSetup;
        private static PlayerLoopSetup<Update> updateSetup;
        private static PlayerLoopSetup<PreLateUpdate> preLateUpdateSetup;
        private static PlayerLoopSetup<PostLateUpdate> postLateUpdateSetup;
        private static PlayerLoopSetup<FixedUpdate> fixedUpdateSetup;
        
        public static event Action OnPlayerLoopEarlyUpdate
        {
            add => AddToSubscribers(value, ref earlyUpdateSetup);
            remove => RemoveFromSubscribers(value, ref earlyUpdateSetup);
        }

        public static event Action OnPlayerLoopUpdate
        {
            add => AddToSubscribers(value, ref updateSetup);
            remove => RemoveFromSubscribers(value, ref updateSetup);
        }

        public static event Action OnPlayerLoopPreLateUpdate
        {
            add => AddToSubscribers(value, ref preLateUpdateSetup);
            remove => RemoveFromSubscribers(value, ref preLateUpdateSetup);
        }

        public static event Action OnPlayerLoopPostLateUpdate
        {
            add => AddToSubscribers(value, ref postLateUpdateSetup);
            remove => RemoveFromSubscribers(value, ref postLateUpdateSetup);
        }

        public static event Action OnPlayerLoopFixedUpdate
        {
            add => AddToSubscribers(value, ref fixedUpdateSetup);
            remove => RemoveFromSubscribers(value, ref fixedUpdateSetup);
        }

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            subscribedSetups = new HashSet<PlayerLoopSetup>();
            RuntimeSafeEditorUtility.RegisterApplicationQuittingCallback(UnsubscribeAllInternalDelegates);
        }
        
        private static void AddToSubscribers<T>(Action subscriber, ref T playerLoopSetup) where T : PlayerLoopSetup, new()
        {
            if (subscriber == null)
            {
                return;
            }
            
            if (playerLoopSetup == null)
            {
                playerLoopSetup = new T();
                ChangeInternalSubscription(playerLoopSetup, Subscription.Add);
            }

            playerLoopSetup.OnUpdate += subscriber;
            playerLoopSetup.SubscribedDelegates.Add(subscriber);
        }

        private static void RemoveFromSubscribers<T>(Action subscriber, ref T playerLoopSetup) where T : PlayerLoopSetup
        {
            if (subscriber == null)
            {
                return;
            }
            
            playerLoopSetup.OnUpdate -= subscriber;
            playerLoopSetup.SubscribedDelegates.Remove(subscriber);

            if (!playerLoopSetup.HasSubscribers)
            {
                ChangeInternalSubscription(playerLoopSetup, Subscription.Remove);
                playerLoopSetup = null;
            }
        }

        private static void ChangeInternalSubscription(PlayerLoopSetup playerLoopSetup, Subscription subscription)
        {
            PlayerLoopSystem playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();

            UpdateFunction updateFunction = playerLoopSetup.UpdateFunction;

            int subsystemIndex = GetIndexOfSubsystem(playerLoop, playerLoopSetup.UpdateType);
            playerLoop.subSystemList[subsystemIndex].updateDelegate -= updateFunction; // Prevent multi-subscriptions
            if (subscription is Subscription.Add)
                playerLoop.subSystemList[subsystemIndex].updateDelegate += updateFunction;
            
            if (subscription is Subscription.Remove)
                subscribedSetups.Remove(playerLoopSetup);
            else if (subscription is Subscription.Add)
                subscribedSetups.Add(playerLoopSetup);
            
            UnityEngine.LowLevel.PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private static int GetIndexOfSubsystem(PlayerLoopSystem playerLoop, Type subsystemType)
        {
            int i;
            for (i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                PlayerLoopSystem playerLoopSystem = playerLoop.subSystemList[i];
                if (playerLoopSystem.type == subsystemType)
                {
                    break;
                }
            }

            return i;
        }
        
        private static void UnsubscribeAllInternalDelegates()
        {
            PlayerLoopSetup[] setups = subscribedSetups.ToArray();
            for (int i = 0; i < setups.Length; i++)
            {
                ChangeInternalSubscription(setups[i], Subscription.Remove);
            }
        }
    }
}