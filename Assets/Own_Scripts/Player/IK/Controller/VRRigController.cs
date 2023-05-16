using Avataris.VR.Scripts.Core;
using Avataris.VR.Scripts.Data.Animation;
using Avataris.VR.Scripts.Player.IK.Mapper;
using Avataris.VR.Scripts.Player.Swim;
using UnityEngine;

namespace Avataris.VR.Scripts.Player.IK.Controller
{
    public class VRRigController : MonoBehaviour
    {
        #region SerializeFields

        [Header("Mapper")]
        [SerializeField] private VRRigMapper head = null;
        [SerializeField] private VRRigMapper rightHand = null;
        [SerializeField] private VRRigMapper leftHand = null;

        [SerializeField] private Transform ikHead = null;
        [SerializeField] private Vector3 headBodyOffset = Vector3.zero;
        [SerializeField] private float turnSmoothness = 0f;

        #endregion

        #region Fields

        private Animator _animator;

        private bool _isSwimming = false;
        private bool _isTreadingWater = false;

        #endregion

        #region Awake | Start | Update

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        #endregion

        #region OnEnable

        private void OnEnable()
        {
            AddEvents();
        }

        #endregion

        #region OnDisable

        private void OnDisable()
        {
            RemoveEvents();
        }

        #endregion

        #region Event: OnLateUpdate

        private void OnLateUpdate()
        {
            transform.position = ikHead.position + headBodyOffset;
            transform.forward = Vector3.Lerp(transform.forward,
                Vector3.ProjectOnPlane(ikHead.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

            head.Map();
            rightHand.Map();
            leftHand.Map();

        }

        #endregion
        
        #region Event: OnSwim

        private void OnSwim()
        {
            _isSwimming = _animator.GetBool(AnimationEnum.IsSwimmingCondition);

            if (_isSwimming != false)
            {
                return;
            }
            
            _animator.SetBool(AnimationEnum.IsSwimmingCondition, true);
            _animator.SetBool(AnimationEnum.TreadingWaterCondition, false);
            headBodyOffset.y = SwimEnum.SwimmingYOffset;
        }

        #endregion

        #region Event: OnTreadingWater

        private void OnTreadingWater()
        {
            _isTreadingWater = _animator.GetBool(AnimationEnum.TreadingWaterCondition);
            
            if (_isTreadingWater != false)
            {
                return;

            }
            
            _animator.SetBool(AnimationEnum.TreadingWaterCondition, true);
            _animator.SetBool(AnimationEnum.IsSwimmingCondition, false);
            headBodyOffset.y = SwimEnum.TreadingWaterYOffset;
        }

        #endregion

        #region AddEvents

        private void AddEvents()
        {
            Game.OnLateUpdate += OnLateUpdate;
            Swimmer.OnSwim += OnSwim;
            Swimmer.OnTreadingWater += OnTreadingWater;
        }

        

        #endregion

        #region RemoveEvents

        private void RemoveEvents()
        {
            Game.OnLateUpdate -= OnLateUpdate;
            Swimmer.OnSwim -= OnSwim;
            Swimmer.OnTreadingWater -= OnTreadingWater;
        }

        #endregion
    }
}
