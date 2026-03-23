using NPCs;
    public class Point
    {
        private int x;
        private int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return new Point(x, y + 1);
                case Direction.Down:
                    return new Point(x, y - 1);
                case Direction.Left:
                    return new Point(x - 1, y);
                case Direction.Right:
                    return new Point(x + 1, y);
                default:
                    return new Point(x, y);
            }
        }

        public double DistanceTo(Point other)
        {
            int dx = this.x - other.x;
            int dy = this.y - other.y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        public bool Equals(Point other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public int X => x;
        public int Y => y;
    }
