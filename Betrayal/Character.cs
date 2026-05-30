using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Betrayal;

// the different states our player can be in
public enum CharacterState
{
    Stopping, Walking, Jumping
}

public enum CharacterStateAnim
{
    Idle, Run, Jump, JumpApex, Fall,
    Attack, Hit, Dead,
    Dash, Slide, Crouch, CrouchWalk, CrouchAttack, TurnAround
}

public class Character
{
    private Texture2D _idleSheet, _runSheet, _jumpSheet, _jumpApexSheet, _fallSheet;
    private Texture2D _attackSheet, _hitSheet, _deadSheet;
    private Texture2D _dashSheet, _slideSheet, _crouchSheet, _crouchWalkSheet, _crouchAttackSheet, _turnAroundSheet;

    public Vector2 position;
    public Vector2 speed;
    public CharacterStateAnim state = CharacterStateAnim.Idle;
    public CharacterState state2 = CharacterState.Stopping;
    public SpriteEffects lookingForward = SpriteEffects.None;
    public bool IsGrounded = false;
    public bool canDoubleJump = false;
    public bool IsCeilingAbove = false;

    public Texture2D SpriteSheet;

    public int frameWeight = 120;  
    public int frameHeight = 80; 
    public int jumpTime = 0;
    private int _currentFrame = 0;
    private float _timer = 0f;
    
    // player stats and combat trackers
    public int Health = 5;
    private int _comboStep = 0;
    private bool _continueCombo = false;
    private MouseState _previousMouse;
    private KeyboardState _previousKeys;

    // timers for moves and invincibility
    public float iFrameTimer = 0f; 
    private float _hitStunTimer = 0f;
    private float _dashTimer = 0f;
    private float _slideTimer = 0f;
    private const float FrameTime = 0.1f;

    // exact dimensions of all our character sprites
    public const int FrameWidth = 120;
    public const int FrameHeight = 80;
    
    private const int IdleFrameCount = 10, RunFrameCount = 10;
    private const int JumpFrameCount = 3, JumpApexFrameCount = 2, FallFrameCount = 3;
    private const int AttackFrameCount = 10, HitFrameCount = 1, DeadFrameCount = 10; 
    private const int DashFrameCount = 2, SlideFrameCount = 4, CrouchFrameCount = 3;         
    private const int CrouchWalkFrameCount = 8, CrouchAttackFrameCount = 4, TurnAroundFrameCount = 3; 

    // checks if we are doing any crouch moves so we can shrink the hitbox
    public bool IsCrouching => state == CharacterStateAnim.Crouch || state == CharacterStateAnim.CrouchWalk || state == CharacterStateAnim.CrouchAttack || state == CharacterStateAnim.Slide;
    
    // the player's physical body hitbox
    public Rectangle HitBox 
    {
        get 
        { 
            // cuts the hitbox in half and pushes it to the floor when crouching
            int emptySpaceAtTop = 30; 
            int height = IsCrouching ? FrameHeight / 2 : (FrameHeight - emptySpaceAtTop); 
            int yOffset = IsCrouching ? FrameHeight / 2 : emptySpaceAtTop;          
            return new Rectangle((int)position.X + (FrameWidth / 2) - 13, (int)position.Y + yOffset, 26, height); 
        }
    }

    // creates a hitbox for our sword only on the exact frames we swing it
    public Rectangle? WeaponHitBox
    {
        get
        {
            int direction = lookingForward == SpriteEffects.None ? 1 : -1;
            
            // standing attack hitbox
            if (state == CharacterStateAnim.Attack && (_currentFrame == 2 || _currentFrame == 7))
                return new Rectangle((int)position.X + (FrameWidth / 2) + (10 * direction), (int)position.Y, 40, FrameHeight);
            
            // crouch attack hitbox (lower to the ground)
            if (state == CharacterStateAnim.CrouchAttack && _currentFrame == 2)
                return new Rectangle((int)position.X + (FrameWidth / 2) + (10 * direction), (int)position.Y + (FrameHeight / 2), 40, FrameHeight / 2);

            return null;
        }
    }

