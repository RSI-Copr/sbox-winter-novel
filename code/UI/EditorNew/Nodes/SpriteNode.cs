using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace TerryNovel.Editor
{
	[Node(Title = "Character sprite", HasInput = false, HasOutput = false)]
	public class SpriteNode: BaseNode
	{
		public string File
		{
			get => selector.Text;
			set => selector.Value = value;
		}
		public string Name
		{
			get => textEntry.Text;
			set => textEntry.Text = value;
		}
		public float Scale
		{
			get
			{
				if ( float.TryParse( scale.Text , out var f) )
				{
					return f;
				}

				return 1;
			}
			set => scale.Text = value.ToString();
		}


		private readonly FileSelector selector;
		private readonly TextEntry textEntry;
		private readonly TextEntry scale;
		public SpriteNode()
		{
			textEntry = Canvas.Add.TextEntryWithPlaceHolder( "Name" );
			selector = Canvas.AddChild<FileSelector>();
			selector.Finder = () => FileSystem.Mounted.FindFile( "assets/textures/", "*.png" );


			scale = Canvas.Add.TextEntryWithPlaceHolder( "Scale" );
			scale.Text = "1";
			scale.Numeric = true;
		}
	}
}
