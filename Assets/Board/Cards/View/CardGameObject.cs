namespace Voidheart {
    using System.Collections.Generic;
    using System;
    using TMPro;
    using UnityEngine;

    public class CardGameObject : MonoBehaviour {

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

        public bool cantrip = false;

        public Dictionary<CardType, GameObject> cardBacks = new Dictionary<CardType, GameObject>();

        public string cardEntityId {
            get;
            set;
        }
        private int cost;
        private string effect;
        private CardType cardType;
        public Vector3 handPosition;
        private Vector3 targetPosition;
        public Vector3 targetScale;
        public Vector3 targetAngle;
        public Vector3 TargetPosition {
            get {
                return targetPosition;
            }
            set {
                if (!locked) {
                    targetPosition = value;
                }
            }
        }
        // public CardController cardController;
        public CardViewController CardViewController;
        public BoardInputController inputController;

        public bool locked;
        public bool inHand;

        // Start is called before the first frame update

        void Start() {
            // cardController = FindObjectOfType<CardController>();
            CardViewController = FindObjectOfType<CardViewController>();
            inputController = FindObjectOfType<BoardInputController>();
            //targetScale = new Vector3(4, 5.5f, 1) * .5f;
            targetPosition = transform.position;
            inHand = false;
        }

        public void InitializeFromEntity(VEntity entity) {
            cardEntityId = entity.id;
            cost = entity.GetVComponent<ManaCostComponent>().cost;
            effect = entity.GetVComponent<EffectTextComponent>().effect;
            cardType = entity.GetVComponent<CardClassComponent>().cardType;

            nameText.text = entity.GetVComponent<CardNameComponent>().name;
            costText.text = cost.ToString();
            effectText.text = effect;
            classText.text = StringUtils.ClassEnumToString(cardType);

            cardBacks.Add(CardType.NEUTRAL, neutralQuad);
            cardBacks.Add(CardType.ORION, orionQuad);
            cardBacks.Add(CardType.OPHELIA, opheliaQuad);
            foreach (CardType t in cardBacks.Keys) {
                cardBacks[t].SetActive(false);
            }
            cardBacks[cardType].SetActive(true);
        }

        void OnMouseEnter() {
            Debug.Log("!");
            if (inHand || cantrip) { //HACK

                inputController.SetCardHovering(this);
            }
        }

        private void OnMouseExit() {
            if (inHand || cantrip) { //HACK
                if (inputController.cardHovering == this) {
                    inputController.SetCardHovering(null);
                }
            }
        }

        private void Update() {
            if (transform.position != targetPosition) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Vector3.Distance(transform.position, targetPosition) / 10);
            }
            if (transform.localScale != targetScale && targetScale != Vector3.zero) {
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Vector3.Distance(transform.localScale, targetScale) / 10);
            }
            /*
            if(Vector3.Magnitude(transform.localEulerAngles - targetAngle) > 1f) {
                transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, targetAngle, Vector3.Distance(transform.localEulerAngles, targetAngle) / 10);
            } else {
                transform.localEulerAngles = targetAngle;
            }
            */
        }

    }

}