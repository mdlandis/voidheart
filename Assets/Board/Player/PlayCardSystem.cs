using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [System.Serializable]
    public struct TargetingState {
        public string cardHoveringId;
        public string cardSelectedId;
        public Coord boardSpaceHovering;
        public Coord spaceSelected;
        public InputState state;
    }

    [System.Serializable]
    public class PlayCardSystem : VAnimationSystem {
        public TargetingState prevTargetingState;
        public TargetingState currTargetingState;

        /*   
        Targeting spaces are the list of ALL possible spaces that a card could conceivably target, for each unit.
        */
        private Dictionary<string, List<Coord>> targetingSpaces;

        /*   
        Group targeting spaces are the list of coordinates that, given a caster and a targeted coordinate,
        will be affected.
         */
        public Dictionary<string, List<Coord>> groupTargetingSpaces;

        public override void Init(VEntityComponentSystemManager newEcsManager) {
            base.Init(newEcsManager);
            targetingSpaces = new Dictionary<string, List<Coord>>();
            groupTargetingSpaces = new Dictionary<string, List<Coord>>();
        }

        public PlayCardSystem() : base() {
            prevTargetingState = new TargetingState {
                cardHoveringId = "",
                cardSelectedId = "",
                boardSpaceHovering = Coord.nullCoord,
                spaceSelected = Coord.nullCoord,
                state = InputState.SELECTCARD
            };
            currTargetingState = prevTargetingState;
        }

        public void PlayCard(string cardId) {

        }

        public bool IsSpaceInRange(Coord coord, string unitId) {
            return targetingSpaces[unitId].Contains(coord);
        }

        public void UpdateBoardTargetingState(string cardHoveringId,
            string cardSelectedId,
            Coord spaceHovering,
            Coord spaceSelected,
            InputState inputState) {
            currTargetingState = new TargetingState {
                cardHoveringId = cardHoveringId,
                cardSelectedId = cardSelectedId,
                boardSpaceHovering = spaceHovering,
                spaceSelected = spaceSelected,
                state = inputState
            };

            if (currTargetingState.state == InputState.SELECTCARD) {
                UpdateBoardForSelectCardState();
            } else if (currTargetingState.state == InputState.SELECTSPACE) {
                UpdateBoardForSelectSpaceState();
            } else if (currTargetingState.state == InputState.CONFLICTRESOLUTION) {
                UpdateBoardForConflictResolutionState();
            }

            prevTargetingState = currTargetingState;
        }

        private void UpdateBoardForSelectCardState() {
            if (currTargetingState.cardHoveringId != prevTargetingState.cardHoveringId || currTargetingState.state != prevTargetingState.state) {
                // ClearCellMarks(VCellSelectedState.GREEN);
                //Un-outline all cells.
                //Remove all ValidSpace marks.
                //Remove all GroupTarget marks.
                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().Remove(null, new string[] {
                    "Outline",
                    "ValidSpace",
                    "GroupTarget"
                });
                IEnumerable<VEntity> characters = ecsManager.GetSystem<UnitFinderSystem>().GetAllPlayerUnits();

                foreach (VEntity c in characters) {
                    GetList(targetingSpaces, c.id).Clear();
                }
                if (currTargetingState.cardHoveringId != "") {
                    VEntity card = ecsManager.GetVEntityById(currTargetingState.cardHoveringId);
                    CardClassComponent cardClass = card.GetVComponent<CardClassComponent>();
                    TargetingComponent cardTargeter = card.GetVComponent<TargetingComponent>();
                    foreach (VEntity character in characters) {
                        CharacterClassComponent characterClass = character.GetVComponent<CharacterClassComponent>();
                        PositionComponent characterPosition = character.GetVComponent<PositionComponent>();
                        if (cardClass.cardType == characterClass.cardType || cardClass.cardType == CardType.NEUTRAL) {
                            foreach (TargetingMethod targetingMethod in cardTargeter.targetingMethods) {
                                List<Coord> newCoords = targetingMethod.SelectCoords(characterPosition.position, ecsManager);
                                foreach (Coord coord in newCoords) {
                                    ProcessCoord(character, coord, cardTargeter);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateBoardForSelectSpaceState() {

            if (currTargetingState.boardSpaceHovering != prevTargetingState.boardSpaceHovering || currTargetingState.state != prevTargetingState.state) {
                IEnumerable<VEntity> characters = ecsManager.GetSystem<UnitFinderSystem>().GetAllPlayerUnits();
                foreach (VEntity c in characters) {
                    GetList(groupTargetingSpaces, c.id).Clear();
                }

                //Re-enable all ValidSpace marks.
                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().SetEnable(null, new string[] {
                    "ValidSpace"
                }, true);
                //Remove all GroupTarget marks.
                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().Remove(null, new string[] {
                    "GroupTarget"
                });

                if (currTargetingState.boardSpaceHovering != Coord.nullCoord) {
                    foreach (VEntity character in characters) {
                        PositionComponent characterPosition = character.GetVComponent<PositionComponent>();
                        if (GetList(targetingSpaces, character.id).Contains(currTargetingState.boardSpaceHovering)) {
                            VEntity card = ecsManager.GetVEntityById(currTargetingState.cardSelectedId);
                            foreach (GroupTargetingMethod groupTargeting in card.GetVComponent<TargetingComponent>().groupTargetingMethods) {
                                List<Coord> newCoords = groupTargeting.SelectGroupCoords(currTargetingState.boardSpaceHovering, characterPosition.position, ecsManager);
                                Debug.Log("Group Targeting Method Coords: " + newCoords.Count);
                                GetList(groupTargetingSpaces, character.id).AddRange(newCoords);
                                //Highlight c. Tag it with GroupTarget, color based on the effectType
                                //Disable ValidSpace mark, if it has one.
                                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().CreateHighlightsWithTags(newCoords, new string[] {
                                    "GroupTarget"
                                }, ColorFromEffectType(groupTargeting.effectType));
                                //Debug.Log(ColorFromEffectType(groupTargeting.effectType));
                                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().SetEnable(GetList(groupTargetingSpaces, character.id), new string[] {
                                    "ValidSpace"
                                }, false);
                            }
                        }
                    }

                }
            }
        }

        void UpdateBoardForConflictResolutionState() {
            if (currTargetingState.boardSpaceHovering != prevTargetingState.boardSpaceHovering || currTargetingState.state != prevTargetingState.state) {
                IEnumerable<VEntity> characters = ecsManager.GetSystem<UnitFinderSystem>().GetAllPlayerUnits();
                //Disable all ValidSpace marks.
                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().SetEnable(null, new string[] {
                    "ValidSpace"
                }, false);
                //Re-enable all GroupTarget marks.
                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().SetEnable(null, new string[] {
                    "GroupTarget"
                }, true);

                if (currTargetingState.boardSpaceHovering != Coord.nullCoord) {
                    foreach (VEntity character in characters) {
                        PositionComponent characterPosition = character.GetVComponent<PositionComponent>();
                        bool matches = (characterPosition.position == currTargetingState.boardSpaceHovering);
                        if (matches) {
                            ecsManager.GetAnimationSystem<HighlightDisplaySystem>().SetEnable(null, new string[] {
                                "GroupTarget"
                            }, false);
                            ecsManager.GetAnimationSystem<HighlightDisplaySystem>().SetEnable(GetList(groupTargetingSpaces, character.id), new string[] {
                                "GroupTarget"
                            }, true);
                        }
                    }
                }
            }
        }

        private Color ColorFromEffectType(CardEffectType effectType) {
            if (effectType == CardEffectType.ATTACK) {
                return ecsManager.GetAnimationSystem<HighlightDisplaySystem>().orangeColor;
            } else if (effectType == CardEffectType.MOVE) {
                return ecsManager.GetAnimationSystem<HighlightDisplaySystem>().darkGreenColor;
            }

            return Color.grey;
        }

        private List<Coord> GetList(Dictionary<string, List<Coord>> dict, string key) {
            if (!dict.ContainsKey(key)) {
                dict.Add(key, new List<Coord>());
            }
            return dict[key];
        }

        private void ProcessCoord(VEntity character, Coord coord, TargetingComponent cardTargeter) {
            //Outline cell c (by toggling each edge).
            ecsManager.GetAnimationSystem<HighlightDisplaySystem>().CreateGreyHighlightWithTags(new Coord[] {
                coord
            }, new string[] {
                "Outline"
            });
            bool passes = true;
            foreach (ValidTargetingMethod validTargetingMethod in cardTargeter.validTargetingMethods) {
                if (!validTargetingMethod.ValidateCoord(coord, ecsManager)) {
                    passes = false;
                }
            }
            if (passes) {
                GetList(targetingSpaces, character.id).Add(coord);
                //Highlight the cell according to validTargetingMethod.effectType. Tag it with ValidSpace
                ecsManager.GetAnimationSystem<HighlightDisplaySystem>().CreateHighlightsWithTags(new List<Coord> {
                    coord
                }, new List<string> {
                    "ValidSpace"
                }, ecsManager.GetAnimationSystem<HighlightDisplaySystem>().lightGreenColor);
            }
        }

        public override bool IsImmediate() {
            return true;
        }

        public override bool ShouldOperate(VEntity entity) {
            return false;
        }
    }
}