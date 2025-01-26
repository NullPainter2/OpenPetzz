//
// api inspired by https://github.com/thenickdude/petz-file-formats/tree/master
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Bhd2
{
    public List<int> ballSizes = new List<int>();

    public Bhd2(string bhdPath, List<string> bdtFiles)
    {
        // Parse BHD

        {
            byte[] bytes = File.ReadAllBytes(bhdPath);

            var parser = new Parser(bytes);

            var header = new BHDHeader();

            header.Parse(parser);
            if (!parser.isOk)
            {
                return;
            }

            // get animation frame offsets

            List<List<UInt32>> frameOffsetsArray = new List<List<UInt32>>();

            for (int animIndex = 0; animIndex < header.animationCount; animIndex++)
            {
                int animationStart = animIndex == 0 ? 0 : header.animationOffsets[animIndex - 1];

                if (animIndex >= header.animationOffsets.Count)
                {
                    continue; // @todo fix this, this is error !!
                }

                int animationSize = header.animationOffsets[animIndex] - animationStart;

                var animParser = new Parser(bytes, header.framesOffset + animationStart * 4);

                List<UInt32> frameOffsets = new List<UInt32>();

                animParser.array(animationSize, () =>
                {
                    UInt32 frameOffset = 0;
                    parser.uint32(ref frameOffset);
                    if (parser.isOk)
                    {
                        frameOffsets.Add(frameOffset);
                    }
                });

                if (animParser.isOk)
                {
                    frameOffsetsArray.Add(frameOffsets);
                }
            }
            
            ballSizes = header.ballSizes;
        }

        // parse skeleton ...
        
        for (int i = 0; i < bdtFiles.Count; i++)
        {
            
            
        }

        // export valid stuff
        


    }
}


public class BHTHeader
{
    public UInt32 fileLength;
    public int version;
    public string copyright;

    public void Parse(Parser parser)
    {
        parser.uint32(ref fileLength);
        parser.uint16(ref version);
        parser.String(ref copyright);
    }
}


class BHTBallPosition
{
    public int x;
    public int y;
    public int z;
    public int q1;
    public int q2;

    public void Parse(Parser parser)
    {
        parser.int16(ref x);
        parser.int16(ref y);
        parser.int16(ref z);
        parser.uint16(ref q1);
        parser.uint16(ref q2);
    }
}

class BHTFrameHeader
{
    public int minx;
    public int miny;
    public int minz;
    public int maxx;
    public int maxy;
    public int maxz;
    public int tag;

    public List<BHTBallPosition> ballPositions = new List<BHTBallPosition>();

    public void Parse(Parser parser, int numBalls)
    {
        parser.int16(ref minx);
        parser.int16(ref miny);
        parser.int16(ref minz);
        parser.int16(ref maxx);
        parser.int16(ref maxy);
        parser.int16(ref maxz);
        parser.uint16(ref tag);

        for (int i = 0; i < numBalls; i++) // parser.array(numBalls, () =>
        {
            var ballPos = new BHTBallPosition();
            ballPos.Parse(parser);
            if (parser.isOk)
            {
                ballPositions.Add(ballPos);
            }
        }
    }
}

public class BHDHeader
{
    public int framesOffset = 0;
    int unknown = 0;
    public int version = 0;
    public int numBalls = 0;
    public List<int> ballSizes = new List<int>();
    public List<int> animationOffsets = new List<int>();
    public int animationCount = 0;

    public void Parse(Parser parser)
    {
        parser.uint16(ref framesOffset);
        parser.uint16(ref unknown);
        parser.uint16(ref version);
        parser.uint16(ref numBalls);
        parser.skipBytes(30);
        parser.array16(numBalls, ref ballSizes);
        parser.uint16(ref animationCount);
        parser.array16(animationCount, ref animationOffsets);
    }
}

public class Parser
{
    byte[] bytes = null;
    int index = 0;
    public bool isOk = true;

    public enum Endianess
    {
        LITTLE_ENDIAN = 0,
        BIG_ENDIAN = 1
    };

    Endianess endian = Endianess.LITTLE_ENDIAN; // @todo handle big endian

    public Parser(byte[] _bytes, int _offset = 0)
    {
        bytes = _bytes;
        index = _offset;
    }

    public void SetEndian(Endianess type = Endianess.LITTLE_ENDIAN)
    {
        endian = type;
    }

    public void SetOffset(int offset)
    {
        index = offset;
    }

    public void skipBytes(int n)
    {
        if (!isOk)
        {
            return;
        }

        if (index + n > bytes.Length)
        {
            isOk = false;

            return;
        }

        index += n;
    }

    public void array16(int n, ref List<int> list)
    {
        for (int i = 0; i < n; i++)
        {
            int value = 0;
            uint16(ref value);
            if (isOk)
            {
                list.Add(value);
            }
        }
    }

    // @fixme This api ... is it even necessary???
    public void array(int n, Action action)
    {
        for (int i = 0; i < n; i++)
        {
            action();
        }
    }

    public void uint32(ref UInt32 outValue)
    {
        if (!isOk)
        {
            return;
        }

        // count = 5
        // maxIndex = 4
        // index = 3 // last one
        // index + 1 // one byte is ok
        // index + 2 // not ok
        // [index][index+1]

        if (index + 4 > bytes.Length)
        {
            isOk = false;
            return;
        }

        outValue = (uint)((bytes[index + 3] << 24) | (bytes[index + 2] << 16) | (bytes[index + 1] << 8) |
                          bytes[index + 0]);

        index += 2;
    }

    public void String(ref string outValue)
    {
        if (!isOk)
        {
            return;
        }

        string temp = "";

        while (true)
        {
            int ch = 0;

            Byte(ref ch);

            if (ch == 0)
            {
                break;
            }

            if (!isOk) // error in file can also end reading ...
            {
                break;
            }

            temp += ch;
        }

        // store only valid value
        if (isOk)
        {
            outValue = temp;
        }
    }


    public void Byte(ref int outValue)
    {
        if (!isOk)
        {
            return;
        }

        if (index + 1 > bytes.Length)
        {
            isOk = false;
            return;
        }

        outValue = bytes[index];

        index += 1;
    }

    public void int16(ref int outValue)
    {
        uint16(ref outValue);

        // @hack handle negative number
        if (outValue > Int16.MaxValue)
        {
            outValue = Int16.MaxValue - outValue;
        }
    }

    public void uint16(ref int outValue)
    {
        if (!isOk)
        {
            return;
        }

        // count = 5
        // maxIndex = 4
        // index = 3 // last one
        // index + 1 // one byte is ok
        // index + 2 // not ok
        // [index][index+1]

        if (index + 2 > bytes.Length)
        {
            isOk = false;
            return;
        }

        outValue = (bytes[index + 1] * 256) + bytes[index + 0];

        index += 2;
    }
}