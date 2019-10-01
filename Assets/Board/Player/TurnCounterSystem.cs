using System;

namespace Voidheart {
    [Serializable]
    public class TurnCounterComponent : VComponentSingleton {
        public int turnCounter;
    }

    [Serializable]
    public class TurnCounterChangeAnimationComponent : VComponent {
        public int prevTurn;
        public int newTurn;
    }

    /// <summary>Keeps track of current turn.</summary>
    [Serializable]
    public class TurnCounterSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<SetLifecycleEventComponent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            if (VEntityComponentSystemManager.GetVComponent<SetLifecycleEventComponent>(entity).newVLifecycle == VLifecycle.EnemySetupStart) {
                var turnCounter = ecsManager.GetVSingletonComponent<TurnCounterComponent>();
                turnCounter.turnCounter += 1;
            }
        }
    }
}