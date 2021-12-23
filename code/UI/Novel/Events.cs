using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace TerryNovel
{


	public class NovelEvent:LibraryClass
	{
		public string[] arguments;
		public NovelEvent( params string[] args )
		{
			arguments = args;
		}
		public void Run()
		{
			Call( arguments );
		}
		public virtual void Call( params string[] args ) { }
		public override bool Equals( object obj )
		{
			if(obj is not NovelEvent ev) return false;


			if ( this.GetType() != ev.GetType()) return false;
			if (arguments == null && ev.arguments == null ) return true;
			if( arguments == null || ev.arguments == null ) return false;
			if ( arguments.Length != ev.arguments.Length ) return false;

			for(int i = 0; i< arguments.Length;i++ )
			{
				if(arguments[i] != ev.arguments[i]) return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine( arguments );
		}
	}

	public interface IEventNode
	{
		public NovelEvent GenerateEvent();
	}


	public class BackGroundChangeEvent : NovelEvent
	{
		public BackGroundChangeEvent( string background )
		{
			arguments = new string[]{background};
		}
		public override void Call( params string[] args )
		{
			Novel.SetBackground( args[0] );
		}
	}

	public class MusicPlayEvent : NovelEvent
	{
		public MusicPlayEvent( string name )
		{
			arguments = new string[] { name };
		}
		public override void Call( params string[] args )
		{
			Music.Set(args[0]);
		}
	}

	public class SoundPlayEvent : NovelEvent
	{
		public SoundPlayEvent( string name )
		{
			arguments = new string[] { name };
		}
		public override void Call( params string[] args )
		{
			Log.Info( args[0] );
			Sound.FromScreen( args[0] );
		}
	}

	public class MusicStopEvent : NovelEvent
	{
		public override void Call( params string[] args )
		{
			Music.Stop();
		}
	}
	public class BlackScreenEvent : NovelEvent
	{
		public BlackScreenEvent(bool value)
		{
			arguments = new string[] { value.ToString() };
		}

		public override void Call( params string[] args )
		{
			Novel.SetBlack( args[0].ToBool() );
		}
	}

	public class SpriteEvent : NovelEvent
	{
		
		public override void Call( params string[] args )
		{
			Novel.RunSpriteEvent( args[0].ToInt(), Enum.Parse<SpriteEventType>( args[1] ), Enum.Parse<SpriteComeFrom>( args[2] ) );
		}
	}

	public enum SpriteEventType
	{
		Spawn,
		Delete,
	}

	public enum SpriteComeFrom
	{
		Left,
		Rigth,
		Center,
	}
}
