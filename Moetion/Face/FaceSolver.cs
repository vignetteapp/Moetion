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
            Face data = new();

            //eye keypoints
            data.EyeInnerCornerL = landmarks[133].ToVector();
            data.EyeInnerCornerR = landmarks[362].ToVector();
            data.EyeOuterCornerL = landmarks[130].ToVector();

            //eye keypoint distances
            data.EyeInnerDistance = data.EyeInnerCornerL.Distance(data.EyeInnerCornerR);
            data.EyeOuterDistance = data.EyeOuterCornerL.Distance(data.EyeOuterCornerR);

            //mouth keypoints
            data.UpperInnerLip = landmarks[13].ToVector();
            data.LowerInnerLip = landmarks[14].ToVector();
            data.MouthCornerLeft = landmarks[61].ToVector();
            data.MouthCornerRight = landmarks[291].ToVector();

            //mouth keypoint distances
            data.MouthOpen = data.UpperInnerLip.Distance(data.LowerInnerLip);
            data.MouthWidth = data.MouthCornerLeft.Distance(data.MouthCornerRight);

            //mouth open and shape ratios

            return data;
        }
    }
}
