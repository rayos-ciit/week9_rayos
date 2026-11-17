using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    #region Serialized Fields

    [Header("Injected Dependencies")]
    [SerializeField] private PerspectiveCamera _perspectiveCamera;
    [SerializeField] private PlayerMovementController _playerMovement;
    [SerializeField] private PlayerHealthSystem _playerHealth;
    [SerializeField] private ObstacleCollisionDetector _collisionDetector;
    [SerializeField] private ObstacleItem _obstaclePrefab;

    [Header("Spawn & Movement Settings")]
    [SerializeField] private float _spawnIntervalSeconds = 1.1f;
    [SerializeField] private float _spawnDepthZ = 25f;
    [SerializeField] private float _despawnDepthZ = -2f;
    [SerializeField] private float _obstacleSpeedZ = 10f;
    [SerializeField] private float _laneWidth = 2.5f;
    [SerializeField] private float _groundYPosition = -2.0f;

    [Header("Obstacle Art Variations")]
    [SerializeField] private Sprite[] _obstacleSprites;

    #endregion

    #region Private Fields

    private readonly List<ObstacleItem> _activeObstacles = new List<ObstacleItem>();
    private float _spawnTimer;

    #endregion

    #region Unity Lifecycle

    private void Update()
    {
        UpdateSpawnTimer();
        UpdateActiveObstacles();
        SortObstaclesByDepth();
    }

    #endregion

    #region Private Methods

    private void UpdateSpawnTimer()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer < _spawnIntervalSeconds)
        {
            return;
        }

        _spawnTimer = 0f;
        SpawnObstacleInRandomLane();
    }

    private void SpawnObstacleInRandomLane()
    {
        if (_obstaclePrefab == null || _perspectiveCamera == null)
        {
            return;
        }

        //-1 (Left), 0 (Center), 1 (Right)
        int randomLaneOffset = Random.Range(-1, 2);
        float laneX = randomLaneOffset * _laneWidth;
        Vector3 spawnLogicalPosition = new Vector3(laneX, _groundYPosition, _spawnDepthZ);

        ObstacleItem newObstacle = Instantiate(_obstaclePrefab, transform);
        Sprite chosenSprite = GetRandomObstacleSprite();

        newObstacle.Initialize(_perspectiveCamera, spawnLogicalPosition, _obstacleSpeedZ, chosenSprite);
        _activeObstacles.Add(newObstacle);
    }

    private void UpdateActiveObstacles()
    {
        float deltaTime = Time.deltaTime;

        for (int index = _activeObstacles.Count - 1; index >= 0; index--)
        {
            ObstacleItem currentObstacle = _activeObstacles[index];
            if (currentObstacle == null)
            {
                _activeObstacles.RemoveAt(index);
                continue;
            }

            currentObstacle.TickMovement(deltaTime);
            EvaluateObstacleCollision(currentObstacle);

            if (currentObstacle.LogicalPosition.z <= _despawnDepthZ)
            {
                _activeObstacles.RemoveAt(index);
                Destroy(currentObstacle.gameObject);
            }
        }
    }

    private void EvaluateObstacleCollision(ObstacleItem obstacle)
    {
        if (_collisionDetector == null || _playerMovement == null || _playerHealth == null)
        {
            return;
        }

        bool isHit = _collisionDetector.CheckCollision(obstacle, _playerMovement);
        if (!isHit)
        {
            return;
        }

        obstacle.MarkAsHit();
        _playerHealth.TakeHitDamage();
    }

    private void SortObstaclesByDepth()
    {
        if (_activeObstacles.Count <= 1)
        {
            return;
        }

        _activeObstacles.Sort((obstacleA, obstacleB) =>
            obstacleB.LogicalPosition.z.CompareTo(obstacleA.LogicalPosition.z));

        for (int index = 0; index < _activeObstacles.Count; index++)
        {
            _activeObstacles[index].SetSortingOrder(index);
        }
    }

    private Sprite GetRandomObstacleSprite()
    {
        if (_obstacleSprites == null || _obstacleSprites.Length == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, _obstacleSprites.Length);
        return _obstacleSprites[randomIndex];
    }

    #endregion
}