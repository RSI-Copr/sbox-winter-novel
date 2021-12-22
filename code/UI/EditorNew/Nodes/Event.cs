using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	

	[Node( Title = "Set background", HasOutput = false )]
	public class BackGroundEventNode: BaseNode, IEventNode
	{
		public string Name
		{
			get => selector.Value;
			set => selector.Value = value;
		}

		private readonly FileSelector selector;
		public BackGroundEventNode()
		{
			selector = AddChild<FileSelector>( );
			selector.Finder = Editor.Backgrounds.Get;
		}

		public NovelEvent GenerateEvent()
		{
			return new BackGroundChangeEvent( Name );
		}
		
		public override void Write( BinaryWriter writer )
		{
			writer.Write( Name );
			
		}
		public override void Read( BinaryReader reader )
		{
			Name = reader.ReadString();
		}
	}

	[Node( Title = "Play music", HasOutput = false )]
	public class MusicEventNode : BaseNode, IEventNode
	{
		public NovelEvent GenerateEvent()
		{
			throw new NotImplementedException();
		}
	}

	public class FileSelector : PopupButton
	{
		private string value;
		public string Value
		{
			get => value;
			set
			{
				Text = value;
				this.value = value;
			}

		}

		public Func<IEnumerable<string>> Finder;


		public FileSelector()
		{
			AddClass( "dropdown" );
			Add.Icon( "expand_more", "dropdown_indicator" );
		}
		public override void Open()
		{
			Popup = new Popup( this, Popup.PositionMode.BelowStretch, 0.0f );
			Popup.AddClass( "flat-top" );

			foreach ( var file in Finder.Invoke() )
			{
				if ( file == Value ) continue;

				var o = Popup.AddOption( file, () => Value = file );

			}
		}

	}

	
}
