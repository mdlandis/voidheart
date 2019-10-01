using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {

    public class CardMover : MonoBehaviour {

        public Transform handLocation;
        public Transform discardLocation;
        public Transform deckLocation;

        public float cardWidth;
        public Vector3 baseScale;

        public float upDistance = 1.5f;
        public float scaleChange = 1.25f;

        BoardInputController inputController;

        // Start is called before the first frame update
        void Start() {
            inputController = GameObject.FindObjectOfType<BoardInputController>();
        }

        public void RecalculateHandAppearance(HandState handState, List<CardGameObject> hand, CardGameObject cantrip, int maxHandSize) {
            int handSize = hand.Count;
            if (handState == HandState.IDLE) {
                for (int i = 0; i < handSize; i++) {
                    hand[i].handPosition = handLocation.position + (i - (float) handSize / 2) * Vector3.right * Mathf.Lerp(cardWidth, 1, ((float) handSize) / (float) maxHandSize);
                    hand[i].handPosition += Vector3.back * i;
                    hand[i].TargetPosition = hand[i].handPosition;
                    hand[i].targetScale = baseScale;
                }
                //cantrip.TargetPosition = cantripLocation.position;
                cantrip.targetScale = cantrip.transform.localScale;
                cantrip.GetComponent<Cantrip>().Hover(false);
            } else if (handState == HandState.HOVER) {
                int hoverIndex = -1;
                for (int i = 0; i < handSize; i++) {
                    if (hand[i] == inputController.cardHovering) {
                        hoverIndex = i;
                    }
                    hand[i].handPosition = handLocation.position + (i - (float) handSize / 2) * Vector3.right * Mathf.Lerp(cardWidth, 1, ((float) handSize) / (float) maxHandSize);
                    hand[i].handPosition += Vector3.back * i;
                    hand[i].TargetPosition = hand[i].handPosition;
                }
                if (hoverIndex == -1) {
                    cantrip.GetComponent<Cantrip>().Hover(true);
                    //cantrip.TargetPosition += Vector3.right;
                } else {
                    for (int i = 0; i < handSize; i++) {
                        if (i != hoverIndex) {
                            hand[i].TargetPosition += Vector3.left * (1.5f / (hoverIndex - i));
                        } else {
                            hand[i].TargetPosition += Vector3.up * upDistance; //+ Vector3.back * 3;
                            hand[i].targetScale = baseScale * scaleChange;
                        }

                    }
                }

            }

            float averageXPos = 0;
            for (int i = 0; i < handSize; i++) {
                averageXPos += hand[i].transform.position.x;
            }
            averageXPos = averageXPos / handSize;

            for (int i = 0; i < handSize; i++) {
                hand[i].targetAngle = new Vector3(0, 0, ((5 * (averageXPos - hand[i].transform.position.x)) + 360) % 360);
            }
        }

        public void MoveCardToDeck(CardGameObject card) {
            card.TargetPosition = deckLocation.position;
        }

        public void MoveCardToDiscard(CardGameObject card) {
            card.TargetPosition = discardLocation.position;
        }

        public void ClickCard(CardGameObject cardSelected) {
            if (cardSelected.GetComponent<Cantrip>() == null) {
                cardSelected.TargetPosition = cardSelected.CardViewController.selectLocation.position;
            }
            cardSelected.locked = true;
        }

        public void ResetCard(CardGameObject cardSelected) {
            cardSelected.locked = false;
            cardSelected.TargetPosition = cardSelected.handPosition;
        }
    }
}