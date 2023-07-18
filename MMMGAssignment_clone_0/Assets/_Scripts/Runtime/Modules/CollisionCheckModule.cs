using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionCheckModule : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _showGizmos = false;
    [SerializeField] private Color _falseGizmosColor = Color.red;
    [SerializeField] private Color _trueGizmosColor = Color.green;

    public abstract bool Hit { get; }

    protected virtual void OnDrawGizmos()
    {
        if (!_showGizmos)
            return;

        if (Hit)
            Gizmos.color = _trueGizmosColor;
        else
            Gizmos.color = _falseGizmosColor;
    }
}
