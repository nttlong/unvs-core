namespace unvs.ui {
    using UnityEngine;
    using unvs.components;
    using unvs.ext;
    using UnityEngine.UI;
    using unvs.types;
    using Unity.VisualScripting;

    public class UnvsDockPanel : MonoBehaviour
    {
        public DockType dockType;
        private void OnValidate()
        {
            var panel= this.transform.parent.GetComponentInParent<UnityEngine.UI.Image>();
            if (panel!=null)
            {
                var layout = panel.GetComponent<HorizontalOrVerticalLayoutGroup>();
                if (layout != null)
                {
                    layout.enabled = false;
                }
            }
           
            doAlign();
        }

        private void doAlign()
        {
            var panel = GetComponent<Image>();
            if (panel == null) return;
            if (dockType == DockType.Top)
            {
                AnchorDockTop(panel);
            }
            else if (dockType == DockType.Bottom)
            {
                AnchorDockBottom(panel);
            }
            else if (dockType == DockType.Left)
            {
                AnchorDockLeft(panel);
            }
            else if (dockType == DockType.Right)
            {
                AnchorDockRight(panel);
            }
            else if (dockType == DockType.Full)
            {
                AnchorDockFull(panel);
            }
        }

        private void AnchorDockTop(Image panel)
        {
            float topOffset = 0;
            float leftOffset = 0;
            float rightOffset = 0;

            Transform parent = panel.transform.parent;
            if (parent != null)
            {
                int myIndex = panel.transform.GetSiblingIndex();
                for (int i = 0; i < myIndex; i++)
                {
                    var child = parent.GetChild(i);
                    if (!child.gameObject.activeSelf) continue;

                    var siblingDock = child.GetComponent<UnvsDockPanel>();
                    var childRect = child as RectTransform;

                    if (siblingDock != null && childRect != null)
                    {
                        if (siblingDock.dockType == DockType.Top)
                            topOffset += childRect.sizeDelta.y;
                        else if (siblingDock.dockType == DockType.Left)
                            leftOffset += childRect.sizeDelta.x;
                        else if (siblingDock.dockType == DockType.Right)
                            rightOffset += childRect.sizeDelta.x;
                    }
                }
            }

            var height = panel.rectTransform.sizeDelta.y;
            panel.rectTransform.anchorMin = new Vector2(0, 1);
            panel.rectTransform.anchorMax = new Vector2(1, 1);
            panel.rectTransform.pivot = new Vector2(0.5f, 1);

            panel.rectTransform.offsetMin = new Vector2(leftOffset, -topOffset - height);
            panel.rectTransform.offsetMax = new Vector2(-rightOffset, -topOffset);
        }

        private void AnchorDockBottom(Image panel)
        {
            float bottomOffset = 0;
            float leftOffset = 0;
            float rightOffset = 0;

            Transform parent = panel.transform.parent;
            if (parent != null)
            {
                int myIndex = panel.transform.GetSiblingIndex();
                for (int i = 0; i < myIndex; i++)
                {
                    var child = parent.GetChild(i);
                    if (!child.gameObject.activeSelf) continue;

                    var siblingDock = child.GetComponent<UnvsDockPanel>();
                    var childRect = child as RectTransform;

                    if (siblingDock != null && childRect != null)
                    {
                        if (siblingDock.dockType == DockType.Bottom)
                            bottomOffset += childRect.sizeDelta.y;
                        else if (siblingDock.dockType == DockType.Left)
                            leftOffset += childRect.sizeDelta.x;
                        else if (siblingDock.dockType == DockType.Right)
                            rightOffset += childRect.sizeDelta.x;
                    }
                }
            }

            var height = panel.rectTransform.sizeDelta.y;
            panel.rectTransform.anchorMin = new Vector2(0, 0);
            panel.rectTransform.anchorMax = new Vector2(1, 0);
            panel.rectTransform.pivot = new Vector2(0.5f, 0);

            panel.rectTransform.offsetMin = new Vector2(leftOffset, bottomOffset);
            panel.rectTransform.offsetMax = new Vector2(-rightOffset, bottomOffset + height);
        }

        private void AnchorDockLeft(Image panel)
        {
            float leftOffset = 0;
            float topOffset = 0;
            float bottomOffset = 0;

            Transform parent = panel.transform.parent;
            if (parent != null)
            {
                int myIndex = panel.transform.GetSiblingIndex();
                for (int i = 0; i < myIndex; i++)
                {
                    var child = parent.GetChild(i);
                    if (!child.gameObject.activeSelf) continue;

                    var siblingDock = child.GetComponent<UnvsDockPanel>();
                    var childRect = child as RectTransform;

                    if (siblingDock != null && childRect != null)
                    {
                        if (siblingDock.dockType == DockType.Left)
                            leftOffset += childRect.sizeDelta.x;
                        else if (siblingDock.dockType == DockType.Top)
                            topOffset += childRect.sizeDelta.y;
                        else if (siblingDock.dockType == DockType.Bottom)
                            bottomOffset += childRect.sizeDelta.y;
                    }
                }
            }

            var width = panel.rectTransform.sizeDelta.x;
            panel.rectTransform.anchorMin = new Vector2(0, 0);
            panel.rectTransform.anchorMax = new Vector2(0, 1);
            panel.rectTransform.pivot = new Vector2(0, 0.5f);

            panel.rectTransform.offsetMin = new Vector2(leftOffset, bottomOffset);
            panel.rectTransform.offsetMax = new Vector2(leftOffset + width, -topOffset);
        }

        private void AnchorDockRight(Image panel)
        {
            float rightOffset = 0;
            float topOffset = 0;
            float bottomOffset = 0;

            Transform parent = panel.transform.parent;
            if (parent != null)
            {
                int myIndex = panel.transform.GetSiblingIndex();
                for (int i = 0; i < myIndex; i++)
                {
                    var child = parent.GetChild(i);
                    if (!child.gameObject.activeSelf) continue;

                    var siblingDock = child.GetComponent<UnvsDockPanel>();
                    var childRect = child as RectTransform;

                    if (siblingDock != null && childRect != null)
                    {
                        if (siblingDock.dockType == DockType.Right)
                            rightOffset += childRect.sizeDelta.x;
                        else if (siblingDock.dockType == DockType.Top)
                            topOffset += childRect.sizeDelta.y;
                        else if (siblingDock.dockType == DockType.Bottom)
                            bottomOffset += childRect.sizeDelta.y;
                    }
                }
            }

            var width = panel.rectTransform.sizeDelta.x;
            panel.rectTransform.anchorMin = new Vector2(1, 0);
            panel.rectTransform.anchorMax = new Vector2(1, 1);
            panel.rectTransform.pivot = new Vector2(1, 0.5f);

            panel.rectTransform.offsetMin = new Vector2(-rightOffset - width, bottomOffset);
            panel.rectTransform.offsetMax = new Vector2(-rightOffset, -topOffset);
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
                    if (siblingDock.dockType == DockType.Top)
                        topOffset += childRect.sizeDelta.y;
                    else if (siblingDock.dockType == DockType.Bottom)
                        bottomOffset += childRect.sizeDelta.y;
                    else if (siblingDock.dockType == DockType.Left)
                        leftOffset += childRect.sizeDelta.x;
                    else if (siblingDock.dockType == DockType.Right)
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