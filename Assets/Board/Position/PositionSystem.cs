using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class PositionComponent : VComponent {
        public Coord position;
    }

    [Serializable]
    public class PositionDisplayComponent : VComponent {
        [System.NonSerialized]
        public Transform mainTransform;
    }

    /// <summary>Keeps track of what is in all positions.</summary>
    [Serializable]
    public class PositionTrackSystem : VSystem {
        // caches what each coordinate contains
        Dictionary<Coord, HashSet<string>> positionCache;
        // tracks where each entity currently is
        Dictionary<string, Coord> positionTracker;

        private void Insert(Coord c, string id) {
            if (positionCache == null) {
                positionCache = new Dictionary<Coord, HashSet<string>>();
                positionTracker = new Dictionary<string, Coord>();
            }

            if (positionCache.ContainsKey(c)) {
                positionCache[c].Add(id);
            } else {
                positionCache[c] = new HashSet<string> {
                    id
                };
            }
            positionTracker[id] = c;
        }

        private void Remove(string id) {
            positionCache[positionTracker[id]].Remove(id);
            positionTracker.Remove(id);
        }

        public void Update(Coord c, string id) {
            Remove(id);
            Insert(c, id);
        }

        // TODO: update position after move event
        public override bool ShouldOperate(VEntity entity) {
            return false;
        }

        public IEnumerable<VEntity> GetAtCoord(Coord c, Predicate<VEntity> conditional = null) {
            HashSet<VEntity> entities = new HashSet<VEntity>();
            if (positionCache.ContainsKey(c)) {
                foreach (string id in positionCache[c]) {
                    VEntity e = ecsManager.GetVEntityById(id);
                    if (conditional == null || conditional(e)) {
                        entities.Add(e);
                    }
                }
            }

            return entities;
        }

        // TODO: update position after move event
        protected override void OnAfterEvent(VEntity entity) { }

        public void Track(VEntity e) {
            var positionComponent = VEntityComponentSystemManager.GetVComponent<PositionComponent>(e);
            var positionDisplay = VEntityComponentSystemManager.GetVComponent<PositionDisplayComponent>(e);
            if (positionDisplay != null) {
                positionDisplay.mainTransform = new GameObject("mainTransform").transform;
                positionDisplay.mainTransform.position = ecsManager.GetSystem<PositionWorldConversionSystem>().GetTransformFromCoord(positionComponent.position);
            }
            Insert(positionComponent.position, e.id);
        }

        public void Untrack(string entityId) {
            Remove(entityId);
        }
    }

    public class PositionWorldConversionSystem : VSystem {
        public Transform worldCenter;
        public Coord centerCoord = new Coord {
            x = 0, y = 0
        };
        public Vector2 SquareDimensions = new Vector2(1.0f, 1.0f);

        public Vector3 GetTransformFromCoord(Coord c) {
            c -= centerCoord;
            return worldCenter.position + new Vector3(c.x * SquareDimensions.x, c.y * SquareDimensions.y);
        }

        public override bool ShouldOperate(VEntity entity) {
            return false;
        }
    }
}