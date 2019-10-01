using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public interface IBoardMarker {
        void SetPosition(Vector3 pos);

        void SetEnable(bool value);

        void DestroyObject();

        void SetRenderPriority(int i);
    }

    [Serializable]
    public class HighlightDisplaySystem : VAnimationSystem {
        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<EntityAppearEvent>(entity);
        }

        public override bool IsImmediate() {
            return true;
        }

        public Color lightGreenColor;
        public Color darkGreenColor;
        public Color redColor;
        public Color greyColor;
        public Color orangeColor;

        [NonSerialized]
        private TagDictionary<IBoardMarker> taggedBoardMarkers;
        [NonSerialized]
        private Dictionary<Coord, HashSet<IBoardMarker>> coordToMarkerSets;

        public HighlightObject highlightObjectPrefab;
        // public OutlineObject outlinePrefab;

        public override void Init(VEntityComponentSystemManager newEcsManager) {
            base.Init(newEcsManager);
            taggedBoardMarkers = new TagDictionary<IBoardMarker>();
            coordToMarkerSets = new Dictionary<Coord, HashSet<IBoardMarker>>();
        }

        private HashSet<IBoardMarker> GetMarkerSet(Coord c) {
            if (!coordToMarkerSets.ContainsKey(c)) {
                coordToMarkerSets.Add(c, new HashSet<IBoardMarker> { });
            }

            return coordToMarkerSets[c];
        }

        public void CreateGreyHighlightWithTags(IEnumerable<Coord> coords, IEnumerable<string> tags) {
            var highlights = CreateHighlightsWithTags(coords, tags, greyColor);
            foreach (HighlightObject h in highlights) {
                h.SetRenderPriority(100);
            }
        }

        private IEnumerable<IBoardMarker> GetSquares(IEnumerable<Coord> coords, IEnumerable<string> tags, bool matchAll = false) {
            HashSet<IBoardMarker> coordsObjects = new HashSet<IBoardMarker>();
            if (coords != null) {
                foreach (Coord c in coords) {
                    coordsObjects.UnionWith(GetMarkerSet(c));
                }
            }

            IEnumerable<IBoardMarker> taggedObjects = new HashSet<IBoardMarker>();
            if (tags != null) {
                taggedObjects = matchAll ? taggedBoardMarkers.GetValuesWithAllTags(tags) : taggedBoardMarkers.GetValuesWithAnyTags(tags);
            }

            if (coords == null) {
                return taggedObjects;
            } else if (tags == null) {
                return coordsObjects;
            } else {
                coordsObjects.IntersectWith(taggedObjects);
                return coordsObjects;
            }
        }

        public IEnumerable<HighlightObject> CreateHighlightsWithTags(IEnumerable<Coord> coords, IEnumerable<string> tags, Color color) {
            HashSet<HighlightObject> objects = new HashSet<HighlightObject>();
            foreach (Coord coord in coords) {
                HighlightObject h = HighlightObject.Instantiate(highlightObjectPrefab);
                objects.Add(h);
                h.SetPosition(ecsManager.GetSystem<PositionWorldConversionSystem>().GetTransformFromCoord(coord));
                h.GetComponent<SpriteRenderer>().color = color;
                taggedBoardMarkers.Add(tags, h);
                GetMarkerSet(coord).Add(h);
            }

            return objects;
        }

        public void SetEnable(IEnumerable<Coord> coords, IEnumerable<string> tags, bool value) {
            foreach (IBoardMarker boardMarker in GetSquares(coords, tags)) {
                boardMarker.SetEnable(value);
            }
        }

        public void Remove(IEnumerable<Coord> coords, IEnumerable<string> tags, bool matchAll = false) {
            IEnumerable<IBoardMarker> objectsToRemove = GetSquares(coords, tags, matchAll);
            taggedBoardMarkers.RemoveObjects(objectsToRemove);
            foreach (IBoardMarker boardMarker in objectsToRemove) {
                boardMarker.DestroyObject();
            }
        }
    }
}