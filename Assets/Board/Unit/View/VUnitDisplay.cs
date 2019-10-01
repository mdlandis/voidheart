using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Voidheart {
    public class VUnitDisplay : MonoBehaviour {

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        /// <summary>
        /// UnitClicked event is invoked when user clicks the unit. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitClicked;
        /// <summary>
        /// UnitHighlighted event is invoked when user moves cursor over the unit. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitHighlighted;
        /// <summary>
        /// UnitDehighlighted event is invoked when cursor exits unit's collider. 
        /// It requires a collider on the unit game object to work.
        /// </summary>
        public event EventHandler UnitDehighlighted;

        /// <summary>
        /// Indicates if movement animation is playing.
        /// </summary>
        public bool isMoving {
            get;
            set;
        }

        protected virtual void OnMouseDown() {
            if (UnitClicked != null)
                UnitClicked.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseEnter() {
            if (UnitHighlighted != null)
                UnitHighlighted.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseExit() {
            if (UnitDehighlighted != null)
                UnitDehighlighted.Invoke(this, new EventArgs());
        }

        public void BindEntity(VEntity entity) {
            SetSprite(VEntityComponentSystemManager.GetVComponent<UnitDisplayComponent>(entity).displaySprite);
        }

        private void SetSprite(Sprite s) {
            spriteRenderer.sprite = s;
        }

        public void ResizeSprite(Vector3 targetSize) {
            spriteRenderer.ResizeToSize(targetSize);
            spriteRenderer.transform.localScale *= .8f;
        }

        public SpriteRenderer GetSpriteRenderer() {
            return spriteRenderer;
        }

    }
}