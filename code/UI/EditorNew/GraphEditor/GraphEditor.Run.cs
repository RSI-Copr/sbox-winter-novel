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
		public StartNode GetStartNode()
		{
			return BaseNode.GetNodeById( 1 ) as StartNode;
		}

		public InfoNode GetInfoNode()
		{
			return BaseNode.GetNodeById( 0 ) as InfoNode;
		}

		public void RunNovel()
		{
			var infoNode = GetInfoNode();
			var start = GetStartNode();

			var info = new NovelInfo()
			{
				Name = infoNode.Title,
				Description = infoNode.Desc,
			};
			
			var parser = new NodeGraphParser();

			Novel.FolderName = Editor.Dir.Name;
			Novel.Start(parser.Parse( start ));


		}

		
	}

	public interface INavigateHelper
	{
		public int MessageId { get; set; }
	}

	class NodeGraphParser
	{
		private readonly NovelInfo Info = new();
		private Message PreviousMessage;
		private int CurrentCharacter = -1;

		public NovelInfo Parse(StartNode start )
		{
			AddCharacters();
			foreach ( var node in start.DependedNodes )
			{
				if ( node is IEventNode ev )
				{
				Log.Info($"Добален ивент на старт  {ev}");
				Info.StartEvents.Add(Info.AddEvent( ev ));
				}
				else
				{
					ParseNode( node );
				}
			}

			return Info;
		}

		private void AddCharacters()
		{
			foreach ( var ch_info in BaseNode.All.OfType<CharacterNode>() )
			{
				Info.Characters.Add( ch_info.Name );
			}
		}

		private void ParseNode( BaseNode node )
		{
			if ( node is MessageNode msg )
			{
				ParseMessage( msg  );
				return;
			}
		}

		private Message ParseMessage( MessageNode msg_node )
		{
			Log.Info( $"Парсится сообщение {msg_node}" );
			Message first = null;

			foreach(var entry in msg_node.Messages )
			{
				var text = entry.Text;
				var msg = Info.AddMessage();

			    if(first == null )
				{
					first = msg;
				}

				msg.Text = entry.Text;


				Log.Info( $"Добавлена линия текста {(text.Length > 5 ? text[..5] + "..." : text) }" );
				
				if ( PreviousMessage != null )
				{
					PreviousMessage.NextMessage = msg.Id;
					msg.PrevMessage = PreviousMessage.Id;
				}
				PreviousMessage = msg;

				var id = entry.CharacterId;

				switch ( id )
				{
					case -1:
						break;

					case -2:
						CurrentCharacter = -1;
					break;

					case -3:
						CurrentCharacter = -2;
					break;

					default:
						CurrentCharacter = Info.Characters.IndexOf( Editor.Characters.GetName( entry.CharacterId ) );
						break;


				}
				
				msg.Character = CurrentCharacter;


				foreach ( var node in entry.Output.NextNodes )
				{
					if ( node is IEventNode ev )
					{
						Log.Info( $"к ней добавлен ивент {ev}" );
						msg.Events.Add( Info.AddEvent( ev.GenerateEvent() ) );
					}
					if ( !entry.IsLast ) continue;

					if(node is MessageNode msg_node2 )
					{
						ParseMessage( msg_node2 );
					}
					if(node is AnswerNode answer )
					{
						ParseAnswer( answer );
					}

					
				}
			}
			return first;
		}

		private void ParseAnswer( AnswerNode answer_node )
		{
			Log.Info( $"Парсится группа ответов {answer_node}" );

			var msg = PreviousMessage;

			var answers = new List<Answer>();
			
			foreach (var entry in answer_node.Answers )
			{
				Log.Info( $"Парсится ответ {entry.Text}" );
				PreviousMessage = null;
				var answer = new Answer()
				{
					Text = entry.Text,
					Events = ParseAnswerEvents( entry.Output ),
					NextMessage = ParseAnswerMessage( entry.Output , msg),
				};
				answers.Add( answer );
			}

			msg.Answers = answers;
		}

		private List<int> ParseAnswerEvents(PlugOut output)
		{
			var list = new List<int>();

			foreach(var ev in output.NextNodes.OfType<IEventNode>() )
			{
				Log.Info( $"к нему добавлен ивент {ev}" );
				list.Add( Info.AddEvent( ev ) );
			}
			return list;
		}

		private int ParseAnswerMessage( PlugOut output, Message curmsg )
		{
			var msg_node = output.NextNodes.FirstOrDefault( n => n is MessageNode ) as MessageNode;
			if ( msg_node == null ) return -1;

			var msg = ParseMessage( msg_node );
			msg.PrevMessage = curmsg.Id;

			return msg.Id;
		}


	}
	

}
