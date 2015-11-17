using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DecalFramework {
    /*
     * Contains Hull Tests and Re-triangulation code. All effort has been
     * made to try and minimize garbage collection as these operations
     * may occur multiple times per frame.
     */
    public class HullTests {
        /*
         * Represents a transformed map point with a point reference. Used
         * internally for triangulation
         */
        public class MapPoint2D {
            private Vector2 map;
            private Vector3 ptRef;

            public Vector2 Map {
                get { return this.map; }
                set { this.map = value; }
            }

            public Vector3 PointRef {
                get { return this.ptRef; }
                set { this.ptRef = value; }
            }
        }

        /*
         * Computes the Barycentric coordinates of triangles a-b-c in regards to point p and
         * stores results in reference u-v-w respectively
         */
        public static void Barycentric(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p, ref float u, ref float v, ref float w) {
            Vector3 m = Vector3.Cross(b - a, c - a);

            float nu;
            float nv;
            float ood;

            float x = Mathf.Abs(m.x);
            float y = Mathf.Abs(m.y);
            float z = Mathf.Abs(m.z);

            // compute areas of plane with largest projections
            if (x >= y && x >= z) {
                // area of PBC in yz plane
                nu = TriArea2D(p.y, p.z, b.y, b.z, c.y, c.z);
                // area of PCA in yz plane
                nv = TriArea2D(p.y, p.z, c.y, c.z, a.y, a.z);
                // 1/2*area of ABC in yz plane
                ood = 1.0f / m.x;
            } else if (y >= x && y >= z) {
                // project in xz plane
                nu = TriArea2D(p.x, p.z, b.x, b.z, c.x, c.z);
                nv = TriArea2D(p.x, p.z, c.x, c.z, a.x, a.z);
                ood = 1.0f / -m.y;
            } else {
                // project in xy plane
                nu = TriArea2D(p.x, p.y, b.x, b.y, c.x, c.y);
                nv = TriArea2D(p.x, p.y, c.x, c.y, a.x, a.y);
                ood = 1.0f / m.z;
            }

            u = nu * ood;
            v = nv * ood;
            w = 1.0f - u - v;
        }

        public static void BarycentricSign(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 p, ref float u, ref float v, ref float w) {
            Vector3 f1 = a - p;
            Vector3 f2 = b - p;
            Vector3 f3 = c - p;

            Vector3 va = Vector3.Cross(a - b, a - c);
            Vector3 va1 = Vector3.Cross(f2, f3);
            Vector3 va2 = Vector3.Cross(f3, f1);
            Vector3 va3 = Vector3.Cross(f1, f2);

            float area = va.magnitude;

            u = va1.magnitude / area * Mathf.Sign(Vector3.Dot(va, va1));
            v = va2.magnitude / area * Mathf.Sign(Vector3.Dot(va, va2));
            w = va3.magnitude / area * Mathf.Sign(Vector3.Dot(va, va3));
        }

        private static List<MapPoint2D> pt2 = new List<MapPoint2D>();
        private static List<MapPoint2D> H = new List<MapPoint2D>();

        /*
         * Construct a Convex Hull from given points pt and a direction. Resulted Vertices are stored
         * in output. This algorithm will map 3D points to a plane represented by the normal.
         */
        public static void ConvexHull2D(List<Vector3> pt, List<Vector3> output, ref Vector3 norm) {
            MapPlanar3D2D(pt, pt2, ref norm);

            pt2.Sort((x, p) =>
            {
                return (x.Map.x < p.Map.x || (x.Map.x == p.Map.x && x.Map.y < p.Map.y)) ? -1 : 1;
            });

            int n = pt.Count;
            int k = 0;

            /*
             * Build the lower HULL
             */
            for (int i = 0; i < n; i++) {
                while (k >= 2 && TriArea2D(H[k - 2].Map.x, H[k - 2].Map.y, H[k - 1].Map.x, H[k - 1].Map.y, pt2[i].Map.x, pt2[i].Map.y) <= 0) k--;

                H.Insert(k++, pt2[i]);
            }

            /*
             * Build the upper HULL
             */
            for (int i = n - 2, t = k + 1; i >= 0; i--) {
                while (k >= t && TriArea2D(H[k - 2].Map.x, H[k - 2].Map.y, H[k - 1].Map.x, H[k - 1].Map.y, pt2[i].Map.x, pt2[i].Map.y) <= 0) k--;

                H.Insert(k++, pt2[i]);
            }

            for (int i = 0; i < k - 1; i++) {
                output.Add(H[i].PointRef);
            }
        }

        private static List<Vector3> opt = new List<Vector3>();

        public static void ConvexHull2DTriangulated(List<Vector3> pt, List<Vector3> tri, ref Vector3 norm) {
            NearestPointTest.triangulate(pt, tri, norm);
            /*opt.Clear();

            ConvexHull2D(pt, opt, ref norm);

            if (opt.Count < 3) {
                return;
            }

            Vector3 line1 = pt[0];

            int count = pt.Count - 1;

            for (int i = 1; i < count; i++) {
                tri.Add(line1);
                tri.Add(pt[i]);
                tri.Add(pt[i + 1]);
            }*/
        }

        /*
        * Project 3D points in List p3 into 2D points onto a virtual plane described by the normal n
        */
        private static void MapPlanar3D2D(List<Vector3> p3, List<MapPoint2D> p2, ref Vector3 n) {
            Vector3 r = Mathf.Abs(n.x) > Mathf.Abs(n.y) ? r = new Vector3(0, 1, 0) : r = new Vector3(1, 0, 0);

            Vector3 v = Vector3.Normalize(Vector3.Cross(r, n));
            Vector3 u = Vector3.Cross(n, v);

            int difference = p3.Count - p2.Count;

            for (int i = 0; i < difference; i++) {
                p2.Add(new MapPoint2D());
            }

            int count = p3.Count;

            for (int i = 0; i < count; i++) {
                MapPoint2D v2 = p2[i];

                v2.Map = new Vector2(Vector3.Dot(p3[i], u), Vector3.Dot(p3[i], v));
                v2.PointRef = p3[i];
            }
        }

        private static List<MapPoint2D> genUVM = new List<MapPoint2D>();

        /*
         * Generates UV Coordinates and stores them in list gen. The list gen will not be cleared, it will
         * only be appended.
         */
        public static void GenUVFromVertex(List<Vector3> vertices, List<Vector2> genUV, ref Vector3 n) {
            genUVM.Clear();

            MapPlanar3D2D(vertices, genUVM, ref n);

            // we now have 2D points, lets generate our UV coordinates
            // find the largest point in either X or Y coordinate to bring everything back to UV space

            float divX = 1.0f;
            float divY = 1.0f;

            for (int i = 0; i < genUVM.Count; i++) {
                if (genUVM[i].Map.x > divX) {
                    divX = genUVM[i].Map.x;
                }

                if (genUVM[i].Map.y > divY) {
                    divY = genUVM[i].Map.y;
                }
            }

            divX = (divX + 1 / 2);
            divY = (divY + 1 / 2);

            // now its a simple manner, U = x / div, V = y / div
            for (int i = 0; i < genUVM.Count; i++) {
                genUV.Add(new Vector2(genUVM[i].Map.x / divX - 0.5f, genUVM[i].Map.y / divY - 0.5f));
            }
        }

        /*
         * Perform a Cube mapping to map an image across a 3D Cube
         */
        public static Vector2 CubeMap3DV(ref Vector3 vec, ref Vector3 n) {
            Vector3 r = Mathf.Abs(n.x) > Mathf.Abs(n.y) ? r = new Vector3(0, 1, 0) : r = new Vector3(1, 0, 0);

            Vector3 v = Vector3.Normalize(Vector3.Cross(r, n));
            Vector3 u = Vector3.Cross(n, v);

            return new Vector2((Vector3.Dot(vec, u) + 1) / 2, (Vector3.Dot(vec, v) + 1) / 2);
        }

        public static bool IsTriClockwise(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 normal) {
            Vector3 n = Vector3.Cross(b - a, c - a);
            return Vector3.Dot(normal, n) >= 0;
        }

        public static bool IsTriClockwise(Vector3 a, Vector3 b, Vector3 c, Vector3 normal) {
            return IsTriClockwise(ref a, ref b, ref c, ref normal);
        }

        /*
         * Computes and returns the area of the 3D triangle from a-b-c
         */
        public static float TriArea3D(ref Vector3 a, ref Vector3 b, ref Vector3 c) {
            return Vector3.Cross(a - b, a - c).magnitude * 0.5f;
        }

        /*
         * Computes and returns the projected area of Triangles with points
         */
        public static float TriArea2D(float x1, float y1, float x2, float y2, float x3, float y3) {
            return (x1 - x2) * (y2 - y3) - (x2 - x3) * (y1 - y2);
        }
    }
}