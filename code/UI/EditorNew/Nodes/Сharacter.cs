using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	[Node( Title = "Character", HasInput = false, HasOutput = false )]
	public class CharacterNode:ProperyNode
	{
		public CharacterNode()
		{
			Editor.Characters.AddNode( this );
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
		}

		
	}
}
