using System;
using System.Collections.Generic;

namespace ships
{
    public class Board
    {
        public enum Status { Empty, ShipHidden, ShipRevealed, ShipArea, Shot, Hint }

        public static int DefaultWidth { get; } = 15;
        public static int DefaultHeight { get; } = 15;

        private int[,] map;

        public int Width { get; }
        public int Height { get; }
        public List<Ship> Ships { get; private set; } = new List<Ship>();
        public List<(int X, int Y)> PlayerShots { get; set; } = new List<(int X, int Y)>();
        public int ShipsHit
        {
            get
            {
                int count = 0;
                foreach (var ship in Ships)
                {
                    if (ship.IsHit) count++;
                }
                return count;
            }
        }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            map = new int[width, height];
        }

        public int this[int x, int y]
        {
            get => map[x, y];
            set => map[x, y] = value;
        }

        private bool CanPlace(Ship ship)
        {
            foreach (var (X, Y) in ship.Coordinates)
            {
                if (IsOutOfBounds(X, Y) || IsBlocked(X, Y))
                    return false;
            }

            foreach (var other in Ships)
            {
                if (ship.Intersects(other))
                    return false;
            }

            return true;
        }

        private bool PlaceShip(int size)
        {
            int x = Game.Rand.Next(Width);
            int y = Game.Rand.Next(Height);

            Ship ship = new Ship(x, y, size);

            if (!CanPlace(ship))
                return false;

            foreach (var (X, Y) in ship.Coordinates)
            {
                map[X, Y] = (int)Status.ShipHidden;
            }

            Ships.Add(ship);
            return true;
        }

        public bool IsBlocked(int x, int y)
        {
            return map[x, y] == (int)Status.ShipHidden || map[x, y] == (int)Status.ShipRevealed;
        }

        public bool IsOutOfBounds(int x, int y)
        {
            return !((0 <= x && x < Width) && (0 <= y && y < Height));
        }

        public void Update(int turnsWithoutHit)
        {
            Array.Clear(map, 0, map.Length);
            foreach (var ship in Ships)
            {
                foreach (var (X, Y) in ship.Area)
                {
                    if (!IsOutOfBounds(X, Y))
                        map[X, Y] = (int)Status.ShipArea;
                }

                foreach (var (X, Y) in PlayerShots)
                {
                    map[X, Y] = (int)Status.Shot;
                }

                foreach (var (X, Y) in ship.Coordinates)
                {
                    map[X, Y] = ship.IsHit
                        ? (int)Status.ShipRevealed
                        : (int)Status.ShipHidden;
                }
            }

            if (turnsWithoutHit > 3)
                PlaceHints();
        }

        public void PlaceShips()
        {
            int maxTries = 100;

            int count = 0;
            for (int i = 0; i < maxTries; i++) {
                if (PlaceShip((int)ShipSizes.Large)) count++;
                if (count == 1) break;
            }

            count = 0;
            for (int i = 0; i < maxTries; i++)
            {
                if (PlaceShip((int)ShipSizes.Big)) count++;
                if (count == 2) break;
            }

            count = 0;
            for (int i = 0; i < maxTries; i++)
            {
                if (PlaceShip((int)ShipSizes.Medium)) count++;
                if (count == 3) break;
            }

            count = 0;
            for (int i = 0; i < maxTries; i++)
            {
                if (PlaceShip((int)ShipSizes.Small)) count++;
                if (count == 4) break;
            }
        }

        public Ship GetShipAtCoordinates(int x, int y)
        {
            foreach (Ship ship in Ships)
            {
                if (ship.Coordinates.Contains((x, y)))
                    return ship;
            }

            return null;
        }

        public void RevealShip(Ship ship)
        {
            foreach (var (X, Y) in ship.Coordinates)
            {
                map[X, Y] = (int)Status.ShipRevealed;
            }

            ship.IsHit = true;
            ship.Speed = 0;
        }

        public bool MoveShip(Ship ship)
        {
            int xx, yy;

            List<(int X, int Y)> oldCoordinates = ship.Coordinates;
            List<(int X, int Y)> newCoordinates = new List<(int X, int Y)>();
            for (int i = 0; i < ship.Coordinates.Count; i++)
            {
                (xx, yy) = ship.Coordinates[i];

                if (ship.IsHorizontal)
                    xx += ship.Speed;
                else
                    yy += ship.Speed;

                if (IsOutOfBounds(xx, yy))
                    return false;

                newCoordinates.Add((X: xx, Y: yy));
            }

            if (Colliding(ship, newCoordinates))
                return false;

            ship.Coordinates = newCoordinates;
            ship.UpdateArea();
            return true;
        }

        public void MoveShips()
        {
            foreach (var ship in Ships)
            {
                if (ship.IsHit)
                    continue;

                if (!MoveShip(ship))
                    ship.Speed *= -1;
            }
        }

        public bool Colliding(Ship ship, List<(int X, int Y)> coordinates)
        {
            foreach (var other in Ships)
            {
                if (ship.Equals(other))
                    continue;

                if (other.Intersects(coordinates))
                    return true;
            }
            return false;
        }

        public void PlaceHints()
        {
            foreach (var ship in Ships)
            {
                if (ship.IsHit || Game.Rand.Next(2) == 0)
                    continue;

                int index = Game.Rand.Next(ship.Coordinates.Count);
                (int x, int y) = ship.Coordinates[index];
                map[x, y] = (int)Status.Hint;
            }
        }
    }
}