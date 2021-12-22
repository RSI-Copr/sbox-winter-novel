using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel
{


	public class NovelEvent
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
}
