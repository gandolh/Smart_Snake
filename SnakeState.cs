using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnake
{
    public class SnakeState : IEquatable<SnakeState?>
    {
        public bool dirIsLeft { get; set; }
        public bool dirIsRight { get; set; }
        public bool dirIsUp { get; set; }
        public bool dirIsDown { get; set; }
        public bool foodIsUp { get; set; }
        public bool foodIsRight { get; set; }
        public bool foodIsDown { get; set; }
        public bool foodIsLeft { get; set; }
        public bool dangerIsLeft { get; set; }
        public bool dangerIsRight { get; set; }
        public bool dangerIsUp { get; set; }
        public bool dangerIsDown { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as SnakeState);
        }

        public bool Equals(SnakeState? other)
        {
            return other is not null &&
                   dirIsLeft == other.dirIsLeft &&
                   dirIsRight == other.dirIsRight &&
                   dirIsUp == other.dirIsUp &&
                   dirIsDown == other.dirIsDown &&
                   foodIsUp == other.foodIsUp &&
                   foodIsRight == other.foodIsRight &&
                   foodIsDown == other.foodIsDown &&
                   foodIsLeft == other.foodIsLeft &&
                   dangerIsLeft == other.dangerIsLeft &&
                   dangerIsRight == other.dangerIsRight &&
                   dangerIsUp == other.dangerIsUp &&
                   dangerIsDown == other.dangerIsDown;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(dirIsLeft);
            hash.Add(dirIsRight);
            hash.Add(dirIsUp);
            hash.Add(dirIsDown);
            hash.Add(foodIsUp);
            hash.Add(foodIsRight);
            hash.Add(foodIsDown);
            hash.Add(foodIsLeft);
            hash.Add(dangerIsLeft);
            hash.Add(dangerIsRight);
            hash.Add(dangerIsUp);
            hash.Add(dangerIsDown);
            return hash.ToHashCode();
        }

        public static bool operator ==(SnakeState? left, SnakeState? right)
        {
            return EqualityComparer<SnakeState>.Default.Equals(left, right);
        }

        public static bool operator !=(SnakeState? left, SnakeState? right)
        {
            return !(left == right);
        }
    }
}
