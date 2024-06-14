using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SAPR.WPF.Models
{
    public class Rectangle
    {
        public Point TopRight { get; set; }
        public Point TopLeft { get; set; }
        public Point BottomLeft { get; set; }
        public Point BottomRight { get; set; }
        public bool IsMain { get; set; }
        public Color Color { get; set; }

        public Rectangle(Point topLeft, Point botRight, Color color, bool isMain)
        {           
            BottomRight = botRight;
            TopLeft = topLeft;
            TopRight = new Point(botRight.X, topLeft.Y);
            BottomLeft = new Point(topLeft.X, botRight.Y);
            Color = color;
            IsMain = isMain;
        }

        public Rectangle(Point topLeft, Point botRight, Point topRight, Point bottomLeft, Color color, bool isMain)
        {
            BottomRight = botRight;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            Color = color;
            IsMain = isMain;
        }
    }
}
