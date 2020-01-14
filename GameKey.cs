using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WhosTurn
{
    public class GameKey
    {
        public string ButtonChar { get; set; }
        public int ASCIICode { get; set; }
        public Color Color { get; set; }

        public GameKey(string buttonChar, int aSCIICode, Color color)
        {
            ButtonChar = buttonChar;
            ASCIICode = aSCIICode;
            Color = color;
        }
    }
}
