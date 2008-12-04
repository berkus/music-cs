using System;
using System.IO;

namespace MusiC
{
	public class TrainLabel : MusiCObject
	{
		String _label;
		String _inDir;
		String _outDir;
		
		public String Label
		{
			get {return _label;}
			set {_label=value;}
		}
		
		public String OutputDir
		{
			get {return _outDir;}
			set {_outDir=value;}
		}
		
		public String InputDir
		{
			get {return _inDir;}
			set {_inDir=value;}
		}
		
		/* override public String ToString()
		{
			String obj=PrintHeader();
			
			obj= obj +
			"[Label]:"+ _label + "\n" +
			"[Input Dir]:"+_inDir + "\n"+
			"[Output Dir]:"+_outDir + "\n";
			
			obj = obj + PrintFooter();
			
			return obj;
		} */
		
		public void Validate()
		{
			if(!Directory.Exists(_outDir))
			{
				/// @todo Throw an Exception
				Console.WriteLine("The specified output directory isnt available");
			}
			
			if(!Directory.Exists(_inDir))
				/// @todo Throw an Exception
				Console.WriteLine("The specified input directory isnt available.");
				
			Print();
		}
	}
}
