// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under LGPL v3. See LICENSE.md for details.

using System.Numerics;

namespace Moetion.Face
{
    public struct FaceData
    {
        public Vector3 EyeInnerCornerL;
        public Vector3 EyeInnerCornerR;
        public Vector3 EyeOuterCornerL;
        public Vector3 EyeOuterCornerR;

        public double EyeInnerDistance;
        public double EyeOuterDistance;

        public Vector3 UpperInnerLip;
        public Vector3 LowerInnerLip;
        public Vector3 MouthCornerLeft;
        public Vector3 MouthCornerRight;

        public double MouthOpen;
        public double MouthWidth;

        public double RatioX;
        public double RatioY;
    }
}
