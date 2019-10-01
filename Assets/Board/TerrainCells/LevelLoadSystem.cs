using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Voidheart {
    /// <summary>Keeps track of current turn.</summary>
    [Serializable]
    public class LevelLoadSystem : VSystem {
        [SerializeField]
        private Level levelToLoad = null;

        [SerializeField]
        private CardDatabaseScriptableObject cardDatabase = null;

        [SerializeField]
        private CardListScriptableObject deckList = null;

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<StartGameComponent>(entity);
        }

        protected override void OnExecuteEvent(VEntity entity) {
            CellContent[, ] _currLevel = levelToLoad.levelData;
            for (int i = 0; i < _currLevel.GetLength(0); i++) {
                for (int j = 0; j < _currLevel.GetLength(1); j++) {
                    Coord c = new Coord(j, i);
                    if (_currLevel[i, j].unitData != null) {
                        EntityScriptableObject unitDataObject = _currLevel[i, j].unitData;
                        ecsManager.CreateEvent("UnitCreate", component : new UnitCreateEvent {
                            position = c,
                                entityBlueprint = unitDataObject.entity
                        });
                    }
                }
            }

            for (int i = 0; i < _currLevel.GetLength(0); i++) {
                for (int j = 0; j < _currLevel.GetLength(1); j++) {
                    Coord c = new Coord(j, i);
                    // create events for making cells
                    ecsManager.CreateEvent("CellCreate", component : new CreateTerrainCellEvent {
                        coord = c,
                            movementCost = _currLevel[i, j].cellStruct.MovementCost,
                            cellType = _currLevel[i, j].cellStruct.cellType
                    });
                }
            }

            //Initialize Deck
            /* foreach(string cardEntityId in deckListObject.decklist) {
                //ecsManager.CreateEvent
            }*/

            CardZoneDataComponent cardZones = ecsManager.GetVSingletonComponent<CardZoneDataComponent>();
            List<VEntity> cards = new List<VEntity>();
            foreach (string cardName in deckList.cardNames) {
                // look up card in card database

                EntityScriptableObject cardEntity = cardDatabase.Cards.Find((scriptableObject) => scriptableObject.entity.GetVComponent<CardNameComponent>().name == cardName);
                Assert.IsNotNull(cardEntity);
                VEntity newCardEntity = ecsManager.InsantiateEntityFromBlueprint(cardEntity.entity);
                newCardEntity.GetVComponent<CardDisplayComponent>().cardDisplay = CardViewController.Instance.InitCard(newCardEntity);
                cards.Add(newCardEntity);

                ecsManager.ExecuteImmediateEvent("AddCardToZone", component : new CardZoneMoveEvent {
                    source = Zone.NULL,
                        destination = Zone.DECK,
                        card = newCardEntity.id,
                });

               
            }

            CardGameObject cantrip = GameObject.FindGameObjectWithTag("cantrip").GetComponent<CardGameObject>();
            EntityScriptableObject cantripEntity = cardDatabase.Cards.Find((scriptableObject) => scriptableObject.entity.GetVComponent<CardNameComponent>().name == "Cantrip");
            Assert.IsNotNull(cantripEntity);
            VEntity newCantripEntity = ecsManager.InsantiateEntityFromBlueprint(cantripEntity.entity);
            newCantripEntity.GetVComponent<CardDisplayComponent>().cardDisplay = CardViewController.Instance.InitCard(newCantripEntity);
            cards.Add(newCantripEntity);
            cantrip.cardEntityId = newCantripEntity.id;
            Debug.Log("Name: " + newCantripEntity.GetVComponent<CardNameComponent>().name);
            cantrip.cantrip = true;
            DeckHelper.Shuffle(cardZones.zones[Zone.DECK]);
            //HACK
        }
    }
}