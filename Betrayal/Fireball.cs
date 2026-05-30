using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Betrayal;

public class Fireball
{
    public Texture2D Texture;
    public Vector2 Position;
    public bool IsActive = true;
    public float Speed = 5f;

    public Rectangle HitBox => new Rectangle((int)Position.X - 15, (int)Position.Y - 15, 30, 30);

    public Fireball(Texture2D texture, Vector2 startPos)
    {
        Texture = texture;
        Position = startPos;
    }

    public void Update()
    {
        Position.Y += Speed;

        if (Position.Y > 500)
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        spriteBatch.Draw(Texture, Position, null, Color.White, -MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
    }
}