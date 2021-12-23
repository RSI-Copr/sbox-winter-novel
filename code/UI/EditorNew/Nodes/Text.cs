using Sandbox.UI;
using Sandbox.UI.Construct;
using System.IO;
using System.Collections.Generic;

namespace TerryNovel.Editor
{
	[Node( Title = "Message" , Group = "Text", HasOutput = false)]
	public class MessageNode : BaseNode
	{
		public readonly List<MessageEntry> Messages = new();

		public class MessageEntry:Panel{

			public readonly PlugOut Output;
			public string Character;

			public int CharacterId = -1;

			private readonly MessageNode Node;
			private TextEntry textEntry;
			private CharacterPanel characterPanel;
			private Button selectBtn;
			public string Text
			{
				get => textEntry.Text;
				set => textEntry.Text = value;
			}


			public MessageEntry( MessageNode node )
			{
				AddClass( "message" );
				selectBtn = Add.Button( "face", "characterselect", OpenСharacterSelector );

				textEntry = Add.TextEntry( "" );
				textEntry.Multiline = true;

				Node = node;
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

				popup.AddOption( "Protagonist", () => SelectCharacter( -3 ) );
				popup.AddOption( "Mind voice", () => SelectCharacter( -2 ) );
			}

			public void SelectCharacter( int id )
			{
				CharacterId = id;
				if (id < -1 )
				{
					
					characterPanel?.Delete();
					characterPanel = null;

					if(id == -2 )
					{
						selectBtn.Text = "psychology";
					}
					else
					{
						selectBtn.Text = "record_voice_over";
					}

					return;
				}


				if ( id == -1 ) return;

				selectBtn.Text = "face";

				
				if( characterPanel == null || !characterPanel.IsValid() )
				{
					characterPanel = new CharacterPanel( id );
					AddChild( characterPanel );
				}
				else
				{
					characterPanel.UpdateFrom( id );
				}
			}

			
			

			public bool IsLast => Node.Messages.IndexOf( this ) == Node.Messages.Count - 1;
			public bool IsFirst => Node.Messages.IndexOf( this ) == 0;

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
			if(target is CharacterPanel ch )
			{
				options.AddOption( "Detach character", () =>
				 {
					 ((MessageEntry)ch.Parent).CharacterId = -1;
					 ch.Delete();
				 });
			}

			var msg = target as MessageEntry ?? target.Parent as MessageEntry ?? target.Parent?.Parent?.Parent as MessageEntry;
			if ( msg == null ) return;

			options.AddOption( "Insert" , () => InsertAfterMessage(msg) );

			if(msg.IsFirst ) return;

			var text = msg.Text;
			options.AddOption( $"Delete line: {(text.Length > 8 ? text[..5] + "..." : text)}" ,()=> RemoveLine( msg ) );
			
		}


		private void InsertAfterMessage( MessageEntry msg)
		{
			int index = Messages.IndexOf( msg ) + 1;

			var msgs = Messages.GetRange( index, Messages.Count - index );
			foreach(var m in msgs )
			{
				m.Parent = null;
			}

			var new_msg = new MessageEntry( this );
			Messages.Insert( index, new_msg );
			Canvas.AddChild( new_msg );

			foreach ( var m in msgs )
			{
				m.Parent = Canvas;
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
				msg.CharacterId = reader.ReadInt32();
			}
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Messages.Count );
			foreach(var entry in Messages )
			{
				writer.Write( entry.Text );
				writer.Write( entry.Output.Id );
				writer.Write( entry.CharacterId );
			}
		}

		public override void OnPostLoad()
		{
			foreach(var msg in Messages )
			{
				msg.SelectCharacter( msg.CharacterId );
			}
		}

		
	}

	[Node( Title = "Answer", Group = "Text" )]
	public class AnswerNode : BaseNode
	{
		private const int MaxAnswers = 4;
		public readonly List<AnswerEntry> Answers = new();

		private Button Button;

		public class AnswerEntry : Panel
		{

			public readonly PlugOut Output;
			private readonly AnswerNode Node;

			private TextEntry textEntry;
			public string Text
			{
				get => textEntry.Text;
				set => textEntry.Text = value;
			}


			public AnswerEntry( BaseNode node )
			{
				AddClass( "message" );
				
				textEntry = Add.TextEntry( "" );
				textEntry.Multiline = true;
				
				Output = new PlugOut( node );
				AddChild( Output );
			}
		}

		public AnswerNode()
		{
			AddLine();
			Button = Add.Button( "add", "plus", () => AddLine() );
		}


		private void ClearLines()
		{
			foreach ( var ans in Answers )
			{
				ans.Delete();
			}

			Answers.Clear();


			Button.SetVisible( false );
		}



		private AnswerEntry AddLine()
		{
			var msg = new AnswerEntry( this );
			Answers.Add( msg );
			Canvas.AddChild( msg );

			if( Answers.Count >= MaxAnswers )
			{
				Button.SetVisible( false );
			}

			return msg;
		}

		private void RemoveLine( AnswerEntry entry )
		{
			Answers.Remove( entry );
			entry.Delete(true);

			if ( Answers.Count < MaxAnswers )
			{
				Button.SetVisible( true );
			}
		}

		protected override void OnEdit( OptionsPanel options, Panel target )
		{
			var msg = target as AnswerEntry ?? target.Parent as AnswerEntry ?? target.Parent?.Parent?.Parent as AnswerEntry;
			if ( msg == null ) return;
			if ( Answers.IndexOf(msg) == 0 ) return;

			var text = msg.Text;
			options.AddOption( $"Delete line {(text.Length > 8 ? text[..5] + "..." : text)}", () => RemoveLine( msg ) );

		}
		

		public override void Read( BinaryReader reader )
		{
			ClearLines();
			int msg_count = reader.ReadInt32();
			for ( int i = 0; i < msg_count; i++ )
			{
				var msg = AddLine();
				msg.Text = reader.ReadString();
				msg.Output.SetId( reader.ReadInt32() );
			}
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Answers.Count );
			foreach ( var entry in Answers )
			{
				writer.Write( entry.Text );
				writer.Write( entry.Output.Id );
			}
		}


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
