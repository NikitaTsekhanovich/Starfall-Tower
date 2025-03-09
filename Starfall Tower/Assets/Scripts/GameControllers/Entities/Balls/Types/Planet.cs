namespace GameControllers.Entities.Balls.Types
{
    public class Planet : Ball
    {
        protected override void SpawnInit()
        {
            AlreadyThrow();
        }
    }
}
