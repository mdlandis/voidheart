using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidheart {
    public enum Direction {
        CARDINAL,
        DIAGONAL,
        OMNIDIRECTIONAL,
        NONE
    }

    public enum RotationAngle {
        NONE,
        NINETY,
        ONEEIGHTY,
        TWOSEVENTY
    }

    [System.Serializable]
    public struct Coord {
        public int x, y;
        public bool isNull;

        public static Coord[] cardinalCoords = {
            new Coord(1, 0),
            new Coord(0, 1),
            new Coord(-1, 0),
            new Coord(0, -1)
        };
        public static Coord[] diagonalCoords = {
            new Coord(1, 1),
            new Coord(-1, 1),
            new Coord(-1, -1),
            new Coord(1, -1)
        };
        public static Coord nullCoord = new Coord(0, 0, true);

        public Coord(int p1, int p2) {
            x = p1;
            y = p2;
            isNull = false;
        }

        public Coord(int p1, int p2, bool nullity) {
            x = p1;
            y = p2;
            isNull = nullity;
        }

        public int Distance(Coord c1) {
            return (int) (Mathf.Abs(c1.x - x) + Mathf.Abs(c1.y - y));
        } //Distance is given using Manhattan Norm.

        public static Coord operator +(Coord c1, Coord c2) {
            return new Coord(c1.x + c2.x, c1.y + c2.y);
        }

        public static Coord operator -(Coord c1, Coord c2) {
            return new Coord(c1.x - c2.x, c1.y - c2.y);
        }

        public static Coord operator *(Coord c1, int scalar) {
            return new Coord(c1.x * scalar, c1.y * scalar);
        }

        public static bool operator ==(Coord c1, Coord c2) {
            return (c1.x == c2.x && c1.y == c2.y && c1.isNull == c2.isNull);
        }

        public List<Coord> GetSurroundingCoords(bool cardinal = true, bool diagonal = true) {
            int length = (cardinal ? cardinalCoords.Length : 0) + (diagonal ? diagonalCoords.Length : 0);
            var coords = new List<Coord>();
            if (cardinal) {
                foreach (Coord c in cardinalCoords) {
                    coords.Add(this + c);
                }
            }

            if (diagonal) {
                foreach (Coord c in cardinalCoords) {
                    coords.Add(this + c);
                }
            }

            return coords;
        }

        public static bool operator !=(Coord c1, Coord c2) {
            return !(c1 == c2);
        }

        public static Coord Rotate(Coord c, RotationAngle r) {
            switch (r) {
                case RotationAngle.NONE:
                    return new Coord(c.x, c.y);
                case RotationAngle.NINETY:
                    return new Coord(-c.y, c.x);
                case RotationAngle.ONEEIGHTY:
                    return new Coord(-c.x, -c.y);
                case RotationAngle.TWOSEVENTY:
                    return new Coord(c.y, -c.x);
                default:
                    // this should never happen
                    return new Coord(0, 0, true);
            }
        }

        public override bool Equals(System.Object obj) {
            if (obj == null) {
                return false;
            }

            Coord c = (Coord) obj;
            return (this == c);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return "(" + x.ToString() + ", " + y.ToString() + ")";
        }

        public static Coord GetDirection(Coord fromCoord, Coord toCoord) {
            int x = toCoord.x - fromCoord.x;
            int y = toCoord.y - fromCoord.y;
            Coord dir = new Coord(System.Math.Sign(x), System.Math.Sign(y));
            return dir;
        }

        public static Coord[] ResolveRelativeCoords(Coord[] coords, Coord offset) {
            Coord[] returnValue = new Coord[coords.Length];
            for (int i = 0; i < coords.Length; ++i) {
                returnValue[i] = coords[i] + offset;
            }
            return returnValue;
        }
    }
}