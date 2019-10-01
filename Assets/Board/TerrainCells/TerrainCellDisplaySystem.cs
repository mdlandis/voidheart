using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Voidheart {
    [Serializable]
    public class TerrainCellDisplayAppearEvent : VComponent {
        public Coord coord;
        public string terrainCellEntityId;
    }

    /// <summary>Creates a terrain cell.</summary>
    [Serializable]
    public class TerrainCellDisplaySystem : VAnimationSystem {
        [SerializeField]
        private VCellDisplay cellDisplayPrefab;

        public override bool ShouldOperate(VEntity entity) {
            return VEntityComponentSystemManager.HasVComponent<TerrainCellDisplayAppearEvent>(entity);
        }

        public override bool IsImmediate() {
            return true;
        }

        public override void DoImmediateAnimation(VEntity entity) {
            TerrainCellDisplayAppearEvent appearEvent = VEntityComponentSystemManager.GetVComponent<TerrainCellDisplayAppearEvent>(entity);
            PositionComponent positionComponent = ecsManager.GetVComponent<PositionComponent>(appearEvent.terrainCellEntityId);
            TerrainCellDisplayComponent displayComponent = ecsManager.GetVComponent<TerrainCellDisplayComponent>(appearEvent.terrainCellEntityId);
            displayComponent.cellDisplay.SetCellId(appearEvent.terrainCellEntityId);
            displayComponent.cellDisplay.SetCellType(displayComponent.cellType);
            displayComponent.cellDisplay.SetState(VCellSelectedState.NORMAL);
            displayComponent.cellDisplay.ResizeSprite(ecsManager.GetSystem<PositionWorldConversionSystem>().SquareDimensions);
            displayComponent.cellDisplay.transform.position = ecsManager.GetSystem<PositionWorldConversionSystem>().GetTransformFromCoord(positionComponent.position);
            displayComponent.cellDisplay.gameObject.SetActive(true);

            displayComponent.cellDisplay.CellHovered += this.OnCellHover;
        }

        public void OnCellHover(string id) {
            BoardInputController.Instance.SetBoardSpaceHovering(ecsManager.GetVEntityById(id).GetVComponent<PositionComponent>().position);
        }

        public void OnCellUnhover(string id) {
            Coord position = ecsManager.GetVEntityById(id).GetVComponent<PositionComponent>().position;
            if (BoardInputController.Instance.boardSpaceHovering == position) {
                BoardInputController.Instance.SetBoardSpaceHovering(Coord.nullCoord);
            }

        }
    }
}