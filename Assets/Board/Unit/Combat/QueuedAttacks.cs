using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidheart {
    public class QueuedActionComponent : VComponent {
        public Coord[] relativeTargetedCoords;
        public List<CardEffectComponent> effects;
    }

    public class QueueActionEvent : VComponent {
        public string entityId;
    }

    public class QueueActionEventSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<QueueActionEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity eventEntity) {
            QueueActionEvent queueActionEvent = eventEntity.GetVComponent<QueueActionEvent>();
            VEntity queuer = ecsManager.GetVEntityById(queueActionEvent.entityId);
            ecsManager.AddComponent(queuer, new QueuedActionComponent {
                relativeTargetedCoords = Coord.cardinalCoords,
                    effects = new List<CardEffectComponent> {
                        new DamageEffectEventComponent {
                            damageAmount = 5
                        }
                    }
            });

            Coord[] targetedCoords = Coord.ResolveRelativeCoords(queuer.GetVComponent<QueuedActionComponent>().relativeTargetedCoords, queuer.GetVComponent<PositionComponent>().position);
            ecsManager.QueueAnimationEvent("highlight", component : new GenericImmediateAnimationEvent {
                a = (passedEcsManager) => {
                    passedEcsManager.GetAnimationSystem<HighlightDisplaySystem>().CreateHighlightsWithTags(targetedCoords, new List<string> {
                        "EnemyAttack",
                        queuer.id
                    }, passedEcsManager.GetAnimationSystem<HighlightDisplaySystem>().redColor);
                }
            });
        }
    }

    public class QueuedActionResolutionSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<QueuedActionComponent>(entity);
        }

        protected override void OnEnemyActionsResolve(VEntity entity) {
            QueuedActionComponent queuedAction = entity.GetVComponent<QueuedActionComponent>();
            List<VComponent> effectComponentList = ecsManager.CloneComponents(queuedAction.effects.Cast<VComponent>().ToList());
            PositionComponent entityPosition = entity.GetVComponent<PositionComponent>();
            Coord[] targetedCoords = Coord.ResolveRelativeCoords(queuedAction.relativeTargetedCoords, entityPosition.position);
            // fill in the components with the right data
            foreach (VComponent component in effectComponentList) {
                if (component is CardEffectComponent c) {
                    c.sourceId = entity.id;
                    c.targetCoord = Coord.nullCoord;
                    c.groupTargetCoords = new List<Coord>(targetedCoords);
                }
            }

            ecsManager.ExecuteImmediateEvent("CardEffect", components : effectComponentList);

            ecsManager.RemoveComponent<QueuedActionComponent>(entity);

            //handle de highlight animation
            ecsManager.QueueAnimationEvent("wait", component : new GenericBlockingAnimationEvent {
                a = (passedEcsManager) => { },
                    duration = 0.5f,
            });

            ecsManager.QueueAnimationEvent("dehighlight", component : new GenericImmediateAnimationEvent {
                a = (passedEcsManager) => {
                    passedEcsManager.GetAnimationSystem<HighlightDisplaySystem>().Remove(targetedCoords, new List<string> {
                        entity.id
                    });
                }
            });

        }
    }

    // <summary>This class observes movement events, and updates the threat ranges of queued actions if a queued attack is pushed.</summary>
    public class QueuedActionOriginMovementSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<MovementEvent>(entity);
        }

        protected override void OnBeforeEvent(VEntity eventEntity) {
            MovementEvent movement = eventEntity.GetVComponent<MovementEvent>();
            VEntity movedEntity = ecsManager.GetVEntityById(movement.sourceId);
            QueuedActionComponent queuedAction = movedEntity.GetVComponent<QueuedActionComponent>();
            if (queuedAction != null) {
                Debug.Log("removing old highlights");
                // update the view with animation event
                Coord[] targetedCoords = Coord.ResolveRelativeCoords(queuedAction.relativeTargetedCoords, movedEntity.GetVComponent<PositionComponent>().position);

                ecsManager.QueueAnimationEvent("highlight", component : new GenericImmediateAnimationEvent {
                    a = (passedEcsManager) => {
                        passedEcsManager.GetAnimationSystem<HighlightDisplaySystem>().Remove(null, new List<string> {
                            movedEntity.id
                        });
                    }
                });
            }
        }

        protected override void OnAfterEvent(VEntity eventEntity) {
            MovementEvent movement = eventEntity.GetVComponent<MovementEvent>();
            VEntity movedEntity = ecsManager.GetVEntityById(movement.sourceId);
            QueuedActionComponent queuedAction = movedEntity.GetVComponent<QueuedActionComponent>();
            if (queuedAction != null) {
                // update the view with animation event
                Coord[] targetedCoords = Coord.ResolveRelativeCoords(queuedAction.relativeTargetedCoords, movedEntity.GetVComponent<PositionComponent>().position);

                ecsManager.QueueAnimationEvent("highlight", component : new GenericImmediateAnimationEvent {
                    a = (passedEcsManager) => {
                        passedEcsManager.GetAnimationSystem<HighlightDisplaySystem>().CreateHighlightsWithTags(targetedCoords, new List<string> {
                            "EnemyAttack",
                            movedEntity.id
                        }, passedEcsManager.GetAnimationSystem<HighlightDisplaySystem>().redColor);
                    }
                });
            }
        }

    }
}