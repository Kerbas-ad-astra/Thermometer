using System;
using UnityEngine;

namespace Thermometer
{

	public class MainWindow : Window
	{
		public MainWindow() {
			title = "Thermometer Settings";
		}
		public override void initGui(WindowSettings settings) {
			settings.Load ();
			base.initGui(settings);
		}
		public override void remove() {
			base.remove();
			settings.Save ();
		}
		public override void onWindow(int wID)
		{
			GUILayout.BeginVertical();{
				if (GUILayout.Button ("Curernt Unit: " + settings.currentUnit)) {
					settings.nextUnit();
				}
				GUILayout.Label("Temperature Threshold: " + settings.getThreshold()*100 + "%");
				settings.setThreshold (GUILayout.HorizontalSlider ((float)settings.getThreshold (), 0, 1));
			}GUILayout.EndVertical();

			base.onWindow (wID);
		}
		public override void closeSubWindow() {
		}
	}

	//Basic window class
	public abstract class Window
	{
		//The settings
		protected WindowSettings settings;
		//The gui style
		protected GUIStyle windowStyle;

		//The id
		protected int windowID;
		//The drawing queue spot
		protected int drawingQueue = 0;
		//The parent window
		protected Window parent;
		//The window title
		protected String title;

		//Called when the window is initialized
		public virtual void initGui(WindowSettings settings) {
			this.settings = settings;
			windowID = UnityEngine.Random.Range(1000, 2000000) + this.GetHashCode();

			windowStyle = new GUIStyle(HighLogic.Skin.window);
			windowStyle.fixedWidth = settings.w;

			RenderingManager.AddToPostDrawQueue(drawingQueue, drawGUI);
		}
		//Called when the window is being drawn
		public virtual void onWindow(int wID){
			GUI.DragWindow(new Rect(0, 0, 10000, 20));

			if (settings.y < 0)
				settings.y = 0;
			if (settings.x < 0)
				settings.x = 0;
			if (settings.x + settings.w > Screen.width)
				settings.x = Screen.width - settings.w;
			if (settings.y + settings.h > Screen.height)
				settings.y = Screen.height - settings.h;
		}
		//Called when we are removing the window
		public virtual void remove() {
			closeSubWindow();
			RenderingManager.RemoveFromPostDrawQueue(drawingQueue, drawGUI);
		}
		//Called by subwindows to close them
		public virtual void closeSubWindow(){}
		//Initial window drawing
		public virtual void drawGUI() {
			GUI.skin = HighLogic.Skin;

			Rect WindowRect = settings.getRect();

			WindowRect = GUILayout.Window(windowID, WindowRect, onWindow, title, windowStyle);

			settings.setFromRect(WindowRect);
		}
	}

	//The window's settings
	public class WindowSettings : ConfigNodeStorage {
		//Constructors
		internal WindowSettings(String FilePath) : base(FilePath) {}
		internal WindowSettings() : base("") {}
		internal WindowSettings(float x, float y, float w, float h) : base("") {
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}
		internal WindowSettings(float x, float y, float w, float h, String FilePath) : base(FilePath) {
			this.x = x;
			this.y = y;
			this.w = w;
			this.h = h;
		}

		//Size information
		public float x = 100;
		public float y = 100;
		public float w = 100;
		public float h = 250;


		[Persistent] public TemperatureUnit currentUnit = TemperatureUnit.CELSIUS;
		[Persistent] private double threshold = 0.75;

		//Turn it into a rectangle
		public Rect getRect() {
			return new Rect(x,y,w,h);
		}
		//Set it from a rectangle
		public void setFromRect(Rect r) {
			x = r.xMin;
			y = r.yMin;
			w = r.width;
			h = r.height;
		}

		public void setThreshold(double thresh) {
			ModuleThermometer.percentHighlight = thresh;
			threshold = thresh;
		}

		public double getThreshold() {
			return threshold;
		}

		public void nextUnit() {
			if (currentUnit == TemperatureUnit.CELSIUS) {
				currentUnit = TemperatureUnit.FAHRENHEIT;
			} else if (currentUnit == TemperatureUnit.FAHRENHEIT) {
				currentUnit = TemperatureUnit.KELVIN;
			} else if (currentUnit == TemperatureUnit.KELVIN) {
				currentUnit = TemperatureUnit.CELSIUS;
			}
			ModuleThermometer.tempUnit = currentUnit;
		}
	}
}

