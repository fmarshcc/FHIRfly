using System.Text;

namespace FhirFly.Hl7;

/// <summary>MLLP framing: &lt;VT&gt; message &lt;FS&gt;&lt;CR&gt;.</summary>
public static class Mllp
{
    public const byte StartBlock = 0x0B;
    public const byte EndBlock = 0x1C;
    public const byte CarriageReturn = 0x0D;

    public static byte[] Frame(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var framed = new byte[body.Length + 3];
        framed[0] = StartBlock;
        body.CopyTo(framed, 1);
        framed[^2] = EndBlock;
        framed[^1] = CarriageReturn;
        return framed;
    }

    /// <summary>Returns the message inside one complete MLLP frame, or null if the frame is incomplete or malformed.</summary>
    public static string? Unframe(ReadOnlySpan<byte> frame)
    {
        if (frame.Length < 3 || frame[0] != StartBlock || frame[^2] != EndBlock || frame[^1] != CarriageReturn)
            return null;
        return Encoding.UTF8.GetString(frame[1..^2]);
    }
}
