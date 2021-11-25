// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System.Numerics;
using System.Reflection.Metadata;

namespace Moetion.Face
{
    public struct Head
    {
        // X, Y and Z represent 3D radian angles.
        // It looks like there are radians, normalized angles and degrees in the same struct.
        // Suggestion: only use one version of them (radians?)
        public double X;
        public double Y;
        public double Z;

        public double Width;
        public double Height;

        /// <summary>
        /// Center of face detection square.
        /// </summary>
        public Vector3 Position;
        // TODO: convert to a Quaternion
        /// <summary>
        /// Euler angles normalized between -1 and 1.
        /// </summary>
        public Vector3 NormalizedAngles;
        // Note: probably not needed.
        public Vector3 Degrees;
    }
}
