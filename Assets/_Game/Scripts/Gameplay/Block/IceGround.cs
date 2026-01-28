using UnityEngine;

public class IceGround : MonoBehaviour
{
    private PlayerInput cachedInput;
    private Rigidbody2D cachedRb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        cachedInput = collision.collider.GetComponent<PlayerInput>();
        cachedRb = collision.collider.GetComponent<Rigidbody2D>();

        if (cachedInput == null || cachedRb == null)
            return;

        cachedInput.On<Vector2>(PlayerInputType.Move, OnMove);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        if (cachedInput != null)
        {
            cachedInput.Off<Vector2>(PlayerInputType.Move, OnMove);
        }

        cachedInput = null;
        cachedRb = null;
    }

    private void OnMove(Vector2 movement)
    {
        // chỉ xử lý ngang
        cachedRb.AddForce(
            new Vector2(movement.x * 2f, 0),
            ForceMode2D.Impulse
        );
    }
}
