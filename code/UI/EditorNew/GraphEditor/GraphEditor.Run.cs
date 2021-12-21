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
		public static void RunNovelFromEditor()
		{
			Instance.Run();
		}

		public StartNode GetStartNode()
		{
			return BaseNode.GetNodeById( 1 ) as StartNode;
		}

		public InfoNode GetInfoNode()
		{
			return BaseNode.GetNodeById( 0 ) as InfoNode;
		}

		public void Run()
		{
			var info = GetInfoNode();

			Novel.Name = info.Title;
			Novel.Description = info.Desc;

			var start = GetStartNode();
			

			foreach(var node in start.DependedNodes )
			{
				Log.Info( node );
			}
		}
	}
}
