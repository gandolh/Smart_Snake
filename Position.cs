namespace SmartSnake
{
    public class Position
    {
        public int Row { get; }
        public int Col { get; }

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public Position Translate(Direction dir)
        {
            return new Position(Row + dir.RowOffset, Col + dir.ColOffset);
        }

        public double DistanceTo(Position other)
        {
            return Math.Sqrt((Row - other.Row) * (Row - other.Row) + (Col - other.Col) * (Col - other.Col));
        }

        public override bool Equals(object? obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Position? left, Position? right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position? left, Position? right)
        {
            return !(left == right);
        }
    }
}
