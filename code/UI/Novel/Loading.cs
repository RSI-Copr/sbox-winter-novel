﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.IO;
using TerryNovel.Editor;

namespace TerryNovel
{
	partial class Novel
	{

		public const string Directory = "novels";
		public static BaseFileSystem FS => FileSystem.Data;

		public static void CreateDir()
		{
			FS.CreateDirectory( Directory );
		}
		public static IEnumerable<string> FindAll(BaseFileSystem fs, string directory )
		{
			foreach ( var dir in fs.FindDirectory( directory ) )
			{
				var filename = fs.FindFile( $"{directory}\\{dir}", "*.novel" ).FirstOrDefault();
				if ( filename == null ) continue;
				yield return $"{directory}\\{dir}\\{filename}";
			}
		}

		public static bool ReadInfo( BaseFileSystem fs, string filename, out string name, out string desc)
		{
			name = null;
			desc = null;

			if ( !fs.FileExists( filename ) ) return false;
			using var filestream = fs.OpenRead( filename );
			using var reader = new BinaryReader( filestream );

			name = reader.ReadString();
			desc = reader.ReadString();

			reader.Close();
			filestream.Close();

			return true;

		}

		public static void LoadFromFile(BaseFileSystem fs, string filename )
		{
			using var filestream = fs.OpenRead( filename );
			using var reader = new BinaryReader( filestream );


			var info = new NovelInfo();

			info.Name = reader.ReadString();
			info.Description = reader.ReadString();

			int msg_count = reader.ReadInt32();

			for ( int i = 0; i < msg_count; i++ )
			{
				var msg = new Message()
				{
					Id = reader.ReadInt32(),
					Text = reader.ReadString(),
					Character = reader.ReadInt32(),
					NextMessage = reader.ReadInt32(),
					PrevMessage = reader.ReadInt32(),
				};

			
				if ( reader.ReadBoolean() )
				{
					int ans_count = reader.ReadInt32();
					msg.Answers = new();

					for(int j = 0;j<ans_count;j++ )
					{
						msg.Answers.Add( new()
						{
							Text = reader.ReadString(),
							NextMessage = reader.ReadInt32(),
							Events = reader.ReadIntList(),

						} );
					}
				}

				msg.Events = reader.ReadIntList();
				info.Messages.Add( msg );
			}

			int events_count = reader.ReadInt32();
			for(int i = 0;i< events_count;i++ )
			{
				var classname = reader.ReadString();
				var ev = Library.Create<NovelEvent>( classname );

				if( reader.ReadBoolean() )
				{
					int args_count = reader.ReadInt32();
					ev.arguments = new string[args_count];

					for ( int j = 0; j < args_count; j++ )
					{
						ev.arguments[i] = reader.ReadString();
					}

				}

				info.Events.Add( ev );
			}

			info.StartEvents = reader.ReadIntList();

			int ch_count = reader.ReadInt32();

			for(int i = 0; i< ch_count; i++ )
			{
				info.Characters.Add( reader.ReadString() );
			}

			int sp_count = reader.ReadInt32();

			for ( int i = 0; i < sp_count; i++ )
			{
				info.Sprites.Add( new Sprite()
				{
					Name = reader.ReadString(),
					Scale = reader.ReadSingle(),
				} ); ;
			}

			reader.Close();
			filestream.Close();

			Novel.Start( info );
		}


		
	}
}
