using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Betrayal;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D sciene2Picture;
    Texture2D sciene3Picture;
    Texture2D sciene4Picture;
    Texture2D sciene5Picture;
    Texture2D sciene6Picture;

    public List<Texture2D> sciene1Dialogue = new List<Texture2D>();
    public List<Texture2D> sciene6Dialogue = new List<Texture2D>();

    int DiyalogSirasi2 = 0;
    int DiyalogSirasi6 = 0;
    bool diyalogAktifMi2 = false;
    bool diyalogAktifMi6 = false;
    bool diyalogTamamlandi2 = false;
    bool diyalogTamamlandi6 = false;
    MouseState oncekiFareDurumu;

    public Character mainCharacter; 
    public Sciene2 sciene2 = new Sciene2();
    public Sciene3 sciene3 = new Sciene3();
    public Sciene4 sciene4 = new Sciene4();
    public Sciene5 sciene5 = new Sciene5();
    public Sciene6 sciene6 = new Sciene6();

    public List<Enemy> scene4Enemies = new List<Enemy>();

    public FlyingDemon flyingDemon;
    Texture2D demonIdleTex;
    Texture2D demonFlyTex;
    Texture2D demonAttackTex;
    Texture2D fireballTex;
    public List<Fireball> activeFireballs = new List<Fireball>();
    public bool hasPlayerMovedInScene5 = false;

    CurrentSciene currentSciene = CurrentSciene.StartScreen;

    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;

    public MenuState menuState = MenuState.Hidden;
    private SpriteFont _font;
    private Texture2D _blankTexture;
    private Texture2D _startBackground;
    private float _deathResetTimer = 0f;
    private Song _bgm;

    private int _startBgCurrentFrame = 0;
    private float _startBgTimer = 0f;
    private const float StartBgFrameTime = 0.04f;
    private const int StartBgFrameCount = 44;
    private const int StartBgCols = 5;
    private const int StartBgFrameWidth = 1920;
    private const int StartBgFrameHeight = 1080;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() { base.Initialize(); }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _font = Content.Load<SpriteFont>("Font");
        _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
        _blankTexture.SetData(new[] { Color.White });
        _startBackground = Content.Load<Texture2D>("StartBackground");

        _bgm = Content.Load<Song>("background_music");
        MediaPlayer.Play(_bgm);
        MediaPlayer.IsRepeating = true;

        sciene2Picture = Content.Load<Texture2D>("Sahne 2.png");
        sciene2.picture2 = sciene2Picture;
        sciene2.HitBoxSciene2();

        sciene3Picture = Content.Load<Texture2D>("Sahne 3.png");
        sciene3.picture3 = sciene3Picture;
        sciene3.HitBoxSciene3();

        sciene4Picture = Content.Load<Texture2D>("Sahne 4.png");
        sciene4.picture4 = sciene4Picture;
        sciene4.HitBoxSciene4();

        sciene5Picture = Content.Load<Texture2D>("Sahne 5.png");
        sciene5.picture5 = sciene5Picture;
        sciene5.HitBoxSciene5();

        sciene6Picture = Content.Load<Texture2D>("Sahne 6.png");
        sciene6.picture6 = sciene6Picture;
        sciene6.HitBoxSciene6();

        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog1.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog2.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog3.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog4.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog5.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog6.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog7.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog8.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog9.png"));
        sciene1Dialogue.Add(Content.Load<Texture2D>("Diyalog10.png"));

        sciene6Dialogue.Add(Content.Load<Texture2D>("LuisDiyalog1.png"));
        sciene6Dialogue.Add(Content.Load<Texture2D>("LuisDiyalog2.png"));
        sciene6Dialogue.Add(Content.Load<Texture2D>("LuisDiyalog3.png"));
        sciene6Dialogue.Add(Content.Load<Texture2D>("LuisDiyalog4.png"));
        sciene6Dialogue.Add(Content.Load<Texture2D>("LuisDiyalog5.png"));
        sciene6Dialogue.Add(Content.Load<Texture2D>("LuisDiyalog6.png"));

        mainCharacter = new Character(
            Content.Load<Texture2D>("_Idle"),
            Content.Load<Texture2D>("_Run"),
            Content.Load<Texture2D>("_Jump"),
            Content.Load<Texture2D>("_JumpFallInbetween"), 
            Content.Load<Texture2D>("_Fall"),
            Content.Load<Texture2D>("_AttackCombo2hit"), 
            Content.Load<Texture2D>("_Hit"),             
            Content.Load<Texture2D>("_Death"),
            Content.Load<Texture2D>("_Dash"),
            Content.Load<Texture2D>("_SlideFull"),
            Content.Load<Texture2D>("_CrouchFull"),
            Content.Load<Texture2D>("_CrouchWalk"),
            Content.Load<Texture2D>("_CrouchAttack"),
            Content.Load<Texture2D>("_TurnAround"),
            new Vector2(100, 300)
        );

        ResetEnemies();

        demonIdleTex = Content.Load<Texture2D>("DemonIdle");
        demonFlyTex = Content.Load<Texture2D>("DemonFly");
        demonAttackTex = Content.Load<Texture2D>("DemonAttack");
        fireballTex = Content.Load<Texture2D>("Fireball");

        flyingDemon = new FlyingDemon(demonIdleTex, demonFlyTex, demonAttackTex, new Vector2(400, 20));
    }

    private void ResetEnemies()
    {
        scene4Enemies.Clear();
        Texture2D skelIdle = Content.Load<Texture2D>("Skeleton Idle");
        Texture2D skelWalk = Content.Load<Texture2D>("Skeleton Walk");
        Texture2D skelReact = Content.Load<Texture2D>("Skeleton React");
        Texture2D skelAttack = Content.Load<Texture2D>("Skeleton Attack");
        Texture2D skelHit = Content.Load<Texture2D>("Skeleton Hit");
        Texture2D skelDead = Content.Load<Texture2D>("Skeleton Dead");

        Enemy s4Enemy1 = new Enemy(skelIdle, skelWalk, skelReact, skelAttack, skelHit, skelDead, new Vector2(100, 350));
        s4Enemy1.PatrolDistance = 60f;

        Enemy s4Enemy2 = new Enemy(skelIdle, skelWalk, skelReact, skelAttack, skelHit, skelDead, new Vector2(700, 13));
        s4Enemy2.PatrolDistance = 40f;
        s4Enemy2.Health = 5;
        s4Enemy2.Tint = new Color(50, 50, 50);

        scene4Enemies.Add(s4Enemy1);
        scene4Enemies.Add(s4Enemy2);
    }

    private void ResetLevel()
    {
        mainCharacter.position = new Vector2(50, 300);
        mainCharacter.Health = 5;
        mainCharacter.state = CharacterStateAnim.Idle;
        mainCharacter.speed = Vector2.Zero;
        mainCharacter.iFrameTimer = 0f;

        ResetEnemies();

        hasPlayerMovedInScene5 = false;
        activeFireballs.Clear(); 
        flyingDemon.Position = new Vector2(400, 20); 
        flyingDemon.State = DemonState.Idle; 
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        MouseState currentMouseState = Mouse.GetState();

        if (currentSciene == CurrentSciene.StartScreen)
        {
            _startBgTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_startBgTimer >= StartBgFrameTime)
            {
                _startBgTimer = 0f;
                _startBgCurrentFrame++;
                if (_startBgCurrentFrame >= StartBgFrameCount)
                {
                    _startBgCurrentFrame = 0;
                }
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                Rectangle mouseRect = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
                
                if (menuState == MenuState.Hidden)
                {
                    if (mouseRect.Intersects(new Rectangle(300, 150, 200, 40))) 
                    {
                        currentSciene = CurrentSciene.Sciene_2;
                    }
                    if (mouseRect.Intersects(new Rectangle(300, 200, 200, 40))) menuState = MenuState.Controls;
                    if (mouseRect.Intersects(new Rectangle(300, 250, 200, 40))) menuState = MenuState.Credits;
                    if (mouseRect.Intersects(new Rectangle(300, 300, 200, 40))) Exit();
                }
                else 
                {
                    if (mouseRect.Intersects(new Rectangle(300, 440, 200, 40))) menuState = MenuState.Hidden;
                }
            }

            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;
            oncekiFareDurumu = currentMouseState;
            return; 
        }

        if (currentKeyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            if (menuState == MenuState.Hidden) menuState = MenuState.Main;
            else menuState = MenuState.Hidden;
        }

        if (menuState != MenuState.Hidden)
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                Rectangle mouseRect = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
                
                if (menuState == MenuState.Main)
                {
                    if (mouseRect.Intersects(new Rectangle(300, 150, 200, 40))) menuState = MenuState.Hidden;
                    if (mouseRect.Intersects(new Rectangle(300, 200, 200, 40))) menuState = MenuState.Controls;
                    if (mouseRect.Intersects(new Rectangle(300, 250, 200, 40))) menuState = MenuState.Credits;
                    if (mouseRect.Intersects(new Rectangle(300, 300, 200, 40))) Exit();
                }
                else if (menuState == MenuState.Controls || menuState == MenuState.Credits)
                {
                    if (mouseRect.Intersects(new Rectangle(300, 440, 200, 40))) menuState = MenuState.Main;
                }
            }

            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;
            return; 
        }

        if (mainCharacter.state == CharacterStateAnim.Dead)
        {
            _deathResetTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            mainCharacter.UpdateAnimations(gameTime);

            if (_deathResetTimer >= 2.0f)
            {
                ResetLevel();
                _deathResetTimer = 0f;
            }
            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;
            oncekiFareDurumu = currentMouseState;
            return;
        }

        Rectangle resetHitBox = new Rectangle(-200, 500, 1100, 10);
        if (mainCharacter.HitBox.Intersects(resetHitBox))
        {
            ResetLevel();
        }

        mainCharacter.IsDialogueActive = diyalogAktifMi2 || diyalogAktifMi6;

        mainCharacter.Update(gameTime);
        mainCharacter.IsGrounded = false;
        mainCharacter.IsCeilingAbove = false; 

        ResolveCollision(new Rectangle(-50, -1000, 50, 2000));
        ResolveCollision(new Rectangle(770, -1000, 50, 1300));

        switch (currentSciene)
        {
            case CurrentSciene.Sciene_2:
                ResolveCollision(sciene2.HitBoxSciene2());

                Rectangle diyalogTetikleyici1 = new Rectangle(370, 370, 1, 50);

                if(!diyalogTamamlandi2 && mainCharacter.HitBox.Intersects(diyalogTetikleyici1))
                {
                    diyalogAktifMi2 = true;
                }

                if (diyalogAktifMi2)
                {
                    if (currentMouseState.LeftButton == ButtonState.Released && oncekiFareDurumu.LeftButton == ButtonState.Pressed)
                    {
                        DiyalogSirasi2++;

                        if (DiyalogSirasi2 >= sciene1Dialogue.Count)
                        {
                            diyalogAktifMi2 = false;
                            diyalogTamamlandi2 = true;
                            DiyalogSirasi2 = 0;
                        }
                    }
                }
        
                if (mainCharacter.HitBox.Intersects(sciene2.Sciene2Door()))
                {
                    currentSciene = CurrentSciene.Sciene_3;
                    mainCharacter.position = new Vector2(50, 300);
                }
                break;

            case CurrentSciene.Sciene_3:
                foreach (Rectangle box in sciene3.HitBoxSciene3()) ResolveCollision(box);

                if (mainCharacter.HitBox.Intersects(sciene3.Sciene3Chest())) sciene3.sciene3Chest = true;

                if (mainCharacter.HitBox.Intersects(sciene3.Sciene3Door()) && sciene3.sciene3Chest == true)
                {
                    currentSciene = CurrentSciene.Sciene_4;
                    mainCharacter.position = new Vector2(50, 300);
                }
                break;

            case CurrentSciene.Sciene_4:
                foreach (Rectangle box in sciene4.HitBoxSciene4()) ResolveCollision(box);

                foreach (Enemy e in scene4Enemies)
                {
                    e.Update(gameTime, new Vector2(mainCharacter.position.X + (mainCharacter.frameWeight / 2f), mainCharacter.position.Y + (mainCharacter.frameHeight / 2f)));
                }

                if (mainCharacter.HitBox.Intersects(sciene4.Sciene4Chest1())) sciene4.sciene4Chest = true;
                if (mainCharacter.HitBox.Intersects(sciene4.Sciene4Chest2())) sciene4.sciene4Chest2 = true;

                if (mainCharacter.HitBox.Intersects(sciene4.Sciene4Door()) && sciene4.sciene4Chest == true && sciene4.sciene4Chest2 == true)
                {
                    hasPlayerMovedInScene5 = false;
                    activeFireballs.Clear(); 
                    flyingDemon.Position = new Vector2(400, 20); 
                    flyingDemon.State = DemonState.Idle; 

                    currentSciene = CurrentSciene.Sciene_5;
                    mainCharacter.position = new Vector2(50, 300);
                }
                break;

            case CurrentSciene.Sciene_5:
                foreach (Rectangle box in sciene5.HitBoxSciene5()) ResolveCollision(box);

                if (mainCharacter.HitBox.Intersects(sciene5.Sciene5Chest1())) sciene5.sciene5Chest1 = true;
                if (mainCharacter.HitBox.Intersects(sciene5.Sciene5Chest2())) sciene5.sciene5Chest2 = true;
                if (mainCharacter.HitBox.Intersects(sciene5.Sciene5Chest3())) sciene5.sciene5Chest3 = true;
                
                if (mainCharacter.position.X > 55) hasPlayerMovedInScene5 = true;

                flyingDemon.Update(gameTime, hasPlayerMovedInScene5, activeFireballs, fireballTex);

                for (int i = activeFireballs.Count - 1; i >= 0; i--)
                {
                    activeFireballs[i].Update();
                    
                    if (activeFireballs[i].HitBox.Intersects(mainCharacter.HitBox))
                    {
                        mainCharacter.TakeDamage(1);
                        activeFireballs[i].IsActive = false; 
                    }

                    if (!activeFireballs[i].IsActive)
                    {
                        activeFireballs.RemoveAt(i);
                    }
                }

                if (mainCharacter.HitBox.Intersects(sciene5.Sciene5Door()) && sciene5.sciene5Chest1 == true && sciene5.sciene5Chest2 == true && sciene5.sciene5Chest3 == true)
                {
                    currentSciene = CurrentSciene.Sciene_6;
                    mainCharacter.position = new Vector2(50, 300);
                }
                break;

            case CurrentSciene.Sciene_6:
                ResolveCollision(sciene6.HitBoxSciene6());

                Rectangle diyalogTetikleyici6 = new Rectangle(350, 380, 1, 50);

                if(!diyalogTamamlandi6 && mainCharacter.HitBox.Intersects(diyalogTetikleyici6))
                {
                    diyalogAktifMi6= true;
                }

                if (diyalogAktifMi6)
                {
                    if (currentMouseState.LeftButton == ButtonState.Released && oncekiFareDurumu.LeftButton == ButtonState.Pressed)
                    {
                        DiyalogSirasi6++;

                        if (DiyalogSirasi6 >= sciene6Dialogue.Count)
                        {
                            diyalogAktifMi6 = false;
                            diyalogTamamlandi6 = true;
                            DiyalogSirasi6 = 0;
                        }
                    }
                }
                
                break;
        }

        mainCharacter.UpdateAnimations(gameTime);

        Rectangle? sword = mainCharacter.WeaponHitBox;
        if (currentSciene == CurrentSciene.Sciene_4)
        {
            foreach (Enemy e in scene4Enemies)
            {
                if (sword.HasValue && sword.Value.Intersects(e.HitBox))
                {
                    e.TakeDamage(1, mainCharacter.position.X + (mainCharacter.frameWeight / 2f));
                }

                Rectangle? enemySword = e.WeaponHitBox;
                if (enemySword.HasValue && enemySword.Value.Intersects(mainCharacter.HitBox))
                {
                    mainCharacter.TakeDamage(1);
                }
            }
        }
        
        _previousKeyboardState = currentKeyboardState;
        _previousMouseState = currentMouseState;
        oncekiFareDurumu = currentMouseState;

        base.Update(gameTime);
    }

    private void ResolveCollision(Rectangle box)
    {
        Rectangle hitBox = mainCharacter.HitBox;
        
        if (hitBox.Intersects(box))
        {
            Rectangle intersection = Rectangle.Intersect(hitBox, box);

            bool fallingOntoTop = mainCharacter.speed.Y > 0 && (hitBox.Bottom - mainCharacter.speed.Y) <= box.Top + 8;

            if (fallingOntoTop)
            {
                mainCharacter.position.Y = box.Top - Character.FrameHeight;
                mainCharacter.speed.Y = 0;
            }
            else if (intersection.Width < intersection.Height)
            {
                if (hitBox.Center.X < box.Center.X)
                {
                    mainCharacter.position.X -= intersection.Width;
                }
                else
                {
                    mainCharacter.position.X += intersection.Width;
                }
            }
            else
            {
                if (hitBox.Center.Y > box.Center.Y && mainCharacter.speed.Y < 0)
                {
                    mainCharacter.position.Y += intersection.Height;
                    mainCharacter.speed.Y = 0;
                }
            }
        }

        hitBox = mainCharacter.HitBox;
        Rectangle footSensor = new Rectangle(hitBox.X + 2, hitBox.Bottom, hitBox.Width - 4, 2);
        if (footSensor.Intersects(box))
        {
            mainCharacter.IsGrounded = true;
        }

        Rectangle standBox = new Rectangle((int)mainCharacter.position.X + (Character.FrameWidth / 2) - 13, (int)mainCharacter.position.Y + 30, 26, 50);
        if (mainCharacter.IsCrouching && standBox.Intersects(box))
        {
            mainCharacter.IsCeilingAbove = true;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        if (currentSciene == CurrentSciene.StartScreen)
        {
            int row = _startBgCurrentFrame / StartBgCols;
            int col = _startBgCurrentFrame % StartBgCols;
            Rectangle sourceRect = new Rectangle(col * StartBgFrameWidth, row * StartBgFrameHeight, StartBgFrameWidth, StartBgFrameHeight);
            
            _spriteBatch.Draw(_startBackground, new Rectangle(0, 0, 800, 480), sourceRect, Color.White);

            if (menuState == MenuState.Hidden)
            {
                _spriteBatch.DrawString(_font, "GRUESOME BETRAYAL", new Vector2(310, 50), Color.Yellow);
                _spriteBatch.DrawString(_font, "PLAY", new Vector2(370, 150), Color.White);
                _spriteBatch.DrawString(_font, "OPTIONS", new Vector2(350, 200), Color.White);
                _spriteBatch.DrawString(_font, "CREDITS", new Vector2(350, 250), Color.White);
                _spriteBatch.DrawString(_font, "EXIT", new Vector2(370, 300), Color.White);
                _spriteBatch.DrawString(_font, "Made by Yamen Alamer & Yusuf Sahin Sekman", new Vector2(20, 455), Color.LightGray);
            }
        }
        else if (currentSciene == CurrentSciene.Sciene_2)
        {
            sciene2.Draw(_spriteBatch);
            if (diyalogAktifMi2)
            {
                Rectangle diyalogKonumu = new Rectangle(0, 0, 800, 480);
                _spriteBatch.Draw(sciene1Dialogue[DiyalogSirasi2], diyalogKonumu, Color.White);
            }
        }
        else if (currentSciene == CurrentSciene.Sciene_3)
        {
            sciene3.Draw(_spriteBatch);
        }
        else if (currentSciene == CurrentSciene.Sciene_4) 
        {
            sciene4.Draw(_spriteBatch);
            foreach (Enemy e in scene4Enemies) e.Draw(_spriteBatch);
        }
        else if (currentSciene == CurrentSciene.Sciene_5)
        {
            sciene5.Draw(_spriteBatch);
            flyingDemon.Draw(_spriteBatch);
            
            foreach (var fireball in activeFireballs)
            {
                fireball.Draw(_spriteBatch);
            }
        }
        else if (currentSciene == CurrentSciene.Sciene_6)
        {
            sciene6.Draw(_spriteBatch);
            if (diyalogAktifMi6)
            {
                Rectangle diyalogKonumu = new Rectangle(0, 0, 800, 480);
                _spriteBatch.Draw(sciene6Dialogue[DiyalogSirasi6], diyalogKonumu, Color.White);
            }
        }
        
        if (currentSciene != CurrentSciene.StartScreen)
        {
            mainCharacter.Draw(_spriteBatch);
        }

        if (menuState != MenuState.Hidden)
        {
            _spriteBatch.Draw(_blankTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black * 0.7f);

            if (menuState == MenuState.Main)
            {
                _spriteBatch.DrawString(_font, "RESUME", new Vector2(350, 150), Color.White);
                _spriteBatch.DrawString(_font, "CONTROLS", new Vector2(350, 200), Color.White);
                _spriteBatch.DrawString(_font, "CREDITS", new Vector2(350, 250), Color.White);
                _spriteBatch.DrawString(_font, "EXIT GAME", new Vector2(350, 300), Color.White);
            }
            else if (menuState == MenuState.Controls)
            {
                _spriteBatch.DrawString(_font, "CONTROLS", new Vector2(350, 100), Color.Yellow);
                _spriteBatch.DrawString(_font, "A / D : Move Left / Right", new Vector2(280, 150), Color.White);
                _spriteBatch.DrawString(_font, "Space / W / Up Arrow : Jump", new Vector2(280, 190), Color.White);
                _spriteBatch.DrawString(_font, "S : Crouch / Slide", new Vector2(280, 230), Color.White);
                _spriteBatch.DrawString(_font, "Shift : Dash", new Vector2(280, 270), Color.White);
                _spriteBatch.DrawString(_font, "Left Click : Attack", new Vector2(280, 310), Color.White);
                
                _spriteBatch.DrawString(_font, "BACK", new Vector2(350, 440), Color.White);
            }
            else if (menuState == MenuState.Credits)
            {
                _spriteBatch.DrawString(_font, "SPECIAL THANKS TO OUR ASSET CREATORS", new Vector2(200, 80), Color.Yellow);
                _spriteBatch.DrawString(_font, "Jesse Munguia", new Vector2(320, 140), Color.White);
                _spriteBatch.DrawString(_font, "aamatniekss", new Vector2(320, 180), Color.White);
                _spriteBatch.DrawString(_font, "Mattz Art", new Vector2(320, 220), Color.White);
                _spriteBatch.DrawString(_font, "motionvibe.club", new Vector2(320, 260), Color.White);

                _spriteBatch.DrawString(_font, "MUSIC BY", new Vector2(320, 320), Color.Yellow);
                _spriteBatch.DrawString(_font, "alkakrab", new Vector2(320, 360), Color.White);

                _spriteBatch.DrawString(_font, "BACK", new Vector2(350, 440), Color.White);
            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

public enum CurrentSciene
{
    StartScreen, Sciene_1, Sciene_2, Sciene_3, Sciene_4, Sciene_5, Sciene_6
}

public enum MenuState 
{ 
    Hidden, Main, Controls, Credits 
}