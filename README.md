# Software Streaming Canary

Minimal Unity project used by the BuildToStream full-stack acceptance test. It
renders a deterministic frame pattern that can be validated in a browser while
running with llvmpipe and the software WebRTC encoder. The project intentionally
has no game assets and does not require a physical GPU.

The cluster builder injects the private `com.appmana.unity` and
`com.unity.webrtc` packages before running the ordinary Unity Gradle tasks. Keep
those private packages out of `Packages/manifest.json`; this repository exercises
the same package-injection path as customer projects.

## Build Contract

- Unity: `6000.5.3f1`
- Linux task: `prepareContextLinux`
- Runtime: the normal multi-platform `unityruntime` entry point and arguments
- Encoder: `AppSettings:useHardwareEncoder=false`
- Graphics: Vulkan on accelerated nodes; explicit GLCore support for llvmpipe
  software acceptance

The canonical end-to-end harness lives in the AppMana repository under
`appmana-build-to-stream-tests`. It creates the BuildToStream resources, waits
for Tekton to publish an immutable `BuildArtifact`, deploys the resulting image,
and verifies decoded frames in a browser.
