﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Plugins
{
    internal class ResizableWindow : UIBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public static ResizableWindow MakeObjectResizable(RectTransform clickableDragZone, RectTransform resizableObject, Vector2 minDimensions, Vector2 referenceResolution, bool preventCameraControl = true, Action onResize = null)
        {
            ResizableWindow mv = clickableDragZone.gameObject.AddComponent<ResizableWindow>();
            mv.toDrag = resizableObject;
            mv.preventCameraControl = preventCameraControl;

            mv.minDimensions = minDimensions;
            mv.referenceResolution = referenceResolution;
            mv.onResize = onResize;
            return mv;
        }

        private Vector2 _cachedDragPosition;
        private Vector2 _cachedMousePosition;
        private bool _pointerDownCalled;
        private BaseCameraControl _cameraControl;
        private BaseCameraControl.NoCtrlFunc _noControlFunctionCached;

        private Action onResize;

        public RectTransform toDrag;
        public bool preventCameraControl;

        public Vector2 minDimensions;
        public Vector2 referenceResolution;

        public override void Awake()
        {
            base.Awake();
            _cameraControl = FindObjectOfType<BaseCameraControl>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (preventCameraControl && _cameraControl)
            {
                _noControlFunctionCached = _cameraControl.NoCtrlCondition;
                _cameraControl.NoCtrlCondition = () => true;
            }
            _pointerDownCalled = true;
            _cachedDragPosition = toDrag.position;
            _cachedMousePosition = Input.mousePosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_pointerDownCalled == false)
                return;

            toDrag.offsetMin = new Vector2(toDrag.offsetMin.x, Input.mousePosition.y * (referenceResolution.y / Screen.height));
            toDrag.offsetMax = new Vector2(Input.mousePosition.x * ((float)Screen.width / Screen.height * referenceResolution.y / Screen.width), toDrag.offsetMax.y);
            if (onResize != null)
                onResize();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerDownCalled == false)
                return;
            if (preventCameraControl && _cameraControl)
                _cameraControl.NoCtrlCondition = _noControlFunctionCached;
            _pointerDownCalled = false;
        }
    }
}