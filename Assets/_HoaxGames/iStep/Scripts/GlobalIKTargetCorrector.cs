using UnityEngine;

namespace HoaxGames
{
    public class GlobalIKTargetCorrector : MonoBehaviour
    {
        [SerializeField] FootIK m_footIK;
        [SerializeField] bool m_negate = true;
        [SerializeField] UpdateMode m_updateMode = UpdateMode.Update;
        [SerializeField] bool m_utilizeLocalStartOffsetToParent = false;
        [SerializeField] bool m_resetToLocalStartPositionWhenDisabled = false;
        [SerializeField] bool m_resetToLocalStartPositionWhenFootIkIsDisabled = false;
        [SerializeField] OffsetMethod m_offsetMethod = OffsetMethod.FULLBODY_OFFSET;

        public enum OffsetMethod
        {
            FULLBODY_OFFSET,
            RIGHT_HAND_OFFSET,
            RIGHT_SHOULDER_OFFSET,
            LEFT_HAND_OFFSET,
            LEFT_SHOULDER_OFFSET
        }

        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate,
            ManualUpdate
        }

        Transform m_transform;
        Transform m_parentTransform;
        Vector3 m_localOffset;

        void Awake()
        {
            m_transform = this.transform;
            m_parentTransform = m_transform.parent;
        }

        private void OnDisable()
        {
            if (m_resetToLocalStartPositionWhenDisabled) m_transform.localPosition = m_localOffset;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_localOffset = m_transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_updateMode == UpdateMode.Update) updateCorrection();
        }

        private void FixedUpdate()
        {
            if (m_updateMode == UpdateMode.FixedUpdate) updateCorrection();
        }

        private void LateUpdate()
        {
            if (m_updateMode == UpdateMode.LateUpdate) updateCorrection();
        }

        public void updateCorrection()
        {
            if (m_footIK == null) return;
            if (m_footIK.enabled == false && m_resetToLocalStartPositionWhenFootIkIsDisabled)
            {
                m_transform.localPosition = m_localOffset;
                return;
            }

            Vector3 offset = m_footIK.fullBodyOffset;

            if (m_offsetMethod == OffsetMethod.RIGHT_HAND_OFFSET)
            {
                offset = m_footIK.getRightHandOffsetVecIncludingLeaning();
            }
            else if(m_offsetMethod == OffsetMethod.RIGHT_SHOULDER_OFFSET)
            {
                offset = m_footIK.getRightShoulderOffsetVecIncludingLeaning();
            }
            else if (m_offsetMethod == OffsetMethod.LEFT_HAND_OFFSET)
            {
                offset = m_footIK.getLeftHandOffsetVecIncludingLeaning();
            }
            else if (m_offsetMethod == OffsetMethod.LEFT_SHOULDER_OFFSET)
            {
                offset = m_footIK.getLeftShoulderOffsetVecIncludingLeaning();
            }

            if (m_negate) offset *= -1;

            if (m_utilizeLocalStartOffsetToParent == false) m_transform.position = m_parentTransform.position + offset;
            else m_transform.position = m_parentTransform.position + m_localOffset + offset;
        }
    }
}