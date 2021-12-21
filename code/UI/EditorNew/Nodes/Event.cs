using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel.Editor
{
	
	public class EventNode : BaseNode
	{

	}


	[Node( Title = "Set background", HasOutput = false )]
	public class BackGroundEventNode: EventNode
	{
		public string Name { get; set; }
	}
}
