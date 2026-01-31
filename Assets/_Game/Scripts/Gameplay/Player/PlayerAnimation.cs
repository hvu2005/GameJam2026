using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerFormController playerFormController;
    private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject mask1;
    [SerializeField] private GameObject mask2;
    [SerializeField] private GameObject mask3;

    [SerializeField] private GameObject[] ps;
    // Start is called before the first frame update
    void Awake()
    {
        playerFormController = GetComponent<PlayerFormController>();
        playerMovement = GetComponent<PlayerMovement>();

        mask1.SetActive(false);
        mask2.SetActive(false);
        mask3.SetActive(false);
    }

    void Start()
    {
        
        EventBus.On<MovementEventData>(PlayerActionEventType.OnMoveStarted, data =>
        {
            animator.SetBool("isMoving", true);
        });
        EventBus.On<MovementEventData>(PlayerActionEventType.OnMoveStopped, data =>
        {
            animator.SetBool("isMoving", false);
        });

        EventBus.On<JumpEventData>(PlayerActionEventType.OnJumpStarted, data =>
        {
            Debug.Log("Jump Started - Setting isJumping true");
            animator.SetBool("isJumping", true);
        });
        EventBus.On<JumpEventData>(PlayerActionEventType.OnLanded, data =>
        {
            animator.SetBool("isJumping", false);
            animator.SetTrigger("isFalling");
        });
        EventBus.On<FormChangeData>(FormEventType.OnFormChanged, data =>
        {
            mask1.SetActive(false);
            mask2.SetActive(false);
            mask3.SetActive(false);
            switch (data.ToFormID)
            {
                case 1:
                    mask1.SetActive(true);
                    break;
                case 2:
                    mask2.SetActive(true);
                    break;
                case 3:
                    mask3.SetActive(true);
                    break;
            }
        });

        EventBus.On<int>(PlayerActionEventType.OnDirectionChanged, data =>
        {
            foreach (var particle in ps)
            {
                particle.transform.localScale = new Vector3(data * Mathf.Abs(particle.transform.localScale.x), particle.transform.localScale.y, particle.transform.localScale.z);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
