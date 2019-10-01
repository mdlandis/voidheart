using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public interface IMovableDisplayComponent {
        Transform getTransform();
    }

    [Serializable]
    public class UnitCreateEvent : VComponent {
        public Coord position;
        public VEntity entityBlueprint;
    }

    [Serializable]
    public class UnitsList : VComponentSingleton {
        public List<string> unitIds;
    }

    [Serializable]
    public class UnitCreationSystem : VSystem {
        [SerializeField]
        private VUnitDisplay unitDisplayPrefab = null;

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<UnitCreateEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            var unitCreateEvent = VEntityComponentSystemManager.GetVComponent<UnitCreateEvent>(entity);
            var newUnitEntity = ecsManager.InsantiateEntityFromBlueprint(unitCreateEvent.entityBlueprint);
            // TODO: replace with object pool
            VEntityComponentSystemManager.GetVComponent<UnitDisplayComponent>(newUnitEntity).unitDisplayGameObject = VUnitDisplay.Instantiate(unitDisplayPrefab);
            VEntityComponentSystemManager.GetVComponent<UnitDisplayComponent>(newUnitEntity).unitDisplayGameObject.gameObject.SetActive(false);
            VEntityComponentSystemManager.GetVComponent<PositionComponent>(newUnitEntity).position = unitCreateEvent.position;
            ecsManager.GetSystem<PositionTrackSystem>().Track(newUnitEntity);
            ecsManager.GetVSingletonComponent<UnitsList>().unitIds.Add(newUnitEntity.id);

            ecsManager.QueueAnimationEvent(ecsManager.CreateEntity("UnitAppear", component : new EntityAppearEvent {
                entityId = newUnitEntity.id
            }));
        }
    }

    [Serializable]
    public class EntityAppearEvent : VComponent {
        public string entityId;
    }

    [Serializable]
    public class UnitCreationDisplaySystem : VAnimationSystem {
        public override bool ShouldOperate(VEntity entity) {
            EntityAppearEvent e = entity.GetVComponent<EntityAppearEvent>();
            return e != null && (ecsManager.GetVEntityById(e.entityId).GetVComponent<UnitDisplayComponent>() != null);
        }

        public override bool IsImmediate() {
            return false;
        }

        public override IEnumerator StartAnimation(VEntity entity, Action yieldAnimation) {
            EntityAppearEvent appearEvent = VEntityComponentSystemManager.GetVComponent<EntityAppearEvent>(entity);
            var unitEntity = ecsManager.GetVEntityById(appearEvent.entityId);

            var unitDisplay = VEntityComponentSystemManager.GetVComponent<UnitDisplayComponent>(unitEntity);
            VEntityComponentSystemManager.GetVComponent<UnitDisplayComponent>(unitEntity).unitDisplayGameObject.gameObject.SetActive(true);
            unitDisplay.unitDisplayGameObject.transform.position = unitEntity.GetVComponent<PositionDisplayComponent>().mainTransform.position;
            unitDisplay.spriteSize = ecsManager.GetSystem<PositionWorldConversionSystem>().SquareDimensions;
            unitDisplay.unitDisplayGameObject.BindEntity(unitEntity);

            unitDisplay.unitDisplayGameObject.ResizeSprite(unitDisplay.spriteSize);
            unitDisplay.getTransform().SetParent(unitEntity.GetVComponent<PositionDisplayComponent>().mainTransform);

            yield return new WaitForSeconds(0.1f);
            yieldAnimation();
        }
    }
}