using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	[Node( Title = "Message" )]
	public class MessageNode : BaseNode
	{
		public class MessageCanvas:Panel{
			private PlugOut Output;
			public CharacterPanel Character;

			public MessageCanvas(BaseNode node)
			{
				AddClass( "message" );
				Add.Button( "face", "characterselect", OpenСharacterSelector );
				Add.TextEntry( "" );
				Output = new PlugOut( node );
				AddChild( Output );
			}

			private void OpenСharacterSelector()
			{
				var ch = Editor.Characters.Get();
				if ( ch.Count == 0 ) return;
				var popup = new Popup( this, Popup.PositionMode.BelowLeft, 0 );
				foreach(var kv in ch )
				{
					popup.AddOption( kv.Value, () => SelectCharacter(kv.Key) );
				}
			}

			private void SelectCharacter( CharacterNode node)
			{
				if( Character == null || Character.Parent == null )
				{
					Character = new CharacterPanel( node );
					AddChild( Character );
				}
				else
				{
					Character.UpdateFrom( node );
				}
			}

			
		}

		

		protected override void CreateContents()
		{
			AddLine();

		    Add.Button( "add", "plus", AddLine );
		}

		private void AddLine()
		{
			var msg = new MessageCanvas( this );
			Canvas.AddChild( msg );
		}
	}

	

	[Node( Title = "Answer" )]
	public class AnswerNode : BaseNode
	{
	}
}
