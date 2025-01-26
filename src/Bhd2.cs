//
// api inspired by https://github.com/thenickdude/petz-file-formats/tree/master
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

class Bhd2
{
    public List<int> ballSizes = new List<int>();

    public BHDHeader header = new BHDHeader();
    public List<BHTFrameHeader> frameHeaders = new List<BHTFrameHeader>();

    public Bhd2(string bhdPath, List<string> bdtFiles)
    {
        List<List<UInt32>> frameOffsetsArray = new List<List<UInt32>>();

        // Parse BHD
        {
            byte[] bytes = File.ReadAllBytes(bhdPath);

            var parser = new Parser(bytes);

            header.Parse(parser);
            if (!parser.isOk)
            {
                return;
            }

            // get animation frame offsets

            for (int animIndex = 0; animIndex < header.animationCount; animIndex++)
            {
                int animationStart = animIndex == 0 ? 0 : header.animationOffsets[animIndex - 1];

                if (animIndex >= header.animationOffsets.Count)
                {
                    GD.Print(String.Format("Animation index is out of range. {1} >= {2}", animIndex,
                        header.animationOffsets.Count));
                    continue; // @todo fix this, this is error !!
                }

                int animationSize = header.animationOffsets[animIndex] - animationStart;

                var animParser = new Parser(bytes, header.framesOffset + animationStart * 4);

                List<UInt32> frameOffsets = new List<UInt32>();

                animParser.Array(animationSize, () =>
                {
                    UInt32 frameOffset = 0;
                    animParser.U32(ref frameOffset);
                    if (animParser.isOk)
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

        for (int bdtIndex = 0; bdtIndex < bdtFiles.Count; bdtIndex++)
        {
            byte[] bytes = File.ReadAllBytes(bdtFiles[bdtIndex]);
            
            // parse header

            var header = new BHTHeader();
            var headerParser = new Parser(bytes);
            header.Parse(headerParser);

            if (bdtIndex >= frameOffsetsArray.Count)
            {
                GD.Print(String.Format("BDT index is out of range. {1} >= {2}", bdtIndex, frameOffsetsArray.Count));
                continue; // @todo error !!!!
            }

            List<UInt32> frameOffsets = frameOffsetsArray[bdtIndex];
            for (int frame = 0; frame < frameOffsets.Count; frame++)
            {
                int frameOffset = (int)frameOffsets[frame];
                var parser = new Parser(bytes, frameOffset); // Yea, yea, but the Petz files don't have > 2 GB :D
                // Unless somebody would make a 2hr movie or animated series with it, ha!
                var frameHeader = new BHTFrameHeader();
                frameHeader.Parse(parser, ballSizes.Count);

                if (parser.isOk)
                {
                    frameHeaders.Add(frameHeader);
                }
            }


            if (true) // @debugger
            {
            }
        }
    }

    public class BHTHeader
    {
        public UInt32 fileLength;
        public int version;
        public string copyright;

        public void Parse(Parser parser)
        {
            parser.U32(ref fileLength);
            parser.U16(ref version);
            parser.String(ref copyright);
        }
    }


    public class BHTBallPosition
    {
        public int x;
        public int y;
        public int z;
        public int q1;
        public int q2;

        public void Parse(Parser parser)
        {
            parser.I16(ref x);
            parser.I16(ref y);
            parser.I16(ref z);
            parser.U16(ref q1);
            parser.U16(ref q2);
        }
    }

    public class BHTFrameHeader
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
            Parser.Endianess previousEndian = parser.endian;

            // parser.SetEndian(Parser.Endianess.BIG_ENDIAN);
            parser.I16(ref minx);
            parser.I16(ref miny);
            parser.I16(ref minz);
            parser.I16(ref maxx);
            parser.I16(ref maxy);
            parser.I16(ref maxz);
            parser.U16(ref tag);
            // parser.SetEndian(previousEndian);

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
        public int version = 0;

        public int numBalls = 0;
        public List<int> ballSizes = new List<int>(); // # numBalls

        public int animationCount = 0;
        public List<int> animationOffsets = new List<int>(); // # animationCount

        public void Parse(Parser parser)
        {
            int unknown = 0;

            parser.U16(ref framesOffset);
            parser.U16(ref unknown);
            parser.U16(ref version);
            parser.U16(ref numBalls);
            parser.SkipBytes(30);
            parser.ArrayU16(numBalls, ref ballSizes);
            parser.SkipBytes(version == 14 ? 160 : 0); // BHD_VERSION_BABYZ?
            parser.U16(ref animationCount);
            parser.ArrayU16(animationCount, ref animationOffsets);
        }
    }

    public class Parser
    {
        byte[] bytes = null;
        int index = 0;
        public bool isOk = true;

        // @todo handle big endian
        //
        public enum Endianess
        {
            LITTLE_ENDIAN = 0,
            BIG_ENDIAN = 1
        };

        public Endianess endian = Endianess.LITTLE_ENDIAN;

        public Parser(byte[] _bytes, int _offset = 0, Endianess _endian = Endianess.LITTLE_ENDIAN)
        {
            bytes = _bytes;
            index = _offset;
            endian = _endian;
        }

        public void SetEndian(Endianess type = Endianess.LITTLE_ENDIAN)
        {
            endian = type;
        }

        public void SetOffset(int offset)
        {
            index = offset;
        }

        public void SkipBytes(int n)
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

        public void ArrayU16(int n, ref List<int> list)
        {
            for (int i = 0; i < n; i++)
            {
                int value = 0;
                U16(ref value);
                if (isOk)
                {
                    list.Add(value);
                }
            }
        }

        public void Array(int n, Action action)
        {
            for (int i = 0; i < n; i++)
            {
                action();
            }
        }

        public void U32(ref UInt32 outValue)
        {
            if (!isOk)
            {
                return;
            }

            if (index + 4 > bytes.Length)
            {
                isOk = false;
                return;
            }

            outValue = (uint)(
                (bytes[index + 3] << 24) |
                (bytes[index + 2] << 16) |
                (bytes[index + 1] << 8) |
                bytes[index + 0]
            );

            index += 4;
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

                temp += (char)ch;
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

        public void I16(ref int outValue)
        {
            U16(ref outValue);

            // @hack handle negative number
            if (outValue > Int16.MaxValue)
            {
                outValue = -1 * (65536 - outValue);
            }
        }

        public void U16(ref int outValue)
        {
            if (!isOk)
            {
                return;
            }

            if (index + 2 > bytes.Length)
            {
                isOk = false;
                return;
            }

            if (endian == Endianess.BIG_ENDIAN)
            {
                outValue = (bytes[index + 0] << 8) | bytes[index + 1];
            }
            else
            {
                outValue = (bytes[index + 1] << 8) | bytes[index + 0];
            }

            index += 2;
        }
    }
}