    public Character(Texture2D idle, Texture2D run, Texture2D jump, Texture2D jumpApex, Texture2D fall, 
                     Texture2D attack, Texture2D hit, Texture2D dead,
                     Texture2D dash, Texture2D slide, Texture2D crouch, Texture2D crouchWalk, Texture2D crouchAttack, Texture2D turnAround, 
                     Vector2 startPos)
    {
        _idleSheet = idle; _runSheet = run; _jumpSheet = jump; _jumpApexSheet = jumpApex; _fallSheet = fall;
        _attackSheet = attack; _hitSheet = hit; _deadSheet = dead;
        _dashSheet = dash; _slideSheet = slide; _crouchSheet = crouch; _crouchWalkSheet = crouchWalk; _crouchAttackSheet = crouchAttack; _turnAroundSheet = turnAround;
        position = startPos;
    }

    public void Update(GameTime gameTime)
    {
        // stop doing things if we are dead
        if (state == CharacterStateAnim.Dead) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // count down the iframes so we aren't invincible forever
        if (iFrameTimer > 0) iFrameTimer -= dt;

        if (IsGrounded) canDoubleJump = true;

        KeyboardState keys = Keyboard.GetState();
        MouseState mouse = Mouse.GetState();

        // if we get hit, let gravity work but ignore player inputs
        if (state == CharacterStateAnim.Hit)
        {
            speed.Y += 0.5f; 
            position += speed;
            _previousMouse = mouse;
            _previousKeys = keys;
            return; 
        }

        // these are timed moves
        if (state == CharacterStateAnim.Dash)
        {
            _dashTimer -= dt;
            speed.X = lookingForward == SpriteEffects.None ? 4.5f : -4.5f; 
            speed.Y = 0; 
            if (_dashTimer <= 0) SetState(CharacterStateAnim.Idle);
            position += speed;
            return;
        }
        else if (state == CharacterStateAnim.Slide)
        {
            _slideTimer -= dt;
            speed.X = lookingForward == SpriteEffects.None ? 4.5f : -4.5f; 
            speed.Y += 0.5f;
            if (_slideTimer <= 0) SetState(CharacterStateAnim.Crouch); 
            position += speed;
            return;
        }

        speed.X = 0; 

        // this is the combat input
        if (mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released && IsGrounded)
        {
            if (keys.IsKeyDown(Keys.S) || IsCeilingAbove)
            {
                SetState(CharacterStateAnim.CrouchAttack);
            }
            else if (state != CharacterStateAnim.Attack)
            {
                SetState(CharacterStateAnim.Attack);
                _comboStep = 1;
                _continueCombo = false;
            }
            else if (state == CharacterStateAnim.Attack && _comboStep == 1)
            {
                // queues up the second hit of the combo
                _continueCombo = true;
            }
        }

        // this is the movement input
        if (state != CharacterStateAnim.Attack && state != CharacterStateAnim.CrouchAttack && state != CharacterStateAnim.TurnAround)
        {
            // pressing left shift will trigger a dash, but i'm thinking of replacing it with a roll instead
            if (keys.IsKeyDown(Keys.LeftShift) && !_previousKeys.IsKeyDown(Keys.LeftShift) && IsGrounded && !IsCeilingAbove)
            {
                SetState(CharacterStateAnim.Dash);
                _dashTimer = 0.3f;
            }
            // trigger a slide only when the player is running and presses s
            else if (keys.IsKeyDown(Keys.S) && IsGrounded && state == CharacterStateAnim.Run)
            {
                SetState(CharacterStateAnim.Slide);
                _slideTimer = 0.4f;
            }
            else
            {
                if ((keys.IsKeyDown(Keys.S) || IsCeilingAbove) && IsGrounded)
                {
                    if (keys.IsKeyDown(Keys.D)) { speed.X = 1.5f; lookingForward = SpriteEffects.None; }
                    else if (keys.IsKeyDown(Keys.A)) { speed.X = -1.5f; lookingForward = SpriteEffects.FlipHorizontally; }
                }
                else
                {
                    if (keys.IsKeyDown(Keys.D)) 
                    { 
                        if (state == CharacterStateAnim.Run && lookingForward == SpriteEffects.FlipHorizontally) SetState(CharacterStateAnim.TurnAround);
                        else { speed.X = 3f; lookingForward = SpriteEffects.None; }
                    }
                    else if (keys.IsKeyDown(Keys.A)) 
                    { 
                        if (state == CharacterStateAnim.Run && lookingForward == SpriteEffects.None) SetState(CharacterStateAnim.TurnAround);
                        else { speed.X = -3f; lookingForward = SpriteEffects.FlipHorizontally; }
                    }
                }
            }

            if (!IsCeilingAbove && ((keys.IsKeyDown(Keys.Space) && !_previousKeys.IsKeyDown(Keys.Space)) ||
                (keys.IsKeyDown(Keys.W) && !_previousKeys.IsKeyDown(Keys.W)) ||
                (keys.IsKeyDown(Keys.Up) && !_previousKeys.IsKeyDown(Keys.Up))))
            {
                if (IsGrounded)
                {
                    speed.Y = -9.5f;
                    IsGrounded = false;
                }
                else if (canDoubleJump)
                {
                    speed.Y = -9.5f;
                    canDoubleJump = false;
                    state = CharacterStateAnim.Jump;
                    _currentFrame = 0;
                    _timer = 0f;
                }
            }
        }

        speed.Y += 0.5f; 
        position += speed;
        _previousMouse = mouse;
        _previousKeys = keys;
    }

