using System.Numerics;
using Akihabara;
using Akihabara.Framework.Protobuf;

namespace Moetion.Extensions
{
    public static class MediapipeExtensions
    {
        public static Vector3 ToVector(this NormalizedLandmark landmark) => new Vector3(landmark.X, landmark.Y, landmark.Z);
    }
}