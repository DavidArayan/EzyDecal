using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DecalFramework {

    /*
     * This class contains functionality for modifying existing meshes
     */
    public class MeshModifiers {
        public const float POS_THRESH = 0.001f;
        public const float NEG_THRESH = -POS_THRESH;

        private static List<Mesh> tmpMeshes = new List<Mesh>();

        // used to store the vertices in the left cut
        private static List<Vector3> leftCutVert = new List<Vector3>();

        // used to store the trianggles in left cut - subindex1
        private static Dictionary<int, List<int>> leftCutTri = new Dictionary<int, List<int>>();

        // used for adding new submesh of cut cross section
        private static List<int> leftCutTri2 = new List<int>();

        // used to store the generated UV coordinates for left cut
        private static List<Vector2> leftCutUV = new List<Vector2>();

        // used to store vertex normals for the left cut
        private static List<Vector3> leftCutNorm = new List<Vector3>();

        // temporary storage for triangulator
        private static List<Vector3> leftTriPT = new List<Vector3>();

        // used to store the vertices in the right cut
        private static List<Vector3> rightCutVert = new List<Vector3>();

        // used to store the triangles in right cut
        private static Dictionary<int, List<int>> rightCutTri = new Dictionary<int, List<int>>();

        // used for adding new submesh of cut cross section
        private static List<int> rightCutTri2 = new List<int>();

        // used to store the generated UV coordinates for right cut
        private static List<Vector2> rightCutUV = new List<Vector2>();

        // used to store vertex normals for the left cut
        private static List<Vector3> rightCutNorm = new List<Vector3>();

        // temporary storage for triangulator
        private static List<Vector3> rightTriPT = new List<Vector3>();

        private static NDPlane planeEQ = new NDPlane();

        private static List<Vector3> closedTri = new List<Vector3>();
        private static List<Vector3> tri = new List<Vector3>();

        private static List<Vector3> intersectionPoints = new List<Vector3>();
        private static List<Vector2> N_UV = new List<Vector2>();

        public static void SliceMeshes(List<SlicerPlane> planes, List<Mesh> meshes, GameObject go, bool isHallow) {
            // all states must be zero
            leftCutVert.Clear();
            leftCutUV.Clear();
            leftCutNorm.Clear();

            rightCutVert.Clear();
            rightCutUV.Clear();
            rightCutNorm.Clear();

            closedTri.Clear();
            tmpMeshes.Clear();
            tri.Clear();

            for (int j = 0; j < planes.Count; j++) {
                // we first need to bring the plane into the coordinate frame of our object
                // these are our original points
                Vector3 position = planes[j].transform.position;
                Vector3 direction = planes[j].transform.TransformDirection(Vector3.up);

                // transform them to our coordinates for proper intersection testing
                Vector3 tPosition = go.transform.InverseTransformPoint(position);
                Vector3 tDirection = go.transform.InverseTransformDirection(direction);

                // setup our NDPlane for intersection
                planeEQ.SetValues(tDirection, tPosition);

                // planeEQ is now in our objects coordinate frame
                // we can now perform our intersection tests
                // this pass will create a new Mesh assembly - clear old data

                // if the count is 0, we need to slice our current mesh
                // we need to slice our original meshes

                for (int k = 0; k < meshes.Count; k++) {
                    Mesh mesh = meshes[k];

                    leftCutVert.Clear();
                    leftCutUV.Clear();
                    leftCutTri2.Clear();
                    leftCutNorm.Clear();

                    rightCutVert.Clear();
                    rightCutUV.Clear();
                    rightCutTri2.Clear();
                    rightCutNorm.Clear();

                    closedTri.Clear();
                    // otherwise grab mesh and start performing full intersections

                    // we want to compute each submesh seperately. - each will be added seperately
                    for (int subIndex = 0; subIndex < mesh.subMeshCount; subIndex++) {
                        // begin looping through all the triangles

                        // clear previous submesh triangle data from leftCut
                        if (leftCutTri.ContainsKey(subIndex)) {
                            leftCutTri[subIndex].Clear();
                        } 
                        else {
                            leftCutTri.Add(subIndex, new List<int>());
                        }

                        // clear previous submesh triangle data from rightCut
                        if (rightCutTri.ContainsKey(subIndex)) {
                            rightCutTri[subIndex].Clear();
                        } 
                        else {
                            rightCutTri.Add(subIndex, new List<int>());
                        }

                        List<int> leftCutTri1 = leftCutTri[subIndex];
                        List<int> rightCutTri1 = rightCutTri[subIndex];

                        // grab triangles from current submesh
                        int[] meshTriangles = mesh.GetTriangles(subIndex);

                        Vector3[] mVerts = mesh.vertices;
                        Vector2[] mUV = mesh.uv;
                        Vector3[] mNormals = mesh.normals;

                        for (int i = 0; i < meshTriangles.Length; i += 3) {
                            Vector3 a = mVerts[meshTriangles[i + 0]];
                            Vector3 b = mVerts[meshTriangles[i + 1]];
                            Vector3 c = mVerts[meshTriangles[i + 2]];

                            // grab original UV coordinates
                            Vector2 uva = mUV[meshTriangles[i + 0]];
                            Vector2 uvb = mUV[meshTriangles[i + 1]];
                            Vector2 uvc = mUV[meshTriangles[i + 2]];

                            // grab the original normals
                            Vector3 na = mNormals[meshTriangles[i + 0]];
                            Vector3 nb = mNormals[meshTriangles[i + 1]];
                            Vector3 nc = mNormals[meshTriangles[i + 2]];

                            Vector3 triN = Vector3.Cross(b - a, c - a);
                            triN.Normalize();

                            // clear our previous triangle buffer
                            leftTriPT.Clear();
                            rightTriPT.Clear();

                            // lets see if this triangle is actually capable of intersection
                            if (planeEQ.SideOfPlane(ref a, NEG_THRESH)) {
                                rightTriPT.Add(a);
                            }

                            if (planeEQ.SideOfPlane(ref a, POS_THRESH)) {
                                leftTriPT.Add(a);
                            }

                            if (planeEQ.SideOfPlane(ref b, NEG_THRESH)) {
                                rightTriPT.Add(b);
                            }

                            if (planeEQ.SideOfPlane(ref b, POS_THRESH)) {
                                leftTriPT.Add(b);
                            }

                            if (planeEQ.SideOfPlane(ref c, NEG_THRESH)) {
                                rightTriPT.Add(c);
                            }

                            if (planeEQ.SideOfPlane(ref c, POS_THRESH)) {
                                leftTriPT.Add(c);
                            }

                            // if counter is 0 or 3, triangle cannot intersect the plane, add as is and early exit
                            if (leftTriPT.Count == 3) {
                                // add vertices to the left cut
                                int vertPosA = leftCutVert.Count;
                                leftCutVert.Add(a);
                                // add new UV coord
                                leftCutUV.Add(uva);
                                // add normal
                                leftCutNorm.Add(na);

                                int vertPosB = leftCutVert.Count;
                                leftCutVert.Add(b);
                                // add new UV coord
                                leftCutUV.Add(uvb);
                                // add normal
                                leftCutNorm.Add(nb);

                                int vertPosC = leftCutVert.Count;
                                leftCutVert.Add(c);
                                // add new UV coord
                                leftCutUV.Add(uvc);
                                // add normal
                                leftCutNorm.Add(nc);

                                // add triangles to specified indexes
                                leftCutTri1.Add(vertPosA);
                                leftCutTri1.Add(vertPosB);
                                leftCutTri1.Add(vertPosC);

                                if (isHallow) {
                                    leftCutTri2.Add(vertPosA);
                                    leftCutTri2.Add(vertPosC);
                                    leftCutTri2.Add(vertPosB);
                                }

                                leftTriPT.Clear();
                            }

                            if (rightTriPT.Count == 3) {
                                // add vertices to the right cut
                                int vertPosA = rightCutVert.Count;
                                rightCutVert.Add(a);
                                // add new UV coord
                                rightCutUV.Add(uva);
                                // add normal
                                rightCutNorm.Add(na);

                                int vertPosB = rightCutVert.Count;
                                rightCutVert.Add(b);
                                // add new UV coord
                                rightCutUV.Add(uvb);
                                // add normal
                                rightCutNorm.Add(nb);

                                int vertPosC = rightCutVert.Count;
                                // check to ensure vertex doesnt exist
                                rightCutVert.Add(c);
                                // add new UV coord
                                rightCutUV.Add(uvc);
                                // add normal
                                rightCutNorm.Add(nc);

                                // add triangles
                                rightCutTri1.Add(vertPosA);
                                rightCutTri1.Add(vertPosB);
                                rightCutTri1.Add(vertPosC);

                                if (isHallow) {
                                    rightCutTri2.Add(vertPosA);
                                    rightCutTri2.Add(vertPosC);
                                    rightCutTri2.Add(vertPosB);
                                }

                                rightTriPT.Clear();
                            }

                            // determine if we need to proceed
                            if (leftTriPT.Count > 0 || rightTriPT.Count > 0) {
                                // otherwise we have an intersection, find the points

                                // find the intersection points between plane and triangle a-b-c
                                intersectionPoints.Clear();
                                int intCounter = planeEQ.IntersectTriangle(ref a, ref b, ref c, intersectionPoints);

                                float u = 0.0f;
                                float v = 0.0f;
                                float w = 0.0f;

                                if (intCounter == 1) {
                                    Vector3 intPoint = intersectionPoints[0];

                                    leftTriPT.Add(intPoint);
                                    rightTriPT.Add(intPoint);

                                    // we also add these points for the closing the mesh
                                    closedTri.Add(intPoint);
                                } 
                                else if (intCounter == 2) {
                                    Vector3 intA = intersectionPoints[0];
                                    Vector3 intB = intersectionPoints[1];

                                    leftTriPT.Add(intA);
                                    leftTriPT.Add(intB);

                                    rightTriPT.Add(intB);
                                    rightTriPT.Add(intA);

                                    closedTri.Add(intA);
                                    closedTri.Add(intB);
                                }

                                // we have our lists, we should triangulate and add the points into our final mesh
                                // lets do the left mesh first - we shouldnt get more than 4 points per intersection
                                tri.Clear();
                                HullTests.ConvexHull2DTriangulated(leftTriPT, tri, ref triN);

                                // used for computing the barycentric coordinates
                                for (int m = 0; m < tri.Count; m += 3) {
                                    Vector3 ma = tri[m + 0];
                                    Vector3 mb = tri[m + 1];
                                    Vector3 mc = tri[m + 2];

                                    bool order = false;

                                    if (IsTriClockwise(ref ma, ref mb, ref mc, ref triN)) {
                                        order = true;
                                    }

                                    // add vertices to the left cut
                                    if (!isHallow) {
                                        int vertPosA = leftCutVert.Count;
                                        leftCutVert.Add(ma);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref ma, ref u, ref v, ref w);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutNorm.Add(triN);

                                        int vertPosB = leftCutVert.Count;
                                        leftCutVert.Add(mb);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mb, ref u, ref v, ref w);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutNorm.Add(triN);

                                        int vertPosC = leftCutVert.Count;
                                        leftCutVert.Add(mc);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mc, ref u, ref v, ref w);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutNorm.Add(triN);

                                        if (order) {
                                            leftCutTri1.Add(vertPosA);
                                            leftCutTri1.Add(vertPosB);
                                            leftCutTri1.Add(vertPosC);
                                        } 
                                        else {
                                            leftCutTri1.Add(vertPosA);
                                            leftCutTri1.Add(vertPosC);
                                            leftCutTri1.Add(vertPosB);
                                        }
                                    } 
                                    else {
                                        int vertPosA1 = leftCutVert.Count;
                                        leftCutVert.Add(ma);
                                        int vertPosA2 = leftCutVert.Count;
                                        leftCutVert.Add(ma);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref ma, ref u, ref v, ref w);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutNorm.Add(triN);
                                        leftCutNorm.Add(-triN);

                                        int vertPosB1 = leftCutVert.Count;
                                        leftCutVert.Add(mb);
                                        int vertPosB2 = leftCutVert.Count;
                                        leftCutVert.Add(mb);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mb, ref u, ref v, ref w);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutNorm.Add(triN);
                                        leftCutNorm.Add(-triN);

                                        int vertPosC1 = leftCutVert.Count;
                                        leftCutVert.Add(mc);
                                        int vertPosC2 = leftCutVert.Count;
                                        leftCutVert.Add(mc);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mc, ref u, ref v, ref w);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutUV.Add(u * uva + v * uvb + w * uvc);
                                        leftCutNorm.Add(triN);
                                        leftCutNorm.Add(-triN);

                                        if (order) {
                                            leftCutTri1.Add(vertPosA1);
                                            leftCutTri1.Add(vertPosB1);
                                            leftCutTri1.Add(vertPosC1);

                                            leftCutTri2.Add(vertPosA2);
                                            leftCutTri2.Add(vertPosC2);
                                            leftCutTri2.Add(vertPosB2);
                                        } 
                                        else {
                                            leftCutTri1.Add(vertPosA1);
                                            leftCutTri1.Add(vertPosC1);
                                            leftCutTri1.Add(vertPosB1);

                                            leftCutTri2.Add(vertPosA2);
                                            leftCutTri2.Add(vertPosB2);
                                            leftCutTri2.Add(vertPosC2);
                                        }
                                    }
                                }

                                // perform same operation for the right hand side mesh
                                tri.Clear();
                                HullTests.ConvexHull2DTriangulated(rightTriPT, tri, ref triN);

                                for (int m = 0; m < tri.Count; m += 3) {
                                    Vector3 ma = tri[m + 0];
                                    Vector3 mb = tri[m + 1];
                                    Vector3 mc = tri[m + 2];

                                    bool order = false;

                                    if (IsTriClockwise(ref ma, ref mb, ref mc, ref triN)) {
                                        order = true;
                                    }

                                    if (!isHallow) {
                                        int vertPosA = rightCutVert.Count;
                                        rightCutVert.Add(ma);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref ma, ref u, ref v, ref w);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutNorm.Add(triN);

                                        int vertPosB = rightCutVert.Count;
                                        rightCutVert.Add(mb);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mb, ref u, ref v, ref w);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutNorm.Add(triN);

                                        int vertPosC = rightCutVert.Count;
                                        rightCutVert.Add(mc);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mc, ref u, ref v, ref w);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutNorm.Add(triN);

                                        if (order) {
                                            rightCutTri1.Add(vertPosA);
                                            rightCutTri1.Add(vertPosB);
                                            rightCutTri1.Add(vertPosC);
                                        } 
                                        else {
                                            rightCutTri1.Add(vertPosA);
                                            rightCutTri1.Add(vertPosC);
                                            rightCutTri1.Add(vertPosB);
                                        }
                                    } 
                                    else {
                                        int vertPosA1 = rightCutVert.Count;
                                        rightCutVert.Add(ma);
                                        int vertPosA2 = rightCutVert.Count;
                                        rightCutVert.Add(ma);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref ma, ref u, ref v, ref w);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutNorm.Add(triN);
                                        rightCutNorm.Add(-triN);

                                        int vertPosB1 = rightCutVert.Count;
                                        rightCutVert.Add(mb);
                                        int vertPosB2 = rightCutVert.Count;
                                        rightCutVert.Add(mb);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mb, ref u, ref v, ref w);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutNorm.Add(triN);
                                        rightCutNorm.Add(-triN);

                                        int vertPosC1 = rightCutVert.Count;
                                        rightCutVert.Add(mc);
                                        int vertPosC2 = rightCutVert.Count;
                                        rightCutVert.Add(mc);

                                        HullTests.Barycentric(ref a, ref b, ref c, ref mc, ref u, ref v, ref w);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutUV.Add(u * uva + v * uvb + w * uvc);
                                        rightCutNorm.Add(triN);
                                        rightCutNorm.Add(-triN);

                                        if (order) {
                                            rightCutTri1.Add(vertPosA1);
                                            rightCutTri1.Add(vertPosB1);
                                            rightCutTri1.Add(vertPosC1);

                                            rightCutTri2.Add(vertPosA2);
                                            rightCutTri2.Add(vertPosC2);
                                            rightCutTri2.Add(vertPosB2);
                                        } 
                                        else {
                                            rightCutTri1.Add(vertPosA1);
                                            rightCutTri1.Add(vertPosC1);
                                            rightCutTri1.Add(vertPosB1);

                                            rightCutTri2.Add(vertPosA2);
                                            rightCutTri2.Add(vertPosB2);
                                            rightCutTri2.Add(vertPosC2);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // we will now attempt to "close off" the mesh completly
                    // using the index information for each intersection point as a clue

                    if (!isHallow) {
                        tri.Clear();
                        Vector3 n = -planeEQ.Normal;

                        HullTests.ConvexHull2DTriangulated(closedTri, tri, ref n);

                        N_UV.Clear();

                        HullTests.GenUVFromVertex(tri, N_UV, ref n);

                        // close off the mesh
                        for (int i = 0; i < tri.Count; i += 3) {
                            Vector3 cta = tri[i + 0];
                            Vector3 ctb = tri[i + 1];
                            Vector3 ctc = tri[i + 2];

                            Vector2 ctauv = N_UV[i + 0];
                            Vector2 ctbuv = N_UV[i + 1];
                            Vector2 ctcuv = N_UV[i + 2];

                            bool order = false;

                            Vector3 norm = -planeEQ.Normal;

                            if (IsTriClockwise(ref cta, ref ctb, ref ctc, ref norm)) {
                                order = true;
                            }

                            int rV1 = rightCutVert.Count;
                            rightCutVert.Add(cta);
                            rightCutUV.Add(ctauv);
                            rightCutNorm.Add(-planeEQ.Normal);

                            int rV2 = rightCutVert.Count;
                            rightCutVert.Add(ctb);
                            rightCutUV.Add(ctbuv);
                            rightCutNorm.Add(-planeEQ.Normal);

                            int rV3 = rightCutVert.Count;
                            rightCutVert.Add(ctc);
                            rightCutUV.Add(ctcuv);
                            rightCutNorm.Add(-planeEQ.Normal);

                            int lV1 = leftCutVert.Count;
                            leftCutVert.Add(cta);
                            leftCutUV.Add(ctauv);
                            leftCutNorm.Add(planeEQ.Normal);

                            int lV2 = leftCutVert.Count;
                            leftCutVert.Add(ctb);
                            leftCutUV.Add(ctbuv);
                            leftCutNorm.Add(planeEQ.Normal);

                            int lV3 = leftCutVert.Count;
                            leftCutVert.Add(ctc);
                            leftCutUV.Add(ctcuv);
                            leftCutNorm.Add(planeEQ.Normal);

                            // register triangle on left side
                            if (order) {
                                leftCutTri2.Add(lV1);
                                leftCutTri2.Add(lV3);
                                leftCutTri2.Add(lV2);

                                // register triangle on right side
                                rightCutTri2.Add(rV1);
                                rightCutTri2.Add(rV2);
                                rightCutTri2.Add(rV3);
                            } 
                            else {
                                leftCutTri2.Add(lV1);
                                leftCutTri2.Add(lV2);
                                leftCutTri2.Add(lV3);

                                // register triangle on right side
                                rightCutTri2.Add(rV1);
                                rightCutTri2.Add(rV3);
                                rightCutTri2.Add(rV2);
                            }
                        }
                    }

                    tri.Clear();
                    closedTri.Clear();

                    // we have finished assembly - add to meshes and exit
                    if (leftCutVert.Count > 3) {
                        Mesh leftMesh = new Mesh();
                        leftMesh.Clear();

                        leftMesh.subMeshCount = mesh.subMeshCount + 1;

                        leftMesh.vertices = leftCutVert.ToArray();
                        leftMesh.uv = leftCutUV.ToArray();
                        leftMesh.normals = leftCutNorm.ToArray();

                        for (int i = 0; i < mesh.subMeshCount; i++) {
                            leftMesh.SetTriangles(leftCutTri[i].ToArray(), i);
                        }

                        leftMesh.SetTriangles(leftCutTri2.ToArray(), mesh.subMeshCount);

                        tmpMeshes.Add(leftMesh);
                    }

                    if (rightCutVert.Count > 3) {
                        Mesh rightMesh = new Mesh();
                        rightMesh.Clear();

                        rightMesh.subMeshCount = mesh.subMeshCount + 1;

                        rightMesh.vertices = rightCutVert.ToArray();
                        rightMesh.uv = rightCutUV.ToArray();
                        rightMesh.normals = rightCutNorm.ToArray();

                        for (int i = 0; i < mesh.subMeshCount; i++) {
                            rightMesh.SetTriangles(rightCutTri[i].ToArray(), i);
                        }

                        rightMesh.SetTriangles(rightCutTri2.ToArray(), mesh.subMeshCount);

                        tmpMeshes.Add(rightMesh);
                    }
                }

                meshes.Clear();

                meshes.AddRange(tmpMeshes);

                tmpMeshes.Clear();
            }
        }

        private static bool IsTriClockwise(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 normal) {
            Vector3 n = Vector3.Cross(b - a, c - a);
            return Vector3.Dot(normal, n) >= 0;
        }
    }
}
