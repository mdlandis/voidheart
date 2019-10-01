using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Voidheart {
    [System.Serializable]
    public class HealthDisplayComponent : VComponent, IMovableDisplayComponent {
        [System.NonSerialized]
        public HealthBarDisplay healthBarDisplay;

        public Transform getTransform() {
            return healthBarDisplay.transform;
        }
    }

    [System.Serializable]
    public class HealthSetAnimationEvent : VComponent {
        public string targetEntity;
        public int currHealth;
        public int maxHealth;
    }

    [System.Serializable]
    public class HealthInitAnimationSystem : VAnimationSystem {

        public HealthBarDisplay healthBarDisplayPrefab;
        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<EntityAppearEvent>(entity) && ecsManager.GetVEntityById(entity.GetVComponent<EntityAppearEvent>().entityId).GetVComponent<HealthDisplayComponent>() != null;
        }

        public override void DoImmediateAnimation(VEntity entity) {
            VEntity targetEntity = ecsManager.GetVEntityByIdIncludingRemoved(entity.GetVComponent<EntityAppearEvent>().entityId);
            HealthDisplayComponent healthDisplay = ecsManager.GetVEntityById(entity.GetVComponent<EntityAppearEvent>().entityId).GetVComponent<HealthDisplayComponent>();
            healthDisplay.healthBarDisplay = HealthBarDisplay.Instantiate(healthBarDisplayPrefab);
            HealthComponent h = targetEntity.GetVComponent<HealthComponent>();
            Assert.IsNotNull(h);
            healthDisplay.healthBarDisplay.Init(h.maxHealth);
            healthDisplay.healthBarDisplay.SetValue(h.currHealth);

            var positionDisplay = targetEntity.GetVComponent<PositionDisplayComponent>();
            healthDisplay.healthBarDisplay.transform.position = positionDisplay.mainTransform.position;
            healthDisplay.healthBarDisplay.transform.SetParent(positionDisplay.mainTransform);
        }
    }

    [System.Serializable]
    public class HealthAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<HealthSetAnimationEvent>(entity);
        }

        public override void DoImmediateAnimation(VEntity entity) {
            HealthSetAnimationEvent healthSetEvent = entity.GetVComponent<HealthSetAnimationEvent>();
            VEntity targetEntity = ecsManager.GetVEntityByIdIncludingRemoved(healthSetEvent.targetEntity);

            HealthDisplayComponent healthDisplay = targetEntity.GetVComponent<HealthDisplayComponent>();
            if (healthDisplay != null) {
                healthDisplay.healthBarDisplay.SetValue(healthSetEvent.currHealth);
            }
        }
    }
}