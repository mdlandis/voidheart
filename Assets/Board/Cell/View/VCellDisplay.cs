using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public class VCellDisplay : MonoBehaviour {
        [SerializeField]
        private Renderer cellRenderer = null;

        [SerializeField]
        private SpriteRenderer spriteRenderer = null;

        public string representedCellId {
            get;
            private set;
        }

        public void SetCellId(string id) {
            representedCellId = id;
        }
        /// <summary>
        /// CellClicked event is invoked when user clicks on the cell. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event Action<string> CellClicked;
        /// <summary>
        /// CellHighlighed event is invoked when cursor enters the cell's collider. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event Action<string> CellHovered;
        /// <summary>
        /// CellDehighlighted event is invoked when cursor exits the cell's collider. 
        /// It requires a collider on the cell game object to work.
        /// </summary>
        public event Action<string> CellUnhovered;

        protected virtual void OnMouseEnter() {
            if (CellHovered != null)
                CellHovered.Invoke(representedCellId);
        }

        protected virtual void OnMouseExit() {
            if (CellUnhovered != null)
                CellUnhovered.Invoke(representedCellId);
        }
        void OnMouseDown() {
            if (CellClicked != null)
                CellClicked.Invoke(representedCellId);
        }

        /// <summary>
        /// Method returns physical cell's dimensions.
        /// It is necessary necessary for grid generators   
        /// </summary>
        public Vector3 GetCellDimensions() {
            return cellRenderer.bounds.size;
        }

        public void ResizeSprite(Vector3 targetSize) {
            spriteRenderer.ResizeToSize(targetSize);
        }

        public void SetCellType(VCellType cellType) {
            if (cellType == VCellType.VOID) {
                this.gameObject.SetActive(false);
            }
        }

        public void SetState(VCellSelectedState state) {
            switch (state) {
                case VCellSelectedState.NORMAL:
                    cellRenderer.material.color = new Color(0.75f, 0.75f, 0.75f);
                    break;
                case VCellSelectedState.GREEN:
                    cellRenderer.material.color = Color.green;
                    break;
                case VCellSelectedState.YELLOW:
                    cellRenderer.material.color = Color.green;
                    break;
                case VCellSelectedState.RED:
                    cellRenderer.material.color = Color.green;
                    break;
                default:
                    break;
            }
        }

        private void SetColor(Color color) {
            var highlighter = transform.Find("Highlighter");
            var spriteRenderer = highlighter.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) {
                spriteRenderer.color = color;
            }
        }
    }

    public enum VCellSelectedState {
        NORMAL,
        RED,
        YELLOW,
        GREEN
    }

    public interface IGraphNode {
        /// <summary>
        /// Method returns distance to a IGraphNode that is given as parameter. 
        /// </summary>
        int GetDistance(IGraphNode other);
    }

    public struct VCellStruct {
        /// <summary>
        /// Cost of moving through the cell.
        /// </summary>
        public int MovementCost;
        public VCellType cellType;
    }

    public enum VCellType {
        NORMAL,
        WALL,
        VOID
    }
}