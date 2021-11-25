// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System;
using Akihabara.Framework.Protobuf;
using Moetion.Extensions;

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
            var eyeInnerDistance = eyeInnerCornerL.Distance(eyeInnerCornerR);
            var eyeOuterDistance = eyeOuterCornerL.Distance(eyeOuterCornerR);

            // Mouth keypoints
            var upperInnerLip = landmarks[13].ToVector();
            var lowerInnerLip = landmarks[14].ToVector();
            var mouthCornerLeft = landmarks[61].ToVector();
            var mouthCornerRight = landmarks[291].ToVector();

            // Mouth keypoint distances
            var mouthOpen = upperInnerLip.Distance(lowerInnerLip);
            var mouthWidth = mouthCornerLeft.Distance(mouthCornerRight);

            // Mouth open and mouth shape ratios
            var ratioY = mouthOpen / eyeInnerDistance;
            var ratioX = mouthWidth / eyeOuterDistance;

            // Normalize and scale mouth open
            ratioY = ratioY.Remap(0.15, 0.7);

            // Normalize and scale mouth shape
            ratioX = ratioX.Remap(0.45, 0.9);
            ratioX = (ratioX - 0.3) * 2;

            var mouthX = ratioX;
            var mouthY = (mouthOpen / eyeInnerDistance).Remap(0.17, 0.5);

            var ratioI = Math.Clamp(mouthX.Remap(0, 1) * 2 * mouthY.Remap(0.2, 0.7), 0, 1);
            var ratioA = mouthY * 0.4 + mouthY * (1 - ratioI) * 0.6;
            var ratioU = mouthY * (1 - ratioI).Remap(0, 0.3) * 0.1;
            var ratioE = ratioU.Remap(0.2, 1) * (1 - ratioI) * 0.3;
            var ratioO = (1 - ratioI) * mouthY.Remap(0.3, 1) * 0.4;

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
    }
}
