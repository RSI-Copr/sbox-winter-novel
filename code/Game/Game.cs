using Sandbox;
using Sandbox.UI;

namespace TerryNovel
{
	public class NovelGame : Game
	{
		public NovelGame() : base()
		{
			Novel.CreateDir();
			Editor.BaseNode.Init();
		}

		public override void Shutdown()
		{
			base.Shutdown();
			if( IsClient ){
				UI.Destroy();
			}
		}

	
		public override void DoPlayerDevCam( Client player ){}
		public override void DoPlayerNoclip ( Client player ){}
		public override void DoPlayerSuicide( Client player ){}
	}
}
