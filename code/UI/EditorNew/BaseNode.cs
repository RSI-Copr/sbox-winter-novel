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
		public static bool AutoIdAssignment { get; set; } = true;
		public static int CurrentId { get; set; }
		public static readonly List<BaseNode> All = new();
		public static BaseNode GetNodeById( int id ) => All.FirstOrDefault( n => n.Id == id );

		

		private readonly static Dictionary<string, Type> NodesNamesDict = new();
		private readonly static Dictionary<Type, NodeAttribute> NodesDict = new();
		public static IReadOnlyDictionary<Type, NodeAttribute> NodesTypes => NodesDict;

		public int Id;
		
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
				if ( Outputs == null ) yield break;
			    foreach(var o in Outputs )
				{
					foreach(var o2 in o.NextInputs )
					{
						yield return o2.Node;
					}
				}
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

			All.Add( this );

			Id = CurrentId;

			if ( AutoIdAssignment )
			{
				CurrentId++;
			}
		}
		

		public virtual void OnPostLoad()
		{

		}


		[Event.Hotload]
		public static void Refill()
		{
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

		public override void OnDeleted()
		{
			All.Remove( this );
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
		public string Group = "Other";
		public bool HasInput = true;
		public bool HasOutput = true;
		public bool CanCreate = true;
	}
}
