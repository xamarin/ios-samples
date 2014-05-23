using System;
using UIKit;

namespace Xamarin.Code
{
	public class NavItem
	{		
		/// <summary>
		/// The name of the nav item, shows up as the label
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		protected string name;
		
		/// <summary>
		/// The UIViewController that the nav item opens. Use this property if you 
		/// wanted to early instantiate the controller when the nav table is built out,
		/// otherwise just set the Type property and it will lazy-instantiate when the 
		/// nav item is clicked on.
		/// </summary>
		public UIViewController Controller
		{
			get { return controller; }
			set { controller = value; }
		}
		protected UIViewController controller;
		
		/// <summary>
		/// Path to the image to show in the nav item
		/// </summary>
		public string ImagePath
		{
			get { return imagePath; }
			set { imagePath = value; }
		}
		protected string imagePath;
		
		/// <summary>
		/// The Type of the UIViewController. Set this to the type and leave the Controller 
		/// property empty to lazy-instantiate the ViewController when the nav item is 
		/// clicked.
		/// </summary>
		public Type ControllerType
		{
			get { return controllerType; }
			set { controllerType = value; }
		}
		protected Type controllerType;
		
		/// <summary>
		/// a list of the constructor args (if neccesary) for the controller. use this in 
		/// conjunction with ControllerType if lazy-creating controllers.
		/// </summary>
		public object[] ControllerConstructorArgs
		{
			get { return controllerConstructorArgs; }
			set
			{
				controllerConstructorArgs = value;
				
				controllerConstructorTypes = new Type[controllerConstructorArgs.Length];
				for (int i = 0; i < controllerConstructorArgs.Length; i++) {
					controllerConstructorTypes[i] = controllerConstructorArgs[i].GetType ();
				}
			}
		}
		protected object[] controllerConstructorArgs = new object[] {};
		
		/// <summary>
		/// The types of constructor args.
		/// </summary>
		public Type[] ControllerConstructorTypes
		{
			get { return controllerConstructorTypes; }
		}
		protected Type[] controllerConstructorTypes = Type.EmptyTypes;
			
		public NavItem ()
		{
		}
		
		public NavItem (string name) : this()
		{
			this.name = name;
		}
		
		public NavItem (string name, UIViewController controller) : this (name)
		{
			this.controller = controller;
		}

		public NavItem (string name, Type controllerType) : this (name)
		{
			this.controllerType = controllerType;
		}

		public NavItem (string name, Type controllerType, object[] controllerConstructorArgs) : this (name, controllerType)
		{
			this.ControllerConstructorArgs = controllerConstructorArgs;
		}
		
		public NavItem (string name, UIViewController controller, string imagePath) : this (name, controller)
		{
			this.imagePath = imagePath;
		}

		public NavItem (string name, string imagePath, Type controllerType) : this (name, controllerType)
		{
			this.imagePath = imagePath;
		}

		public NavItem (string name, string imagePath, Type controllerType, object[] controllerConstructorArgs) : this (name, controllerType, controllerConstructorArgs)
		{
			this.imagePath = imagePath;
		}
	}
}
