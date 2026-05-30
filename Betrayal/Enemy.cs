using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Betrayal
{
    public enum EnemyState
    {
        PatrolWalk, PatrolIdle, React, Chase, Attack, Hurt, Dead  
    }

    public class Enemy
    {
        private Texture2D _idleSheet, _walkSheet, _reactSheet, _attackSheet, _hurtSheet, _deadSheet;

        public int Health = 3; 
        private bool _hurtFinished = false;
        private float _iFrameTimer = 0f;

        public float PatrolDistance = 80f;
        public float DetectionRange = 250f;
        public Color Tint = Color.White;

        public Vector2 Position;
        private float CenterX => Position.X + (WalkFrameWidth / 2f);

        public Rectangle HitBox => new Rectangle((int)Position.X + 11, (int)Position.Y + 4, 22, 33);

        public Rectangle? WeaponHitBox
        {
            get
            {
                if (_state == EnemyState.Attack && (_currentFrame >= 7 && _currentFrame <= 9))
                {
                    int direction = _facing == SpriteEffects.None ? 1 : -1;
                    return new Rectangle((int)CenterX + (10 * direction) - 20, (int)Position.Y, 40, AttackFrameHeight);
                }
                return null;
            }
        }

        private const float PatrolSpeed = 1.5f;
        private const float ChaseSpeed = 2.8f;
        private EnemyState _state;
        private SpriteEffects _facing = SpriteEffects.None;

        private float _spawnX;
        private const float PatrolIdleDuration = 1.5f;
        private float _patrolIdleTimer = 0f;
        private int _patrolDirection = 1;

        private const float AttackRange = 30f;

        private int _currentFrame = 0;
        private float _animTimer = 0f;
        private const float FrameTime = 0.1f;
        private bool _reactFinished = false;

        private const int WalkFrameWidth = 22, WalkFrameHeight = 33, WalkFrameCount = 13;
        private const int IdleFrameWidth = 24, IdleFrameHeight = 32, IdleFrameCount = 11;
        private const int ReactFrameWidth = 22, ReactFrameHeight = 32, ReactFrameCount = 4;
        private const int AttackFrameWidth = 43, AttackFrameHeight = 37, AttackFrameCount = 18;
        private const int HurtFrameWidth = 30, HurtFrameHeight = 32, HurtFrameCount = 8;
        private const int DeadFrameWidth = 33, DeadFrameHeight = 32, DeadFrameCount = 15;

        public Enemy(Texture2D idle, Texture2D walk, Texture2D react, Texture2D attack, Texture2D hurt, Texture2D dead, Vector2 startPos)
        {
            _idleSheet = idle; _walkSheet = walk; _reactSheet = react; _attackSheet = attack; _hurtSheet = hurt; _deadSheet = dead;
            Position = startPos;
            _spawnX = startPos.X;
            _state = EnemyState.PatrolWalk;
        }

        public void TakeDamage(int damage, float playerX)
        {
            if (_state == EnemyState.Dead || _iFrameTimer > 0) return;

            Health -= damage;
            _iFrameTimer = 0.4f; 
            
            _facing = playerX >= CenterX ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (Health <= 0)
            {
                _state = EnemyState.Dead;
                _currentFrame = 0;
            }
            else
            {
                _state = EnemyState.Hurt;
                _currentFrame = 0;
                _hurtFinished = false;
            }
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_iFrameTimer > 0) _iFrameTimer -= dt;

            float distToPlayer = Vector2.Distance(Position, playerPosition);
            float horizDistToPlayer = Math.Abs(playerPosition.X - CenterX);
            
        
            float vertDistToPlayer = Math.Abs(playerPosition.Y - Position.Y);

            switch (_state)
            {
                case EnemyState.PatrolWalk: HandlePatrolWalk(dt, distToPlayer, vertDistToPlayer, playerPosition); break;
                case EnemyState.PatrolIdle: HandlePatrolIdle(dt, distToPlayer, vertDistToPlayer, playerPosition); break;
                case EnemyState.React:      HandleReact(playerPosition); break;
                case EnemyState.Chase:      HandleChase(horizDistToPlayer, vertDistToPlayer, playerPosition); break;
                case EnemyState.Attack:     HandleAttack(horizDistToPlayer, vertDistToPlayer, playerPosition); break;
                case EnemyState.Hurt:
                    if (_hurtFinished) { _state = EnemyState.Chase; _currentFrame = 0; }
                    break;
                case EnemyState.Dead: break;
            }

            AdvanceAnimation(dt);
        }

        private void HandlePatrolWalk(float dt, float distToPlayer, float vertDistToPlayer, Vector2 playerPosition)
        {
            // only react if they are close AND on the same floor
            if (distToPlayer <= DetectionRange && vertDistToPlayer < 30f) { BeginReact(playerPosition); return; }
            
            Position.X += PatrolSpeed * _patrolDirection;
            _facing = _patrolDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (Math.Abs(Position.X - _spawnX) >= PatrolDistance)
            {
                _state = EnemyState.PatrolIdle;
                _patrolIdleTimer = PatrolIdleDuration;
                _currentFrame = 0;
            }
        }

        private void HandlePatrolIdle(float dt, float distToPlayer, float vertDistToPlayer, Vector2 playerPosition)
        {
            if (distToPlayer <= DetectionRange && vertDistToPlayer < 30f) { BeginReact(playerPosition); return; }
            
            _patrolIdleTimer -= dt;
            if (_patrolIdleTimer <= 0f)
            {
                _patrolDirection *= -1; 
                _state = EnemyState.PatrolWalk;
                _currentFrame = 0;
            }
        }

        private void BeginReact(Vector2 playerPosition)
        {
            _state = EnemyState.React;
            _currentFrame = 0;
            _reactFinished = false;
            _facing = playerPosition.X >= CenterX ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void HandleReact(Vector2 playerPosition) { if (_reactFinished) { _state = EnemyState.Chase; _currentFrame = 0; } }

        private void HandleChase(float horizDist, float vertDistToPlayer, Vector2 playerPosition)
        {
            
            if (vertDistToPlayer >= 30f)
            {
                _state = EnemyState.PatrolWalk;
                _currentFrame = 0;
                return;
            }

            float dirX = playerPosition.X > CenterX ? 1f : -1f;
            _facing = dirX > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (horizDist <= AttackRange) { _state = EnemyState.Attack; _currentFrame = 0; return; }
            Position.X += dirX * ChaseSpeed;

            // force him to stay strictly on his platform so he never falls off
            if (Math.Abs(Position.X - _spawnX) > PatrolDistance + 100f)
            {
                Position.X = _spawnX + ((PatrolDistance + 100f) * dirX);
            }
        }

        private void HandleAttack(float horizDist, float vertDistToPlayer, Vector2 playerPosition)
        {
            if (vertDistToPlayer >= 30f)
            {
                _state = EnemyState.PatrolWalk;
                _currentFrame = 0;
                return;
            }

            float dirX = playerPosition.X > CenterX ? 1f : -1f;
            _facing = dirX > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (horizDist > AttackRange * 1.8f) { _state = EnemyState.Chase; _currentFrame = 0; }
        }

        private void AdvanceAnimation(float dt)
        {
            _animTimer += dt;
            if (_animTimer < FrameTime) return;

            _animTimer = 0f;
            _currentFrame++;
            int total = GetFrameCount();

            if (_currentFrame >= total)
            {
                if (_state == EnemyState.React) { _currentFrame = total - 1; _reactFinished = true; }
                else if (_state == EnemyState.Hurt) { _currentFrame = total - 1; _hurtFinished = true; }
                else if (_state == EnemyState.Dead) { _currentFrame = total - 1; }
                else { _currentFrame = 0; }
            }
        }

        private int GetFrameCount()
        {
            return _state switch {
                EnemyState.PatrolWalk => WalkFrameCount, EnemyState.Chase => WalkFrameCount, EnemyState.PatrolIdle => IdleFrameCount,
                EnemyState.React => ReactFrameCount, EnemyState.Attack => AttackFrameCount, EnemyState.Hurt => HurtFrameCount,
                EnemyState.Dead => DeadFrameCount, _ => IdleFrameCount
            };
        }

        private (Texture2D sheet, int frameW, int frameH) GetSheetInfo()
        {
            return _state switch {
                EnemyState.PatrolWalk => (_walkSheet, WalkFrameWidth, WalkFrameHeight), EnemyState.Chase => (_walkSheet, WalkFrameWidth, WalkFrameHeight),
                EnemyState.PatrolIdle => (_idleSheet, IdleFrameWidth, IdleFrameHeight), EnemyState.React => (_reactSheet, ReactFrameWidth, ReactFrameHeight),
                EnemyState.Attack => (_attackSheet, AttackFrameWidth, AttackFrameHeight), EnemyState.Hurt => (_hurtSheet, HurtFrameWidth, HurtFrameHeight),
                EnemyState.Dead => (_deadSheet, DeadFrameWidth, DeadFrameHeight), _ => (_idleSheet, IdleFrameWidth, IdleFrameHeight)
            };
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var (sheet, frameW, frameH) = GetSheetInfo();
            Rectangle sourceRect = new Rectangle(_currentFrame * frameW, 0, frameW, frameH);

            float bodyCenterOffset = WalkFrameWidth / 2f;
            float originX = (_facing == SpriteEffects.None) ? bodyCenterOffset : frameW - bodyCenterOffset;

            Vector2 origin = new Vector2(originX, frameH);
            float anchorX = Position.X + bodyCenterOffset;
            float anchorY = Position.Y + AttackFrameHeight;

            spriteBatch.Draw(sheet, new Vector2((int)anchorX, (int)anchorY), sourceRect, Tint, 0f, origin, 1f, _facing, 0f);
        }
    }
}