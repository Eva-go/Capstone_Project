using UnityEngine;
using Photon.Pun;

public class NodeController : MonoBehaviourPun
{
    // ��ü�� �ִ� ü��
    public int maxHealth = 100;

    // ���� ü��
    [SerializeField]
    private int currentHealth;

    public Animator animator;

    // �ʱ� ����
    void Start()
    {
        currentHealth = maxHealth;
    }

    // ü���� ���ҽ�Ű�� �޼���
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
        }

        if (currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("Destroy");
            }
            else
            {
                Die(); // �ִϸ����Ͱ� ���� ��� ��� ����
            }
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ���ϴ� �޼���
    public void Die()
    {
        Debug.Log(gameObject.name + "��(��) �ı��Ǿ����ϴ�!");
        // ��ü�� ��Ȱ��ȭ�ϰų� ������ �� �ֽ��ϴ�.
        gameObject.SetActive(false);
    }
}