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
        public static Face Solve(NormalizedLandmarkList list)
        {
            var landmarks = list.Landmark;
            Face face = new();

            return face;
        }

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
                X = rotate.X * Math.PI,
                Y = rotate.Y * Math.PI,
                Z = rotate.Z * Math.PI,
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
    }
}
