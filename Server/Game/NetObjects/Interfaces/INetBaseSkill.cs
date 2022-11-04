namespace Server.Game
{
    public interface INetBaseSkill : INetUpdatable
    {
        public int Id { get; init; }
        public NetCharacter Character { get; }
        public bool Performing { get; set; }
        public bool Active { get; set; }
        public void HandleInput(in InputData input);
        public IEnumerator<int> Co_Perform();
    }
}
