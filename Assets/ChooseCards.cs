using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public struct OfferingChances {
        public float common;
        public float rare;
        public float legendary;
    }

    public class ChooseCards : MonoBehaviour {
        public CardDatabaseScriptableObject AllCards;
        public CardListScriptableObject Deck = null;
        public OfferingChances OfferingChances;
        [SerializeField]
        public GameObject submitButton;

        // Start is called before the first frame update
        void Start() {
            List<EntityScriptableObject> opheliaPool = AllCards.Cards.FindAll(c => VEntityComponentSystemManager.GetVComponent<CardClassComponent>(c.entity).cardType == CardType.OPHELIA || VEntityComponentSystemManager.GetVComponent<CardClassComponent>(c.entity).cardType == CardType.NEUTRAL);
            foreach (Transform child in transform.Find("OpheliaCards")) {
                child.gameObject.GetComponent<MetaCardGameObject>().InitializeFields(GenerateRandomCard(opheliaPool).entity);
            }
            List<EntityScriptableObject> orionPool = AllCards.Cards.FindAll(c => VEntityComponentSystemManager.GetVComponent<CardClassComponent>(c.entity).cardType == CardType.ORION || VEntityComponentSystemManager.GetVComponent<CardClassComponent>(c.entity).cardType == CardType.NEUTRAL);
            foreach (Transform child in transform.Find("OrionCards")) {
                child.gameObject.GetComponent<MetaCardGameObject>().InitializeFields(GenerateRandomCard(orionPool).entity);
            }
        }

        // Update is called once per frame
        void Update() {

        }

        public EntityScriptableObject GenerateRandomCard(List<EntityScriptableObject> pool) {
            CardRarity rarity; // Default to common rarity
            float rand = UnityEngine.Random.Range(0.0f, 1.0f);

            if (rand <= OfferingChances.legendary && pool.FindAll(c => VEntityComponentSystemManager.GetVComponent<CardRarityComponent>(c.entity).cardRarity == CardRarity.LEGENDARY).Count > 0)
                rarity = CardRarity.LEGENDARY;
            else if (rand <= OfferingChances.legendary + OfferingChances.rare && pool.FindAll(c => VEntityComponentSystemManager.GetVComponent<CardRarityComponent>(c.entity).cardRarity == CardRarity.RARE).Count > 0)
                rarity = CardRarity.RARE;
            else rarity = CardRarity.COMMON;

            var matchingCards = pool.FindAll(c => VEntityComponentSystemManager.GetVComponent<CardRarityComponent>(c.entity).cardRarity == rarity);
            if (matchingCards.Count == 0) return null;

            EntityScriptableObject card = matchingCards[UnityEngine.Random.Range(0, matchingCards.Count)];
            matchingCards.Remove(card);
            return card;
        }

        public void AddToDeck() {
            Deck.Add(VEntityComponentSystemManager.GetVComponent<CardNameComponent>(transform.Find("OpheliaCards").GetComponent<ChooseSingleCard>().selectedCard).name);
            Deck.Add(VEntityComponentSystemManager.GetVComponent<CardNameComponent>(transform.Find("OrionCards").GetComponent<ChooseSingleCard>().selectedCard).name);
        }

    }
}