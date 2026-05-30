using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Sciene3
{
    public Texture2D picture3;
    public bool sciene3Chest = false;

    public List<Rectangle> HitBoxSciene3()
    {
        List<Rectangle> boxes = new List<Rectangle>();
        boxes.Add(new Rectangle(0, 387, 180, 85));
        boxes.Add(new Rectangle(285, 390, 32, 90));
        boxes.Add(new Rectangle(385, 320, 32, 32));
        boxes.Add(new Rectangle(490, 250, 32, 32));
        boxes.Add(new Rectangle(400, 180, 32, 32));
        boxes.Add(new Rectangle(505, 115, 32, 32));
        boxes.Add(new Rectangle(615, 120, 32, 32));
        
        
        boxes.Add(new Rectangle(688, 50, 81, 32));
        
        boxes.Add(new Rectangle(550, 393, 250, 89));
        return boxes;
    }

    public Rectangle ResetHitBox()
    {
        return new Rectangle(-50, 500, 900, 10);
    }

    public Rectangle Sciene3Door()
    {
        return new Rectangle(770, 335, 30, 60);
    }

    public Rectangle Sciene3Chest()
    {
        return new Rectangle(742, 23, 32, 32);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(picture3, new Vector2(0, 0), Color.White);
    }
}