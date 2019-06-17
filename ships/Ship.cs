using System;
using System.Collections.Generic;

namespace ships
{
    public enum ShipSizes { Small = 2, Medium, Big, Large }

    public class Ship
    {
        private struct ShipType
        {
            public int Size;
            public int Speed;
            public bool Collision;

            public ShipType(int size, int speed, bool collision)
            {
                Size = size;
                Speed = speed;
                Collision = collision;
            }
        }

        private static readonly Dictionary<int, ShipType> shipTypes = new Dictionary<int, ShipType>
        {
            { (int)ShipSizes.Small, new ShipType((int)ShipSizes.Small, 1, false) },
            { (int)ShipSizes.Medium, new ShipType((int)ShipSizes.Medium, 1, true) },
            { (int)ShipSizes.Big, new ShipType((int)ShipSizes.Big, 1, true) },
            { (int)ShipSizes.Large, new ShipType((int)ShipSizes.Large, 1, true) }
        };

        private ShipType type;

        public int X { get; }
        public int Y { get; }
        public int Size { get => type.Size; }
        public bool Collision { get => type.Collision; }
        public int Speed { get; set; }
        public List<(int X, int Y)> Coordinates { get; set; }
        public List<(int X, int Y)> Area { get; private set; }
        public bool IsHit { get; set; } = false;
        public bool IsHorizontal { get; private set; }

        public Ship(int x, int y, int type)
        {
            X = x;
            Y = y;
            this.type = shipTypes[type];
            Speed = (Game.Rand.Next(1) == 1) ? this.type.Speed : (this.type.Speed * -1);

            AssignCoordinates();
        }

        public override string ToString()
        {
            return base.ToString() + $"({X},{Y})";
        }

        private void AssignCoordinates()
        {
            Coordinates = new List<(int X, int Y)>();
            Area = new List<(int X, int Y)>();
            int xx, yy;

            IsHorizontal = (Game.Rand.Next(2) == 1);
            if (IsHorizontal)
            {
                yy = Y;
                for (int i = 0; i < type.Size; i++)
                {
                    xx = X + i;
                    var coord = (X: xx, Y: yy);
                    Coordinates.Add(coord);
                }
            }
            else
            {
                xx = X;
                for (int i = 0; i < type.Size; i++)
                {
                    yy = Y + i;
                    var coord = (X: xx, Y: yy);
                    Coordinates.Add(coord);
                }
            }

            UpdateArea();
        }

        public bool Intersects(List<(int X, int Y)> coordinates)
        {
            foreach (var coordinate in coordinates)
            {
                if (Area.Contains(coordinate))
                    return true;
            }

            return false;
        }

        public bool Intersects(Ship other)
        {
            return Intersects(other.Area);
        }

        public void UpdateArea()
        {
            if (IsHit)
                return;

            Area.Clear();

            foreach (var c in Coordinates)
            {
                var directions = new List<(int X, int Y)>
                {
                    (0, 1), (0, -1), (1, 0), (-1, 0)
                };

                foreach (var dir in directions)
                {
                    int xx = dir.X + c.X;
                    int yy = dir.Y + c.Y;
                    var coord = (X: xx, Y: yy);
                    if (Area.Contains(coord))
                        continue;
                    Area.Add(coord);
                }
            }
        }

        public void TakeHit()
        {
            IsHit = true;
            Speed = 0;
            Area.Clear();
        }
    }
}
