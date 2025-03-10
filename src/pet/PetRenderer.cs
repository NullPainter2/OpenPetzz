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
	private string[] texturePaths = new string[] { "./art/textures/flower.bmp" };
	
	private List<Texture2D> textureList = new List<Texture2D>();

	//To do: let a Manager class take care of this
	private Texture2D palette;

	private TextureAtlas textureAtlas = null;
	//Methods

	public override void _Ready()
	{

		//dummy
		
		//var shadee = ShaderManager.FetchShader("ball");
		
		//.Print(shadee.Code);		
		LoadTextures();
		//Prepare the Textures
		var texture = textureList[1];

		palette = GD.Load<Texture2D>("res://Resource/palettes/petzpalette.png");
		
		textureAtlas = new TextureAtlas();
		
		AddChild(textureAtlas);

		//Create dummy ballz for now.
		for (int i = 1; i <= 3; i++)
		{

			int color = 85;
			
			Ball dummyBall = new Ball(texture, palette, 100 - i*20, color, 4, 1, 39);

			Vector2 dummyCoord = new Vector2(i * 100 - 200, 0);

			coordArray.Add(new Vector3(dummyCoord.X, dummyCoord.Y, 0));
			dummyBall.Position = dummyCoord;

			dummyBall.ZIndex = 0;

			//add them to the lists
			this.ballz.Add(dummyBall);
			AddChild(dummyBall);
		}

		for (int l = 0; l < 2; l++)
		{

			Line dummyLine = new Line(null, null, this.ballz[l], this.ballz[l + 1], -1, 1, 39, 39);

			//add them to the lists
			this.linez.Add(dummyLine);
			AddChild(dummyLine);
		}
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
