// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System.Numerics;

namespace Moetion.Pose
{
    public struct Pose
    {
        public Arm LeftArm;
        public Arm RightArm;
        public Leg LeftLeg;
        public Leg RightLeg;
        public Vector3 Spine;
        public Hips Hips;
    }
}
