namespace Server.Game
{
	public abstract class NetBaseComponent
	{
		public NetObject NetObj { get; init; }
		public NetObjectId NetObjId => NetObj.ObjectId;
		public NetWorld World => NetObj.World;
		public NetObjectBuilder ObjectBuilder => NetObj.World.ObjectBuilder;
		public sVector3 Position { get => NetObj.Position; set => NetObj.Position = value; }
		public sQuaternion Rotation { get => NetObj.Rotation; set => NetObj.Rotation = value; }

		public bool Active
		{
			get => _active;
			set
			{
				if (_active is false && value is true)
				{
					OnAwake();
				}

				_active = value;
			}
		}

		private bool _active = true;

		public virtual void Start() { }

		public virtual void OnAwake() { }

		public override int GetHashCode() => NetObjId.GetHashCode();
	}
}
