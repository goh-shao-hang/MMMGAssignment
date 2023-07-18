using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Modules
{
    public class Raycast3DModule : CollisionCheckModule
    {
        [Header("Settings")]
        [SerializeField] private Vector3 _raycastDirection = Vector3.down;
        [SerializeField] private float _raycastDistance;
        [SerializeField] private bool _followPlayerRotation = false;
        [SerializeField] private LayerMask _targetLayerMask;

        private RaycastHit _hitInfo;

        public override bool Hit => Physics.Raycast(transform.position, _followPlayerRotation ? transform.TransformDirection(_raycastDirection) : _raycastDirection, _raycastDistance, _targetLayerMask);

        public RaycastHit HitInfo()
        {
            Physics.Raycast(transform.position, _raycastDirection, out _hitInfo, _raycastDistance, _targetLayerMask);
            return _hitInfo;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.DrawLine(transform.position, transform.position + (_followPlayerRotation ? transform.TransformDirection(_raycastDirection) : _raycastDirection) * _raycastDistance);
        }

    }
}