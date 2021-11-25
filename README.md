# Moetion

A solver library for MediaPipe output in C#, inspired of [kalidokit](https://github.com/yeemachine/kalidokit). 

## Differences from kalidokit

While this is using similar behaviors from kalidokit, we've made substantial changes that differentiate it from the original implementation:

- We use [Quaternions](https://www.allaboutcircuits.com/technical-articles/dont-get-lost-in-deep-space-understanding-quaternions/) to prevent gimbal-locking and prevent the edge cases we see from Euler Angle-based implementations
- We've re-written the code to take advantage of C# and .NET as much as possible, but we kept the definition names similar or relatively similar.
- We use [Mediapipe.NET](https://github.com/vignetteapp/Akihabara), a Vignette-maintained library to interact with MediaPipe.

This list is non-exhaustive however, but the list of changes we're doing makes it sufficient to call it it's own library rather than a deriverative.

## Copyright

Moetion is Copyright &copy; The Vignette Authors, licensed under the [Microsoft Reciprocal License](https://opensource.org/licenses/MS-RL). See the LICENSE file for more details.
