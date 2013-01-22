#region

using System;

#endregion

namespace Ship.Game.WorldGeneration.Noise
{
   public class SimplexNoise2
   {

       private static int _i;
       private static int _j;
       private static int k;
       private static readonly int[] A = new int[] {0, 0, 0};
    private static float _u, _v, w, s;
    private const float onethird = 0.333333333f;
    private const float onesixth = 0.166666667f;
    private static readonly int[] T = new []{0x15, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a};
 
    // returns a value in the range of about [-0.347 .. 0.347]
    public static float Noise(float x, float y, float z) {
        // Skew input space to relative coordinate in simplex cell
        s = (x + y + z) * onethird;
        _i = fastfloor(x+s);
        _j = fastfloor(y+s);
        k = fastfloor(z+s);
 
        // Unskew cell origin back to (x, y , z) space
        s = (_i + _j + k) * onesixth;
        _u = x - _i + s;
        _v = y - _j + s;
        w = z - k + s;;
 
        A[0] = A[1] = A[2] = 0;
 
        // For 3D case, the simplex shape is a slightly irregular tetrahedron.
        // Determine which simplex we're in
        int hi = _u >= w ? _u >= _v ? 0 : 1 : _v >= w ? 1 : 2;
        int lo = _u < w ? _u < _v ? 0 : 1 : _v < w ? 1 : 2;

        return kMethod(hi) + kMethod(3 - hi - lo) + kMethod(lo) + kMethod(0);
    }
 
    private static int fastfloor(float n) {
        return n > 0 ? (int) n : (int) n - 1;
    }
 
    private static float kMethod(int a) 
    {
        s = (A[0] + A[1] + A[2]) * onesixth;
        float x = _u - A[0] + s;
        float y = _v - A[1] + s;
        float z = w - A[2] + s;
        float t = 0.6f - x * x - y * y - z * z;
        int h = shuffle(_i + A[0], _j + A[1], k + A[2]);
        A[a]++;
        if (t < 0) return 0;
        int b5 = h >> 5 & 1;
        int b4 = h >> 4 & 1;
        int b3 = h >> 3 & 1;
        int b2 = h >> 2 & 1;
        int b = h & 3;
        float p = b == 1 ? x : b == 2 ? y : z;
        float q = b == 1 ? y : b == 2 ? z : x;
        float r = b == 1 ? z : b == 2 ? x : y;
        p = b5 == b3 ? -p : p;
        q = b5 == b4 ? -q: q;
        r = b5 != (b4^b3) ? -r : r;
        t *= t;
        return 8 * t * t * (p + (b == 0 ? q + r : b2 == 0 ? q : r));
    }
 
    private static int shuffle(int i, int j, int k) {
        return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) +
               b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
    }
 
    private static int b(int i, int j, int k, int B) {
        return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
    }
 
    private static int b(int N, int B) {
        return N >> B & 1;
    }
 
}
}