using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel
{
	partial class Novel {
		
		public static string Name { get; set; }
		public static string Description { get; set; }
		private static Dictionary<int, Message> MessagesDict = new();


	}

	public class NovelEvent
	{

	}

	public class Message
	{
		public string Text;
		public Message NextMessage;

		public List<Question> Questions;
		public List<NovelEvent> Events;

	}

	public class Question
	{
		public string Text;
		public Message NextMessage;
		public List<NovelEvent> Events;
	}
}
