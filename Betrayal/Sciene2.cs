using System.Net.Mime;
using Betrayal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public class Sciene2
{
    public Texture2D picture2;
    
    



    public Rectangle HitBoxSciene2()
    {
        return new Rectangle(10, 384, 900, 96);
    }

    public Rectangle Sciene2Door()
    {
        return new Rectangle(805, 380, 30, 60);
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(picture2, new Vector2(0,0) , Color.White);
    }
}