    // calculates what animation to play after checking floor collisions
    public void UpdateAnimations(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // freeze in the hit pose until the hit stun finishes
        if (state == CharacterStateAnim.Hit)
        {
            _hitStunTimer -= dt;
            if (_hitStunTimer <= 0) SetState(CharacterStateAnim.Idle);
            return; 
        }

        if (state != CharacterStateAnim.Attack && state != CharacterStateAnim.Dead && state != CharacterStateAnim.Dash && 
            state != CharacterStateAnim.Slide && state != CharacterStateAnim.CrouchAttack && state != CharacterStateAnim.TurnAround)
        {
            DetermineMovementState(Keyboard.GetState());
        }
        
        AdvanceAnimation(dt);
    }

    // this calculates the player's state based on their speed and keys
    private void DetermineMovementState(KeyboardState keys)
    {
        if (!IsGrounded)
        {
            if (speed.Y < -1.5f) SetState(CharacterStateAnim.Jump);
            else if (speed.Y >= -1.5f && speed.Y <= 1.5f) SetState(CharacterStateAnim.JumpApex);
            else SetState(CharacterStateAnim.Fall);
        }
        else 
        {
            if (keys.IsKeyDown(Keys.S) || IsCeilingAbove)
            {
                if (speed.X != 0) SetState(CharacterStateAnim.CrouchWalk);
                else SetState(CharacterStateAnim.Crouch);
            }
            else
            {
                if (speed.X != 0) SetState(CharacterStateAnim.Run);
                else SetState(CharacterStateAnim.Idle);
            }
        }
    }

    // this hurts the player and activates iframes
    public void TakeDamage(int damage)
    {
        if (state == CharacterStateAnim.Dead || iFrameTimer > 0) return;

        Health -= damage;
        iFrameTimer = 1.0f; 
        
        if (Health <= 0) SetState(CharacterStateAnim.Dead);
        else { SetState(CharacterStateAnim.Hit); _hitStunTimer = 0.4f; }
    }

    // safely sets a new state and resets the animation frame
    private void SetState(CharacterStateAnim newState)
    {
        if (state == newState) return; 
        state = newState;
        _currentFrame = 0;
        _timer = 0f;
    }

