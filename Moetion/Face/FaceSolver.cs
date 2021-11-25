// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using Akihabara.Framework.Protobuf;
using Moetion.Extensions;

namespace Moetion.Face
{
    public sealed class FaceSolver
    {
        public FaceData Solve(NormalizedLandmarkList list)
        {
            FaceData data = new FaceData();

            //eye keypoints
            data.EyeInnerCornerL = list.Landmark[133].ToVector();
            data.EyeInnerCornerR = list.Landmark[362].ToVector();
            data.EyeOuterCornerL = list.Landmark[130].ToVector();

            //eye keypoint distances
            data.EyeInnerDistance = data.EyeInnerCornerL.Distance(data.EyeInnerCornerR);
            data.EyeOuterDistance = data.EyeOuterCornerL.Distance(data.EyeOuterCornerR);

            //mouth keypoints
            data.UpperInnerLip = list.Landmark[13].ToVector();
            data.LowerInnerLip = list.Landmark[14].ToVector();
            data.MouthCornerLeft = list.Landmark[61].ToVector();
            data.MouthCornerRight = list.Landmark[291].ToVector();

            //mouth keypoint distances
            data.MouthOpen = data.UpperInnerLip.Distance(data.LowerInnerLip);
            data.MouthWidth = data.MouthCornerLeft.Distance(data.MouthCornerRight);

            //mouth open and shape ratios

            return data;
        }
    }
}
