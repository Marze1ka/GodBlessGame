using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform[] _points;
    
    [SerializeField]
    private NavMeshAgent _agent;

    [SerializeField]
    private float _stopDistance = 2f;

    private Vector3 _currentPoint;


    private void Start()
    {
        SetupCurrentPoint();
    }

    private void Update()
    {
        if(_currentPoint == null)
        {
            return;
        }
        _agent.SetDestination(_currentPoint);

        if(Vector3.Distance(transform.position, _currentPoint) < _stopDistance ) 
        {
            SetupCurrentPoint();
        }
    }

    private Vector3 GetRandomPoint()
    {
       return _points[Random.Range(0, _points.Length)].position;
    }

    private void SetupCurrentPoint()
    {
        _currentPoint = GetRandomPoint();
    }
}
