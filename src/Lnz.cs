using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot; // For Vector3 (instead of using System.Numerics)

// @todo get proper ball id 
// @todo validate
// @todo add test

public class Lnz
{
    // [Move]
    public class Move
    {
        // https://web.archive.org/web/20190805174318fw_/http://www.angelfire.com/moon2/petzzoo2/id27.htm		//

        public int BallNumber = -1;
        public Vector3 Offset = new Vector3(0, 0, 0);
        public int AnchorBallID = -1; // optional

        public static Move FromLine(string str)
        {
            Move result = new Move();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.BallNumber);
            parser.Vector3(ref result.Offset);
            if (!parser.isOK)
            {
                return null;
            }

            // optional, so we can ignore if it isn't there
            parser.Int(ref result.AnchorBallID);

            return result;
        }
    };

    // [Fur Pattern Balls]
    // ==================== 
    // ; addballs which are part of fur only 
    //     addballs:	92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103	; tail
    public class FurPatternBall
    {
        // public List<int> BallID = new List<int>();

        public static FurPatternBall FromLine(string str)
        {
            // @todo do
            return null;
        }
    };

    //  [Add ball]
    public class AddBall
    {
        // https://web.archive.org/web/20190805174318fw_/http://www.angelfire.com/moon2/petzzoo2/id27.htm		//

        public int BallID = 0;
        public Vector3 Offset = new Vector3(0, 0, 0); // in relation to center of the base
        public int Color = 0;
        public int OutlineColor = 0;
        public int spckCol = 0; // @todo ???
        public float Fuzz = 0;
        public int Group = 0; // @todo same as ball info
        public int Outline = 0;
        public float BallSize = 0;
        public float BodyArea = 0;
        public int AddGroup = 0; // ???
        public int Texture = 0;

        public static AddBall FromLine(string str)
        {
            AddBall result = new AddBall();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.BallID);
            parser.Vector3(ref result.Offset);
            parser.Int(ref result.Color);
            parser.Int(ref result.OutlineColor);
            parser.Int(ref result.spckCol);
            parser.Float(ref result.Fuzz);
            parser.Int(ref result.Group);
            parser.Enum(ref result.Outline, new int[] { -1, 0, -2 });
            parser.Float(ref result.BallSize);
            parser.Float(ref result.BodyArea);
            parser.Int(ref result.AddGroup);
            parser.Int(ref result.Texture);

            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    // [Body Area]
    public class BodyArea
    {
        // https://web.archive.org/web/20190805174318fw_/http://www.angelfire.com/moon2/petzzoo2/id27.htm		//

        public int BallID = 0;
        public float Area = 0;

        public static BodyArea FromLine(string str)
        {
            var result = new BodyArea();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.BallID);
            parser.Float(ref result.Area);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    // [Omissions]
    public class Omission
    {
        // https://web.archive.org/web/20190805174318fw_/http://www.angelfire.com/moon2/petzzoo2/id27.htm		//

        public int IgnoredID = -1;

        public static Omission FromLine(string str)
        {
            var result = new Omission();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.IgnoredID);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    // [Project Ball]
    public class ProjectBall
    {
        // https://web.archive.org/web/20190805174318fw_/http://www.angelfire.com/moon2/petzzoo2/id27.htm		//

        public int FixedBallID = -1;
        public int BallToMoveID = -1;
        public float Distance = 0;

        public static ProjectBall FromLine(string str)
        {
            ProjectBall result = new ProjectBall();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.FixedBallID);
            parser.Int(ref result.BallToMoveID);
            parser.Float(ref result.Distance);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    // [Ballz Info]
    public class BallzInfo
    {
        // https://web.archive.org/web/20190805174318fw_/http://www.angelfire.com/moon2/petzzoo2/id27.htm		//

        public int Color = 0;
        public int OutlineColor = 0;
        public int spklCl = 0; // ???
        public float Fuzz = 0;
        public int otlntTyp = 0;
        public float SizeDif = 0;
        public int Group = 0;
        public int Texture = 0;
        public string BallID; // can be "eBall_irisL"

        public static BallzInfo FromLine(string str)
        {
            BallzInfo result = new BallzInfo();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.Color);
            parser.Int(ref result.OutlineColor);
            parser.Int(ref result.spklCl);
            parser.Float(ref result.Fuzz);
            parser.Int(ref result.otlntTyp);
            parser.Float(ref result.SizeDif);
            parser.Int(ref result.Group);
            parser.Int(ref result.Texture);
            parser.String(ref result.BallID);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    // [Paint Ballz]
    public class PaintBall
    {
        //
        // https://web.archive.org/web/20240313175338/https://homebody.eu/carolyn/downloabx/tutorials/tutorialbits/LNZ2nd.txt
        //
        // ;base ball diameter(% of baseball) direction color outlinecolor fuzz outline group texture

        public int BaseBall = 0; // @todo ball index
        public float Diameter = 0;
        public Vector3 Direction = new Vector3(0, 0, 0);
        public int Color = 0;
        public int OutlineColor = 0;
        public float Fuzz = 0;
        public int Outline = 0;
        public int Group = 0;
        public int Texture = 0;

        public static PaintBall FromLine(string str)
        {
            PaintBall result = new PaintBall();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.BaseBall);
            parser.Percentage(ref result.Diameter);
            parser.Vector3(ref result.Direction);
            parser.Int(ref result.Color);
            parser.Int(ref result.OutlineColor);
            parser.Float(ref result.Fuzz);
            parser.Int(ref result.Outline);
            parser.Int(ref result.Group);
            parser.Int(ref result.Texture);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    // https://web.archive.org/web/20220702142153/https://petz.filthyhippie.net/tutorials/lnz-pro-101/
    public class Line
    {
        // The 1st column is the start ball, or the ball number the line comes from.
        public int StartBall = 0;

        // The 2nd column is the end ball, the ball number the line connects to.
        public int EndBall = 0;

        // The 3rd column is the fuzz amount of the line.
        public float FuzzAmount = 0;

        // The 4th column always needs to be -1. This makes the line take on the color AND texture of it’s start ball.
        public int Color = 0;

        // The 5th and 6th columns are the outline colors of the left and right side of the line, respectively.
        public int LeftOutlineColor = 0;

        public int RightOutlineColor = 0;

        // The 7th and 8th columns are the starting thickness and ending thickness of the line.
        public int StartThickness = 0;
        public int EndThickness = 0;

        public static Line FromLine(string str)
        {
            Line result = new Line();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.StartBall);
            parser.Int(ref result.EndBall);
            parser.Float(ref result.FuzzAmount);
            parser.Int(ref result.Color); // The color of the line (usually left at -1).
            parser.Int(ref result.LeftOutlineColor);
            parser.Int(ref result.RightOutlineColor);
            parser.Int(ref result.StartThickness);
            parser.Int(ref result.EndThickness);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    public class Whisker
    {
        public int Start = 0;
        public int End = 0;
        public int Color = 0;

        public static Whisker FromLine(string str)
        {
            Whisker result = new Whisker();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.Start);
            parser.Int(ref result.End);
            parser.Int(ref result.Color);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    // [Ear Extension]
    public class EarExtension
    {
        public int Percent = 0;

        public static EarExtension FromLine(string str)
        {
            EarExtension result = new EarExtension();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.Percent);
            if (!parser.isOK)
            {
                return null;
            }

            return result;
        }
    };

    /*

        https://petz.miraheze.org/wiki/LNZ

     */
    public class Eye
    {
        public int X = 0;
        public int Y = 0;
        public string ID = "";

        /*
        EXAMPLE:

        [Eyes]
        12, 34			RightEye/leftEye
        28, 56 			RightIris/leftIris

        */
        public static Eye FromLine(string str)
        {
            Eye result = new Eye();

            RowParser parser = new RowParser(str);
            parser.Int(ref result.X);
            parser.Int(ref result.Y);
            parser.String(ref result.ID);
            if (!parser.isOK)
            {
                return null;
            }

            return result;

            // var parts = str.Split(
            //     new char[] { ' ', '\t', ',' },
            //     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            // );
            //
            // if (parts.Length != 3)
            // {
            //     return null;
            // }
            //
            // var result = new Eye();
            //
            // if (!int.TryParse(parts[0], out result.X))
            // {
            //     return null;
            // }
            //
            // if (!int.TryParse(parts[1], out result.Y))
            // {
            //     return null;
            // }
            //
            // result.ID = parts[2];
            //
            // return result;
        }
    };

    public List<Eye> Eyes = new List<Eye>();
    public List<Whisker> Whiskers = new List<Whisker>();
    public List<EarExtension> EarExtensions = new List<EarExtension>();
    public List<Line> Linez = new List<Line>();
    public List<PaintBall> PaintBallz = new List<PaintBall>();
    public List<BallzInfo> BallzInfoz = new List<BallzInfo>();
    public List<Move> Movez = new List<Move>();
    public List<ProjectBall> ProjectBallz = new List<ProjectBall>();
    public List<int> Omissions = new List<int>();
    public List<AddBall> AddBallz = new List<AddBall>();
    public List<BodyArea> BodyAreaz = new List<BodyArea>();
    public List<FurPatternBall> FurPatternBalls = new List<FurPatternBall>();

    public void Parse(string fileName)
    {
        var lines = File.ReadAllLines(fileName);

        int lineIndex = 0;
        while (lineIndex < lines.Length) // for (int i = 0; i < lines.Length;)
        {
            // Console.WriteLine("Parsing line '" + lines[lineIndex] + '"');

            var line = lines[lineIndex].Trim();

            // Lines sometimes have a comments after the section name.
            // That comment can be whatever, sometimes it is a proper comment too (";", "//").
            // -> use StartsWith

            if (line.StartsWith("[Fur Pattern Balls]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = FurPatternBall.FromLine(row);
                    if (item != null)
                    {
                        FurPatternBalls.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Body Area]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = BodyArea.FromLine(row);
                    if (item != null)
                    {
                        BodyAreaz.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Add ball]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = AddBall.FromLine(row);
                    if (item != null)
                    {
                        AddBallz.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Omissions]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = Omission.FromLine(row);
                    if (item != null)
                    {
                        Omissions.Add(item.IgnoredID);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Project Ball]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = ProjectBall.FromLine(row);
                    if (item != null)
                    {
                        ProjectBallz.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Move]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = Move.FromLine(row);
                    if (item != null)
                    {
                        Movez.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Ballz Info]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = BallzInfo.FromLine(row);
                    if (item != null)
                    {
                        BallzInfoz.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Paint Ballz]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = PaintBall.FromLine(row);
                    if (item != null)
                    {
                        PaintBallz.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Linez]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = Line.FromLine(row);
                    if (item != null)
                    {
                        Linez.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else if (line.StartsWith("[Eyes]"))
            {
                ForeachRowInSection((row) =>
                {
                    var item = Eye.FromLine(row);
                    if (item != null)
                    {
                        Eyes.Add(item);
                    }
                }, lines, ref lineIndex);
            }
            else
            {
                lineIndex++;
            }
        }
    }

    void ForeachRowInSection(Action<string> lineCallback, string[] lines, ref int lineIndex)
    {
        // next line
        lineIndex++;

        while (lineIndex < lines.Length)
        {
            string line = lines[lineIndex].Trim();

            // Remove comment (Some Petz 3 files had it)
            int commentIndex = line.IndexOf("//", 0, StringComparison.Ordinal);
            if (commentIndex != -1)
            {
                line = line.Substring(0, commentIndex);
                line = line.Trim();
            }

            // Console.WriteLine("Parsing item ... '" + lines[lineIndex] + '"');

            // skip empty lines
            bool isEmptyLine = line.Length == 0;
            if (isEmptyLine)
            {
                lineIndex++;

                continue;
            }

            bool nextEntry = line[0] == '[';
            if (nextEntry)
            {
                return; // all items are parsed, keep the current line index
            }

            bool isComment = line[0] == ';' || line[0] == '#';
            if (!isComment)
            {
                lineCallback(line);
            }

            lineIndex++; // next line
        }
    }

    class RowParser
    {
        string[] parts;
        int index = 0;
        public bool isOK = true;

        public RowParser(string row)
        {
            parts = row.Split(
                new char[] { ' ', '\t', ',' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
        }

        public void Percentage(ref float percentage)
        {
            this.Float(ref percentage);
            if (isOK)
            {
                percentage = percentage / 100f;
            }
        }

        public void Vector3(ref Vector3 outValue)
        {
            float X = 0;
            float Y = 0;
            float Z = 0;

            if (!isOK)
            {
                return;
            }

            // need to have space for 3 items
            if (index + 2 >= parts.Length)
            {
                isOK = false;
                return;
            }

            this.Float(ref X);
            this.Float(ref Y);
            this.Float(ref Z);

            if (isOK)
            {
                outValue.X = X;
                outValue.Y = Y;
                outValue.Z = Z;
            }
            else
            {
                isOK = false;
            }
        }

        public void Int(ref int intValue)
        {
            if (!isOK)
            {
                return;
            }

            if (index >= parts.Length)
            {
                isOK = false;
                return;
            }

            if (int.TryParse(parts[index], out intValue))
            {
                index++; // prepare to read next value
            }
            else
            {
                isOK = false;
            }
        }


        public void Enum(ref int outValue, int[] validValues)
        {
            if (!isOK)
            {
                return;
            }

            if (index >= parts.Length)
            {
                isOK = false;
                return;
            }

            int value = 0;
            if (int.TryParse(parts[index], out value))
            {
                if (validValues.Contains(value))
                {
                    outValue = value;

                    index++; // prepare to read next value
                }
                else
                {
                    isOK = false;
                }
            }
            else
            {
                isOK = false;
            }
        }

        public void Float(ref float outValue)
        {
            if (!isOK)
            {
                return;
            }

            if (index >= parts.Length)
            {
                isOK = false;
                return;
            }

            // ".3" can't be parsed by just `float.TryParse(".3", out value)`
            if (float.TryParse(
                    parts[index],
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out outValue
                ))
            {
                index++; // prepare to read next value
            }
            else
            {
                isOK = false;
            }
        }

        public void String(ref string stringValue)
        {
            if (!isOK)
            {
                return;
            }

            if (index >= parts.Length)
            {
                isOK = false;
                return;
            }

            stringValue = parts[index];
            index++;
        }
    };


    public void Test()
    {
        // test parsing the sequence and float values
        var rp = new RowParser(".25 0.125 123456 123.125");

        float X = 0;
        
        rp.Float(ref X);
        if (X != .25f)
        {
            GD.Print("Parsing .25 failed");
        }
        
        rp.Float(ref X);
        if (X != .125f)
        {
            GD.Print("Parsing 0.125 failed");
        }

        rp.Float(ref X);
        if (X != 123456f)
        {
            GD.Print("Parsing 123456f failed");
        }

        rp.Float(ref X);
        if (X != 123.125f)
        {
            GD.Print("Parsing 123.125 failed");
        }

    }

    //
    // EXAMPLE:
    //
    // public static void Main()
    // {
    //     var FILE = "W1ACLOWN.LNZ"; //"YellowBird.lnz";
    //
    //     var parsed = new Linez();
    //
    //     parsed.Parse(FILE);
    //
    //     //
    //     // Print the result ...
    //     //
    //
    //     foreach (var eye in parsed.Eyes)
    //     {
    //         Console.WriteLine("eye = {0},{1},{2}", eye.X, eye.Y, eye.ID);
    //     }
    //
    //     if (true) // @debug to put breakpoint
    //     {
    //     }
    // }
}