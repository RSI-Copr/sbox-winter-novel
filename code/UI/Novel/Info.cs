using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel
{
	public class NovelInfo
	{
		public string Name;
		public string Description;
		public readonly List<Message> Messages = new();
		public readonly List<NovelEvent> Events = new();
		public readonly List<int> StartEvents = new();
		public readonly List<string> Characters = new();


		public Message AddMessage()
		{
			var msg = new Message
			{
				Id = Messages.Count
			};
			Messages.Add( msg );

			return msg;
		}
		
		public int AddEvent(NovelEvent ev )
		{
			int index = Events.FindIndex( e => e.Equals( ev ) );

			if ( index == -1)
			{
				Events.Add( ev );
				return Events.Count - 1;
			}
			else
			{
				return index;
			}
		}
		public int AddEvent(IEventNode eventNode )
		{
			return AddEvent( eventNode.GenerateEvent() );
		}

		public void RunMessageEvents(Message msg)
		{
			if ( msg == null ) return;
			foreach(int i in msg.Events )
			{
				Events[i].Run();
			}
		}
		public void RunAnswerEvents(Answer ans )
		{
			foreach ( int i in ans.Events )
			{
				Events[i].Run();
			}
		}
		public void RunEvents()
		{
			foreach ( int i in StartEvents )
			{
				Events[i].Run();
			}
		}
	}

	public class Message
	{
		public int Id;
		public string Text;
		public int NextMessage = -1;
		public int PrevMessage = -1;
		public int Character = -1;
		public List<Answer> Answers;
		public List<int> Events = new();

		public bool HasAnswers => Answers != null;



	}

	public class Answer
	{
		public string Text;
		public int NextMessage = -1;
		public List<int> Events;
	}
}
