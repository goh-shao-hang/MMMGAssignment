using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCells.Modules
{
    public class SphereCheckModule : CollisionCheckModule
    {
        [Header("Settings")]
        [SerializeField] private float _checkRadius;
        [SerializeField] private LayerMask _targetLayerMask;

        public override bool Hit => Physics.CheckSphere(transform.position, _checkRadius, _targetLayerMask);

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.DrawWireSphere(transform.position, _checkRadius);
        }


    }
}