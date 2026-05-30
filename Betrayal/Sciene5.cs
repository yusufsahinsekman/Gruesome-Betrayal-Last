using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Sciene5
{
    public Texture2D picture5;
    public bool sciene5Chest1 = false;
    public bool sciene5Chest2 = false;
    public bool sciene5Chest3 = false;

    public List<Rectangle> HitBoxSciene5()
    {
        List<Rectangle> boxes = new List<Rectangle>();
        boxes.Add(new Rectangle(0, 387, 255, 90));
        boxes.Add(new Rectangle(20, 125, 96, 32));
        
        boxes.Add(new Rectangle(180, 175, 30, 32));
        boxes.Add(new Rectangle(295, 280, 30, 32));
        
        boxes.Add(new Rectangle(375, 390, 160, 92));
        boxes.Add(new Rectangle(598, 255, 30, 32));
        
        boxes.Add(new Rectangle(705, 145, 300, 32));
        
        boxes.Add(new Rectangle(644, 388, 157, 93));
        return boxes;
    }

    public Rectangle Sciene5Door()
    {
        return new Rectangle(770, 335, 30, 60);
    }

    public Rectangle Sciene5Chest1()
    {
        return new Rectangle(50, 96, 32, 32);
    }

    public Rectangle Sciene5Chest2()
    {
        return new Rectangle(437, 360, 32, 32);
    }
    
    public Rectangle Sciene5Chest3()
    {
        return new Rectangle(740, 115, 32, 32);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(picture5, new Vector2(0, 0), Color.White);
    }
}