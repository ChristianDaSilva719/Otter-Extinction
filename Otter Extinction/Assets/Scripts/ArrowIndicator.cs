using UnityEngine;

// Attach this to your arrow prefab. It positions the arrow on a circle around the
// player and rotates it to point at whatever "target" it's been given.
public class ArrowIndicator : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float radius = 2f;

    public float arrowDefaultAngle = 0f;

    public bool reverseRotationDirection = false;

    public bool mirrorArrow = true;

    private void LateUpdate()
    {
        if (player == null || target == null) return;

        Vector2 direction = (Vector2)target.position - (Vector2)player.position;
        if (direction.sqrMagnitude < 0.0001f) return; // avoid a NaN rotation if right on top of the target

        direction.Normalize();

        // Sit the arrow on a circle around the player, facing the target
        transform.position = (Vector2)player.position + direction * radius;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float rotationZ = targetAngle - arrowDefaultAngle;
        if (reverseRotationDirection) rotationZ = -rotationZ;

        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        // Mirror the underlying art on its local X axis, independent of rotation/radius above
        Vector3 scale = transform.localScale;
        scale.x = mirrorArrow ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}