using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class PaintBallGroup: MeshInstance2D
{
	private Ball baseBall = null;
	private List <PaintBall> paintBallz = null;
	//Since Godot doesn't have 3D rotation for 2D objects, own 3D rotation "field" needs to be declared.
	public Vector3 rotation = new Vector3(0, 0, 0);
	
	private Mesh meshBuffer = null;
	private ShaderMaterial material = null;
	

	public PaintBallGroup(Ball _base, List < PaintBall > _paintBallz) 
	{
		baseBall = _base;
		paintBallz = _paintBallz;

		Position = new Vector2(0.0f, 0.0f);
	}

	public override void _Ready() 
	{
		material = ShaderManager.FetchShaderMaterial("paintball");
		
		this.Material = material;
		
		var st = new SurfaceTool();

		st.Begin(Mesh.PrimitiveType.Triangles);

		st.SetCustomFormat(0, SurfaceTool.CustomFormat.RgFloat);
		
		//For the sake of keeping paintballz of one ball to one drawcall (for performance reasons), we need to generate a single surface from the list of paintballz and pass their data as vertex attributes.

		foreach (var paintBall in paintBallz)
		{
			// "color" (it is infact, not color)
			//CUSTOM0.r channel: Radius (relative to the base ball, usually between 0.0 to 1.0)
			//CUSTOM0.g channel: The amount of fuzz.
			var info = new Color(paintBall.Size, paintBall.Fuzz, 0.0f, 0.0f);
			
			//This includes info for 3D coordinations of the paintball (.rgb channels) as well as the color index on the palette (.a channel)
			//NOTE: they are clamped to the [0.0, 1.0] range
			st.SetColor(paintBall.Coordinations);
			st.SetCustom(0, info);
			//And finally, add the vertex position (relativr)
			st.AddVertex(new Vector3(-1, -1, 0));
			
			//Repeat this 5 more times (a quad requires two triangles, requiring 6 vertices per quad)
			
			st.SetColor(paintBall.Coordinations);
			st.SetCustom(0, info);
			st.AddVertex(new Vector3(-1, 1, 0));
			
			st.SetColor(paintBall.Coordinations);
			st.SetCustom(0, info);
			st.AddVertex(new Vector3(1, 1, 0));
			
			st.SetColor(paintBall.Coordinations);
			st.SetCustom(0, info);
			st.AddVertex(new Vector3(-1, -1, 0));
			
			st.SetColor(paintBall.Coordinations);
			st.SetCustom(0, info);
			st.AddVertex(new Vector3(1, -1, 0));
			
			st.SetColor(paintBall.Coordinations);
			st.SetCustom(0, info);
			st.AddVertex(new Vector3(1, 1, 0));

		}

		// And finally, generate the mesh.
		meshBuffer = st.Commit();
		
		this.Mesh = meshBuffer;
		
		//Uniform variables are universal for all of the paintballz of a ball.
		//the base ball's fuzz, diameter and position (center) are rrquired for creating the clipping mask in the shader
		this.material.SetShaderParameter("fuzz", baseBall.fuzz);
		this.material.SetShaderParameter("diameter", baseBall.diameter);

		this.material.SetShaderParameter("tex", baseBall.texture);
		this.material.SetShaderParameter("palette", baseBall.palette);

		this.material.SetShaderParameter("center", this.GlobalPosition);
	}

	public override void _Process( double delta )
	{
		this.material.SetShaderParameter(StringManager.S("center"), this.GlobalPosition);
		this.material.SetShaderParameter(StringManager.S("rotation"), baseBall.rotation);
	}
}
