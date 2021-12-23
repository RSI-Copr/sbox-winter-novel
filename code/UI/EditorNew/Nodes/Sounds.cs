using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace TerryNovel.Editor
{
	
	

	[Node( Title = "Stop current music", Group = "Events", HasOutput = false )]
	public class MusicStopNode : BaseNode, IEventNode
	{
		public NovelEvent GenerateEvent()
		{
			return new MusicStopEvent();
		}
	}

	[Node( Title = "Set playing music", Group = "Events", HasOutput = false )]
	public class MusicNode : BaseNode,IEventNode
	{
		public string Name
		{
			get => selector.Value;
			set => selector.Value = value;
		}

		private readonly FileSelector selector;
		public MusicNode()
		{
			selector = Canvas.AddChild<FileSelector>();
			selector.Finder = () => FileSystem.Mounted.FindFile( "assets/music/", "*.sound" );
		}
		public NovelEvent GenerateEvent()
		{
			return new MusicPlayEvent( Name.Replace( ".sound", "" ) );
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Name );
		}

		public override void Read( BinaryReader reader )
		{
			Name = reader.ReadString();
		}
	}

	[Node( Title = "Play sound", Group = "Events", HasOutput = false )]
	public class SoundNode : BaseNode, IEventNode
	{
		public string Name
		{
			get => selector.Value;
			set => selector.Value = value;
		}

		private readonly FileSelector selector;
		public SoundNode()
		{
			selector = Canvas.AddChild<FileSelector>();
			selector.Finder = () => FileSystem.Mounted.FindFile( "assets/sounds/", "*.sound" );
		}
		public NovelEvent GenerateEvent()
		{
			return new SoundPlayEvent( Name.Replace( ".sound", "" ) );
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Name );
		}

		public override void Read( BinaryReader reader )
		{
			Name = reader.ReadString();
		}
	}
}
