using System.Net.Mime;
using Betrayal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public class Sciene6
{
    public Texture2D picture6;
    



    public Rectangle HitBoxSciene6()
    {
        return new Rectangle(0, 387, 900, 96);
    }

   


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(picture6, new Vector2(0,0) , Color.White);
    }
}