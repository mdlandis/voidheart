using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Voidheart {
    public class BlockBuffComponent : VComponent {
        public int blockAmount;
    }

    public class BlockDisplayComponent : VComponent {
        [System.NonSerialized]
        public NumberedIconDisplay blockDisplay;
    }

    public class BlockApplyEffectEvent : VComponent, CardEffectComponent {
        public int blockAmount;

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

    [System.Serializable]
    public class BlockApplySystem : VSystem {
        [SerializeField]
        private NumberedIconDisplay iconDisplayPrefab = null;

        public override bool ShouldOperate(VEntity entity) {
            return entity.HasVComponent<BlockApplyEffectEvent>();
        }

        protected override void OnExecuteEvent(VEntity eventEntity) {
            BlockApplyEffectEvent blockEvent = eventEntity.GetVComponent<BlockApplyEffectEvent>();
            VEntity blockingEntity = ecsManager.GetVEntityById(blockEvent.sourceId);

            if (!blockingEntity.HasVComponent<BlockBuffComponent>()) {
                ecsManager.AddComponent(blockingEntity, new BlockBuffComponent { });
                ecsManager.AddComponent(blockingEntity, new BlockDisplayComponent {
                    blockDisplay = NumberedIconDisplay.Instantiate(iconDisplayPrefab)
                });

                NumberedIconDisplay blockDisplayForAppear = blockingEntity.GetVComponent<BlockDisplayComponent>().blockDisplay;
                blockDisplayForAppear.gameObject.transform.SetParent(blockingEntity.GetVComponent<PositionDisplayComponent>().mainTransform);
                blockDisplayForAppear.gameObject.transform.localPosition = new Vector3(.1f, .1f, 0.0f);

                ecsManager.QueueAnimationEvent("blockAppearAnim", component : new GenericImmediateAnimationEvent {
                    a = (passedEcsManager) => {
                        blockDisplayForAppear.Appear();
                    }
                });
            }

            BlockBuffComponent blockComponent = blockingEntity.GetVComponent<BlockBuffComponent>();
            blockComponent.blockAmount += (blockEvent.blockAmount);

            string blockAmountString = blockComponent.blockAmount.ToString();
            NumberedIconDisplay blockDisplay = blockingEntity.GetVComponent<BlockDisplayComponent>().blockDisplay;

            ecsManager.QueueAnimationEvent("blockIncrement", component : new GenericImmediateAnimationEvent {
                a = (passedEcsManager) => {
                    blockDisplay.SetValue(blockAmountString);
                }
            });
        }
    }

    public class BlockWearingOffSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return entity.HasVComponent<BlockBuffComponent>();
        }

        protected override void OnEnemyActionsEnd(VEntity entity) {
            ecsManager.RemoveComponent<BlockBuffComponent>(entity);

            if (entity.HasVComponent<BlockDisplayComponent>()) {
                NumberedIconDisplay blockDisplay = entity.GetVComponent<BlockDisplayComponent>().blockDisplay;
                ecsManager.RemoveComponent<BlockDisplayComponent>(entity);
                ecsManager.QueueAnimationEvent("blockRemoveEvent", component : new GenericImmediateAnimationEvent {
                    a = (passedEcsManager) => {
                        blockDisplay.Break();
                    }
                });
            }
        }
    }

    public class BlockDeductDamgeSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return entity.HasVComponent<LowLevelDealDamageEvent>();
        }

        protected override void OnBeforeEvent(VEntity entity) {
            LowLevelDealDamageEvent dealDamage = entity.GetVComponent<LowLevelDealDamageEvent>();
            VEntity blockingEntity = ecsManager.GetVEntityById(dealDamage.receiverId);

            BlockBuffComponent block = blockingEntity.GetVComponent<BlockBuffComponent>();

            if (block != null) {
                int blockDeduct = Math.Min(block.blockAmount, dealDamage.damageAmount);

                block.blockAmount -= blockDeduct;
                dealDamage.damageAmount -= blockDeduct;

                if (blockingEntity.HasVComponent<BlockDisplayComponent>()) {
                    int newBlockAmount = block.blockAmount;
                    NumberedIconDisplay blockDisplay = blockingEntity.GetVComponent<BlockDisplayComponent>().blockDisplay;
                    ecsManager.QueueAnimationEvent("blockRemoveEvent", component : new GenericBlockingAnimationEvent {
                        a = (passedEcsManager) => {
                                blockDisplay.transform.DOShakePosition(.5f, .1f);
                                blockDisplay.SetValue(newBlockAmount);
                            },
                            duration = 0.5f
                    });
                }
            }
        }
    }
}