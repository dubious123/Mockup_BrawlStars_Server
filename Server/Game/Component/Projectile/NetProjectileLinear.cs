namespace Server.Game
{
	public class NetProjectileLinear : NetProjectile
	{
		public NetCollider2D Collider { get; private set; }
		public NetObject Owner { get; private set; }
		public Action<NetProjectile> OnReachedMaxRadius { get; set; }
#if CLIENT
	public sVector3 MoveDir => _moveDir;
	public int CurrentTravelTime => _currentTravelTime;
	public int MaxTravelTime => _maxTravelTime;
#endif

		private sVector3 _moveDir;
		private sfloat _speed, _maxDistance;
		private int _currentTravelTime, _maxTravelTime;

		public override void Start()
		{
			NetObj.Active = false;
			Collider = this.GetComponent<NetCollider2D>();
		}

		public override void OnAwake()
		{
			_maxTravelTime = (int)(_maxDistance / _speed * 60);
		}

		public NetProjectile SetSpeed(sfloat speed)
		{
			_speed = speed;
			return this;
		}

		public NetProjectile SetMaxDistance(sfloat distance)
		{
			_maxDistance = distance;
			return this;
		}

		public NetProjectile SetAngle(sfloat angle)
		{
			_moveDir = new sVector3(sMathf.Cos(angle), sfloat.Zero, sMathf.Sin(angle));
			return this;
		}

		public NetProjectile SetOwner(NetObject owner)
		{
			Owner = owner;
			return this;
		}

		public override void Reset()
		{
			_currentTravelTime = 0;
			OnReachedMaxRadius = default;
			NetObj.Active = false;
		}

		public vioverridertual private void Update()
		{
			if (Active == false)
			{
				return;
			}

			if (_currentTravelTime > _maxTravelTime)
			{
				OnReachedMaxRadius?.Invoke(this);
				NetObj.Active = false;
				_currentTravelTime = 0;
				World.ProjectileSystem.Return(this);
				return;
			}

			NetObj.Position += NetObj.Rotation * (_speed * _moveDir * Define.FixedDeltaTime);
			++_currentTravelTime;
		}
	}
}
