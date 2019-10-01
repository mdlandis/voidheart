using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Voidheart {

    public enum HandState {
        IDLE,
        HOVER,
        SELECTED
    }

    public class CardViewController : SerializedSingleton<CardViewController> {
        [SerializeField]
        CardGameObject cardPrefab = null;
        public Transform selectLocation;
        [SerializeField]
        int maxMana = 0;

        [SerializeField]
        int maxHandSize = 0;
        [SerializeField]
        TextMeshPro deckText = null;

        [SerializeField]
        TextMeshPro handText = null;

        [SerializeField]
        TextMeshPro discardText = null;

        [SerializeField]
        TextMeshPro manaText = null;
        public int virtualHandBuffer;

        CardMover cardMover;

        CardGameObject cantrip;

        VEntityComponentSystemManager board;

        [ShowInInspector, ReadOnly]
        int mana;

        List<CardGameObject> deck;
        List<CardGameObject> hand;
        List<CardGameObject> discardPile;

        public HandState handState;
        public HandState MyHandState {
            get {
                return handState;
            }
            set {
                handState = value;
                RecalculateHandAppearance();
            }
        }

        private List<CardGameObject> getZoneList(Zone z) {
            switch (z) {
                case Zone.DECK:
                    return deck;
                case Zone.DISCARD:
                    return discardPile;
                case Zone.HAND:
                    return hand;
                default:
                    return null;
            }
        }

        private HandControlSystem handControlSystem;

        void Start() {
            deck = new List<CardGameObject>();
            hand = new List<CardGameObject>();
            discardPile = new List<CardGameObject>();
            board = FindObjectOfType<VEntityComponentSystemManager>();
            cardMover = FindObjectOfType<CardMover>();
            cantrip = GameObject.FindWithTag("cantrip").GetComponent<CardGameObject>();
            handControlSystem = VEntityComponentSystemManager.Instance.GetSystem<HandControlSystem>();
        }

        public CardGameObject InitCard(VEntity e) {
            CardGameObject newCard = Instantiate<CardGameObject>(cardPrefab);
            newCard.InitializeFromEntity(e);
            newCard.transform.parent = this.transform;
            return newCard;
        }

        internal void CardMove(Zone source, Zone destination, CardGameObject cardGameObject) {
            if (destination == Zone.INPLAY) {
                return;
            }

            if (source == Zone.INPLAY) {
                source = Zone.HAND;
            }

            if (source != Zone.NULL) {
                getZoneList(source).Remove(cardGameObject);
            }

            if (source == Zone.HAND) {
                cardGameObject.inHand = false;
            }

            if (destination != Zone.NULL) {
                getZoneList(destination).Add(cardGameObject);
            }

            if (destination == Zone.HAND) {
                cardGameObject.inHand = true;
            }

            if (source == Zone.HAND || destination == Zone.HAND) {
                RecalculateHandAppearance();
            }

            if (destination == Zone.DISCARD) {
                cardMover.MoveCardToDiscard(cardGameObject);
            }

            if (destination == Zone.DECK) {
                cardMover.MoveCardToDeck(cardGameObject);
            }
        }

        private void RecalculateHandAppearance() {
            cardMover.RecalculateHandAppearance(handState, hand, cantrip, maxHandSize);
        }

        // Update is called once per frame
        void Update() {
            deckText.text = "" + deck.Count;
            handText.text = "" + hand.Count;
            discardText.text = "" + discardPile.Count;
            manaText.text = mana + "/" + maxMana;
        }

        public void SetMana(int mana) {
            this.mana = mana;
        }

        public void SetMaxMana(int mana) {
            this.maxMana = mana;
        }

        public bool IsPlayable(string cardId) {
            return handControlSystem.IsPlayable(cardId);
        }
    }
}