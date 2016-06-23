﻿using UnityEngine;

namespace UnityEditor.Experimental
{
    internal class CommentResize : IManipulate
    {
        private bool m_Active;
        private Vector2 m_Start;
        private Vector2 m_MinimumScale;
        //private GUIStyle style = new GUIStyle("WindowBottomResize");

        public CommentResize(Vector2 minimumScale)
        {
            m_MinimumScale = minimumScale;
        }

        public bool GetCaps(ManipulatorCapability cap)
        {
            return false;
        }

        public void AttachTo(CanvasElement element)
        {
            element.MouseDown += OnMouseDown;
            element.MouseDrag += OnMouseDrag;
            element.MouseUp += OnMouseUp;
        }

        private bool OnMouseDown(CanvasElement element, Event e, Canvas2D parent)
        {
            Rect r = element.boundingRect;
            Rect widget = r;
            widget.min = new Vector2(r.max.x - 32f, r.max.y - 32f);

            if (widget.Contains(parent.MouseToCanvas(e.mousePosition)))
            {
                parent.StartCapture(this, element);
                parent.ClearSelection();
                m_Active = true;
                m_Start = parent.MouseToCanvas(e.mousePosition);
                e.Use();
            }

            return true;
        }

        private bool OnMouseDrag(CanvasElement element, Event e, Canvas2D parent)
        {
            if (!m_Active || e.type != EventType.MouseDrag)
                return false;

            Vector2 newPosition = parent.MouseToCanvas(e.mousePosition);
            Vector2 diff = newPosition - m_Start;
            m_Start = newPosition;
            Vector3 newScale = element.scale;
            newScale.x = Mathf.Max(m_MinimumScale.x, newScale.x + diff.x);
            newScale.y = Mathf.Max(m_MinimumScale.y, newScale.y + diff.y);

            element.scale = newScale;

            element.DeepInvalidate();
            e.Use();
            return true;
        }

        private bool OnMouseUp(CanvasElement element, Event e, Canvas2D parent)
        {
            if (m_Active)
            {
                parent.EndCapture();
                parent.RebuildQuadTree();
            }
            m_Active = false;

            return true;
        }
    };
}
