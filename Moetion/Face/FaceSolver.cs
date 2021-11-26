// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System;
using System.Numerics;
using Akihabara.Framework.Protobuf;
using Moetion.Extensions;
using static Moetion.Extensions.VectorExtensions;

namespace Moetion.Face
{
    public static class FaceSolver
    {
        public static readonly int[] EyeLeftPoints = new int[] { 130, 133, 160, 159, 158, 144, 145, 153 };
        public static readonly int[] EyeRightPoints = new int[] { 263, 362, 387, 386, 385, 373, 374, 380 };
        public static readonly int[] BrowLeftPoints = new int[] { 35, 244, 63, 105, 66, 229, 230, 231 };
        public static readonly int[] BrowRightPoints = new int[] { 265, 464, 293, 334, 296, 449, 450, 451 };
        public static readonly int[] PupilLeftPoints = new int[] { 468, 469, 470, 471, 472 };
        public static readonly int[] PupilRightPoints = new int[] { 473, 474, 475, 476, 477 };

        public static Face Solve(NormalizedLandmarkList list)
        {
            var landmarks = list.Landmark;
            Face face = new();

            return face;
        }

        #region Mouth Calculations
        public static Mouth CalcMouth(NormalizedLandmarkList list)
        {
            var landmarks = list.Landmark;

            // Eye keypoints
            var eyeInnerCornerL = landmarks[133].ToVector();
            var eyeInnerCornerR = landmarks[362].ToVector();
            var eyeOuterCornerL = landmarks[130].ToVector();
            var eyeOuterCornerR = landmarks[263].ToVector();

            // Eye keypoint distances
            var eyeInnerDistance = Vector3.Distance(eyeInnerCornerL, eyeInnerCornerR);
            var eyeOuterDistance = Vector3.Distance(eyeOuterCornerL, eyeOuterCornerR);

            // Mouth keypoints
            var upperInnerLip = landmarks[13].ToVector();
            var lowerInnerLip = landmarks[14].ToVector();
            var mouthCornerLeft = landmarks[61].ToVector();
            var mouthCornerRight = landmarks[291].ToVector();

            // Mouth keypoint distances
            var mouthOpen = Vector3.Distance(upperInnerLip, lowerInnerLip);
            var mouthWidth = Vector3.Distance(mouthCornerLeft, mouthCornerRight);

            // Mouth open and mouth shape ratios
            var ratioY = mouthOpen / eyeInnerDistance;
            var ratioX = mouthWidth / eyeOuterDistance;

            // Normalize and scale mouth open
            ratioY = ratioY.Remap(0.15f, 0.7f);

            // Normalize and scale mouth shape
            ratioX = ratioX.Remap(0.45f, 0.9f);
            ratioX = (ratioX - 0.3f) * 2;

            var mouthX = ratioX;
            var mouthY = (mouthOpen / eyeInnerDistance).Remap(0.17f, 0.5f);

            var ratioI = Math.Clamp(mouthX.Remap(0, 1) * 2 * mouthY.Remap(0.2f, 0.7f), 0, 1);
            var ratioA = mouthY * 0.4f + mouthY * (1 - ratioI) * 0.6f;
            var ratioU = mouthY * (1 - ratioI).Remap(0, 0.3f) * 0.1f;
            var ratioE = ratioU.Remap(0.2f, 1) * (1 - ratioI) * 0.3f;
            var ratioO = (1 - ratioI) * mouthY.Remap(0.3f, 1) * 0.4f;

            return new Mouth
            {
                X = ratioX,
                Y = ratioY,
                Shape = new MouthShape
                {
                    A = ratioA,
                    I = ratioI,
                    U = ratioU,
                    E = ratioE,
                    O = ratioO,
                },
            };
        }
        #endregion

