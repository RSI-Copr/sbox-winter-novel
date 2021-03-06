using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	[Node(Title = "Character sprite", Group = "Info", HasInput = false, HasOutput = false)]
	public class SpriteNode: BaseNode
	{
		public string File
		{
			get => selector.Text;
			set => selector.Value = value;
		}
		public string Name
		{
			get => textEntry.Text;
			set => textEntry.Text = value;
		}
		public float Scale
		{
			get
			{
				if ( float.TryParse( scale.Text , out var f) )
				{
					return f;
				}

				return 1;
			}
			set => scale.Text = value.ToString();
		}


		private readonly StringSelector selector;
		private readonly TextEntry textEntry;
		private readonly TextEntry scale;
		public SpriteNode()
		{
			textEntry = Canvas.Add.TextEntryWithPlaceHolder( "Name" );
			selector = Canvas.AddChild<StringSelector>();
			selector.Finder = () => FileSystem.Mounted.FindFile( "assets/textures/", "*.png" );


			scale = Canvas.Add.TextEntryWithPlaceHolder( "Scale" );
			scale.Text = "1";
			scale.Numeric = true;

			Editor.Sprites.Register( this );

			textEntry.AddEventListener( "onchange", () => {
				Editor.Sprites.Update( this );
			} );
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write(Name);
			writer.Write( File );
			writer.Write( Scale );
		}

		public override void Read( BinaryReader reader )
		{
			Name = reader.ReadString();
			File = reader.ReadString();
			Scale = reader.ReadSingle();
		}

		public override void OnPostLoad()
		{
			Editor.Sprites.Update( this );
		}

		public override void OnDeleted()
		{
			Editor.Sprites.Unregister( this );
			base.OnDeleted();
		}
	}


	[Node( Title = "Sprite event", Group = "Events", HasInput = true, HasOutput = false )]
	public class SpriteEventNode : BaseNode,IEventNode {

		public string Name
		{
			get => selector.Value;
			set => selector.Value = value;
		}

		private SpriteSelector selector;
		public int SpriteId =-1;

		private EnumSelector<SpriteEventType> spawntype;
		private EnumSelector<SpriteComeFrom> camefrom;
		

	    public SpriteEventNode()
		{
			selector = Canvas.AddChild<SpriteSelector>();
			selector.OnSelect = ( id ) => SpriteId = id;

			spawntype = Canvas.AddChild<EnumSelector<SpriteEventType>>();
		
			camefrom = Canvas.AddChild<EnumSelector<SpriteComeFrom>>();
			camefrom.Value = SpriteComeFrom.Center;

		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( SpriteId );
			writer.Write( (int)spawntype.Value );
			writer.Write( (int)camefrom.Value );
		}

		public override void Read( BinaryReader reader )
		{
			SpriteId = reader.ReadInt32();
			spawntype.Value = (SpriteEventType)reader.ReadInt32();
			camefrom.Value = (SpriteComeFrom)reader.ReadInt32();
		}

		public NovelEvent GenerateEvent()
		{
			if ( SpriteId == -1 ) return null;

			return new SpriteEvent()
			{
				arguments = new string[]{ 
					Editor.Sprites.GetSpriteGeneratedId( SpriteId ).ToString(),
					spawntype.StringValue,
					camefrom.StringValue,
				}
			};
		}

		class SpriteSelector : PopupButton
		{
			private string value;
			public string Value
			{
				get => value;
				set
				{
					Text = value;
					this.value = value;
				}

			}

			public Action<int> OnSelect;


			public SpriteSelector()
			{
				AddClass( "dropdown" );
				Add.Icon( "expand_more", "dropdown_indicator" );
			}
			public override void Open()
			{
				Popup = new Popup( this, Popup.PositionMode.BelowStretch, 0.0f );
				Popup.AddClass( "flat-top" );

				foreach ( var kv in Editor.Sprites.Get( ))
				{
					if ( kv.Value == Value ) continue;

					var o = Popup.AddOption( kv.Value, () => {

						Value = kv.Value;
						OnSelect?.Invoke( kv.Key );
					} );

				}
			}
		}



	}

	public class EnumSelector<T> : PopupButton where T: struct,Enum
	{
		private T value;
		public T Value
		{
			get => value;
			set
			{
				Text = value.ToString();
				this.value = value;
			}

		}

		public new string StringValue => Value.ToString();
		
		public EnumSelector()
		{
			AddClass( "dropdown" );
			Add.Icon( "expand_more", "dropdown_indicator" );

			Value = Enum.GetValues<T>().First();
		}
		public override void Open()
		{
			Popup = new Popup( this, Popup.PositionMode.BelowStretch, 0.0f );
			Popup.AddClass( "flat-top" );

			foreach ( var file in Enum.GetValues<T>() )
			{
				if ( file.Equals(Value) ) continue;

				var o = Popup.AddOption( file.ToString(), () => Value = file );

			}
		}

	}


}
