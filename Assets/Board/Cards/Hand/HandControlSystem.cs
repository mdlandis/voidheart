using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public class PlayerHandComponent : VComponentSingleton {
        public int startTurnDrawCount;
        public int maxHandSize;
    }

    [System.Serializable]
    public class HandDrawSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<PlayerHandComponent>(entity);
        }

        protected override void OnPlayerTurnStart(VEntity entity) {
            PlayerHandComponent playerHandComponent = entity.GetVComponent<PlayerHandComponent>();

            ecsManager.CreateEvent("StartTurnDraw", component : new CardDrawEvent {
                numCards = playerHandComponent.startTurnDrawCount
            });
        }
    }

    [System.Serializable]
    public class HandControlSystem : VSystem {

        public override bool ShouldOperate(VEntity entity) {
            return false;
        }

        public bool IsPlayable(string cardId) {
            VEntity cardEntity = ecsManager.GetVEntityById(cardId);
            CardZoneDataComponent cardZoneData = ecsManager.GetVSingletonComponent<CardZoneDataComponent>();
            PlayerManaComponent mana = ecsManager.GetVSingletonComponent<PlayerManaComponent>();
            CurrentLifecycleComponent currLifecycle = ecsManager.GetVSingletonComponent<CurrentLifecycleComponent>();
            //HACK
            return currLifecycle.currentLifecycle == VLifecycle.PlayerTurnExecute && (cardZoneData.zones[Zone.HAND].Contains(cardId) || (cardEntity.GetVComponent<CardNameComponent>().name == "Cantrip" && !ecsManager.GetSystem<CantripSystem>().cantripUsed)) && cardEntity.GetVComponent<ManaCostComponent>().cost <= mana.currMana;
        }
    }

    [System.Serializable]
    public class HandDiscardSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<SetLifecycleEventComponent>(entity);
        }

        protected override void OnBeforeEvent(VEntity entity) {
            if (VEntityComponentSystemManager.GetVComponent<SetLifecycleEventComponent>(entity).newVLifecycle == VLifecycle.PlayerTurnStart) {
                CardZoneDataComponent cardZoneData = ecsManager.GetVSingletonComponent<CardZoneDataComponent>();
                ecsManager.ExecuteImmediateEvent("discardCards", component : new CardDiscardEvent {
                    cardIds = cardZoneData.zones[Zone.HAND].ToArray()
                });
            }
        }
    }
}