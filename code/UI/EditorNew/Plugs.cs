using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	public class Plug : Panel
	{
		public readonly BaseNode Node;
		public Plug( BaseNode node)
		{
			Node = node;
			AddClass( "plug" );
			Add.Label( "navigate_next" );
		}
	}

	public class PlugIn : Plug
	{

		public PlugIn( BaseNode node ):base(node)
		{
			AddClass( "input" );
			
		}



	}
	public class PlugOut : Plug
	{
		public readonly Color Color;
		private Color GenerateRandomColor()
		{
			return Color.Random;
		}
		public PlugOut( BaseNode node ) : base( node )
		{
			Color = GenerateRandomColor();
			Style.BackgroundColor = Color;
			AddClass( "output" );
		}
	}

	class Connection
	{
		public PlugIn Input;
		public PlugOut Output;

		public bool IsValid => Input.Parent != null && Output.Parent != null;
	}
}
