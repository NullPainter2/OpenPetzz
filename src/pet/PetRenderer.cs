using Godot;
using System;
using System.Collections.Generic;

//To Do: re-think if this class should inherit from Node2D
public partial class PetRenderer : Node2D
{
	public Vector3 rotation = new Vector3(0, 0, 0);

	//Coordination container (temporary)
	private List<Vector3> coordArray = new List<Vector3> ();

	//Geometry containers
	private List<Ball> ballz = new List<Ball> (); //store ballz
	private List<Line> linez = new List<Line> (); //store ballz

	//this member is temporary 
	private string[] texturePaths = new string[] { /*"./art/textures/flower.bmp"*/ "./Resource/textures/ziverre/ribbon.bmp" };
	
	private List<Texture2D> textureList = new List<Texture2D>();

	//To do: let a Manager class take care of this
	private Texture2D palette;
	//private Texture2D pal;

	private TextureAtlas textureAtlas = null;

	private int animIndex = 0;
	//Methods

	public override void _Ready()
	{
		
		AnimationManager.FetchCatBhd();
		
		LoadTextures();
		//Prepare the Textures
		var texture = textureList[0];

		Texture2D palette = PaletteManager.FetchPalette("petz");
		
		//Ignore until texture atlas is implemented
		/*textureAtlas = new TextureAtlas();
		
		AddChild(textureAtlas);*/

		//Create dummy ballz for now.
		// for (int i = 1; i <= 1; i++)
		for (var ballIndex = 0; ballIndex < AnimationManager.CatBhd.frameHeaders[0].ballPositions.Count; ballIndex++)
		{
			var ballPos = AnimationManager.CatBhd.frameHeaders[0].ballPositions[ballIndex];

			int color = 40;

			int diameter = AnimationManager.CatBhd.header.ballSizes[ballIndex]; // 64
			Ball dummyBall = new Ball(texture, palette, diameter, color, 4, 1, 39);

			Vector2 dummyCoord = new Vector2(ballPos.y, ballPos.z);// new Vector2(0, 0);

			coordArray.Add(new Vector3(dummyCoord.X, dummyCoord.Y, 0));
			dummyBall.Position = dummyCoord;

			dummyBall.ZIndex = 0;

			//add them to the lists
			this.ballz.Add(dummyBall);
			AddChild(dummyBall);
			
			List <PaintBall> paintBallz = new List<PaintBall>();
			
			paintBallz.Add(new PaintBall(new Vector3(1.0f, 0.0f, 0.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(-1.0f, 0.0f, 0.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(0.0f, 1.0f, 0.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(0.0f, -1.0f, 0.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(0.0f, 0.0f, 1.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(0.0f, 0.0f, -1.0f), 0.25f, 95.0f));
			
			//
			paintBallz.Add(new PaintBall(new Vector3(1.0f, 1.0f, 1.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(1.0f, 1.0f, -1.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(1.0f, -1.0f, -1.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(1.0f, -1.0f, 1.0f), 0.25f, 95.0f));
			
			paintBallz.Add(new PaintBall(new Vector3(-1.0f, -1.0f, 1.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(-1.0f, -1.0f, -1.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(-1.0f, 1.0f, 1.0f), 0.25f, 95.0f));
			paintBallz.Add(new PaintBall(new Vector3(-1.0f, 1.0f, -1.0f), 0.25f, 95.0f));
			
			PaintBallGroup pbg = new PaintBallGroup(dummyBall, paintBallz);
			dummyBall.AddChild(pbg);
		}

		//ignore for now
		/*for (int l = 0; l < 2; l++)
		{

			Line dummyLine = new Line(null, null, this.ballz[l], this.ballz[l + 1], -1, 1, 39, 39);

			//add them to the lists
			this.linez.Add(dummyLine);
			AddChild(dummyLine);
		}*/
	}

	public override void _Process(double delta)
	{
		rotation.Y += (float)0.025; 
		UpdateGeometries();
	}

	// CUSTOM Methods

	private void LoadTextures(){
		//start with adding the empty texture for the sake of texture index of -1
		textureList.Add(TextureManager.FetchEmptyTexture());
		
		foreach (string texturePath in texturePaths)
		{
			Texture2D fetchedTexture = TextureManager.FetchTexture(texturePath);
			
			textureList.Add(fetchedTexture);
		}
	}

	//NOTE: Order of updating matters!
	private void UpdateGeometries(){
		UpdateMainBallz();
		UpdateLinez();
	}
	
	//To Do: implement the rotation vector math for x and z rotation
	private void UpdateMainBallz()
	{
		animIndex += 1;

		float rYSin = (float)Math.Sin(rotation.Y);
		float rYCos = (float)Math.Cos(rotation.Y);
		
		float rZSin = (float)Math.Sin(rotation.Z);
		float rZCos = (float)Math.Cos(rotation.Z);
		

		for (int index = 0; index < this.ballz.Count; index++)
		{
			int frame = ( animIndex ) % AnimationManager.CatBhd.frameHeaders.Count;
			var ballPos = AnimationManager.CatBhd.frameHeaders[frame].ballPositions[index];

			Vector3 coord = new Vector3(ballPos.x,ballPos.y,ballPos.z);
			
			// Vector3 coord = this.coordArray[index];

			float xf = coord.X;
			float yf = coord.Y;
			float zf = coord.Z;
			
			float zz = zf;

			zf = (zz * rYCos) - (xf * rYSin);
			xf = (xf * rYCos) + (zz * rYSin);
			
			float yf2 = (yf * rZCos) - (xf * rZSin);
			float xf2 = (xf * rZCos) + (yf * rZSin);

			float z = (float)Math.Round(zf);
			float y = (float)Math.Round(yf2);
			float x = (float)Math.Round(xf2);

			Vector2 v = new Vector2(x, y);

			ballz[index].Position = v;
			//Since Godot renders Nodes with highest Z on top of others unlike original petz l, we set negative of it
			this.ballz[index].ZIndex = (int)-z;
		}
	}
	
	private void UpdateLinez(){
		foreach (Line line in this.linez){
			line.ZIndex = Math.Min(line.start.ZIndex, line.end.ZIndex) - 1;
		}
	}

}
