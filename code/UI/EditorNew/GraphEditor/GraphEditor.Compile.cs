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
		public void GenetateNovelFile()
		{
			var infoNode = GetInfoNode();
			var start = GetStartNode();


			var parser = new NodeGraphParser();
			var info = parser.Parse( start );

			info.Name = infoNode.Title;
			info.Description = infoNode.Desc;

			using var file_stream = FileSystem.Data.OpenWrite( $"novels/{Editor.Dir.Name}/.novel" );
			using var writer = new BinaryWriter( file_stream );

			writer.Write( info.Name );
			writer.Write( info.Description );

			writer.Write( info.Messages.Count );

			foreach ( var msg in info.Messages )
			{
				writer.Write( msg.Id );
				writer.Write( msg.Text );
				writer.Write( msg.Character );
				writer.Write( msg.NextMessage );
				writer.Write( msg.PrevMessage );

				bool has_answers = msg.Answers != null;
				writer.Write( has_answers );

				if ( has_answers )
				{
					writer.Write( msg.Answers.Count );

					foreach ( var ans in msg.Answers )
					{
						writer.Write( ans.Text );
						writer.Write( ans.NextMessage );
						writer.WriteIntList( ans.Events );
					}
				}

				writer.WriteIntList( msg.Events );

			}

			writer.Write( info.Events.Count );

			foreach ( var ev in info.Events )
			{
				writer.Write( ev.ClassInfo.Name );
				bool has_arguments = ev.arguments != null;
				writer.Write( has_arguments );
				if ( has_arguments )
				{
					writer.Write( ev.arguments.Length );

					for ( int i = 0; i < ev.arguments.Length;i++ ) 
					{
						writer.Write( ev.arguments[i] );
					}
				}
			}

			writer.WriteIntList( info.StartEvents );

			writer.Write( info.Characters.Count );

			foreach(var ch in info.Characters )
			{
				writer.Write( ch );
			}

			writer.Write( info.Sprites.Count );

			foreach ( var sp in info.Sprites )
			{
				writer.Write( sp.Name );
				writer.Write( sp.Scale );
			}


			writer.Close();
			file_stream.Close();
		}

	}

	public static class WriterExt
	{
		public static void WriteIntList(this BinaryWriter writer, List<int> list )
		{
			writer.Write( list.Count );
			foreach(int i in list )
			{
				writer.Write( i );
			}
		}

		public static List<int> ReadIntList(this BinaryReader reader)
		{
			int count = reader.ReadInt32();
			var list = new List<int>();

			for(int i = 0;i< count;i++ )
			{
				list.Add( reader.ReadInt32() );
			}

			return list;
		}
	}
	
}
