using Sandbox.UI;
using Sandbox.UI.Construct;
using System.IO;
using System.Collections.Generic;

namespace TerryNovel.Editor
{
	[Node( Title = "Message" )]
	public class MessageNode : BaseNode
	{
		private readonly List<MessageEntry> Messages = new();

		public class MessageEntry:Panel{

			public readonly PlugOut Output;
			public CharacterPanel Character;
			private TextEntry textEntry;
			
			public string Text
			{
				get => textEntry.Text;
				set => textEntry.Text = value;
			}


			public MessageEntry(BaseNode node)
			{
				AddClass( "message" );
				Add.Button( "face", "characterselect", OpenСharacterSelector );

				textEntry = Add.TextEntry( "" );
				textEntry.Multiline = true;


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

			private void SetCharacter(int id)
			{

			}
			
		}

		
		public MessageNode()
		{
			AddLine();
			Add.Button( "add", "plus", () => AddLine() );
		}
	

		private void ClearLines()
		{
			foreach(var msg in Messages )
			{
				msg.Delete();
			}

			Messages.Clear();
		}

		

		private MessageEntry AddLine()
		{
			var msg = new MessageEntry( this );
			Messages.Add( msg );
			Canvas.AddChild( msg );
			return msg;
		}

		private void RemoveLine( MessageEntry entry)
		{
			Messages.Remove( entry );
			entry.Delete();
		}

		protected override void OnEdit( OptionsPanel options, Panel target )
		{
			if(target is MessageEntry msg )
			{
				options.AddOption( $"Delete line {msg.Text.Substring( 0, 5 )}..." ,()=> RemoveLine( msg ) );
			}
		}

		public override void Read( BinaryReader reader )
		{
			ClearLines();
			int msg_count = reader.ReadInt32();
			for(int i = 0; i < msg_count; i++ )
			{
				var msg = AddLine();
				msg.Text = reader.ReadString();
				msg.Output.SetId( reader.ReadInt32() );
			}
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Messages.Count );
			foreach(var entry in Messages )
			{
				writer.Write( entry.Text );
				writer.Write( entry.Output.Id );
			}
		}
	}

	

	[Node( Title = "Answer" )]
	public class AnswerNode : BaseNode
	{
	}

	[Node( Title = "Commentary",HasInput = false, HasOutput = false )]
	public class CommentNode : BaseNode
	{
		public string Text
		{
			get => text.Text;
			set => text.Text = value;
		}
		private TextEntry text;
		public CommentNode()
		{
			text = Canvas.Add.TextEntryWithPlaceHolder( "Comment" );
			text.Multiline = true;
		}

		public override void Read( BinaryReader reader )
		{
			Text = reader.ReadString();
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Text );
		}
	}
}
