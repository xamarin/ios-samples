using System;
using UIKit;

namespace FontList.Code {

	public class NavItem
	{
		/// <summary>
		/// The name of the nav item, shows up as the label
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// The UIViewController that the nav item opens. Use this property if you 
		/// wanted to early instantiate the controller when the nav table is built out,
		/// otherwise just set the Type property and it will lazy-instantiate when the 
		/// nav item is clicked on.
		/// </summary>
		public UIViewController Controller { get; set; }
		
		/// <summary>
		/// Path to the image to show in the nav item
		/// </summary>
		public string ImagePath { get; set; }
		
		/// <summary>
		/// The Type of the UIViewController. Set this to the type and leave the Controller 
		/// property empty to lazy-instantiate the ViewController when the nav item is 
		/// clicked.
		/// </summary>
		public Type ControllerType { get; set; }

		/// <summary>
		/// The font used to display the item.
		/// </summary>
		public UIFont Font { get; set; }
		
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
				
				controllerConstructorTypes = new Type[this.controllerConstructorArgs.Length];
				for(int i = 0; i < this.controllerConstructorArgs.Length; i++)
				{
					controllerConstructorTypes[i] = controllerConstructorArgs[i].GetType();
				}
			}
		}
		protected object[] controllerConstructorArgs = new object[] {};
		
		/// <summary>
		/// The types of constructor args.
		/// </summary>
		public Type[] ControllerConstructorTypes
		{
			get { return this.controllerConstructorTypes; }
		}
		protected Type[] controllerConstructorTypes = Type.EmptyTypes;
	

		
		public NavItem ()
		{
		}
		
		public NavItem (string name) : this()
		{
			Name = name;
		}
		
		public NavItem (string name, UIViewController controller) : this(name)
		{
			Controller = controller;
		}

		public NavItem (string name, Type controllerType) : this(name)
		{
			ControllerType = controllerType;
		}

		public NavItem (string name, Type controllerType, object[] controllerConstructorArgs) : this(name, controllerType)
		{
			ControllerConstructorArgs = controllerConstructorArgs;
		}
		
		public NavItem (string name, UIViewController controller, string imagePath) : this(name, controller)
		{
			ImagePath = imagePath;
		}

		public NavItem (string name, string imagePath, Type controllerType) : this(name, controllerType)
		{
			ImagePath = imagePath;
		}

		public NavItem (string name, string imagePath, Type controllerType, object[] controllerConstructorArgs) : this(name, controllerType, controllerConstructorArgs)
		{
			ImagePath = imagePath;
		}
	}
}