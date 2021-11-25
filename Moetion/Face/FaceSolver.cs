// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

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
            Mouth data = new();

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
            // var ratioXY = mouthWidth / mouthOpen;
            var ratioY = mouthOpen / eyeInnerDistance;
            var ratioX = mouthWidth / eyeOuterDistance;


            return data;
        }
    }
}
