using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] PlayerState curState;
    public PlayerState CurState { get { return curState; } set { curState = value; } }


    [SerializeField] int maxHp;
    public int MaxHp { get { return maxHp; } }


    [SerializeField] int hp;
    public int Hp { get { return hp; } set { hp = value; } }


    [SerializeField] int damage;
    public int Damage { get { return damage; } }


    private void Awake()
    {
        hp = maxHp;
    }
}
