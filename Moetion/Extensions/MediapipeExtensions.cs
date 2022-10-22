// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System.Numerics;
using Google.Protobuf.Collections;
using Mediapipe.Net.Framework.Protobuf;

namespace Moetion.Extensions;

public static class MediapipeExtensions
{
    #region NormalizedLandmark
    public static Vector3 ToVector(this NormalizedLandmark landmark) => new Vector3(landmark.X, landmark.Y, landmark.Z);
    public static Vector2 ToVector2(this NormalizedLandmark landmark) => new Vector2(landmark.X, landmark.Y);

    public static Vector3 FindRotation(this NormalizedLandmark landmark, NormalizedLandmark other, bool normalize = true)
        => landmark.ToVector().FindRotation(other.ToVector(), normalize);

    public static Vector3 RollPitchYaw(this RepeatedField<NormalizedLandmark> lm, int a, int b, int? oc = null)
        => VectorExtensions.RollPitchYaw(lm[a].ToVector(), lm[b].ToVector(), oc is int c ? lm[c].ToVector() : null);

    public static float AngleBetweenLandmarks(this RepeatedField<NormalizedLandmark> lm, int a, int b, int c)
        => VectorExtensions.AngleBetween3DCoords(lm[a].ToVector(), lm[b].ToVector(), lm[c].ToVector());
    #endregion

    #region Landmark
    public static Vector3 ToVector(this Landmark landmark) => new Vector3(landmark.X, landmark.Y, landmark.Z);
    public static Vector2 ToVector2(this Landmark landmark) => new Vector2(landmark.X, landmark.Y);

    public static Vector3 FindRotation(this Landmark landmark, Landmark other, bool normalize = true)
        => landmark.ToVector().FindRotation(other.ToVector(), normalize);

    public static Vector3 RollPitchYaw(this RepeatedField<Landmark> lm, int a, int b, int? oc = null)
        => VectorExtensions.RollPitchYaw(lm[a].ToVector(), lm[b].ToVector(), oc is int c ? lm[c].ToVector() : null);

    public static float AngleBetweenLandmarks(this RepeatedField<Landmark> lm, int a, int b, int c)
        => VectorExtensions.AngleBetween3DCoords(lm[a].ToVector(), lm[b].ToVector(), lm[c].ToVector());
    #endregion
}
