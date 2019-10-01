using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class MovementEvent : VComponent, CardEffectComponent {

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

    public class MovementAnimationEvent : VComponent {
        public string entityToMove;
        public Coord from;
        public Coord to;
    }

    public class MovementSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<MovementEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            MovementEvent moveEvent = VEntityComponentSystemManager.GetVComponent<MovementEvent>(entity);
            PositionTrackSystem positionTrackSystem = ecsManager.GetSystem<PositionTrackSystem>();

            // check to make sure that the square is unoccupied
            bool isUnoccupied = true;
            foreach (VEntity v in positionTrackSystem.GetAtCoord(moveEvent.targetCoord)) {
                if (VEntityComponentSystemManager.HasVComponent<MovementBlockComponent>(v)) {
                    isUnoccupied = false;
                }
            }

            if (isUnoccupied) {
                PositionComponent movedPosition = ecsManager.GetVComponent<PositionComponent>(moveEvent.sourceId);
                Coord prevPosition = movedPosition.position;
                movedPosition.position = moveEvent.targetCoord;
                positionTrackSystem.Update(movedPosition.position, moveEvent.sourceId);

                ecsManager.QueueAnimationEvent(ecsManager.CreateEntity("MoveAnim", component : new MovementAnimationEvent {
                    entityToMove = moveEvent.sourceId,
                        from = prevPosition,
                        to = moveEvent.targetCoord
                }));
            }
        }
    }

    public class MovementAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return false;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<MovementAnimationEvent>(entity);
        }

        public override IEnumerator StartAnimation(VEntity entity, Action yieldAnimation) {
            MovementAnimationEvent moveEvent = entity.GetVComponent<MovementAnimationEvent>();
            Tween myTween = ecsManager.GetVEntityById(moveEvent.entityToMove).GetVComponent<PositionDisplayComponent>().mainTransform.DOMove(ecsManager.GetSystem<PositionWorldConversionSystem>().GetTransformFromCoord(moveEvent.to), 0.5f);
            yield return myTween.WaitForCompletion();
            yieldAnimation();
        }
    }
}