    // this moves the animation forward frame by frame
    private void AdvanceAnimation(float dt)
    {
        _timer += dt;
        if (_timer > FrameTime)
        {
            // handles our 2hit combo
            if (state == CharacterStateAnim.Attack)
            {
                if (_comboStep == 1 && _currentFrame == 4) 
                {
                    if (_continueCombo) { _comboStep = 2; _continueCombo = false; _currentFrame++; }
                    else SetState(CharacterStateAnim.Idle);
                }
                else if (_comboStep == 2 && _currentFrame >= 9) SetState(CharacterStateAnim.Idle);
                else _currentFrame++;
            }
            else 
            {
                _currentFrame++;
                int totalFrames = GetFrameCount();

                if (_currentFrame >= totalFrames)
                {
                    // animations that freeze on the last frame
                    if (state == CharacterStateAnim.Dead || state == CharacterStateAnim.Jump || state == CharacterStateAnim.JumpApex || state == CharacterStateAnim.Fall || state == CharacterStateAnim.Crouch)
                        _currentFrame = totalFrames - 1; 
                    
                    // animations that return to idle when finished
                    else if (state == CharacterStateAnim.TurnAround || state == CharacterStateAnim.CrouchAttack)
                        SetState(CharacterStateAnim.Idle);
                    
                    // animations that loop infinitely
                    else _currentFrame = 0; 
                }
            }
            _timer = 0f;
        }
    }

    private int GetFrameCount()
    {
        return state switch
        {
            CharacterStateAnim.Attack => AttackFrameCount, CharacterStateAnim.Hit => HitFrameCount, CharacterStateAnim.Dead => DeadFrameCount,
            CharacterStateAnim.Jump => JumpFrameCount, CharacterStateAnim.JumpApex => JumpApexFrameCount, CharacterStateAnim.Fall => FallFrameCount,
            CharacterStateAnim.Run => RunFrameCount, CharacterStateAnim.Dash => DashFrameCount, CharacterStateAnim.Slide => SlideFrameCount,
            CharacterStateAnim.Crouch => CrouchFrameCount, CharacterStateAnim.CrouchWalk => CrouchWalkFrameCount, 
            CharacterStateAnim.CrouchAttack => CrouchAttackFrameCount, CharacterStateAnim.TurnAround => TurnAroundFrameCount,
            _ => IdleFrameCount
        };
    }

    private Texture2D GetCurrentTexture()
    {
        return state switch
        {
            CharacterStateAnim.Attack => _attackSheet, CharacterStateAnim.Hit => _hitSheet, CharacterStateAnim.Dead => _deadSheet,
            CharacterStateAnim.Jump => _jumpSheet, CharacterStateAnim.JumpApex => _jumpApexSheet, CharacterStateAnim.Fall => _fallSheet,
            CharacterStateAnim.Run => _runSheet, CharacterStateAnim.Dash => _dashSheet, CharacterStateAnim.Slide => _slideSheet,
            CharacterStateAnim.Crouch => _crouchSheet, CharacterStateAnim.CrouchWalk => _crouchWalkSheet, 
            CharacterStateAnim.CrouchAttack => _crouchAttackSheet, CharacterStateAnim.TurnAround => _turnAroundSheet,
            _ => _idleSheet
        };
    }

    // draws the character and handles transparency if we have iframes
    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D sheet = GetCurrentTexture();
        if (sheet == null) return; 

        Rectangle sourceRect = new Rectangle((_currentFrame * FrameWidth) + 1, 0, FrameWidth - 2, FrameHeight);
        Vector2 origin = new Vector2(FrameWidth / 2f, FrameHeight);
        float anchorX = position.X + (FrameWidth / 2f);
        float anchorY = position.Y + FrameHeight;

        Color drawColor = iFrameTimer > 0 ? Color.White * 0.5f : Color.White;
        spriteBatch.Draw(sheet, new Vector2((int)anchorX, (int)anchorY), sourceRect, drawColor, 0f, origin, 1f, lookingForward, 0f);
    }
}