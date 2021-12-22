using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.IO;

namespace TerryNovel.Editor
{
	[Node( Title = "Character", HasInput = false, HasOutput = false )]
	public class CharacterNode:BaseNode
	{
		private TextEntry textEntry;
		public CharacterNode()
		{
			textEntry = Canvas.Add.TextEntryWithPlaceHolder("Name");

			Editor.Characters.AddNode( this );
			textEntry.AddEventListener( "onchange", () => {
				Name = textEntry.Text;
			} );

		}

		private string name;
		public string Name
		{
			get => name;
			set
			{
				name = value;
				Editor.Characters.Update( this, value );
			}

		}

		public override void OnDeleted()
		{
			Editor.Characters.Remove( this );
			base.OnDeleted();
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Name );
		}

		public override void Read( BinaryReader reader )
		{
			var name = reader.ReadString();
			Name = name;
			textEntry.Text = name;
		}

	}
}
