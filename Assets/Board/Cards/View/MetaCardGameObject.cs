using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Voidheart {
    public class MetaCardGameObject : MonoBehaviour {
        [SerializeField]
        private TextMeshPro nameText = null;

        [SerializeField]
        private TextMeshPro costText = null;

        [SerializeField]
        private TextMeshPro effectText = null;

        [SerializeField]
        private TextMeshPro classText = null;

        [SerializeField]
        private GameObject orionQuad = null;

        [SerializeField]
        private GameObject opheliaQuad = null;

        [SerializeField]
        private GameObject neutralQuad = null;

        [SerializeField]
        private GameObject selectedText = null;

        public Dictionary<CardType, GameObject> cardBacks;

        private int cost;
        private string effect;

        VEntity cardTemplate;

        public void InitializeFields(VEntity cardTemplate) {
            if (cardTemplate == null) return;
            this.cardTemplate = cardTemplate;
            cardBacks = new Dictionary<CardType, GameObject>();
            cardBacks.Add(CardType.NEUTRAL, neutralQuad);
            cardBacks.Add(CardType.ORION, orionQuad);
            cardBacks.Add(CardType.OPHELIA, opheliaQuad);
            foreach (CardType t in cardBacks.Keys) {
                cardBacks[t].SetActive(false);
            }

            cost = VEntityComponentSystemManager.GetVComponent<ManaCostComponent>(cardTemplate).cost;
            effect = VEntityComponentSystemManager.GetVComponent<EffectTextComponent>(cardTemplate).effect;
            CardType cardType = VEntityComponentSystemManager.GetVComponent<CardClassComponent>(cardTemplate).cardType;

            nameText.text = VEntityComponentSystemManager.GetVComponent<CardNameComponent>(cardTemplate).name;
            costText.text = cost.ToString();
            effectText.text = effect;
            classText.text = StringUtils.ClassEnumToString(cardType);
            cardBacks[cardType].SetActive(true);
        }

        private void OnMouseDown() {
            bool isActive = selectedText.activeSelf;

            ChooseSingleCard chooseSingleCard = transform.parent.gameObject.GetComponent<ChooseSingleCard>();
            chooseSingleCard.UnselectAll();

            if (isActive) selectedText.SetActive(false);
            else {
                selectedText.SetActive(true);
                chooseSingleCard.selectedCard = cardTemplate;
            }
        }

        public void Unselect() {
            selectedText.SetActive(false);
        }
    }
}