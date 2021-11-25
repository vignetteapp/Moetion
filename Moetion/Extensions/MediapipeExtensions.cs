// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the Microsoft Reciprocal License. See LICENSE for details.

using System.Numerics;
using Akihabara.Framework.Protobuf;

namespace Moetion.Extensions
{
    public static class MediapipeExtensions
    {
        public static Vector3 ToVector(this NormalizedLandmark landmark) => new Vector3(landmark.X, landmark.Y, landmark.Z);
        public static Vector2 ToVector2(this NormalizedLandmark landmark) => new Vector2(landmark.X, landmark.Y);
    }
}
