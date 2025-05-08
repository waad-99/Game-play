using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace WaadNamespace
{
    public class WaveTriggerButton : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
    {
        class PressInfo
        {
            internal UnityEngine.XR.Interaction.Toolkit.Interactors.IXRHoverInteractor m_Interactor;
            internal bool m_InPressRegion = false;
            internal bool m_WrongSide = false;
        }

        [SerializeField] Transform m_Button = null;
        [SerializeField] float m_PressDistance = 0.1f;
        [SerializeField] float m_PressBuffer = 0.01f;
        [SerializeField] float m_ButtonOffset = 0.0f;
        [SerializeField] float m_ButtonSize = 0.1f;
        [SerializeField] bool m_ToggleButton = false;

        [Header("🎯 Events")]
        [SerializeField] UnityEvent m_OnPress;
        [SerializeField] UnityEvent m_OnRelease;
        [SerializeField] public UnityEvent<float> m_OnValueChange;

        bool m_Pressed = false;
        bool m_Toggled = false;
        float m_Value = 0f;
        Vector3 m_BaseButtonPosition = Vector3.zero;

        Dictionary<UnityEngine.XR.Interaction.Toolkit.Interactors.IXRHoverInteractor, PressInfo> m_HoveringInteractors = new();
        bool isGameWon = false;

        public override bool IsHoverableBy(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRHoverInteractor interactor)
        {
            return !(interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor) && base.IsHoverableBy(interactor);
        }

        void Start()
        {
            if (m_Button != null)
                m_BaseButtonPosition = m_Button.position;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetButtonHeight(m_Toggled ? -m_PressDistance : 0f);
            hoverEntered.AddListener(StartHover);
            hoverExited.AddListener(EndHover);
        }

        protected override void OnDisable()
        {
            hoverEntered.RemoveListener(StartHover);
            hoverExited.RemoveListener(EndHover);
            base.OnDisable();
        }

        void StartHover(HoverEnterEventArgs args)
        {
            m_HoveringInteractors[args.interactorObject] = new PressInfo { m_Interactor = args.interactorObject };
        }

        void EndHover(HoverExitEventArgs args)
        {
            m_HoveringInteractors.Remove(args.interactorObject);
            SetButtonHeight(m_Toggled && m_ToggleButton ? -m_PressDistance : 0f);
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && m_HoveringInteractors.Count > 0 && !isGameWon)
                UpdatePress();
        }

        void UpdatePress()
        {
            float minHeight = m_Toggled && m_ToggleButton ? -m_PressDistance : 0f;

            foreach (var pressInfo in m_HoveringInteractors.Values)
            {
                var pos = pressInfo.m_Interactor.GetAttachTransform(this).position;
                var localOffset = transform.InverseTransformVector(pos - m_BaseButtonPosition);
                bool inRegion = Mathf.Abs(localOffset.x) < m_ButtonSize && Mathf.Abs(localOffset.z) < m_ButtonSize;

                if (inRegion && !pressInfo.m_InPressRegion)
                    pressInfo.m_WrongSide = localOffset.y < m_ButtonOffset;

                if (inRegion && !pressInfo.m_WrongSide)
                    minHeight = Mathf.Min(minHeight, localOffset.y - m_ButtonOffset);

                pressInfo.m_InPressRegion = inRegion;
            }

            minHeight = Mathf.Max(minHeight, -(m_PressDistance + m_PressBuffer));
            bool pressed = m_ToggleButton ? (minHeight <= -(m_PressDistance + m_PressBuffer)) : (minHeight < -m_PressDistance);
            float distance = Mathf.Max(0f, -minHeight - m_PressBuffer);
            m_Value = distance / m_PressDistance;

            if (m_ToggleButton)
            {
                if (pressed && !m_Pressed)
                {
                    m_Toggled = !m_Toggled;
                    if (m_Toggled) m_OnPress.Invoke(); else m_OnRelease.Invoke();
                }
            }
            else
            {
                if (pressed && !m_Pressed) m_OnPress.Invoke();
                else if (!pressed && m_Pressed) m_OnRelease.Invoke();
            }

            m_Pressed = pressed;

            if (m_Pressed) m_OnValueChange.Invoke(m_Value);
            SetButtonHeight(minHeight);
        }

        void SetButtonHeight(float height)
        {
            if (m_Button != null)
            {
                var pos = m_Button.localPosition;
                pos.y = height;
                m_Button.localPosition = pos;
            }
        }

        void OnDrawGizmosSelected()
        {
            Vector3 pos = m_Button != null ? m_Button.localPosition : Vector3.zero;
            pos.y += m_ButtonOffset - (m_PressDistance * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(pos, new Vector3(m_ButtonSize, m_PressDistance, m_ButtonSize));
        }

        void OnValidate() => SetButtonHeight(0.0f);

        public void GameWon()
        {
            isGameWon = true;
            m_Toggled = false;
            SetButtonHeight(0f);
            m_OnRelease.Invoke();

            // استدعاء Reset للـ CapsuleSpawner
            CapsuleSpawner spawner = FindObjectOfType<CapsuleSpawner>();
            if (spawner != null)
            {
                spawner.RestartSpawner();
            }
        }

        public void RestartButton()
        {
            isGameWon = false;
            m_Toggled = false;
            SetButtonHeight(0f);
        }

        public void RestartGame()
        {
            CapsuleSpawner spawner = FindObjectOfType<CapsuleSpawner>();
            if (spawner != null)
            {
                spawner.RestartSpawner();
            }

            RestartButton();
        }
    }
}
