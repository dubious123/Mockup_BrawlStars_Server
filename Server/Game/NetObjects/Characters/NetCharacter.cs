using static Enums;

namespace Server.Game
{
    public abstract class NetCharacter : INetCollidable2D, INetUpdatable, ISendHit, ITakeHit
    {
        public NetWorld World { get; init; }
        public sVector3 Position { get; set; }
        public sVector3 Forward => Rotation * sVector3.forward;
        public sQuaternion Rotation { get; set; }
        public NetObjectTag Tag { get; set; }
        public INetCollider2D Collider { get; init; }
        public sfloat MoveSpeed { get; set; }
        public sfloat LookSpeed { get; set; }
        //public bool BasicAttackBtnPressed { get; set; }
        //public bool SkillBtnPressed { get; set; }
        public bool CanControlMove { get; set; }
        public bool CanControlLook { get; set; }

        public sVector3 TargetMoveDir { get; set; }
        public sfloat MoveSmoothTime { get; set; }
        public sVector3 TargetLookDir { get; set; }

        public int MaxHp { get; protected set; }
        public int Hp { get; protected set; }

        private sVector3 _smoothVelocity;

        public NetCharacter(sVector3 position, sQuaternion rotation, NetObjectTag tag, NetWorld world)
        {
            Collider = new NetCircleCollider2D(this, sVector2.zero, (sfloat)0.5f);
            Position = position;
            Rotation = rotation;
            Tag = tag;
            World = world;
            MoveSpeed = (sfloat)6f;
            LookSpeed = (sfloat)360f;
            MoveSmoothTime = (sfloat)0.01f;
            CanControlMove = true;
            CanControlLook = true;
        }

        public virtual void Update()
        {
            if (CanControlMove && TargetMoveDir != sVector3.zero)
            {
                Move(MoveSpeed * Define.FixedDeltaTime * TargetMoveDir);
            }

            if (CanControlLook && TargetLookDir != sVector3.zero)
            {
                var targetRotation = sQuaternion.LookRotation(TargetLookDir, sVector3.up);
                Rotation = sQuaternion.RotateTowards(Rotation, targetRotation, Define.FixedDeltaTime * LookSpeed);
            }
        }

        public virtual void Move(sVector3 deltaPos)
        {
            var beforePos = Position;
            Position += deltaPos;
            if (CheckCollision(NetObjectTag.Wall))
            {
                Position = beforePos;
            }
        }

        public virtual void UpdateInput(in InputData input)
        {
            TargetMoveDir = sVector3.SmoothDamp(TargetMoveDir, input.MoveInput, ref _smoothVelocity, MoveSmoothTime, sfloat.PositiveInfinity, Define.FixedDeltaTime);
            TargetLookDir = input.LookInput;
        }

        public virtual bool CheckCollision(NetObjectTag tag)
        {
            return World.Physics2D.DetectCollision(Collider, tag);
        }

        public virtual void GetCollisions(NetObjectTag tag, List<INetCollider2D> collisions)
        {
            World.Physics2D.GetCollisions(Collider, tag, collisions);
        }

        public virtual void SendHit(ITakeHit target, in HitInfo info)
        {
            if (info.Damage > 0)
            {
                target.TakeMeleeDamage(info.Damage);
            }
        }

        public virtual bool CanBeHit()
        {
            return IsDead() == false;
        }

        public virtual void TakeMeleeDamage(int damage)
        {
            Hp -= damage;

            if (IsDead())
            {
                OnDead();
            }
        }

        public virtual bool IsDead()
        {
            return Hp <= 0;
        }

        public virtual void OnDead()
        {
            CanControlMove = false;
            CanControlLook = false;
        }
    }
}
