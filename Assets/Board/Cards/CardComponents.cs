using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public enum CardType {
        NEUTRAL,
        OPHELIA,
        ORION
    }

    public enum CardEffectType {
        ATTACK,
        MOVE,
        BOTH
    }

    public enum CardRarity {
        TOKEN,
        BASIC,
        COMMON,
        RARE,
        LEGENDARY
    }

    [Serializable]
    public class ManaCostComponent : VComponent {
        public int cost;
    }

    [Serializable]
    public class EffectTextComponent : VComponent {
        public string effect;
    }

    [Serializable]
    public class CardClassComponent : VComponent {
        public CardType cardType;
    }

    [Serializable]
    public class CardNameComponent : VComponent {
        public string name;
    }

    [Serializable]
    public class CardRarityComponent : VComponent {
        public CardRarity cardRarity;
    }

    [Serializable]
    public class CardDisplayComponent : VComponent, ICloneable {
        public CardGameObject cardDisplay;

        public object Clone() {
            return new CardDisplayComponent {
                cardDisplay = this.cardDisplay
            };
        }
    }

    [Serializable]
    public class TargetingComponent : VComponent {
        public List<TargetingMethod> targetingMethods = new List<TargetingMethod>();
        public List<ValidTargetingMethod> validTargetingMethods = new List<ValidTargetingMethod>();
        public List<GroupTargetingMethod> groupTargetingMethods = new List<GroupTargetingMethod>();
    }

    [Serializable]
    public class GameplayEffectComponent : VComponent {
        public List<CardEffectComponent> effectEvents = new List<CardEffectComponent>();
    }

    [Serializable]
    public class CardPlayEvent : VComponent {
        public string cardId;
        public Coord targetSpace;
        public string casterId;
        public Dictionary<CardEffectType, List<Coord>> groupTargetingSpaces;
    }
}