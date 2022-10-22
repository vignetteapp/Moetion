// Copyright (c) The Vignette Authors
// This file is part of Moetion.
// Moetion is licensed under the BSD 3-Clause License. See LICENSE for details.

using System.Numerics;
using Moetion.Face;
using Moetion.Hands;
using Moetion.Pose;

namespace Moetion;

public static class RestingDefaults
{
    public static readonly Face.Face Face = new Face.Face
    {
        Eyes = new Eyes
        {
            Left = 1,
            Right = 1,
        },
        Mouth = new Mouth
        {
            X = 0,
            Y = 0,
            Shape = new Phoneme
            {
                A = 0,
                E = 0,
                I = 0,
                O = 0,
                U = 0,
            },
        },
        Head = new Head
        {
            X = 0,
            Y = 0,
            Z = 0,
            Width = .3f,
            Height = .6f,
            Position = {
                X = .5f,
                Y = .5f,
                Z = 0,
            },
        },
        Brow = 0,
        Pupils = new Vector2
        {
            X = 0,
            Y = 0,
        },
    };

    public static readonly Pose.Pose Pose = new Pose.Pose
    {
        RightArm = new Arm
        {
            Upper = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -1.25f,
            },
            Lower = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            },
            Hand = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            },
        },
        LeftArm = new Arm
        {
            Upper = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 1.25f,
            }, // Y = 0 > -.5 // Z = -.5>.5
            Lower = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            }, // X = 0 > -4, Z = 0 to -.9
            Hand = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            },
        },
        RightLeg = new Leg
        {
            Upper = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            },
            Lower = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            },
        },
        LeftLeg = new Leg
        {
            Upper = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            },
            Lower = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 0,
            },
        },
        Spine = new Vector3
        {
            X = 0,
            Y = 0,
            Z = 0,
        },
        Hips = {
            Position = new Vector3 {
                X = 0,
                Y = 0,
                Z = 0,
            },
            Rotation = new Vector3 {
                X = 0,
                Y = 0,
                Z = 0,
            },
        },
    };

    public static readonly Hand RightHand = new Hand
    {
        Wrist = new Vector3
        {
            X = -0.13f,
            Y = -0.07f,
            Z = -1.04f,
        },
        Thumb = new Finger
        {
            Proximal = new Vector3
            {
                X = -.23f,
                Y = -.33f,
                Z = -.12f,
            },
            Intermediate = new Vector3
            {
                X = -.2f,
                Y = -.199f,
                Z = -.0139f,
            },
            Distal = new Vector3
            {
                X = -.2f,
                Y = .002f,
                Z = .15f,
            },
        },
        Index = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.24f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.25f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.06f,
            },
        },
        Middle = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.09f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.44f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.06f,
            },
        },
        Ring = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.13f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.4f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.04f,
            },
        },
        Little = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.09f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.225f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = -.1f,
            },
        },
    };

    public static readonly Hand LeftHand = new Hand
    {
        Wrist = new Vector3
        {
            X = -.13f,
            Y = -.07f,
            Z = -1.04f,
        },
        Thumb = new Finger
        {
            Proximal = new Vector3
            {
                X = -.23f,
                Y = .33f,
                Z = .12f,
            },
            Intermediate = new Vector3
            {
                X = -.2f,
                Y = .25f,
                Z = .05f,
            },
            Distal = new Vector3
            {
                X = -.2f,
                Y = .17f,
                Z = -.06f,
            },
        },
        Index = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .24f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .25f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .06f,
            },
        },
        Middle = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .09f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .44f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .066f,
            },
        },
        Ring = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .13f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .4f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .049f,
            },
        },
        Little = new Finger
        {
            Proximal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .17f,
            },
            Intermediate = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .4f,
            },
            Distal = new Vector3
            {
                X = 0,
                Y = 0,
                Z = .1f,
            },
        }
    };
}
