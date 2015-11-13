using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DecalFramework {
    [System.Serializable]
    public class NDPlane {

        public const float ERROR_TOL = 0.001f;

        private Vector3 n;
        private float d;

        public NDPlane() {}

        public NDPlane(Vector3 dir, Vector3 point) {
            SetValues(dir, point);
        }

        public void SetValues(Vector3 dir, Vector3 point) {
            this.n = dir;
            this.d = Vector3.Dot(n, point);
        }

        public Vector3 Normal {
            get { return this.n; }
        }

        public float Direction {
            get { return this.d; }
        }

        /*
         * Test and return which side of this infinite ND Plane
         * the point PT is on. Takes a reference value.
         */
        public bool SideOfPlane(ref Vector3 pt) {
            return Vector3.Dot(n, pt) >= d - ERROR_TOL;
        }

        /*
         * Test and return which side of this infinite ND Plane
         * the point PT is on. Takes a reference value. Include custom tolerance value
         */
        public bool SideOfPlane(ref Vector3 pt, float tolerance) {
            return Vector3.Dot(n, pt) >= d - tolerance;
        }

        /*
         * Intersects the line made from Points a and b. Returns true
         * If an intersection is found, otherwise returns false.
         */
        public bool IntersectLine(ref Vector3 a, ref Vector3 b) {
            Vector3 ab = b - a;

            float t = (d - Vector3.Dot(n, a)) / Vector3.Dot(n, ab);

            return t >= -ERROR_TOL && t <= (1.000f + ERROR_TOL);
        }

        /*
         * Intersects the line made from Points a and b. Returns true
         * If an intersection is found, otherwise returns false. The intersection
         * Point will be stored in q.
         */
        public bool IntersectLine(ref Vector3 a, ref Vector3 b, ref Vector3 q) {
            Vector3 ab = b - a;

            float t = (d - Vector3.Dot(n, a)) / Vector3.Dot(n, ab);

            if (t >= -ERROR_TOL && t <= (1.000f + ERROR_TOL)) {
                q = a + t * ab;

                return true;
            }

            return false;
        }

        private static Vector3 intersectionPoint = new Vector3();

        /*
         * Intersect the triangle composed of points a-b-c and store potential (maximum of 2) intersection
         * points in List fptr. The number of intersections will be returned as a result.
         * NOTE: List fpts will not be cleared, previous points in the list will remain.
         */
        public int IntersectTriangle(ref Vector3 a, ref Vector3 b, ref Vector3 c, List<Vector3> fpts) {
            int intersectionCounter = 0;

            // test segment a-b
            if (IntersectLine(ref a, ref b, ref intersectionPoint)) {
                intersectionCounter++;

                fpts.Add(intersectionPoint);
            }

            // test segment a-c
            if (IntersectLine(ref a, ref c, ref intersectionPoint)) {
                intersectionCounter++;

                fpts.Add(intersectionPoint);
            }

            // test segment b-c
            if (IntersectLine(ref b, ref c, ref intersectionPoint)) {
                intersectionCounter++;

                fpts.Add(intersectionPoint);
            }

            return intersectionCounter;
        }

        /*
         * Check if point p lies inside the inner bounds of all six planes
         */
        public static bool SideOfSixPlanes(NDPlane p1, NDPlane p2, NDPlane p3, NDPlane p4, NDPlane p5, NDPlane p6, ref Vector3 p) {
            return p1.SideOfPlane(ref p) &&
                    p2.SideOfPlane(ref p) &&
                    p3.SideOfPlane(ref p) &&
                    p4.SideOfPlane(ref p) &&
                    p5.SideOfPlane(ref p) &&
                    p6.SideOfPlane(ref p);
        }

        /*
         * Check if points contained in the list lpts are inside the inner bounds of all six planes. If they are, they will
         * be stored in the list fpts. NOTE: List fpts will not be cleared, previous points in the list will remain.
         */
        public static void SideOfSixPlanesFilter(NDPlane p1, NDPlane p2, NDPlane p3, NDPlane p4, NDPlane p5, NDPlane p6, List<Vector3> lpts, List<Vector3> fpts) {
            int count = lpts.Count;

            if (count == 0) {
                return;
            }

            for (int i = 0; i < count; i++) {
                Vector3 pt = lpts[i];

                if (SideOfSixPlanes(p1, p2, p3, p4, p5, p6, ref pt)) {
                    fpts.Add(pt);
                }
            }
        }

        /*
         * Performs an intersection test of triangle made from p1 - p2 - p3 and the Cube made from six ND-Planes. All
         * Intersection results (and contained in the ND-Cube) will be stored in fpts. NOTE: List fpts will not be cleared
         * and previous points in the list will remain.
         */
        public static void IntersectSixPlanesTriangle(NDPlane yPosPlane, NDPlane yNegPlane, NDPlane xPosPlane, NDPlane xNegPlane, NDPlane zPosPlane, NDPlane zNegPlane,
                                                        ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, List<Vector3> fpts) {
            /*
             * Quick Exit Code - If the triangle p1-p2-p3 is completly contained in the Cube, no intersection
             * is required, simply re-add all the points and return
             */
            int inTris = 0;

            if (SideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, ref p1)) {
                fpts.Add(p1);

                inTris++;
            }

            if (SideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, ref p2)) {
                fpts.Add(p2);

                inTris++;
            }

            if (SideOfSixPlanes(yPosPlane, yNegPlane, xPosPlane, xNegPlane, zPosPlane, zNegPlane, ref p3)) {
                fpts.Add(p3);

                inTris++;
            }

            if (inTris == 3) {
                return;
            }

            yPosPlane.IntersectTriangle(ref p1, ref p2, ref p3, fpts);
            yNegPlane.IntersectTriangle(ref p1, ref p2, ref p3, fpts);
            xPosPlane.IntersectTriangle(ref p1, ref p2, ref p3, fpts);
            xNegPlane.IntersectTriangle(ref p1, ref p2, ref p3, fpts);
            zPosPlane.IntersectTriangle(ref p1, ref p2, ref p3, fpts);
            zNegPlane.IntersectTriangle(ref p1, ref p2, ref p3, fpts);
        }
    }
}
