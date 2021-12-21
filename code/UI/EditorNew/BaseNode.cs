using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TerryNovel.Editor
{

	public class BaseNode:Panel
	{

	

		private readonly static Dictionary<string, Type> NodesNamesDict = new();
		private readonly static Dictionary<Type, NodeAttribute> NodesDict = new();
		public static IReadOnlyDictionary<string, Type> NodesNames => NodesNamesDict;

		public int Id ;
		
		private Panel canvas;
		protected Panel Canvas
		{
			get
			{
				if(canvas == null )
				{
					canvas = AddChild<Panel>( "canvas" );
				}
				return canvas;
			}
		}

		protected NodeAttribute Attribute => NodesDict.GetValueOrDefault( this.GetType() );


		protected virtual void CreateContents()
		{

		}

		public readonly PlugIn BaseInput;
		public readonly PlugOut BaseOutput;



		private readonly List<PlugOut> Outputs;
		private readonly List<PlugIn> Inputs;

		public void AddOutput( PlugOut output )
		{
			Outputs.Add( output );
		}


		public IEnumerable<BaseNode> DependedNodes
		{
			get
			{
				foreach(var ouput in Outputs )
				{
					
				}
				return null;
			}
		}
		

		

		public BaseNode()
		{
			
			this.LoadStyleSheet( "BaseNode" );
			AddClass( "node" );
			AddClass( this.GetType().Name.ToLower() );

			Add.Label( Attribute.Title, "title" );
			var plug_canvas = AddChild<Panel>( "plugs" );

			CreateContents();
			
			if( Attribute.HasInput )
			{

				BaseInput = new PlugIn(this);
				plug_canvas.AddChild( BaseInput );
			}
			if ( Attribute.HasOutput )
			{
				Outputs = new();
				BaseOutput = new PlugOut( this );
				AddOutput( BaseOutput );
				plug_canvas.AddChild( BaseOutput );
			}
			
		}
		




		[Event.Hotload]
		public static void Refill()
		{
			NodesNamesDict.Clear();
			NodesDict.Clear();
			Init();
		}

		public static void Init()
		{
			foreach(var type in Library.GetAll<BaseNode>() )
			{
				var attr = type.GetCustomAttribute<NodeAttribute>();
				if ( attr == null ) continue;

				NodesDict.Add( type, attr );

				if ( attr.CanCreate )
				{
					NodesNamesDict.Add( attr.Title, type );
				}
				
			}
		}


		protected override void OnMouseUp( MousePanelEvent e )
		{

			if ( e.Button == "mouseright" && Attribute.CanCreate )
			{
				var options = new OptionsPanel();
				OnEdit( options, e.Target );

				options.AddOption( "Delete", () => Delete() );
			}
		}
		

		
		protected virtual void OnEdit( OptionsPanel options, Panel target )
		{
		
		}
		public virtual void Read(BinaryReader reader ) { }
		public virtual void Write( BinaryWriter writer ) { }


	}

	[AttributeUsage(AttributeTargets.Class)]
	public class NodeAttribute:Attribute
	{
		public string Title;
		public bool HasInput = true;
		public bool HasOutput = true;
		public bool CanCreate = true;
	}
}
