namespace unvs.ui {
    using UnityEngine;
    using unvs.components;
    using unvs.ext;
    using UnityEngine.UI;
    using unvs.types;

    public class UnvsDockPanel : MonoBehaviour
    {
        public DockTYpe dockType;
        private void OnValidate()
        {
            doAlign();
        }

        private void doAlign()
        {
            var panel = GetComponent<Image>();
            if (panel == null) return;
            if (dockType == DockTYpe.Top)
            {
                panel.AnchorDockTop();
            }
            if (dockType == DockTYpe.Bottom)
            {
                panel.AnchorDockBottom();
            }
            if (dockType == DockTYpe.Left)
            {
                panel.AnchorDockLeft();
            }
            if (dockType == DockTYpe.Right)
            {
                panel.AnchorDockRight();
            }
            if (dockType == DockTYpe.Full)
            {
                AnchorDockFull(panel);
            }
        }

        private void AnchorDockFull(Image panel)
        {
            float topOffset = 0;
            float bottomOffset = 0;
            float leftOffset = 0;
            float rightOffset = 0;

            Transform parent = panel.transform.parent;
            if (parent == null) return;

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child == panel.transform || !child.gameObject.activeSelf) continue;

                var siblingDock = child.GetComponent<UnvsDockPanel>();
                var childRect = child as RectTransform;

                if (siblingDock != null && childRect != null)
                {
                    if (siblingDock.dockType == DockTYpe.Top)
                        topOffset += childRect.sizeDelta.y;
                    else if (siblingDock.dockType == DockTYpe.Bottom)
                        bottomOffset += childRect.sizeDelta.y;
                    else if (siblingDock.dockType == DockTYpe.Left)
                        leftOffset += childRect.sizeDelta.x;
                    else if (siblingDock.dockType == DockTYpe.Right)
                        rightOffset += childRect.sizeDelta.x;
                }
            }

            panel.rectTransform.anchorMin = new Vector2(0, 0);
            panel.rectTransform.anchorMax = new Vector2(1, 1);
            panel.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            panel.rectTransform.offsetMin = new Vector2(leftOffset, bottomOffset);
            panel.rectTransform.offsetMax = new Vector2(-rightOffset, -topOffset);
        }
    }
    
}