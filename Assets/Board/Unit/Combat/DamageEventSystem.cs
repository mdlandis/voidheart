using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voidheart {

    /*
     * How to get enemies to deal damage
     * 
     * First: Getting enemy attack patterns
     * 
     * Use the existing group targeting architecture.
     * For example, let's say you want to make an enemy that attacks in cardinal directions.
     * 
     * Add a TargetingComponent to the enemy entity. Ignore Targeting Methods and Valid Targeting Methods
     * we only care about Group Targeting Methods.
     * 
     * Add a group targeting method, choose Local Cardinal (Cardinal might also work)
     * Offset 0 length 1
     * 
     * Now wherever your code that prompts enemies to attack is, do the following:\
     * 
     * First we get the list of coords you are attacking.
     * 
     * List<Coord> selected = new List<Coord>();
     * foreach(GroupTargetingMethod m in enemy.GetVComponent<TargetingComponent>().groupTargetingMethods) {
     *   selected.AddRange(m.SelectCoords(enemy.coord, enemy.coord, ecsManager)
     *   //Pass in as arguments the enemy's coordinate for the first 2, and the EcsManager instance for the third 
     * }
     * 
     * Do ecsManager.createEvent("enemyDamage", component: new DamageEventComponent {
     *   team = Team.ENEMY,
     *   damageAmount = //however much damage
     *   sourceId = enemy.entityId (get the entity Id of the enemy you're operating on, forgot the command)
     *   targetCoord = Coord.nullCoord (you can probably ignore this, if you wanna be safe set it to the enemy's coord)
     *   groupTargetCoords = selected //the list we just made
     * });
     * 
     * Should work, but updating the highlighting to work with this is a whole different beast
     * 
     */

    [Serializable]
    public class AoeDamageEventComponent : VComponent {
        public string fromId;
        public Coord coord;
        public int damageAmount;
    }

    [Serializable]
    public class DamageEffectEventComponent : VComponent, CardEffectComponent {
        public int damageAmount;

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
    public class DamageEventSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<DamageEffectEventComponent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            DamageEffectEventComponent damageComponent = VEntityComponentSystemManager.GetVComponent<DamageEffectEventComponent>(entity);
            VEntity damageDealingEnitty = ecsManager.GetVEntityById(damageComponent.sourceId);
            foreach (Coord c in damageComponent.groupTargetCoords) {
                VEntity damageDealtEntity = ecsManager.GetSystem<UnitFinderSystem>().GetUnitAtCoord(c);
                if (damageDealtEntity != null && damageDealtEntity.GetVComponent<TeamComponent>().team != damageDealingEnitty.GetVComponent<TeamComponent>().team) {
                    ecsManager.ExecuteImmediateEvent("lowLevelDamageEvent", component : new LowLevelDealDamageEvent {
                        damageAmount = damageComponent.damageAmount,
                            sourceId = damageComponent.sourceId,
                            receiverId = damageDealtEntity.id
                    });
                }
            }
        }
    }

    [Serializable]
    public class LowLevelDealDamageEvent : VComponent {
        public int damageAmount;

        public string sourceId;
        public string receiverId;
    }

    [Serializable]
    public class LowLevelDamageSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<LowLevelDealDamageEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            LowLevelDealDamageEvent damageEvent = entity.GetVComponent<LowLevelDealDamageEvent>();

            if (damageEvent.damageAmount <= 0) {
                return;
            }

            VEntity damageDealtEntity = ecsManager.GetVEntityById(damageEvent.receiverId);
            HealthComponent h = VEntityComponentSystemManager.GetVComponent<HealthComponent>(damageDealtEntity);
            h.currHealth -= damageEvent.damageAmount;

            if (h.currHealth <= 0) {
                h.currHealth = 0;
                ecsManager.CreateEvent("unitDeath", component : new DeathEventComponent {
                    id = damageDealtEntity.id
                });
            }

            ecsManager.QueueAnimationEvent("setHealth", components : new VComponent[] {
                new HealthSetAnimationEvent {
                    targetEntity = damageDealtEntity.id,
                        currHealth = h.currHealth,
                        maxHealth = h.maxHealth
                },
                new UnitDamagedAnimationEvent {
                    targetEntity = damageDealtEntity.id
                }
            });
        }
    }

    [System.Serializable]
    public class UnitDamagedAnimationEvent : VComponent {
        public string targetEntity;
    }

    [System.Serializable]
    public class UnitDamagedAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<UnitDamagedAnimationEvent>(entity);
        }

        public override void DoImmediateAnimation(VEntity entity) {
            UnitDamagedAnimationEvent unitDamagedEvent = entity.GetVComponent<UnitDamagedAnimationEvent>();
            VEntity targetEntity = ecsManager.GetVEntityByIdIncludingRemoved(unitDamagedEvent.targetEntity);

            UnitDisplayComponent unitDisplay = targetEntity.GetVComponent<UnitDisplayComponent>();
            if (unitDisplay != null) {
                var tween = unitDisplay.unitDisplayGameObject.GetSpriteRenderer().DOColor(Color.red, 0.1f).SetLoops(4, LoopType.Yoyo);
            }
        }
    }

    [Serializable]
    public class DeathEventComponent : VComponent {
        public string id;
    }

    [Serializable]
    public class DeathEventSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<DeathEventComponent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            DeathEventComponent ecDeath = VEntityComponentSystemManager.GetVComponent<DeathEventComponent>(entity);
            VEntity dyingEntity = ecsManager.GetVEntityById(ecDeath.id);

            if (dyingEntity != null) {
                HealthComponent h = dyingEntity.GetVComponent<HealthComponent>();
                if (h != null && h.currHealth > 0) {
                    return;
                }

                ecsManager.MarkRemovalEntity(dyingEntity);

                // remove the entity from all relevant things
                UnitsList unitsList = ecsManager.GetVSingletonComponent<UnitsList>();
                unitsList.unitIds.Remove(dyingEntity.id);
                PositionTrackSystem positionTrackSystem = ecsManager.GetSystem<PositionTrackSystem>();
                positionTrackSystem.Untrack(dyingEntity.id);

                ecsManager.QueueAnimationEvent("DeathAnimation", component : new DeathAnimationEvent {
                    deathTransform = dyingEntity.GetVComponent<PositionDisplayComponent>().mainTransform,
                        dyingEntityId = dyingEntity.id
                });
            }
        }
    }

    [System.Serializable]
    public class DeathAnimationEvent : VComponent {
        public Transform deathTransform;
        public string dyingEntityId;
    }

    [System.Serializable]
    public class DeathAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<DeathAnimationEvent>(entity);
        }

        public override void DoImmediateAnimation(VEntity entity) {
            DeathAnimationEvent deathAnimationEvent = entity.GetVComponent<DeathAnimationEvent>();

            if (deathAnimationEvent.deathTransform != null) {
                deathAnimationEvent.deathTransform.gameObject.SetActive(false);
                GameObject.Destroy(deathAnimationEvent.deathTransform.gameObject);
            }

            ecsManager.GetAnimationSystem<HighlightDisplaySystem>().Remove(null, new string[] {
                deathAnimationEvent.dyingEntityId
            });
        }
    }

}