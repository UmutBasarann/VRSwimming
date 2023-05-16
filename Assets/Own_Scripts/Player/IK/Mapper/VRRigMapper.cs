using UnityEngine;

namespace Avataris.VR.Scripts.Player.IK.Mapper
{
    public class VRRigMapper : MonoBehaviour
    {
        #region SerializeFields

        [SerializeField] private Transform vrTarget = null;
        [SerializeField] private Vector3 trackingPositionOffset = Vector3.zero;
        [SerializeField] private Vector3 trackingRotationOffset = Vector3.zero;

        #endregion

        #region Map

        public void Map()
        {
            transform.position = vrTarget.TransformPoint(trackingPositionOffset);
            transform.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }

        #endregion
    }
}
