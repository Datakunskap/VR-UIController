using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OTHStudios
{
    public class OTHMenuControl : OTHOnCollisionCue
    {
        [Header("Control Key")]
        [Tooltip("The key gameobject used to control the UI")]
        public GameObject key;
        [System.Serializable]
        public class InputEvents
        {
            [Tooltip("Event to call when key is turned to the right")]
            public UnityEvent onRight;
            [Tooltip("Event to call when key is turned to the left")]
            public UnityEvent onLeft;
            [Tooltip("Event to call to open the menu")]
            public UnityEvent openMenu;
            [Tooltip("Event to call to close the menu")]
            public UnityEvent closeMenu;
        }
        [Header("Menu Manager Events")]
        public InputEvents _inputEvents;

        [Tooltip("The animator controlling the key-lock turn")]
        private Animator keyTurnAnimator;
        private bool isInputCooldown;
        private bool isActive;

        // Start is called before the first frame update
        void Start()
        {
            _onTriggerEnter.targetedColliders.Add(key.GetComponent<Collider>());
            _onTriggerStay.targetedColliders.Add(key.GetComponent<Collider>());
            _onTriggerExit.targetedColliders.Add(key.GetComponent<Collider>());
            keyTurnAnimator = GetComponentInChildren<Animator>();
            base._onTriggerEnter.shouldActivate = true;
            base._onTriggerStay.shouldActivate = true;
            base._onTriggerExit.shouldActivate = true;
            _inputEvents.closeMenu.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            if (isActive)
            {
                if (!isInputCooldown)
                {
                    if (IsRotatedRight())
                    {
                        _inputEvents.onRight.Invoke();

                        StartCoroutine(StartInputCooldown(.55f));
                        LockTwistRightAnim();
                    }
                    else if (IsRotatedLeft())
                    {
                        _inputEvents.onLeft.Invoke();

                        StartCoroutine(StartInputCooldown(.55f));
                        LockTwistLeftAnim();
                    }
                    else
                    {
                        // Neutral key animation
                    }
                }
            }
            else
            {
                // Key not inserted --> Reset UI?
            }
        }

        protected override void ApprovedOnTriggerEnter(Collider other)
        {
            base.ApprovedOnTriggerEnter(other);
            _inputEvents.openMenu.Invoke();
            isActive = true;
        }

        protected override void ApprovedOnTriggerStay(Collider other)
        {    
            base.ApprovedOnTriggerEnter(other);
            isActive = true;
        }

        protected override void ApprovedOnTriggerExit(Collider other)
        {
            base.ApprovedOnTriggerExit(other);
            _inputEvents.closeMenu.Invoke();
            isActive = false;
        }

        protected virtual void LockTwistRightAnim()
        {
            keyTurnAnimator.Play("Key Twist Right");
        }

        protected virtual void LockTwistLeftAnim()
        {
            keyTurnAnimator.Play("Key Twist Left");
        }

        protected virtual bool IsRotatedRight()
        {
            float angleBetween = Vector3.Angle(transform.right, key.transform.up);
            /*if (base._debugAll)
                Debug.Log("Angle: " + angleBetween);*/

            return angleBetween <= 55f;
        }

        protected virtual bool IsRotatedLeft()
        {
            float angleBetween = Vector3.Angle(transform.right, key.transform.up);
            /*if (base._debugAll)
                Debug.Log("Angle: " + angleBetween);*/

            return angleBetween >= 125f;
        }

        protected IEnumerator StartInputCooldown(float cooldown)
        {
            isInputCooldown = true;
            yield return new WaitForSeconds(cooldown);
            isInputCooldown = false;
        }
    }
}
