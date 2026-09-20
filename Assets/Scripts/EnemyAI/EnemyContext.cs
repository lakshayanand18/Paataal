using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyContext
{
    private Rigidbody _rb;
    private float _maxForce;
    private Transform _player;
    private Transform _enemyTransform;
    private float _distanceFromPlayer;
    private ParticleSystem[] _lasers;
    private Collider _playerCollider;
    LayerMask _playerLayerMask;
    LayerMask _obstacleLayerMask;

    //A* variables

    private bool _isLeader;
    private MonoBehaviour _monoBehaviour;
    private float _speed;
    private HealthSystem UIEnemyHealth;
    private float _turnSpeed;

    private float _separationDistance;
    private float _separation;
    private float _cohesion;
    private float _alignment;
    private float _avoidance;
    private float _maxFlockingDistanceFromPlayer;
    private float _returnToPlayerWeight;
    private int _health;
    private HealthSystem _healthSystem;
    private PlayerStateMachine _playerStateMachine;

    // flags
    private bool _isSeeking;
    private bool _isAttacking;
    private bool _isFlocking;

    private bool _isSloted;
    private bool _isAttackingPlayer;

    private int _assignedSlotIndex;
    private bool _isWaitingCoroutineRunning;
    private bool _isAttackingCoroutineRunning;




    public EnemyContext(
        Rigidbody rb,
        Transform player,
        float maxForce,
        Collider playerCollider,
        Transform enemyTransform,
        float speed,
        float turnSpeed,
        ParticleSystem[] lasers,
        LayerMask playerLayerMask,
        LayerMask obstacleLayerMask,
        bool isLeader,
        MonoBehaviour monoBehaviour,
        float separationDistance,
        float separation,
        float cohesion,
        float alignment,
        float avoidance,
        float maxFlockingDistanceFromPlayer,
        float returnToPlayerWeight,
        int health,
        HealthSystem healtSystem,
        bool isSeeking,
        bool isSloted,

        PlayerStateMachine playerStateMachine
        
        )
    {
        _rb = rb;
        _maxForce = maxForce;
        _player = player;
        _isLeader = isLeader;
        _playerCollider = playerCollider;
        _enemyTransform = enemyTransform;
        _lasers = lasers;
        _speed = speed;
        _turnSpeed = turnSpeed;
        _playerLayerMask = playerLayerMask;
        _obstacleLayerMask = obstacleLayerMask;
        _monoBehaviour = monoBehaviour;
        _separationDistance = separationDistance;
        _separation = separation;
        _cohesion = cohesion;
        _alignment = alignment;
        _avoidance = avoidance;
        _maxFlockingDistanceFromPlayer = maxFlockingDistanceFromPlayer;
        _returnToPlayerWeight = returnToPlayerWeight;
        _health = health;
        _healthSystem = healtSystem;
        _isSeeking = isSeeking;
        _isSloted = isSloted;
        _playerStateMachine = playerStateMachine;
    }

    public Rigidbody Rb => _rb;
    public float MaxForce => _maxForce;
    public Transform Player => _player;
    public bool IsLeader { get { return _isLeader; } set { _isLeader = value; } } // is leader
    public Collider PlayerCollider => _playerCollider;
    public Transform EnemyTransform => _enemyTransform;
    public ParticleSystem[] Lasers => _lasers;
    public float DistanceFromPlayer { get { return _distanceFromPlayer; } set { _distanceFromPlayer = value; } }

    public LayerMask PlayerLayerMask { get { return _playerLayerMask; } set { _playerLayerMask = value; } }
    public LayerMask ObstacleLayerMask { get { return _obstacleLayerMask; } set { _obstacleLayerMask = value; } }

    public MonoBehaviour MonoBehaviour { get { return _monoBehaviour; } set { _monoBehaviour = value; } }

    public float SeparationDistance => _separationDistance;

    public float SeparationWeight => _separation;
    public float CohesionWeight => _cohesion;
    public float AlignmentWeight => _alignment;
    public float AvoidanceWeight => _avoidance;
    public float MaxFlockingDistanceFromPlayer => _maxFlockingDistanceFromPlayer;
    public float ReturnToPlayerWeight => _returnToPlayerWeight;

    public float Speed { get { return _speed; } set { _speed = value; } }
    public float TurnSpeed => _turnSpeed;
    public int Health => _health;
    public HealthSystem HealthSystem => _healthSystem;
    public bool IsSeeking
    {
        get { return _isSeeking; }
        set { _isSeeking = value; }

    }
    public bool IsAttacking
    {
        get { return _isAttacking; }
        set { _isAttacking = value; }

    }
    public bool IsAttackingPlayer
    {
        get { return _isAttackingPlayer; }
        set { _isAttackingPlayer = value; }

    }
    public bool IsFlocking
    {
        get { return _isFlocking; }
        set { _isFlocking = value; }

    }
    public bool IsSloted
    {
        get { return _isSloted; }
        set { _isSloted = value; }
    }
    public bool IsWaitingCoroutineRunning
    {
        get { return _isWaitingCoroutineRunning; }
        set { _isWaitingCoroutineRunning = value; }
    }
    public bool IsAttackingCoroutineRunning
    {
        get { return _isAttackingCoroutineRunning; }
        set { _isAttackingCoroutineRunning = value; }
    }
    public int AssignedSlotIndex
    {
        get { return _assignedSlotIndex; }
        set { _assignedSlotIndex = value; }

    }
    public PlayerStateMachine PlayerStateMachine
    {
        get { return _playerStateMachine; }
        
    }
}

