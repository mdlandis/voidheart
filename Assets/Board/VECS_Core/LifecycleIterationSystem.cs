using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class LifecycleIterationSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<SetLifecycleEventComponent>(entity);
        }

        protected override void OnAfterEvent(VEntity entity) {
            var currentTurn = ecsManager.GetVSingletonComponent<CurrentLifecycleComponent>();
            // for all other phases, automatically move onto the next one.
            if (currentTurn.currentLifecycle != VLifecycle.PlayerTurnExecute) {
                VEntity newEvent = ecsManager.CreateEvent("SetNextLifecycleEvent");
                ecsManager.AddComponent(newEvent, new SetLifecycleEventComponent {
                    newVLifecycle = VLifecycleHelper.Increment(currentTurn.currentLifecycle)
                });
            }
        }
    }
}