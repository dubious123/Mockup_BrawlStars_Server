public class NetProjectile : NetBaseComponent, INetUpdatable
{
	public NetCollider2D Collider { get; private set; }
	public NetObject Owner { get; private set; }
	private sVector3 _moveDir;
	private sfloat _speed;

	private int _maxTravelTime;
	private int _currentTravelTime;

	public override void Start()
	{
		NetObj.Active = false;
		Collider = this.GetComponent<NetCollider2D>();
	}

	public NetProjectile SetSpeed(sfloat speed)
	{
		_speed = speed;
		return this;
	}

	public NetProjectile SetMaxTravelTime(int time)
	{
		_maxTravelTime = time;
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

	public void Reset()
	{
		_currentTravelTime = 0;
		NetObj.Active = false;
	}

	public void Update()
	{
		if (Active == false)
		{
			return;
		}

		if (_currentTravelTime > _maxTravelTime)
		{
			NetObj.Active = false;
			_currentTravelTime = 0;
			World.ProjectileSystem.Return(this);
			return;
		}

		NetObj.Position += NetObj.Rotation * (_speed * _moveDir * Define.FixedDeltaTime);
		++_currentTravelTime;
	}
}
