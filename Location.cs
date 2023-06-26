using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Newtonsoft.Json;

namespace WDS_Dispatches
{
    public class CubeHex {
        public double Q { get; set; }
        public double R { get; set; }
        public double S { get; set; }

        public static double lerp (double a, double b, double t) {
            return a + t*(b - a);
        }

        public static CubeHex CubeLerp(CubeHex a, CubeHex b, double t) {
            return new CubeHex(
                lerp(a.Q, b.Q, t),
                lerp(a.R, b.R, t),
                lerp(a.S, b.S, t)
            );
        }

        public CubeHex(double q, double r, double s) {
            Q = q;
            R = r;
            S = s;
        }

        public CubeHex(Location l) {
            this.Q = (double)l.X;
            this.R = (double)(l.Y - (l.X + (l.X & 1)) / 2);
            this.S = (double)(-this.Q - this.R);
        }

        public CubeHex Subtract(CubeHex b) {
            return new CubeHex(
                this.Q - b.Q, 
                this.R - b.R, 
                this.S - b.S
            ); 
        }

        public int DistanceTo(CubeHex b){
            CubeHex vec = this.Subtract(b);

            return ((int)(Math.Abs(vec.Q) + Math.Abs(vec.R) + Math.Abs(vec.S))) / 2;
        }

        public CubeHex MoveTowards(CubeHex b, int steps) {
            int dist = this.DistanceTo(b);

            if(dist == 0) {
                return this;
            }

            return CubeLerp(
                this, 
                b, 
                (double)(1.0 / dist) * Math.Min(steps, dist)
            );
        }

        public CubeHex Round() {
            double q = Math.Round(this.Q);
            double r = Math.Round(this.R);
            double s = Math.Round(this.S);

            double q_diff = Math.Abs(q - this.Q);
            double r_diff = Math.Abs(r - this.R);
            double s_diff = Math.Abs(s - this.S);

            if (q_diff > r_diff && q_diff > s_diff) {
                q = -r - s;
            } else if (r_diff > s_diff) {
                r = -q - s;
            } else {
                s = -q - r;
            }

            return new CubeHex(q, r, s);
        }

        public Location ToLocation() {
            CubeHex rounded = this.Round();

            int q = (int)rounded.Q;
            int r = (int)rounded.R;

            return new Location(
                q,
                r + (q + (q & 1)) / 2
            );
        }
    }

    public class Location {
        public int X { get; set; }
        public int Y { get; set; }
        public override string ToString() { 
            if (!IsPresent()) {
                return "(not present)";
            }

            return "(" + X + ", " + Y + ")"; 
        }

        public Location(int x = -1, int y = -1)
        {
            Set(x, y);
        }

        public bool IsPresent() {
            return (X != -1 && Y != -1);
        }

        private static Location evenq_to_axial(Location hex) {
            int q = hex.X;
            int r = hex.Y - (hex.X + (hex.X & 1)) / 2;
            return new Location(q, r);
        }

        private static int axial_distance(Location a, Location b) {
            return (
                Math.Abs(a.X - b.X) +
                Math.Abs(a.X + a.Y - b.X - b.Y) +
                Math.Abs(a.Y - b.Y)
            ) / 2;
        }

        public void Set(int x, int y) {
            X = x;
            Y = y;
        }

        public int DistanceTo(Location b) {
            if(!IsPresent() || !b.IsPresent()) {
                return 999999;
            }

            Location a_axial = evenq_to_axial(this);
            Location b_axial = evenq_to_axial(b);

            return axial_distance(a_axial, b_axial);
        }

        public bool Equals(Location other) {
            return this.X == other.X && this.Y == other.Y;
        }

        public Location MoveTowards(Location b, int steps) {
            CubeHex cube_a = new CubeHex(this);
            CubeHex cube_b = new CubeHex(b);

            return cube_a.MoveTowards(cube_b, steps).ToLocation();
        }
    }
}
