using System;
using UnityEngine;


namespace Avataris.VR.Scripts.Core
{
    public class Game : MonoBehaviour
    {
        #region Event | Action

        public static event Action OnAwake;
        public static event Action OnStart;
        public static event Action OnUpdate;
        public static event Action OnFixedUpdate;
        public static event Action OnLateUpdate;

        #endregion

        #region Awake | Start | Update

        private void Awake()
        {
            if (OnAwake != null)
            {
                OnAwake();
            }
        }

        private void Start()
        {
            if (OnStart != null)
            {
                OnStart();
            }
        }

        private void Update()
        {
            if (OnUpdate != null)
            {
                OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (OnFixedUpdate != null)
            {
                OnFixedUpdate();
            }
        }

        private void LateUpdate()
        {
            if (OnLateUpdate != null)
            {
                OnLateUpdate();
            }
        }

        #endregion
    }
}
