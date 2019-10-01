using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public enum InputState {
        SELECTCARD,
        SELECTSPACE,
        CONFLICTRESOLUTION
    }

    public class BoardInputController : SerializedSingleton<BoardInputController> {

        public Coord boardSpaceHovering {
            get;
            private set;
        }

        public void SetBoardSpaceHovering(Coord c) {
            boardSpaceHovering = c;
            SendEventToBoard();
        }

        public CardGameObject cardHovering {
            get;
            private set;
        }
        public void SetCardHovering(CardGameObject c) {
            cardHovering = c;
            if (cardController.MyHandState != HandState.SELECTED) {
                if (cardHovering != null && cardController.IsPlayable(cardHovering.cardEntityId)) {
                    cardController.MyHandState = HandState.HOVER;
                } else {
                    cardController.MyHandState = HandState.IDLE;
                }
            }

            SendEventToBoard();
        }

        public CardGameObject cardSelected;

        Coord spaceSelected;

        public InputState state;
        public InputState State {
            get {
                return state;
            }
            set {
                state = value;
                SendEventToBoard();
            }
        }

        public VEntityComponentSystemManager boardState;

        public CardViewController cardController;
        public CardMover cardMover;

        private void Start() {
            cardHovering = null;
            boardSpaceHovering = Coord.nullCoord;
            cardController = FindObjectOfType<CardViewController>();
            boardState = VEntityComponentSystemManager.Instance;
            cardMover = FindObjectOfType<CardMover>();
            SendEventToBoard();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                HandleLeftClick();
            }
            if (Input.GetMouseButtonDown(1)) {
                HandleRightClick();
            }
        }

        void SendEventToBoard() {
            boardState.GetAnimationSystem<PlayCardSystem>().UpdateBoardTargetingState(cardHovering != null ? cardHovering.cardEntityId : "", cardSelected != null ? cardSelected.cardEntityId : "", boardSpaceHovering, spaceSelected, state);
        }

        void HandleLeftClick() {
            if (State == InputState.SELECTCARD) {
                if (cardHovering != null) {
                    //cardController.AttemptPlayCard(cardHovering);
                    if (cardController.IsPlayable(cardHovering.cardEntityId)) {
                        cardSelected = cardHovering;
                        cardMover.ClickCard(cardSelected);
                        State = InputState.SELECTSPACE;
                        cardController.MyHandState = HandState.SELECTED;
                    }
                }
            } else if (State == InputState.SELECTSPACE) {
                IEnumerable<VEntity> playableUnitIds = boardState.GetSystem<UnitFinderSystem>().GetAllPlayerUnits();
                if (boardSpaceHovering != Coord.nullCoord) {
                    Debug.Log("Clicking space");
                    int belongs = 0;
                    IList<VEntity> belongingList = new List<VEntity>();
                    foreach (VEntity v in playableUnitIds) {
                        if (boardState.GetAnimationSystem<PlayCardSystem>().IsSpaceInRange(boardSpaceHovering, v.id)) {
                            belongs++;
                            belongingList.Add(v);
                        }
                    }
                    Debug.Log(belongs);
                    if (belongs == 0) {
                        //invalid space
                    } else {
                        spaceSelected = boardSpaceHovering;
                        if (belongs > 1) {
                            State = InputState.CONFLICTRESOLUTION;
                        } else if (belongs == 1) {
                            FinishPlayingCard(cardSelected, spaceSelected, belongingList[0]);
                        }
                    }
                }
            } else if (State == InputState.CONFLICTRESOLUTION) {
                if (boardSpaceHovering != Coord.nullCoord) {
                    VEntity selectedChar = boardState.GetSystem<UnitFinderSystem>().GetUnitAtCoord(boardSpaceHovering);
                    if (selectedChar != null) {
                        CharacterClassComponent charClass = selectedChar.GetVComponent<CharacterClassComponent>();
                        if (charClass != null) {
                            FinishPlayingCard(cardSelected, spaceSelected, selectedChar);
                        }
                    }
                }
            }
        }

        void FinishPlayingCard(CardGameObject card, Coord space, VEntity caster) {
            Debug.Log("Finish playing card");
            cardSelected.locked = false;
            cardSelected = null;

            VEntity cardPlayed = boardState.GetVEntityById(card.cardEntityId);
            TargetingComponent targeting = VEntityComponentSystemManager.GetVComponent<TargetingComponent>(cardPlayed);
            Dictionary<CardEffectType, List<Coord>> mapping = new Dictionary<CardEffectType, List<Coord>>();
            foreach (GroupTargetingMethod m in targeting.groupTargetingMethods) {
                if (!mapping.ContainsKey(m.effectType)) {
                    mapping[m.effectType] = new List<Coord>();
                }
                mapping[m.effectType].AddRange(m.SelectGroupCoords(space, caster.GetVComponent<PositionComponent>().position, boardState));
            }
            foreach (CardEffectType t in mapping.Keys) {
                Debug.Log("Type Key: " + t);
            }

            boardState.CreateEvent("playCard", component : new CardPlayEvent {
                cardId = card.cardEntityId,
                    targetSpace = space,
                    casterId = caster.id,
                    groupTargetingSpaces = mapping
            });

            if(card.cantrip)
            {
                boardState.CreateEvent("cantripUsed", component: new CantripUseEvent
                {
                });
            }

            spaceSelected = Coord.nullCoord;
            State = InputState.SELECTCARD;
            cardController.MyHandState = HandState.IDLE;
        }

        void HandleRightClick() {
            if (State == InputState.SELECTCARD) {

            } else if (State == InputState.SELECTSPACE) {
                cardMover.ResetCard(cardSelected);
                cardSelected = null;
                cardController.MyHandState = HandState.IDLE;
                State = InputState.SELECTCARD;
            } else if (State == InputState.CONFLICTRESOLUTION) {
                spaceSelected = Coord.nullCoord;
                State = InputState.SELECTSPACE;
            }
        }
    }
}