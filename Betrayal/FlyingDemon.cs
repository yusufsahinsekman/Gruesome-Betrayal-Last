using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Betrayal;

public enum DemonState 
{ 
    Idle, Fly, Attack 
}

public class FlyingDemon
{
    private Texture2D _idleSheet, _flySheet, _attackSheet;
    public Vector2 Position;
    public DemonState State = DemonState.Idle;

    private int _currentFrame = 0;
    private float _animTimer = 0f;
    private const float FrameTime = 0.1f;

    private const int FrameWidth = 81;
    private const int FrameHeight = 71;
    private const float Speed = 5f;
    private int _direction = -1; 
    private SpriteEffects _facing = SpriteEffects.None;

    private float _patrolLeft = 100f;
    private float _patrolRight = 700f;

    private float _attackCooldown = 1.0f; 
    private float _attackTimer = 1.0f;
    private bool _hasFiredThisAttack = false;

    public FlyingDemon(Texture2D idle, Texture2D fly, Texture2D attack, Vector2 startPos)
    {
        _idleSheet = idle;
        _flySheet = fly;
        _attackSheet = attack;
        Position = startPos;
    }

    public void Update(GameTime gameTime, bool playerHasMoved, List<Fireball> activeFireballs, Texture2D fireballTexture)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        switch (State)
        {
            case DemonState.Idle:
                if (playerHasMoved) 
                {
                    State = DemonState.Fly;
                    _currentFrame = 0;
                }
                break;

            case DemonState.Fly:
                Position.X += Speed * _direction;
                _facing = _direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                if (Position.X <= _patrolLeft) _direction = 1;
                if (Position.X >= _patrolRight) _direction = -1;

                _attackTimer -= dt;
                if (_attackTimer <= 0)
                {
                    State = DemonState.Attack;
                    _currentFrame = 0;
                    _hasFiredThisAttack = false;
                }
                break;

            case DemonState.Attack:
                if (_currentFrame == 4 && !_hasFiredThisAttack)
                {
                    Vector2 spawnPos = new Vector2(Position.X + (FrameWidth / 2f), Position.Y + FrameHeight);
                    activeFireballs.Add(new Fireball(fireballTexture, spawnPos));
                    _hasFiredThisAttack = true;
                }
                break;
        }

        AdvanceAnimation(dt);
    }

    private void AdvanceAnimation(float dt)
    {
        _animTimer += dt;
        if (_animTimer >= FrameTime)
        {
            _animTimer = 0f;
            _currentFrame++;

            int frameCount = State == DemonState.Attack ? 8 : 4; 

            if (_currentFrame >= frameCount)
            {
                if (State == DemonState.Attack)
                {
                    State = DemonState.Fly;
                    _attackTimer = _attackCooldown;
                }
                _currentFrame = 0;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D currentSheet = State switch
        {
            DemonState.Fly => _flySheet,
            DemonState.Attack => _attackSheet,
            _ => _idleSheet
        };

        Rectangle sourceRect = new Rectangle(_currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
        spriteBatch.Draw(currentSheet, Position, sourceRect, Color.White, 0f, Vector2.Zero, 1f, _facing, 0f);
    }
}