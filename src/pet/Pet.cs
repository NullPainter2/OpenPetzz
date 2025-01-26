using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

public partial class Pet : Node2D
{
	private PetRenderer petRenderer;


	public override void _Ready()
	{
		World.pets.Add(this);

		// @xxx testing ...
		{

			var FILE = "./Resource/linez/calico-petz3.lnz"; //"YellowBird.lnz";

			var parsed = new Lnz();

			parsed.Parse(FILE);

			//
			// Print the result ...
			//

			foreach (var item in parsed.Eyes)
			{
				GD.Print(String.Format("eye = {0},{1},{2}", item.X, item.Y, item.ID));
			}

			foreach (var item in parsed.Whiskers)
			{
				GD.Print(String.Format("whisker = {0},{1},{2}", item.Start, item.End, item.Color));
			}

			foreach (var item in parsed.EarExtensions)
			{
				GD.Print(String.Format("ear extension = {0}", item.Percent));
			}

			foreach (var item in parsed.Linez)
			{
				GD.Print(String.Format("linez = {0},{1},{2},{3},{4},{5},{6}",
					item.StartBall,
					item.EndBall,
					item.FuzzAmount,
					item.LeftOutlineColor,
					item.RightOutlineColor,
					item.StartThickness,
					item.EndThickness
				));
			}

			foreach (var item in parsed.PaintBallz)
			{
				GD.Print(String.Format("PaintBallz = {0},{1},{2}",
					item.Direction.X,
					item.Direction.Y,
					item.Direction.Z
				));
			}
		}
		
		PetRenderer petRenderer = new PetRenderer();
		AddChild(petRenderer);
	}

	public override void _ExitTree()
	{
		World.pets.Remove(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
	}
}
