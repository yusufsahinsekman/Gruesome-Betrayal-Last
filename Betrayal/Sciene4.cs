using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Sciene4
{
    public Texture2D picture4;
    public bool sciene4Chest = false;
    public bool sciene4Chest2 = false;

    public List<Rectangle> HitBoxSciene4()
    {
        List<Rectangle> boxes = new List<Rectangle>();
        boxes.Add(new Rectangle(0, 387, 284, 90));
        
        boxes.Add(new Rectangle(186, 295, 32, 32));
        boxes.Add(new Rectangle(73, 245, 32, 32));
        boxes.Add(new Rectangle(172, 170, 32, 32));
        boxes.Add(new Rectangle(0, 110, 94, 32));
        
        boxes.Add(new Rectangle(225, 105, 250, 96));
        
        boxes.Add(new Rectangle(255, -200, 225, 260)); 
        
        boxes.Add(new Rectangle(535, 150, 32, 32));
        boxes.Add(new Rectangle(605, 105, 32, 32));
        
        
        boxes.Add(new Rectangle(660, 50, 109, 32)); 
        
        boxes.Add(new Rectangle(613, 390, 190, 90));
        return boxes;
    }

    public Rectangle Sciene4Door()
    {
        return new Rectangle(770, 335, 30, 60);
    }

    public Rectangle Sciene4Chest1()
    {
        return new Rectangle(10, 80, 32, 32);
    }

    public Rectangle Sciene4Chest2()
    {
        return new Rectangle(760, 20, 32, 32);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(picture4, new Vector2(0, 0), Color.White);
    }
}