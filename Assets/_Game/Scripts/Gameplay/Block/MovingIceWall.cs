using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class MovingIceWall : MovingHazard
{
    protected override void OnActivate(PlayerEntity target)
    {
        // Không làm gì cả -> Chỉ va chạm vật lý (Đẩy)
    }
}