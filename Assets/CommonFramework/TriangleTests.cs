using UnityEngine;
using System.Collections;

namespace DecalFramework {

    /*
     * Contains Tests against Triangles. All effort has been made
     * to try and reduce the amount of garbage being generated.
     */
    public class TriangleTests {
        /*
         * For triangle composed of a-b-c, stores a point in q in the triangle that is 
         * losest to p
         */
        public static void ClosestPointTriangle(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p, ref Vector3 q) {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = p - a;

            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);

            if (d1 <= 0.0f && d2 <= 0.0f) {
                q = a;

                return;
            }

            Vector3 bp = p - b;

            float d3 = Vector3.Dot(ab, bp);
            float d4 = Vector3.Dot(ac, bp);

            if (d3 >= 0.0f && d4 <= d3) {
                q = b;

                return;
            }

            float vc = d1 * d4 - d3 * d2;

            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f) {
                float v = d1 / (d1 - d3);

                q =  a + v * ab;

                return;
            }

            Vector3 cp = p - c;

            float d5 = Vector3.Dot(ab, cp);
            float d6 = Vector3.Dot(ac, cp);

            if (d6 >= 0.0f && d5 <= d6) {
                q = c;

                return;
            }

            float vb = d5 * d2 - d1 * d6;

            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f) {
                float w = d2 / (d2 - d6);

                q = a + w * ac;

                return;
            }

            float va = d3 * d6 - d5 * d4;

            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f) {
                float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));

                q =  b + w * (c - b);

                return;
            }

            float denom = 1.0f / (va + vb + vc);
            float vn = vb * denom;
            float wn = vc * denom;

            q =  a + ab * vn + ac * wn;
        }

        /*
         * Check to ensure the point p is contained in the triangle a-b-c. Returns true or false
         */
        public static bool PointInTriangle(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p) {
            Vector3 ca = a - p;
            Vector3 cb = b - p;
            Vector3 cc = c - p;

            float fab = Vector3.Dot(ca, cb);
            float fac = Vector3.Dot(ca, cc);
            float fbc = Vector3.Dot(cb, cc);
            float fcc = Vector3.Dot(cc, cc);

            if (fbc * fac - fcc * fab < 0.0f) {
                return false;
            }

            float bb = Vector3.Dot(cb, cb);

            if (fab * fbc - fac * bb < 0.0f) {
                return false;
            }

            return true;
        }

        /*
         * Intersect the line made by p-q on triangle made by a-b-c and store the intersection point in r. Returns true
         * if there was an intersection
         */
        public static bool IntersectLineTriangle(ref Vector3 p, ref Vector3 q, ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 r) {
            Vector3 pq = q - p;
            Vector3 pa = a - p;
            Vector3 pb = b - p;
            Vector3 pc = c - p;

            Vector3 m = Vector3.Cross(pq, pc);

            float u = Vector3.Dot(pb, m);
            float v = -Vector3.Dot(pa, m);

            if (Mathf.Sign(u) != Mathf.Sign(v)) {
                return false;
            }

            // scalar triple product
            float w = Vector3.Dot(pq, Vector3.Cross(pb, pa));

            if (Mathf.Sign(u) != Mathf.Sign(w)) {
                return false;
            }

            float denom = 1.0f / (u + v + w);

            r = ((u * denom) * a) + ((v * denom) * b) + ((w * denom) * c);

            return true;
        }
    }
}
