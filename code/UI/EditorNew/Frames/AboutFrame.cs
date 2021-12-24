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
			Canvas.Add.Label( "Hello from Russia! We're 3 friends: russian_engineer, StrayFrog/MrSIr and янаС/sanikP. Russian_engineer is a programmer, very forgetful, MrSIr is an artist and designer, he's also laziest one, and янаC is a UI manager - super human. About Game:This is a visual novels compilation and visual novel framework.You can make your own visual novel here, and play the main novel \"Terry's flutter heart\" Novel editor is not finished yet if you want fully use it contact us is Discord", "about");
		}
	}
}
