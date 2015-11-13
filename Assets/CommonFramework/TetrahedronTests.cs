using UnityEngine;
using System.Collections;

namespace DecalFramework {

    /*
     * Contains tests performed on Tetrahedrons
     */
    public class TetrahedronTests {
        /*
         * for tetrahedron a-b-c-d return a point q in tetrahedron that is closest to p
         */
        public static void ClosestPointTetrahedron(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, ref Vector3 p, ref Vector3 q) {
            q = p;

            Vector3 closestPoint = new Vector3();

            float bestSqDist = float.MaxValue;

            // a-b-c
            if (PointOutsideOfPlane(ref p, ref a, ref b, ref c)) {
                TriangleTests.ClosestPointTriangle(ref a, ref b, ref c, ref p, ref closestPoint);

                float sqDist = (q - p).sqrMagnitude;

                if (sqDist < bestSqDist) {
                    bestSqDist = sqDist;
                    q = closestPoint;
                }
            }

            // a-c-d
            if (PointOutsideOfPlane(ref p, ref a, ref c, ref d)) {
                TriangleTests.ClosestPointTriangle(ref a, ref c, ref d, ref p, ref closestPoint);

                float sqDist = (q - p).sqrMagnitude;

                if (sqDist < bestSqDist) {
                    bestSqDist = sqDist;
                    q = closestPoint;
                }
            }

            // a-d-b
            if (PointOutsideOfPlane(ref p, ref a, ref d, ref b)) {
                TriangleTests.ClosestPointTriangle(ref a, ref d, ref b, ref p, ref closestPoint);

                float sqDist = (q - p).sqrMagnitude;

                if (sqDist < bestSqDist) {
                    bestSqDist = sqDist;
                    q = closestPoint;
                }
            }

            // b-d-c
            if (PointOutsideOfPlane(ref p, ref b, ref d, ref c)) {
                TriangleTests.ClosestPointTriangle(ref b, ref d, ref c, ref p, ref closestPoint);

                float sqDist = (q - p).sqrMagnitude;

                if (sqDist < bestSqDist) {
                    bestSqDist = sqDist;

                    q = closestPoint;
                }
            }
        }

        private static bool PointOutsideOfPlane(ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c) {
            return Vector3.Dot(p - a, Vector3.Cross(b - a, c - a)) >= 0.0f;
        }
    }
}
