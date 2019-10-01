using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class PushEventComponent : VComponent, CardEffectComponent {
        public int squares;
        public RotationAngle dir;

        public string sourceId {
            get;
            set;
        }
        public Coord targetCoord {
            get;
            set;
        }
        public List<Coord> groupTargetCoords {
            get;
            set;
        }
    }

    [Serializable]
    public class PushEventSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<PushEventComponent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            PushEventComponent pushComponent = VEntityComponentSystemManager.GetVComponent<PushEventComponent>(entity);

            foreach (VEntity enemyEntity in ecsManager.FilterEntities(test: (ventity) => {
                    return ventity.HasVComponent<TeamComponent>() && ventity.HasVComponent<PositionComponent>() && ventity.GetVComponent<TeamComponent>().team == Team.ENEMY;
                })) {
                PositionComponent currPosition = enemyEntity.GetVComponent<PositionComponent>();
                ecsManager.CreateEvent("wind", component : new MovementEvent {
                    sourceId = enemyEntity.id,
                        targetCoord = currPosition.position + Coord.Rotate(new Coord(pushComponent.squares, 0), pushComponent.dir)
                });
            }
        }
    }
}