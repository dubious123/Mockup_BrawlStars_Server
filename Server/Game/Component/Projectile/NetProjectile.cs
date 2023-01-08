using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

using Server.Game;
using Server.Logs;

using static Enums;

public class NetProjectile : NetBaseComponent, INetUpdatable
{
	private sVector3 _moveDir;
	private sfloat _speed;

	private int _maxTravelTime;
	private int _currentTravelTime;

	public override void Start()
	{
		Active = false;
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

	public void Reset()
	{
		_currentTravelTime = 0;
		Active = false;
	}

	public void Update()
	{
		if (Active == false)
		{
			return;
		}

		if (_currentTravelTime > _maxTravelTime)
		{
			Active = false;
			return;
		}

		NetObj.Position += NetObj.Rotation * (_speed * _moveDir * Define.FixedDeltaTime);
		Loggers.Debug.Information("{0}", NetObj.Position);
		//Owner.World.FindAllAndBroadcast(target =>
		//{
		//	if (target == Owner)
		//	{
		//		return false;
		//	}

		//	if (target.Type == NetObjectType.Character || target.Type == NetObjectType.Wall)
		//	{
		//		return Collider.CheckCollision((target as INetCollidable2D).Collider);
		//	}

		//	return false;
		//}
		//, target =>
		//{
		//	if (World.GameRule.CanSendHit(Owner, target))
		//	{
		//		Owner.SendHit(target as ITakeHit, _hitInfo);
		//	}

		//	Disable();
		//});
		++_currentTravelTime;
	}
}
