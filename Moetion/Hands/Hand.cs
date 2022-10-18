// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System.Numerics;

namespace Moetion.Hands;

public struct Hand
{
    public Vector3[] Palm;
    // TODO: convert to Quaternion
    public Vector3 Rotation;
    public Vector3 Wrist;

    #region Fingers
    public Finger Thumb;
    public Finger Index;
    public Finger Middle;
    public Finger Ring;
    public Finger Little;
    #endregion
}