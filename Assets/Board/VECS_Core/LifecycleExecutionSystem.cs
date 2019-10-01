using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class LifecycleExecutionSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<SetLifecycleEventComponent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            var currentTurn = ecsManager.GetVSingletonComponent<CurrentLifecycleComponent>();
            currentTurn.currentLifecycle = VEntityComponentSystemManager.GetVComponent<SetLifecycleEventComponent>(entity).newVLifecycle;

            if (currentTurn.currentLifecycle != VLifecycle.PlayerTurnExecute) {
                foreach (VEntity e in new List<VEntity>(ecsManager.GetAllEntities())) {
                    foreach (VSystem system in ecsManager.GetAllSystems()) {
                        if (system.ShouldOperate(e)) {
                            system.OnLifecycle(e, currentTurn.currentLifecycle);
                        }

                        ecsManager.GetGameplayEventQueue().Flush();
                    }
                }
            }
        }
    }
}