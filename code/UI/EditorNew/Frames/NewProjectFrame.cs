using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	public class NewProjectFrame:Frame
	{
		
		public NewProjectFrame()
		{
			this.LoadDefaultStyleSheet();
			HeaderText = "Create novel";


			var entry = Canvas.Add.TextEntry( "" );
			Canvas.Add.Button( "Создать",()=>CreateProject( entry.Text) );
		
		}

		private void CreateProject(string name)
		{
			Editor.CrateProject( name );
			Delete();
		}
	}
}
