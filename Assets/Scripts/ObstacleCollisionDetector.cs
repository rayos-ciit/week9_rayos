using UnityEngine;

public class ObstacleCollisionDetector : MonoBehaviour
{
    #region Serialized Fields

    [Header("Collision Thresholds")]
    [SerializeField] private float _zHitRange = 0.65f;
    [SerializeField] private float _xLaneHitThreshold = 0.85f;
    [SerializeField] private float _jumpClearanceHeight = 1.0f;

    #endregion

    #region Public Methods

    public bool CheckCollision(ObstacleItem obstacle, PlayerMovementController player)
    {
        if (obstacle == null || player == null)
        {
            return false;
        }

        if (obstacle.HasTriggeredHit)
        {
            return false;
        }

        Vector3 displacement = obstacle.LogicalPosition - player.LogicalPosition;

        //check Z depth range
        float zDistance = Mathf.Abs(displacement.z);
        if (zDistance > _zHitRange)
        {
            return false;
        }

        //check X lane alignment
        float xDistance = Mathf.Abs(displacement.x);
        if (xDistance > _xLaneHitThreshold)
        {
            return false;
        }

        //check Y height (if player jumped high enough above ground, they evade the obstacle)
        float playerHeightAboveGround = player.LogicalPosition.y - player.GroundYPosition;
        if (playerHeightAboveGround >= _jumpClearanceHeight)
        {
            return false;
        }

        return true;
    }

    #endregion
}