        #region Head Calculations
        public static Head CalcHead(NormalizedLandmarkList list)
        {
            // Find 3 vectors that form a plane to represent the head
            var plane = FaceEulerPlane(list);
            var rotate = RollPitchYaw(plane[0], plane[1], plane[2]);
            // Find center of face detection box
            var midPoint = Vector3.Lerp(plane[0], plane[1], 0.5f);
            // Roughly find the dimensions of the face detection box
            var width = Vector3.Distance(plane[0], plane[1]);
            var height = Vector3.Distance(midPoint, plane[2]);

            // Flip
            rotate.X *= -1;
            rotate.Y *= -1;

            return new Head
            {
                X = rotate.X * MathF.PI,
                Y = rotate.Y * MathF.PI,
                Z = rotate.Z * MathF.PI,
                Width = width,
                Height = height,
                Position = Vector3.Lerp(midPoint, plane[2], 0.5f),
                NormalizedAngles = new Vector3
                {
                    X = rotate.X,
                    Y = rotate.Y,
                    Z = rotate.Z,
                },
                Degrees = new Vector3
                {
                    X = rotate.X * 180,
                    Y = rotate.Y * 180,
                    Z = rotate.Z * 180,
                },
            };
        }

        public static Vector3[] FaceEulerPlane(NormalizedLandmarkList list)
        {
            var landmarks = list.Landmark;

            // Create face detection square bounds
            var topLeft = landmarks[21].ToVector();
            var topRight = landmarks[251].ToVector();
            var bottomRight = landmarks[397].ToVector();
            var bottomLeft = landmarks[172].ToVector();
            var bottomMidpoint = Vector3.Lerp(bottomRight, bottomLeft, 0.5f);

            // TODO: idk, this array processing looks ugly.
            return new Vector3[] { topLeft, topRight, bottomMidpoint };
        }
        #endregion

        #region Eye Calculations
        public static float GetEyeOpen(NormalizedLandmarkList list, Side side, float high = .85f, float low = .55f)
        {
            var landmarks = list.Landmark;

            var eyePoints = side == Side.Right ? EyeRightPoints : EyeLeftPoints;
            var eyeDistance = EyeLidRatio(
                landmarks[eyePoints[0]],
                landmarks[eyePoints[1]],
                landmarks[eyePoints[2]],
                landmarks[eyePoints[3]],
                landmarks[eyePoints[4]],
                landmarks[eyePoints[5]],
                landmarks[eyePoints[6]],
                landmarks[eyePoints[7]]);

            // Human eye width to height ratio is roughly .3
            var maxRatio = 0.285f;
            // Compare ratio against max ratio
            var ratio = Math.Clamp(eyeDistance / maxRatio, 0, 2);
            // Remap eye open and close ratios to increase sensitivity
            var eyeOpenRatio = ratio.Remap(low, high);

            return eyeOpenRatio;
        }

        public static float EyeLidRatio(
            NormalizedLandmark outerCorner,
            NormalizedLandmark innerCorner,
            NormalizedLandmark outerUpperLid,
            NormalizedLandmark midUpperLid,
            NormalizedLandmark innerUpperLid,
            NormalizedLandmark outerLowerLid,
            NormalizedLandmark midLowerLid,
            NormalizedLandmark innerLowerLid)
        {
            var eyeOuterCorner = outerCorner.ToVector2();
            var eyeInnerCorner = innerCorner.ToVector2();

            var eyeOuterUpperLid = outerUpperLid.ToVector2();
            var eyeMidUpperLid = midUpperLid.ToVector2();
            var eyeInnerUpperLid = innerUpperLid.ToVector2();

            var eyeOuterLowerLid = outerLowerLid.ToVector2();
            var eyeMidLowerLid = midLowerLid.ToVector2();
            var eyeInnerLowerLid = innerLowerLid.ToVector2();

            // Use 2D Distances instead of 3D for less jitter
            var eyeWidth = Vector2.Distance(eyeOuterCorner, eyeInnerCorner);
            var eyeOuterLidDistance = Vector2.Distance(eyeOuterUpperLid, eyeOuterLowerLid);
            var eyeMidLidDistance = Vector2.Distance(eyeMidUpperLid, eyeMidLowerLid);
            var eyeInnerLidDistance = Vector2.Distance(eyeInnerUpperLid, eyeInnerLowerLid);
            var eyeLidAvg = (eyeOuterLidDistance + eyeMidLidDistance + eyeInnerLidDistance) / 3;
            var ratio = eyeLidAvg / eyeWidth;

            return ratio;
        }
        #endregion
    }
}
