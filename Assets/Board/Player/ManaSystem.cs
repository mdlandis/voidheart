using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class PlayerManaComponent : VComponentSingleton {
        public int currMana;
        public int maxMana;
    }

    public class ManaRefillSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<PlayerManaComponent>(entity);
        }

        protected override void OnPlayerTurnStart(VEntity entity) {
            PlayerManaComponent playerMana = entity.GetVComponent<PlayerManaComponent>();
            int oldMana = playerMana.currMana;
            playerMana.currMana = playerMana.maxMana;

            ecsManager.QueueAnimationEvent("ManaAnimation", component : new PlayerManaSetAnimation {
                oldMana = oldMana,
                    newMana = playerMana.currMana,
                    maxMana = playerMana.maxMana
            });
        }
    }

    [System.Serializable]
    public class PlayerManaSetAnimation : VComponent {
        public int oldMana;
        public int newMana;
        public int maxMana;
    }

    public class ManaRefillAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<PlayerManaSetAnimation>(entity);
        }

        public override void DoImmediateAnimation(VEntity entity) {
            CardViewController.Instance.SetMana(entity.GetVComponent<PlayerManaSetAnimation>().newMana);
            CardViewController.Instance.SetMaxMana(entity.GetVComponent<PlayerManaSetAnimation>().maxMana);
        }
    }
}