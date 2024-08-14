using static System.Activator;

namespace RPG2D
{
    public abstract class PlayerCommand: ICommand
    {
        protected readonly PlayerController player;

        protected PlayerCommand(PlayerController player)
        {
            this.player = player;
        }

        public abstract void Execute();
        
        public static T Create<T>(PlayerController player) where T : PlayerCommand
        {
            return (T) CreateInstance(typeof(T), player);
        }
    }
    public class AttackCommand : PlayerCommand
    {
        public AttackCommand(PlayerController player) : base(player) { }

        public override void Execute()
        {
            player.Attack();
        }
    }
    public class JumpCommand : PlayerCommand
    {
        public JumpCommand(PlayerController player) : base(player) { }

        public override void Execute()
        {
            player.Jump();
        }
    }
    public class DashCommand : PlayerCommand
    {
        public DashCommand(PlayerController player) : base(player) { }
        public override void Execute()
        {
            player.Dash();
        }
    }
}
