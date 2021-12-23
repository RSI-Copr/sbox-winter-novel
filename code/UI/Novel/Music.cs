using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace TerryNovel
{
	public static class Music
	{
		private static Sound? Current;
		public static void Set(string name )
		{
			Current?.Stop();
			Current = Sound.FromScreen( name );
		}

		public static void Mute(bool value)
		{
			Current?.SetVolume( value ? 0 : 1 );
			
		}

		public static void Pause( bool value )
		{
			Mute( value );
		}

		public static void Stop()
		{
			Current?.Stop();
		}
	}
}
