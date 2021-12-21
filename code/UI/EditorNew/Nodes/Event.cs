using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	
	public class EventNode : BaseNode
	{

	}


	[Node( Title = "Set background", HasOutput = false )]
	public class BackGroundEventNode: BaseNode
	{
		public string Name
		{
			get => selector.Value;
			set => selector.SetPropertyObject("value", value );
		}

		private DropDown selector;
		public BackGroundEventNode()
		{
			var dropdown = new DropDown( this );
			foreach (var f in Editor.Backgrounds.Get() )
			{
				dropdown.Options.Add( new Option( f, f ) );
			}

			

		}
	}
}
