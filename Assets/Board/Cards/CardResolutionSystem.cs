using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidheart {
    public class CardManaDeductSystem : VSystem {

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardPlayEvent>(entity);
        }

        protected override void OnBeforeEvent(VEntity eventEntity) {
            CardPlayEvent cardPlayEvent = eventEntity.GetVComponent<CardPlayEvent>();
            VEntity card = ecsManager.GetVEntityById(cardPlayEvent.cardId);
            PlayerManaComponent playerMana = ecsManager.GetVSingletonComponent<PlayerManaComponent>();
            int prevMana = playerMana.currMana;
            playerMana.currMana -= card.GetVComponent<ManaCostComponent>().cost;
            ecsManager.QueueAnimationEvent("cardPlayedManaDrain", component : new PlayerManaSetAnimation {
                maxMana = playerMana.maxMana,
                    newMana = playerMana.currMana,
                    oldMana = prevMana,
            });
        }
    }

    public class CardMoveInPlaySystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardPlayEvent>(entity);
        }

        protected override void OnBeforeEvent(VEntity eventEntity) {
            CardPlayEvent cardPlayEvent = eventEntity.GetVComponent<CardPlayEvent>();
            VEntity card = ecsManager.GetVEntityById(cardPlayEvent.cardId);

            //ecsManager.ExecuteImmediateEvent("cardMoveInPlay", component: new CardZoneMoveEvent
            //{
            //    source = Zone.HAND,
            //    destination = Zone.INPLAY,
            //    card = card.id,
            //});
        }
    }

    public class CardResolutionSystem : VSystem {
        CardEffectType EffectComponentToEffectTypeHelper(CardEffectComponent c) {
            if (c is DamageEffectEventComponent) {
                return CardEffectType.ATTACK;
            } else if (c is MovementEvent) {
                return CardEffectType.MOVE;
            } else {
                return CardEffectType.BOTH;
            }
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardPlayEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity eventEntity) {
            CardPlayEvent cardPlayEvent = eventEntity.GetVComponent<CardPlayEvent>();
            VEntity caster = ecsManager.GetVEntityById(cardPlayEvent.casterId);
            VEntity card = ecsManager.GetVEntityById(cardPlayEvent.cardId);
            Coord dest = cardPlayEvent.targetSpace;
            Dictionary<CardEffectType, List<Coord>> groupTargetCoords = cardPlayEvent.groupTargetingSpaces;

            foreach (CardEffectType t in groupTargetCoords.Keys) {
                Debug.Log("Type Key: " + t);
            }

            // create a new event entity with all the card actions
            GameplayEffectComponent effects = card.GetVComponent<GameplayEffectComponent>();

            List<VComponent> effectComponentList = ecsManager.CloneComponents(effects.effectEvents.Cast<VComponent>().ToList());
            // fill in the components with the right data
            foreach (VComponent component in effectComponentList) {
                if (component is CardEffectComponent c) {
                    c.sourceId = caster.id;
                    c.targetCoord = dest;
                    c.groupTargetCoords = groupTargetCoords[EffectComponentToEffectTypeHelper(c)];

                    if (c is DamageEffectEventComponent d) {
                        Debug.Log("Doing damage to " + c.groupTargetCoords.Count + " spaces.");
                    }
                }
            }

            ecsManager.ExecuteImmediateEvent("CardEffect", components : effectComponentList);

            // move the card from the hand to the deck.
            ecsManager.ExecuteImmediateEvent("cardMoveToDiscard", component : new CardZoneMoveEvent {
                source = Zone.HAND,
                    destination = Zone.DISCARD,
                    card = card.id,
            });
        }
    }
}