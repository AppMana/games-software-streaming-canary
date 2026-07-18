using UnityEngine;

namespace AppMana.SoftwareStreamingCanary
{
  public static class FramePattern
  {
    public const int SequenceBits = 16;

    public static ushort GrayCode(ushort sequence)
    {
      return (ushort)(sequence ^ (sequence >> 1));
    }

    public static bool CellValue(int cell, ushort sequence)
    {
      if (cell is < 0 or >= 20)
      {
        return false;
      }

      if (cell < 3)
      {
        return cell != 1;
      }

      var gray = GrayCode(sequence);
      if (cell < 19)
      {
        return (gray & (1 << (cell - 3))) != 0;
      }

      var parity = 0;
      for (var bit = 0; bit < SequenceBits; bit++)
      {
        parity ^= (gray >> bit) & 1;
      }
      return parity != 0;
    }
  }
}
