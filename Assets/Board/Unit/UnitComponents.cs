using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class HealthComponent : VComponent {
        public int currHealth;
        public int maxHealth;
    }

    public enum Team {
        ALLY,
        ENEMY,
        NEUTRAL
    }

    [Serializable]
    public class IsUnitComponent : VComponent {

    }

    [Serializable]
    public class MovementBlockComponent : VComponent {

    }

    [Serializable]
    public class TeamComponent : VComponent {
        public Team team;
    }

    [Serializable]
    public class NameComponent : VComponent {
        public string name;
    }

    [Serializable]
    public class CharacterClassComponent : VComponent {
        public CardType cardType;
    }

    [Serializable]
    public class EnemyPathComponenet : VComponent {
        public Dictionary<Coord, List<Coord>> cachedPaths = null;
    }

    [Serializable]
    public class NPCAIComponent : VComponent {

    }

    [Serializable]
    public class UnitDisplayComponent : VComponent, ICloneable, IMovableDisplayComponent {
        [NonSerialized]
        public VUnitDisplay unitDisplayGameObject;
        public Sprite displaySprite;
        public Vector3 spriteSize;

        public object Clone() {
            return new UnitDisplayComponent {
                unitDisplayGameObject = unitDisplayGameObject,
                    displaySprite = displaySprite,
                    spriteSize = spriteSize
            };
        }

        public Transform getTransform() {
            return unitDisplayGameObject.transform;
        }
    }
}