namespace Voidheart {
#if UNITY_EDITOR
    using Sirenix.OdinInspector.Editor.Drawers;
    using Sirenix.Utilities.Editor;
#endif
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using Sirenix.OdinInspector;
    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    public struct CellContent {
        public VCellStruct cellStruct;
        public EntityScriptableObject unitData; // nullable
        public string note;
    }

    [CreateAssetMenu]
    public class Level : SerializedScriptableObject {
        [FoldoutGroup("ResetTable")]
        [SerializeField]
        private int height = 3;
        [FoldoutGroup("ResetTable")]
        [SerializeField]
        private int width = 2;

        [FoldoutGroup("ResetTable")]
        [PropertyOrder(0)]
        [Button("Reset Table")]
        private void ClearTable() {
            levelData = new CellContent[height, width];
        }

#if UNITY_EDITOR
        [TabGroup("Level Layout")]
        [PropertyOrder(11)]
        [Title("Cell Type to paint")]
        [EnumToggleButtons]
        [ShowInInspector]
        public VCellType cellTypePaint {
            get {
                return _cellTypePaint;
            }
            set {
                _cellTypePaint = value;
            }
        }
#endif

        [HideInInspector]
        public CellContent[, ] levelData;

        [TabGroup("Level Layout")]
        [PropertyOrder(12)]
        [ShowInInspector]
        [TableMatrix(HorizontalTitle = "Level Terrain", SquareCells = true, DrawElementMethod = "DrawCellType", RowHeight = 16)]
        public CellContent[, ] _levelLayoutLevelData {
            get {
                return levelData;
            }
            set {
                levelData = value;
            }
        }

        [TabGroup("Unit Placement")]
        [PropertyOrder(20)]
        [ShowInInspector]
        private int turns;

        [TabGroup("Unit Placement")]
        [PropertyOrder(21)]
        [ShowInInspector]
        public CellContent[, ] unitPlacementLevelData {
            get {
                return levelData;
            }
            set {
                levelData = value;
            }
        }

#if UNITY_EDITOR
        public static VCellType _cellTypePaint;

        private static CellContent DrawCellType(Rect rect, CellContent value) {
            // if clicked
            if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) &&
                rect.Contains(Event.current.mousePosition) && value.cellStruct.cellType != _cellTypePaint) {
                value.cellStruct.cellType = _cellTypePaint;
                GUI.changed = true;
                Event.current.Use();
            }

            UnityEditor.EditorGUI.DrawRect(rect.Padding(1), CellTypeToColor(value.cellStruct.cellType));

            return value;
        }

        public static Color CellTypeToColor(VCellType cellType) {
            Color toDraw;
            switch (cellType) {
                case VCellType.NORMAL:
                    toDraw = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    break;
                case VCellType.WALL:
                    toDraw = new Color(1.0f, 0.0f, 0.0f);
                    break;
                case VCellType.VOID:
                    toDraw = new Color(0.0f, 0.0f, 0.0f, 0.5f);
                    break;
                default:
                    toDraw = new Color(0.0f, 0.0f, 0.0f, 0.5f);
                    break;
            }
            return toDraw;
        }
#endif
    }
#if UNITY_EDITOR
    internal sealed class UnitDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, CellContent>
        where TArray : IList {
            protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings() {
                return new TableMatrixAttribute() {
                    SquareCells = true,
                };
            }

            protected override CellContent DrawElement(Rect rect, CellContent value) {
                EditorGUI.DrawRect(rect, Level.CellTypeToColor(value.cellStruct.cellType));

                if (value.unitData != null) {
                    // Item count
                    var countRect = rect.Padding(2).AlignBottom(16);
                    EditorGUI.DrawRect(countRect, new Color(0.0f, 0.0f, 0.0f, 0.5f));
                    GUI.Label(countRect, VEntityComponentSystemManager.GetVComponent<NameComponent>(value.unitData.entity).name, SirenixGUIStyles.CenteredGreyMiniLabel);
                }

                Texture texture = null;
                if (value.unitData != null && VEntityComponentSystemManager.HasVComponent<UnitDisplayComponent>(value.unitData.entity)) {
                    texture = GUIHelper.GetAssetThumbnail(VEntityComponentSystemManager.GetVComponent<UnitDisplayComponent>(value.unitData.entity).displaySprite, typeof(EntityScriptableObject), true);
                }

                value.unitData = (EntityScriptableObject) SirenixEditorFields.UnityPreviewObjectField(rect.Padding(16), value.unitData, texture, typeof(EntityScriptableObject));
                return value;
            }

            protected override void DrawPropertyLayout(GUIContent label) {
                base.DrawPropertyLayout(label);

                // Draws a drop-zone where we can destroy items.
                var rect = GUILayoutUtility.GetRect(0, 40).Padding(2);
                var id = DragAndDropUtilities.GetDragAndDropId(rect);
                DragAndDropUtilities.DrawDropZone(rect, null as UnityEngine.Object, null, id);
                DragAndDropUtilities.DropZone<EntityScriptableObject>(rect, null, false, id);
            }
        }
#endif
}