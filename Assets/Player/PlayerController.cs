using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerBaseState[] states = new PlayerBaseState[(int)PlayerState.Size];

    [SerializeField] Animator[] animator;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer[] render;

    [SerializeField] PlayerModel playerModel;
    [SerializeField] PlayerBattle playerBattle;
    [SerializeField] PlayerRopeAction playerRopeAction;

    [SerializeField] float movePower; [SerializeField] float maxMoveSpeed;
    [SerializeField] float jumpPower; [SerializeField] float maxFallSpeed;

    [SerializeField] bool isJump = true;
    [SerializeField] bool isGrounded;

    Coroutine climbRoutine;
    int groundLayerMask;
    int climbLayer;

    #region 애니메이션
    int curHashBody;
    int curHashArm;

    int idleHashBody = Animator.StringToHash("Idle-Body");

    int runStartHashBody = Animator.StringToHash("RunStart-Body");
    int runEndHashBody = Animator.StringToHash("RunEnd-Body");

    int jumpHashBody = Animator.StringToHash("Jump-Body");

    int fallHashBody = Animator.StringToHash("Fall-Body");


    int climbWallIdleHashBody = Animator.StringToHash("ClimbWallIdle-Body");

    int climbWallHashBody = Animator.StringToHash("ClimbWall-Body");

    int wallSlideHashBody = Animator.StringToHash("WallSlide-Body");

    int hitHashBody = Animator.StringToHash("Hit-Body");
    //=========================================================
    //=========================================================
    int idleHashArm = Animator.StringToHash("Idle-Arm");

    int runStartHashArm = Animator.StringToHash("RunStart-Arm");
    int runEndHashArm = Animator.StringToHash("RunEnd-Arm");

    int jumpHashArm = Animator.StringToHash("Jump-Arm");

    int fallHashArm = Animator.StringToHash("Fall-Arm");


    int climbWallIdleHashArm = Animator.StringToHash("ClimbWallIdle-Arm");

    int climbWallHashArm = Animator.StringToHash("ClimbWall-Arm");

    int wallSlideHashArm = Animator.StringToHash("WallSlide-Arm");

    int hitHashArm = Animator.StringToHash("Hit-Arm");
    #endregion

    private void Awake()
    {
        #region State
        states[(int)PlayerState.Idle] = new IdleState(this);
        states[(int)PlayerState.Run] = new RunState(this);
        states[(int)PlayerState.Jump] = new JumpState(this);
        states[(int)PlayerState.Fall] = new FallState(this);

        states[(int)PlayerState.ClimbWallIdle] = new ClimbWallIdleState(this);
        states[(int)PlayerState.ClimbWall] = new ClimbWallState(this);
        states[(int)PlayerState.WallSlide] = new WallSlideState(this);

        states[(int)PlayerState.Hit] = new HitState(this);
        #endregion

        rigid = GetComponent<Rigidbody2D>();
        playerModel = GetComponent<PlayerModel>();
        playerBattle = GetComponent<PlayerBattle>();
        playerRopeAction = GetComponent<PlayerRopeAction>();

        groundLayerMask = LayerMask.GetMask("Ground");
        climbLayer = LayerMask.GetMask("Climb");

        curHashBody = idleHashBody;
        curHashArm = idleHashArm;

        StartCoroutine(CheckGround());
    }

    private void Start() { states[(int)playerModel.CurState].Enter(); }

    private void Update() { states[(int)playerModel.CurState].Update(); }

    public void ChangeState(PlayerState state)
    {
        states[(int)playerModel.CurState].Exit();
        playerModel.CurState = state;
        states[(int)playerModel.CurState].Enter();
    }

    //====================================================================================================================
    //====================================================================================================================

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer != climbLayer) return;
        if (playerModel.CurState == PlayerState.ClimbWall || playerModel.CurState == PlayerState.ClimbWallIdle) return;

        // 플레이어 오브젝트가 트리거에 닿으면 벽타기 스테이트로 전환
        ChangeState(PlayerState.ClimbWall);
    }

    //====================================================================================================================
    //====================================================================================================================

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (1 << collision.gameObject.layer != climbLayer) return;
        if (isGrounded)
        {
            ChangeState(PlayerState.Run);
        }
        else if (rigid.velocity.y > 0f)
        {
            isJump = false;
            ChangeState(PlayerState.Jump);
        }
        else
        {
            ChangeState(PlayerState.Fall);
        }
    }

    //====================================================================================================================
    //====================================================================================================================

    void Attack()
    {
        ChangeState(PlayerState.Jump);
    }
    void Hit()
    {
        ChangeState(PlayerState.Hit);
    }

    //====================================================================================================================
    //====================================================================================================================

    IEnumerator CheckGround()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);
        while (true)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.4f, groundLayerMask);

            if (hit.collider != null)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
            yield return delay;
        }
    }

    //====================================================================================================================
    //====================================================================================================================

    class IdleState : PlayerBaseState
    {
        public IdleState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.animator[0].Play(player.idleHashBody);
            player.animator[1].Play(player.idleHashArm);

            player.rigid.velocity = new Vector2(0, player.rigid.velocity.y);
        }

        public override void Update()
        {
            // 행동 구현

            // State 전환
            if (player.isGrounded)
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) { player.ChangeState(PlayerState.Run); }
                else if (Input.GetKeyDown(KeyCode.Space)) { player.ChangeState(PlayerState.Jump); }
            }
            else
            {
                if (player.rigid.velocity.y < 0f) { player.ChangeState(PlayerState.Fall); }
            }
        }
    }

    //====================================================================================================================

    class RunState : PlayerBaseState
    {
        public RunState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.animator[0].Play(player.runStartHashBody);
            player.animator[1].Play(player.runStartHashArm);
        }

        public override void Update()
        {
            // 행동 구현
            float x = Input.GetAxisRaw("Horizontal");

            player.rigid.AddForce(Vector2.right * x * player.movePower, ForceMode2D.Force);

            if (player.rigid.velocity.x > player.maxMoveSpeed) { player.rigid.velocity = new Vector2(player.maxMoveSpeed, player.rigid.velocity.y); }
            else if (player.rigid.velocity.x < -player.maxMoveSpeed) { player.rigid.velocity = new Vector2(-player.maxMoveSpeed, player.rigid.velocity.y); }

            if (player.rigid.velocity.y < -player.maxFallSpeed) { player.rigid.velocity = new Vector2(player.rigid.velocity.x, -player.maxFallSpeed); }

            if (x < 0)
            {
                player.render[0].flipX = true;
                player.render[1].flipX = true;
                player.render[1].transform.position = new Vector3(player.transform.position.x + 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
            }
            else if (x > 0)
            {
                player.render[0].flipX = false;
                player.render[1].flipX = false;
                player.render[1].transform.position = new Vector3(player.transform.position.x - 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
            }

            // State 전환
            if (player.isGrounded)
            {
                if (x == 0)
                { player.ChangeState(PlayerState.Idle); }
                else if (Input.GetKeyDown(KeyCode.Space)) { player.ChangeState(PlayerState.Jump); }
            }
            else
            {
                if (player.rigid.velocity.y < 0f) { player.ChangeState(PlayerState.Fall); }
            }
        }
    }

    //====================================================================================================================

    class JumpState : PlayerBaseState
    {
        public JumpState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.animator[0].Play(player.jumpHashBody);
            player.animator[1].Play(player.jumpHashArm);

            // 행동 구현
            if (!player.isJump) { player.isJump = true; return; }
            player.rigid.velocity = new Vector2(player.rigid.velocity.x, 0);
            player.rigid.AddForce(Vector2.up * player.jumpPower, ForceMode2D.Impulse);
        }

        public override void Update()
        {
            float x = Input.GetAxisRaw("Horizontal");
            if (x > 0)
            {
                player.render[0].flipX = false;
                player.render[1].flipX = false;
                player.render[1].transform.position = new Vector3(player.transform.position.x - 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
                player.rigid.velocity = new Vector2(player.movePower, player.rigid.velocity.y);
            }
            else if (x < 0)
            {
                player.render[0].flipX = true;
                player.render[1].flipX = true;
                player.render[1].transform.position = new Vector3(player.transform.position.x + 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
                player.rigid.velocity = new Vector2(-player.movePower, player.rigid.velocity.y);
            }

            // State 전환
            if (player.rigid.velocity.y < 0f) { player.ChangeState(PlayerState.Fall); }
        }
    }

    //====================================================================================================================

    class FallState : PlayerBaseState
    {
        public FallState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.animator[0].Play(player.fallHashBody);
            player.animator[1].Play(player.fallHashArm);
        }

        public override void Update()
        {
            // 행동 구현
            float x = Input.GetAxisRaw("Horizontal");
            if (x > 0)
            {
                player.render[0].flipX = false;
                player.render[1].flipX = false;
                player.render[1].transform.position = new Vector3(player.transform.position.x - 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
                player.rigid.velocity = new Vector2(player.movePower, player.rigid.velocity.y);
            }
            else if (x < 0)
            {
                player.render[0].flipX = true;
                player.render[1].flipX = true;
                player.render[1].transform.position = new Vector3(player.transform.position.x + 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
                player.rigid.velocity = new Vector2(-player.movePower, player.rigid.velocity.y);
            }

            // State 전환
            if (player.isGrounded) { player.ChangeState(PlayerState.Idle); }
        }
    }

    class ClimbWallIdleState : PlayerBaseState
    {
        public ClimbWallIdleState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.animator[0].Play(player.climbWallIdleHashBody);
            player.animator[1].Play(player.climbWallIdleHashArm);

            player.rigid.gravityScale = 0;
            player.rigid.velocity = Vector2.zero;
        }

        public override void Update()
        {
            // 행동 구현

            // State 전환
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            { player.ChangeState(PlayerState.ClimbWall); }
            else if (Input.GetKeyDown(KeyCode.Space)) { player.ChangeState(PlayerState.Jump); }
        }

        public override void Exit() { player.rigid.gravityScale = 1; }
    }

    //====================================================================================================================

    class ClimbWallState : PlayerBaseState
    {
        public ClimbWallState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.animator[0].Play(player.climbWallHashBody);
            player.animator[1].Play(player.climbWallHashArm);

            player.rigid.gravityScale = 0;
        }

        public override void Update()
        {
            // 행동 구현
            float x = Input.GetAxisRaw("Horizontal");
            if (x > 0)
            {
                player.render[0].flipX = false;
                player.render[1].flipX = false;
                player.render[1].transform.position = new Vector3(player.transform.position.x - 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
                player.rigid.velocity = new Vector2(player.movePower, player.rigid.velocity.y);
            }
            else if (x < 0)
            {
                player.render[0].flipX = true;
                player.render[1].flipX = true;
                player.render[1].transform.position = new Vector3(player.transform.position.x + 0.4f, player.transform.position.y + 1.2f, player.transform.position.z);
                player.rigid.velocity = new Vector2(-player.movePower, player.rigid.velocity.y);
            }
            else { player.rigid.velocity = new Vector2(0, player.rigid.velocity.y); }


            float y = Input.GetAxisRaw("Vertical");
            if (y > 0)
            {
                player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.movePower);
                if (player.rigid.velocity.y > player.movePower) { player.rigid.velocity = new Vector2(player.rigid.velocity.x, player.movePower); }
            }
            else if (y < 0)
            {
                player.rigid.velocity = new Vector2(player.rigid.velocity.x, -player.movePower);
                if (player.rigid.velocity.y < -player.movePower) { player.rigid.velocity = new Vector2(player.rigid.velocity.x, -player.movePower); }
            }
            else
            { player.rigid.velocity = new Vector2(player.rigid.velocity.x, 0); }


            // State 전환
            if (y == 0 && x == 0) { player.ChangeState(PlayerState.ClimbWallIdle); }
            else if (Input.GetKeyDown(KeyCode.Space))
            { player.ChangeState(PlayerState.Jump); }
        }

        public override void Exit() { player.rigid.gravityScale = 1; }
    }

    //====================================================================================================================

    class WallSlideState : PlayerBaseState
    {
        public WallSlideState(PlayerController player) : base(player) { }

        public override void Enter() { }

        public override void Update()
        {
            // 행동 구현

            // State 전환
        }

        public override void Exit() { }
    }

    class HitState : PlayerBaseState
    {
        public HitState(PlayerController player) : base(player) { }

        public override void Enter()
        {
            player.animator[0].Play(player.hitHashBody);
            player.animator[2].Play(player.hitHashArm);

            //player.StartCoroutine(player.HitDelayRoutine());
            player.rigid.velocity = Vector2.zero;
        }

        public override void Exit() { }
    }

    //====================================================================================================================
    //====================================================================================================================
}

