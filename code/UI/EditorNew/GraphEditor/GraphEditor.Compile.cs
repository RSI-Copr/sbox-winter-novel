using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TerryNovel.Editor
{
	partial class GraphEditor
	{
		public void GenetateNovelFile()
		{
			var infoNode = GetInfoNode();
			var start = GetStartNode();


			var parser = new NodeGraphParser();
			var info = parser.Parse( start );

			info.Name = infoNode.Title;
			info.Description = infoNode.Desc;

			
		}

	}
}
