using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel.Editor
{
	static class Lines
	{
		public static void DrawBroken( Vector2 pos1, Vector2 pos2, Color col, float t = 10f )
		{
			float half_t = t / 2;

			float x = 0, y = 0, w = 0, h = 0;

			x = MathF.Min( pos1.x, pos2.x );
			w = MathF.Abs( pos1.x - pos2.x );

			y = MathF.Min( pos1.y, pos2.y );
			h = MathF.Abs( pos1.y - pos2.y );

			Render.UI.Box( new Rect( pos1.x + half_t, y + half_t, t, h + half_t ), col );
			Render.UI.Box( new Rect( x + half_t, pos2.y + half_t, w + half_t, t ), col );
		}
	}
}
