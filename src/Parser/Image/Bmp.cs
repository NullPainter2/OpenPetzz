using Godot;
using System;

public class Bmp {

	private uint Width {get; set;} = 0;
	private uint Height {get; set;} = 0;
	
	private Image Palette { get; set; } = null;
	private Image Raster { get; set; } = null;
	
	public uint BitCount { get; private set; } = 0;
	public bool Loaded { get; private set; } = false;
	
	[Flags] public enum LoadType {
	    Palette = 1,
	    Raster = 2,
	    All = 3,
	}
	
	
	public void LoadFile(string _path, LoadType _type){
		
		try {
			using var file = FileAccess.Open(_path, FileAccess.ModeFlags.Read);
			
			int junk = 0;
			//Ignore first 14 bytes
			for (junk = 0; junk < 7; junk++)
				file.Get16();
			
			uint headerSize = file.Get32();
			
			if (headerSize != 40)
			{
			    PrintError("HeaderSize is not 40");
				return;
			}
			
			//Read width and height;
			if (_type.HasFlag(LoadType.Palette))
			{
    			    Width = file.Get32();
    			    Height = file.Get32();
    			
    			//Between 1 and 512 (inclusive) only
    			
    			    if (Width <= 0 || Height <= 0)
                            {
    			        PrintError("Width or Height is 0 or below.");
    				return;
    			    }
    			
    			    if (Width > 1024 || Height > 1024)
    			    {
    			        PrintError("Width or Height is above 1024.");
    				return;
    			    }
			}
			
			//Ignore these two bytes too
			file.Get16();
			
			BitCount = file.Get16();
			
			if (BitCount != 8 && BitCount != 24)
    			{
    			    PrintError("Invalid Bit Count");
    				return;
    			}
			
			uint compression = file.Get32();
			
			if (compression != 0)
    			{
    			    PrintError("Compression is not supported.");
    				return;
    			}
				
			//Ignore 20 more bytes
			for (junk = 0; junk < 5; junk++)
				file.Get32();
			
			//if 8 bit, then there must be the index palette, in BGR0 format 
			if (BitCount == 8){
				
			    if (_type.HasFlag(LoadType.Palette))
			    {
    				byte[] pal = file.GetBuffer(1024);
    				//generate the palette
				    Palette = Image.CreateFromData(256, 1, false, Image.Format.Rgba8, pal);
			    } else {
			        //Bye bye those 1024 bytes 
			        file.GetBuffer(1024);
			    }
			}
			
			//Now for the raster part
			
			if (_type.HasFlag(LoadType.Raster))
			    {
			
    			    int bytesPerTexel = BitCount == 24 ? 3 : 1;
    			
    			    byte[] raster = file.GetBuffer(Width * Height * bytesPerTexel);
    			
    			    if (bytesPerTexel == 3){
    				Raster = Image.CreateFromData((int)Width, (int)Height, false, Image.Format.Rgb8, raster);
    			    } else {
    				Raster = Image.CreateFromData((int)Width, (int)Height, false, Image.Format.R8, raster);
    			    }
			}
			//All good? then it means successfully loaded
			Loaded = true;
			
		}catch (Exception e){
			
		}
	}
	
	public Texture2D GetPalette() {
		if (Loaded && Palette != null){
			
			var textureImg = ImageTexture.CreateFromImage(Palette);
			Texture2D texture = textureImg as Texture2D;
			
			return texture;
		}
		else
		{
			return null;
		}
	}
	
	public Texture2D GetData() {
		if (Loaded && Raster != null){
			
			var textureImg = ImageTexture.CreateFromImage(Raster);
			Texture2D texture = textureImg as Texture2D;
			
			return texture;
		}
		else
		{
			return null;
		}
	}
	
	private void PrintError(string message)
	{
	    GD.Print("BMP loader error: "+ message);
	}
	
}
