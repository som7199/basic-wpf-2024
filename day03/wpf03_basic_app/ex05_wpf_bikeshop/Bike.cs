using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ex05_wpf_bikeshop
{
    // Notifier을 상속받으면 AutoProperty {get;set;}은 사용 불가..
    public class Bike : Notifier
    {
        private double speed;   // 멤버변수
        private Color color;

        public double Speed     // 속성
        {
            get { return speed; }
            set 
            { 
                speed = value; 
                // 속성값이 변경되는 것을 알려주려면 이 작업이 반드시 필요함
                OnProPertyChanged(nameof(Speed));   // "Speed"
            }
        }

        public Color Color 
        {
            get { return color; }
            set 
            { 
                color = value;
                OnProPertyChanged(nameof(Color));
            } 
        }

    }
}
