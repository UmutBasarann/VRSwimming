using System;
using Avataris.VR.Scripts.Core;
using Avataris.VR.Scripts.Data.Swim;
using Avataris.VR.Scripts.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Avataris.VR.Scripts.Player.Swim
{
    [RequireComponent(typeof(Rigidbody))]
    public class Swimmer : MonoBehaviour
    {
        #region Event | Action

        public static event Action OnSwim;
        public static event Action OnTreadingWater;

        #endregion

        #region Fields

        #region InputActions

        private XRIDefaultInputActions _xriInputAction;
        private InputAction _bothGripHold;
        private InputAction _leftControllerVelocity;
        private InputAction _rightControllerVelocity;

        #endregion

        #region References

        private DataLoader _dataLoader;
        private SwimData _swimData;
        private Rigidbody _rigidbody;
        private Transform _trackingReference;

        #endregion

        #region Velocity Values

        private Vector3 _leftHandVelocity;
        private Vector3 _rightHandVelocity;
        private Vector3 _localVelocity;
        private Vector3 _worldVelocity;

        #endregion

        private float _clampedYPosition = 0f;
        private float _cooldownTimer = 0f;

        #endregion

        #region Awake | Start | Update

        private void Awake()
        {
            _xriInputAction = new XRIDefaultInputActions();
            
            _dataLoader = new DataLoader();
            _swimData = _dataLoader.LoadFromJson<SwimData>("Data/swimData");
            
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.mass = 20f;
            _rigidbody.useGravity = false;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            _trackingReference = transform;
        }

        #endregion

        #region OnEnable

        private void OnEnable()
        {
            SetInputActions();
            EnableInputActions();
            
            _clampedYPosition = Mathf.Clamp(transform.localPosition.y, -Single.Epsilon, 0f);
            
            AddEvents();
        }

        #endregion

        #region OnDisable

        private void OnDisable()
        {
            DisableInputActions();
            RemoveEvents();
        }

        #endregion

        #region OnFixedUpdate

        private void OnFixedUpdate()
        {
            _cooldownTimer += Time.fixedDeltaTime;
            
            if (IsOnSurface())
            {
                if (OnTreadingWater != null)
                {
                    OnTreadingWater();
                }
            }
            else
            {
                if (OnSwim != null)
                {
                    OnSwim();
                }
            }
            
            if (_cooldownTimer <= _swimData.minTimeBetweenStrokes)
            {
                return;
            }
            
            Swim();
        }

        #endregion

        #region SetInputActions

        private void SetInputActions()
        {
            _bothGripHold = _xriInputAction.XRICombinations.BothGripHold;
            _leftControllerVelocity = _xriInputAction.XRILeftHand.Velocity;
            _rightControllerVelocity = _xriInputAction.XRIRightHand.Velocity;
        }

        #endregion

        #region EnableInputActions

        private void EnableInputActions()
        {
            _bothGripHold.Enable();
            _leftControllerVelocity.Enable();
            _rightControllerVelocity.Enable();
        }

        #endregion

        #region DisableInputActions

        private void DisableInputActions()
        {
            _bothGripHold.Disable();
            _leftControllerVelocity.Disable();
            _rightControllerVelocity.Disable();
        }

        #endregion

        #region Swim

        private void Swim()
        {
            if (_bothGripHold.IsPressed())
            {
                GetVelocityValues();
                
                Vector3 appliedVelocity = IsOnSurface() ? new Vector3(_worldVelocity.x, -transform.up.y, _worldVelocity.z) : _worldVelocity;
            
                if (_localVelocity.sqrMagnitude > Mathf.Sqrt(_swimData.minForce))
                {
                    _rigidbody.AddForce(appliedVelocity * _swimData.swimForce, ForceMode.Acceleration);
                    _cooldownTimer = 0f;
                }
            }
            
            ApplyDragForce();
        }

        #endregion

        #region Get: VelocityValues

        private void GetVelocityValues()
        { 
            _leftHandVelocity = _leftControllerVelocity.ReadValue<Vector3>();
            _rightHandVelocity = _rightControllerVelocity.ReadValue<Vector3>();
            _localVelocity = (_leftHandVelocity + _rightHandVelocity) * SwimEnum.VelocityMultiplier;
            _worldVelocity = _trackingReference.TransformDirection(_localVelocity); 
            
            Debug.Log("Velocity Values Set.");
        }

        #endregion

        #region ApplyDragForce

        private void ApplyDragForce()
        {
            Vector3 dragForce = transform.localPosition.y < SwimEnum.DragForceLocalY ? _rigidbody.velocity : new Vector3(_rigidbody.velocity.x, transform.up.y, _rigidbody.velocity.z);

            if (_rigidbody.velocity.sqrMagnitude <= SwimEnum.DragForceApplicationValue)
            {
                return;
            }
            
            _rigidbody.AddForce(-dragForce * _swimData.dragForce, ForceMode.Acceleration);
            
            Debug.Log("Applying drag force.");
        }

        #endregion

        #region IsOnSurface

        private bool IsOnSurface()
        {
            if (transform.position.y < _clampedYPosition)
            {
                return false;
            }

            return true;
        }

        #endregion


        #region AddEvents

        private void AddEvents()
        {
            Game.OnFixedUpdate += OnFixedUpdate;
        }

        #endregion

        #region RemoveEvents

        private void RemoveEvents()
        {
            Game.OnFixedUpdate -= OnFixedUpdate;
        }

        #endregion
    }
}
