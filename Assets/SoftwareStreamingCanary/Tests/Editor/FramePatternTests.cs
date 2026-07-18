using NUnit.Framework;
using UnityEditor;
using UnityEngine.Rendering;

namespace AppMana.SoftwareStreamingCanary.Tests
{
  public class FramePatternTests
  {
    [Test]
    public void LinuxPlayerSupportsTheRuntimeVulkanContract()
    {
      Assert.That(PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.StandaloneLinux64), Is.False);
      Assert.That(
        PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneLinux64),
        Does.Contain(GraphicsDeviceType.Vulkan));
    }

    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(2, 3)]
    [TestCase(3, 2)]
    [TestCase(65535, 32768)]
    public void GrayCodeMatchesReference(int sequence, int expected)
    {
      Assert.That(FramePattern.GrayCode((ushort)sequence), Is.EqualTo((ushort)expected));
    }

    [Test]
    public void EverySequenceHasSyncBitsAndValidParity()
    {
      for (var sequence = 0; sequence <= ushort.MaxValue; sequence++)
      {
        Assert.That(FramePattern.CellValue(0, (ushort)sequence), Is.True);
        Assert.That(FramePattern.CellValue(1, (ushort)sequence), Is.False);
        Assert.That(FramePattern.CellValue(2, (ushort)sequence), Is.True);

        var parity = false;
        for (var cell = 3; cell < 19; cell++)
        {
          parity ^= FramePattern.CellValue(cell, (ushort)sequence);
        }
        Assert.That(FramePattern.CellValue(19, (ushort)sequence), Is.EqualTo(parity));
      }
    }
  }
}
