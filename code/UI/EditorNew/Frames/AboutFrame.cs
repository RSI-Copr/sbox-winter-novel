using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerryNovel.Editor;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel
{
	public class AboutFrame:Frame
	{
        public AboutFrame()
		{
			Canvas.Add.Label("About us: we are three friends from russia", "about");
		}
	}
}
