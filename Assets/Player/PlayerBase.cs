
public enum PlayerState
  {
    Idle, Run, Jump, Fall, Land,
    ClimbWallIdle, ClimbWall, WallSlide,
    Hit,
    Size
  }

public abstract class PlayerBase
{
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}

class PlayerBaseState : PlayerBase
{
    protected PlayerController player;

    public PlayerBaseState(PlayerController player)
    {
        this.player = player;
    }
}
