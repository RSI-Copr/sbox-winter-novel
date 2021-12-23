using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel.Editor
{
	partial class Editor
	{
		public static DirectiryHandler Dir = new();
		public class DirectiryHandler
		{
			public string Current => $"novels/{Name}";
			public string Name { get; set; }
			public string Backgrounds => Current + "/backgrounds/";
			public string Sounds => Current + "/sounds";
			public string ProjectFile => Current + "/.novel_project";
			

			public void CreateAll()
			{
				var fs = Novel.FS;
				fs.CreateDirectory( Current );
				fs.CreateDirectory( Backgrounds );
				fs.CreateDirectory( Sounds );
			}
		}

		public static bool Active { get; set; }

		public static void CrateProject(string name )
		{
			Dir.Name = name;
			Dir.CreateAll();
			GraphEditor.Reset();
			Active = true;
		}

		public static NovelInfo Info { get; set; }
	}
}
