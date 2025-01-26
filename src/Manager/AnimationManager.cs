using Godot;
using System;
using System.Collections.Generic;
using OpenPetz;

public static class AnimationManager {
	
	private static Bhd2 CatBhd { get; set; } = null;
	
	//Methods
	
	private static void LoadCatBhd()
	{
		
	}
	
	public static void FetchCatBhd()
	{
		List<string> bdtFiles = new List<string>();
		bdtFiles.Add("./ptzfiles/cat/CAT0.bdt");
		
		// CatBhd = new Bhd("./ptzfiles/cat/CAT.bhd", bdtFiles);/**/
		CatBhd = new Bhd2("./ptzfiles/cat/CAT.bhd", bdtFiles);
	}

}

