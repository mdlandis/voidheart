using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Voidheart {
    public class HighlightObject : MonoBehaviour, IBoardMarker {
        public SpriteRenderer sprite;

        public void SetPosition(Vector3 pos) {
            gameObject.transform.position = pos;
        }

        public void SetEnable(bool value) {
            sprite.enabled = value;
        }

        public void DestroyObject() {
            Destroy(gameObject);
        }

        public void SetColor(Color c) {
            sprite.color = c;
        }

        public void SetRenderPriority(int i) {
            sprite.sortingOrder = i;
        }
    }
}