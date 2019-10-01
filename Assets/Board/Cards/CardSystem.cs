using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Voidheart {

    public enum Zone {
        DECK,
        HAND,
        DISCARD,
        NULL,
        INPLAY,
    }

    [Serializable]
    public class CardZoneDataComponent : VComponentSingleton {
        public Dictionary<Zone, List<string>> zones;
    }

    [Serializable]
    public class CardZoneMoveEvent : VComponent {
        public Zone source;
        public Zone destination;
        public string card;
    }

    public class CardZoneSystem : VSystem {
        protected override void OnExecuteEvent(VEntity entity) {
            CardZoneMoveEvent eventity = VEntityComponentSystemManager.GetVComponent<CardZoneMoveEvent>(entity);
            CardZoneDataComponent comp = ecsManager.GetVSingletonComponent<CardZoneDataComponent>();
            if (eventity.source != Zone.NULL) {
                //Assert.IsTrue(comp.zones[eventity.source].Contains(eventity.card));
                if(!comp.zones[eventity.source].Contains(eventity.card))
                {
                    return; //HACK
                }
                comp.zones[eventity.source].Remove(eventity.card);
                comp.zones[eventity.destination].Add(eventity.card);
            } else {
                comp.zones[eventity.destination].Add(eventity.card);
            }

            ecsManager.QueueAnimationEvent(ecsManager.CreateEntity("CardMovementAnimation", component : new CardZoneMoveEvent {
                source = eventity.source,
                    destination = eventity.destination,
                    card = eventity.card
            }));
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardZoneMoveEvent>(entity);
        }
    }

    public class CardZoneMoveAnimationSystem : VAnimationSystem {
        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardZoneMoveEvent>(entity);
        }

        public override void DoImmediateAnimation(VEntity entity) {
            CardZoneMoveEvent moveEvent = entity.GetVComponent<CardZoneMoveEvent>();
            CardViewController.Instance.CardMove(moveEvent.source, moveEvent.destination, ecsManager.GetVEntityById(moveEvent.card).GetVComponent<CardDisplayComponent>().cardDisplay);
        }
    }

    [Serializable]
    public class CardDrawEvent : VComponent {
        public int numCards;
    }

    public class CardDrawSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardDrawEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            CardDrawEvent cardDraw = entity.GetVComponent<CardDrawEvent>();
            CardZoneDataComponent zoneData = ecsManager.GetVSingletonComponent<CardZoneDataComponent>();
            PlayerHandComponent PlayerHandComponent = ecsManager.GetVSingletonComponent<PlayerHandComponent>();
            for (int cardDrawIndex = 0; cardDrawIndex < cardDraw.numCards; ++cardDrawIndex) {
                // get top card of deck             
                //If deck is empty, shuffle discard into deck
                if (zoneData.zones[Zone.DECK].Count == 0) {
                    //If there is no discard pile, draw fizzles.
                    if (zoneData.zones[Zone.DISCARD].Count == 0) {
                        return;
                    }
                    for (int i = zoneData.zones[Zone.DISCARD].Count - 1; i >= 0; i--) {
                        ecsManager.ExecuteImmediateEvent("moveZone", component : new CardZoneMoveEvent {
                            source = Zone.DISCARD,
                                destination = Zone.DECK,
                                card = zoneData.zones[Zone.DISCARD][i]
                        });
                    }
                    DeckHelper.Shuffle(zoneData.zones[Zone.DECK]);
                }

                string cardToDraw = zoneData.zones[Zone.DECK][0];
                //If there's space, draw, else discard.
                if (zoneData.zones[Zone.HAND].Count < PlayerHandComponent.maxHandSize) {
                    ecsManager.ExecuteImmediateEvent("moveZone", component : new CardZoneMoveEvent {
                        source = Zone.DECK,
                            destination = Zone.HAND,
                            card = cardToDraw
                    });
                } else {
                    ecsManager.ExecuteImmediateEvent("moveZone", component : new CardZoneMoveEvent {
                        source = Zone.DECK,
                            destination = Zone.DISCARD,
                            card = cardToDraw
                    });
                }
            }
        }
    }

    [Serializable]
    public class CardDiscardEvent : VComponent {
        public string[] cardIds;
    }

    public class CardDiscardSystem : VSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardDiscardEvent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            CardDiscardEvent discardedCards = entity.GetVComponent<CardDiscardEvent>();
            CardZoneDataComponent zoneData = ecsManager.GetVSingletonComponent<CardZoneDataComponent>();
            PlayerHandComponent PlayerHandComponent = ecsManager.GetVSingletonComponent<PlayerHandComponent>();
            foreach (string cardToDiscard in zoneData.zones[Zone.HAND].ToArray()) {
                if (Array.IndexOf(discardedCards.cardIds, cardToDiscard) >= 0) {
                    ecsManager.ExecuteImmediateEvent("moveZone", component : new CardZoneMoveEvent {
                        source = Zone.HAND,
                            destination = Zone.DISCARD,
                            card = cardToDiscard
                    });
                }
            }
        }
    }

    [Serializable]
    public class CardCreateEvent : VComponent {
        public VEntity blueprintCard;
        public Zone destination;
    }

    public class CardCreationSytem : VSystem {
        protected override void OnExecuteEvent(VEntity entity) {
            CardCreateEvent eventity = VEntityComponentSystemManager.GetVComponent<CardCreateEvent>(entity);
            VEntity newCard = ecsManager.InsantiateEntityFromBlueprint(eventity.blueprintCard);
            ecsManager.CreateEvent("CardMove", component : new CardZoneMoveEvent {
                source = Zone.NULL,
                    destination = eventity.destination,
                    card = newCard.id
            });
        }

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<CardCreateEvent>(entity);
        }
    }
}