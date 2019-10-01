using System;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class CurrentLifecycleComponent : VComponentSingleton {
        public VLifecycle currentLifecycle;
    }

    [Serializable]
    public class SetLifecycleEventComponent : VComponentSingleton {
        public VLifecycle newVLifecycle;
    }

    // not sure how many of these needed. Some might be unnecessary.
    public enum VLifecycle {
        EnemySetupStart = 0,
        EnemySetupExecute = 1,
        PlayerTurnStart = 2,
        PlayerTurnExecute = 3,
        PlayerTurnEnd = 4,
        EnemyActionsStart = 5,
        EnemyActionsResolve = 6,
        EnemyActionsEnd = 7,

        OnBeforeEvent = -1,
        OnExecuteEvent = -2,
        OnAfterEvent = -3,
    }

    public static class VLifecycleHelper {
        public static VLifecycle Increment(VLifecycle v) {
            switch (v) {
                case VLifecycle.EnemySetupStart:
                    return VLifecycle.EnemySetupExecute;
                case VLifecycle.EnemySetupExecute:
                    return VLifecycle.PlayerTurnStart;
                case VLifecycle.PlayerTurnStart:
                    return VLifecycle.PlayerTurnExecute;
                case VLifecycle.PlayerTurnExecute:
                    return VLifecycle.PlayerTurnEnd;
                case VLifecycle.PlayerTurnEnd:
                    return VLifecycle.EnemyActionsStart;
                case VLifecycle.EnemyActionsStart:
                    return VLifecycle.EnemyActionsResolve;
                case VLifecycle.EnemyActionsResolve:
                    return VLifecycle.EnemyActionsEnd;
                case VLifecycle.EnemyActionsEnd:
                    return VLifecycle.EnemySetupStart;
                default:
                    throw new Exception("Trying to increment non-incrementable lifecycle");
            }
        }
    }
}