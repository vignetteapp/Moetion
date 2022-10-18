// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System.Numerics;

namespace Moetion.Face;

public struct Face
{
    public Head Head;
    public Eyes Eyes;
    // Weird... It only has one value for the eye brows in the original code.
    // TODO: investigate getting 2 different brow values. Can't get enough of raised brow faces ',:v
    public float Brow;
    // Same question for Pupils (?)
    public Vector2 Pupils;
    public Mouth Mouth;
}