using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace TerryNovel.Editor
{
	public class ProperyNode :BaseNode
	{
		protected override void CreateContents()
		{
			foreach(var prop in GetType().GetProperties() )
			{
				if ( !prop.DeclaringType.IsSubclassOf( typeof( ProperyNode ) ) ) continue;
				var type = prop.PropertyType;
				
				switch ( type.Name )
				{
					case "String":
						var txt = Canvas.Add.TextEntryWithPlaceHolder( prop.Name );
						txt.AddEventListener( "onchange", () =>{
							prop.SetValue( this, txt.Text );
						} );
						break;
				}
			}
		}
	}
}
