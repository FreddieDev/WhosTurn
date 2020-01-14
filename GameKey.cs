using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WhosTurn
{
    public class GameKey : ICloneable
    {
        public int Pos { get; set; } // Incremmenting int that represents the position for the key
        public string ButtonChar { get; set; }
        public int ASCIICode { get; set; }
        public Color Color { get; set; }
        public Ellipse Ellipse { get; set; }
        public TextBlock TextBlock { get; set; }

        public GameKey(int pos, string buttonChar, int aSCIICode, Color color)
        {
            Pos = pos;
            ButtonChar = buttonChar;
            ASCIICode = aSCIICode;
            Color = color;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
