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

	private Lnz parsedLNZFile = null;
	//Methods

	public void Init(Lnz _parsedLNZFile)
	{
		parsedLNZFile = _parsedLNZFile;
	}
	
	public override void _Ready()
	{
		LoadTextures();
		//Prepare the Textures
		var texture = textureList[0];

		Texture2D palette = PaletteManager.FetchPalette("funnytime");
		
		//Ignore until texture atlas is implemented
		/*textureAtlas = new TextureAtlas();
		
		AddChild(textureAtlas);*/

		//Create dummy ballz for now.
		for (int i = 1; i <= 1; i++)
		{

			int color = 40;
			
			Ball dummyBall = new Ball(texture, palette, 64, color, 4, 1, 39);

			Vector2 dummyCoord = new Vector2(0, 0);

			coordArray.Add(new Vector3(dummyCoord.X, dummyCoord.Y, 0));
			dummyBall.Position = dummyCoord;

			dummyBall.ZIndex = 0;

			//add them to the lists
			this.ballz.Add(dummyBall);
			AddChild(dummyBall);
			
			List <PaintBall> paintBallz = new List<PaintBall>();
			
			foreach(var paintBall in parsedLNZFile.PaintBallz)
			{
				Vector3 coords = paintBall.Direction;
				float size = paintBall.Diameter / 100.0f;
				float colorIndex = (float) paintBall.Color;
				paintBallz.Add(new PaintBall(coords, size, colorIndex)); 
			}
			
			/*
			
			// le CookieFactory(tm)
			paintBallz.Add(new PaintBall(new Vector3(0.07f, -0.78f, -0.5f), 0.19f, 90.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.83f, -0.63f, 0.89f), 0.1f, 91.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.57f, -0.25f, -0.24f), 0.16f, 92.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.52f, -0.3f, -0.86f), 0.14f, 93.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.73f, -0.52f, -0.6f), 0.04f, 95.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.98f, 0.73f, 0.98f), 0.02f, 96.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.33f, 0.77f, -0.52f), 0.22f, 97.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.11f, -0.61f, -0.55f), 0.14f, 10.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.09f, 0.9f, 0.93f), 0.11f, 20.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.31f, -0.22f, -0.92f), 0.09f, 30.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.87f, -0.82f, 0.3f), 0.21f, 95.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.35f, -0.87f, -1f), 0.08f, 95.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.81f, -0.72f, -0.11f), 0.08f, 183.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.54f, -0.91f, 0.73f), 0.17f, 183.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.25f, 0.4f, 0.76f), 0.23f, 183.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.94f, -0.44f, 0.81f), 0.04f, 183.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.19f, 0.64f, -0.04f), 0.22f, 183.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.04f, -0.25f, -0.82f), 0.18f, 183.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(-0.11f, 0.63f, 0.64f), 0.24f, 183.0f)); 
			paintBallz.Add(new PaintBall(new Vector3(0.33f, 0.43f, -0.7f), 0.22f, 183.0f)); 
			
			*/
			
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
		//rotation.Y += (float)0.05; 
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

		float rYSin = (float)Math.Sin(rotation.Y);
		float rYCos = (float)Math.Cos(rotation.Y);
		
		float rZSin = (float)Math.Sin(rotation.Z);
		float rZCos = (float)Math.Cos(rotation.Z);
		
		for (int index = 0; index < this.ballz.Count; index++)
		{

			Vector3 coord = this.coordArray[index];

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
