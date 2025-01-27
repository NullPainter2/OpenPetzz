using Godot;
using System;
using System.Collections.Generic;

public static class ShaderManager {
    private static Dictionary<string, Shader> fetchedShaders = new Dictionary<string, Shader>();
	
    //temporary
    private static string colorShaderCode = "";
    //also temporary
    private static string circleShaderCode = "";
    //To do: improve this
    private static Dictionary<string, Shader> fetchedComponentShaders = new Dictionary<string, Shader>();
	
    public static Shader FetchShader(string name)
    {
        if (fetchedShaders.ContainsKey(name))
        {
            return fetchedShaders[name];
        }
		
        if (colorShaderCode == ""){
            using var compFile = FileAccess.Open("./shaders/components/color.shader", FileAccess.ModeFlags.Read);
            colorShaderCode = compFile.GetAsText();
        }
		
        if (circleShaderCode == ""){
            using var compFile = FileAccess.Open("./shaders/components/circle.shader", FileAccess.ModeFlags.Read);
            circleShaderCode = compFile.GetAsText();
        }
		
        try{
            using var file = FileAccess.Open("./shaders/"+name+".shader", FileAccess.ModeFlags.Read);
			
            string code = file.GetAsText();
			
            code = code.Replace("@LoadColorShaderComponent", colorShaderCode);
            code = code.Replace("@LoadCircleShaderComponent", circleShaderCode);
			
            Shader shader = new Shader();
			
            shader.Code = code;
			
            return shader;
        } catch( Exception e){
            return null;
        }
    }
	
    public static ShaderMaterial FetchShaderMaterial(string name)
    {
        var shader = ShaderManager.FetchShader(name);
		
        var shaderMaterial = new ShaderMaterial();
		
        shaderMaterial.Shader = shader;
		
        return shaderMaterial;
    }
}