using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class NodeController : MonoBehaviourPun
{
    // 물체의 최대 체력
    public int maxHealth = 100;

    // 현재 체력
    [SerializeField]
    private int currentHealth;

    public Animator animator;

    public Image image;

    // 초기 설정
    void Start()
    {
        currentHealth = maxHealth;
    }

    // 체력을 감소시키는 메서드
    public void TakeDamage(int amount)
    {
        if (!photonView.IsMine)
            return;
        Debug.Log(amount);
        photonView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, amount);
    }

    [PunRPC]
    void RPC_TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(currentHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hit");
            Debug.Log(currentHealth);
        }

        if (currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("Destroy");
            }
            else
            {
                Die(); // 애니메이터가 없는 경우 즉시 제거
            }
        }
    }

    // 애니메이션 이벤트가 호출하는 메서드
    public void Die()
    {
        Debug.Log(gameObject.name + "이(가) 파괴되었습니다!");
        // 물체를 비활성화하거나 제거할 수 있습니다.
        gameObject.SetActive(false);
    }
}